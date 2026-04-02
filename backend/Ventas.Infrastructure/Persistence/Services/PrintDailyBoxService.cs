using Mapster;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Ventas.Application.Entities.DailyBoxes;
using Ventas.Application.Entities.Externas.Prints.DailyBoxDocument;
using Ventas.Application.Entities.PaymentMethods;
using Ventas.Application.Entities.PaymentMethods.DTOs;
using Ventas.Application.Entities.VoucherPayments.DTOs;
using Ventas.Application.Entities.Vouchers.DTOs;
using Ventas.Infrastructure.Persistence.Reporting;
using static Ventas.Infrastructure.Persistence.Reporting.DailyBoxDocument;

namespace Ventas.Infrastructure.Persistence.Services
{
    public class PrintDailyBoxService : IDailyBoxDocumentService
    {
        private readonly IDailyBoxRepository _dailyBoxRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;

        public PrintDailyBoxService(IDailyBoxRepository dailyBoxRepository, IPaymentMethodRepository paymentMethodRepository)
        {
            _dailyBoxRepository = dailyBoxRepository;
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task<byte[]> PrintDailyBox(int id)
        {
            var dailyBox = (await _dailyBoxRepository.SearchAsync(
                t => t.Id == id,
                ["Vouchers.User.PointOfSale", "Vouchers.VoucherPayments.PaymentMethod", "Vouchers.VoucherType", "User.PointOfSale"]
            )).First();

            var paymentMethods = (await _paymentMethodRepository.SearchAsync()).ToList();

            var voucherPayments = dailyBox.Vouchers.SelectMany(t => t.VoucherPayments).ToList();
            List<VoucherPaymentOutput> payments = new List<VoucherPaymentOutput>();
            foreach(var paymentMethod in paymentMethods)
            {
                payments.Add(new VoucherPaymentOutput()
                {
                    Amount = voucherPayments.Where(t => t.PaymentMethodId == paymentMethod.Id).Sum(t => t.Amount),
                    PaymentMethod = paymentMethod.Adapt<PaymentMethodOutput>()
                });
            }

            var command = new DailyBoxDocumentCommand (
                DailyBoxDescription: "Caja diaria " + dailyBox.Number,
                DailyBoxDate: dailyBox.Date,
                PointOfSaleDescription: dailyBox.User?.PointOfSale?.Name ?? "",
                Vouchers: dailyBox.Vouchers.Adapt<List<VoucherOutput>>(),
                Payments: payments
            );

            IDocument document = new DailyBoxDocument(command);

            return document.GeneratePdf();
        }
    }
}
