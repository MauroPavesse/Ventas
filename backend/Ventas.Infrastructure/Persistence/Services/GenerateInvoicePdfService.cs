using Mapster;
using Net.Codecrete.QrCodeGenerator;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Ventas.Application.Entities.Configurations;
using Ventas.Application.Entities.Externas.GeneratePdf;
using Ventas.Application.Entities.TaxConditions;
using Ventas.Application.Entities.Vouchers;
using Ventas.Application.Entities.Vouchers.DTOs;
using Ventas.Domain.Enums;
using Ventas.Infrastructure.Persistence.Reporting;

namespace Ventas.Infrastructure.Persistence.Services
{
    public class GenerateInvoicePdfService : IGenerateInvoicePdfService
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ITaxConditionRepository _taxConditionRepository;

        public GenerateInvoicePdfService(IVoucherRepository voucherRepository, IConfigurationRepository configurationRepository, ITaxConditionRepository taxConditionRepository)
        {
            _voucherRepository = voucherRepository;
            _configurationRepository = configurationRepository;
            _taxConditionRepository = taxConditionRepository;
        }

        public async Task<byte[]> GenerateInvoicePdf(int id, bool isTicket = false)
        {
            var voucher = (await _voucherRepository.SearchAsync(
                t => t.Id == id,
                ["VoucherDetails.Product", "User.PointOfSale", "Customer", "VoucherType"]
            )).First();

            var taxConditions = (await _taxConditionRepository.GetAllAsync());

            var configurations = (await _configurationRepository.GetAllAsync());
            var businessName = configurations.FirstOrDefault(c => c.Variable == "empresa")?.StringValue ?? string.Empty;
            var businessCuit = configurations.FirstOrDefault(c => c.Variable == "cuit")?.StringValue ?? string.Empty;
            var businessTaxConditionId = configurations.FirstOrDefault(c => c.Variable == "condicionFiscalId")?.NumericValue ?? 1;
            var businessAddress = configurations.FirstOrDefault(c => c.Variable == "cuit")?.StringValue ?? string.Empty;

            byte[]? qrBytes = null;
            if (voucher.VoucherTypeId == (int)VoucherTypeEnum.FACTURA_A || voucher.VoucherTypeId == (int)VoucherTypeEnum.FACTURA_B)
            {
                string tipoDocRec = "", nroDocRec = "";
                if (voucher.Customer == null)
                {
                    tipoDocRec = "99";
                    nroDocRec = "0";
                }
                else
                {
                    var taxConditionCustomer = taxConditions.First(t => t.Id == voucher.Customer.TaxConditionId);
                    switch (taxConditionCustomer.Id)
                    {
                        case (int)TaxConditionEnum.CONSUMIDOR_FINAL:
                            tipoDocRec = "96";
                            nroDocRec = voucher.Customer.Document.ToString();
                            break;
                        case (int)TaxConditionEnum.RESPONSABLE_INSCRIPTO:
                            tipoDocRec = "80";
                            nroDocRec = voucher.Customer.Cuit;
                            break;
                        case (int)TaxConditionEnum.RESPONSABLE_MONOTRIBUTO:
                            tipoDocRec = "80";
                            nroDocRec = voucher.Customer.Cuit;
                            break;
                        default:
                            tipoDocRec = "99";
                            nroDocRec = "0";
                            break;
                    }
                }
                string jsonComprobante = "{" +
                        "\"ver\":1," +
                        "\"fecha\":\"" + voucher.DateCreation.ToString("yyyy-MM-dd") + "\"," +
                        "\"cuit\":" + businessCuit + "," +
                        "\"ptoVta\":" + voucher.User!.PointOfSale!.Number + "," +
                        "\"tipoCmp\":" + voucher.VoucherType!.Code + "," +
                        "\"nroCmp\":" + voucher.Number + "," +
                        "\"importe\":" + voucher.VoucherDetails.Sum(t => t.AmountFinal).ToString("F2").Replace(",", ".") + "," +
                        "\"moneda\":\"PES\"," +
                        "\"ctz\":1," +
                        "\"tipoDocRec\":" + tipoDocRec + "," +
                        "\"nroDocRec\":" + nroDocRec + "," +
                        "\"tipoCodAut\":\"E\"," +
                        "\"codAut\":\"" + voucher.CAE + "\"" +
                        "}";
                var jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonComprobante);
                string jsonBase64 = Convert.ToBase64String(jsonBytes);
                string urlFinal = "https://www.afip.gob.ar/fe/qr/?p=" + jsonBase64;

                var qr = QrCode.EncodeText(urlFinal, QrCode.Ecc.Medium);
                qrBytes = qr.ToPngBitmap(scale: 4); // Esto genera el byte[] que recibe el comando
            }

            var command = new InvoiceDocument.InvoiceDocumentCommand(
                Voucher: voucher.Adapt<VoucherOutput>(),
                BusinessName: businessName,
                BusinessCuit: businessCuit,
                BusinessTaxCondition: taxConditions.First(t => t.Id == Convert.ToInt32(businessTaxConditionId)).Description,
                BusinessAddress: voucher.User?.PointOfSale?.Address ?? "",
                CustomerTaxCondition: voucher.Customer != null ? taxConditions.First(t => t.Id == voucher.Customer.TaxConditionId).Description : taxConditions.First(t => t.Id == (int)TaxConditionEnum.CONSUMIDOR_FINAL).Description,
                QrCodeImage: qrBytes
            );

            IDocument document = isTicket
                ? new TicketDocument(command)
                : new InvoiceDocument(command);

            return document.GeneratePdf();
        }
    }
}
