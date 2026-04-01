using Mapster;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Ventas.Application.Entities.Configurations;
using Ventas.Application.Entities.Externas.Prints.BudgetDocument;
using Ventas.Application.Entities.TaxConditions;
using Ventas.Application.Entities.Vouchers;
using Ventas.Application.Entities.Vouchers.DTOs;
using Ventas.Domain.Enums;
using Ventas.Infrastructure.Persistence.Reporting;
using static Ventas.Infrastructure.Persistence.Reporting.BudgetDocument;

namespace Ventas.Infrastructure.Persistence.Services
{
    public class PrintBudgetService : IBudgetDocumentService
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ITaxConditionRepository _taxConditionRepository;

        public PrintBudgetService(IVoucherRepository voucherRepository, IConfigurationRepository configurationRepository, ITaxConditionRepository taxConditionRepository)
        {
            _voucherRepository = voucherRepository;
            _configurationRepository = configurationRepository;
            _taxConditionRepository = taxConditionRepository;
        }

        public async Task<byte[]> PrintBudget(int id)
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
            
            var command = new BudgetDocumentCommand(
                Voucher: voucher.Adapt<VoucherOutput>(),
                BusinessName: businessName,
                BusinessCuit: businessCuit,
                BusinessTaxCondition: taxConditions.First(t => t.Id == Convert.ToInt32(businessTaxConditionId)).Description,
                BusinessAddress: voucher.User?.PointOfSale?.Address ?? "",
                CustomerTaxCondition: voucher.Customer != null ? taxConditions.First(t => t.Id == voucher.Customer.TaxConditionId).Description : taxConditions.First(t => t.Id == (int)TaxConditionEnum.CONSUMIDOR_FINAL).Description            );

            IDocument document = new BudgetDocument(command);

            return document.GeneratePdf();
        }
    }
}
