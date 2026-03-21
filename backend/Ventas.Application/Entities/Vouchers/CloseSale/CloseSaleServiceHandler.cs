using Mapster;
using MediatR;
using Ventas.Application.Entities.Products.DTOs;
using Ventas.Application.Entities.TaxRates;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Application.Entities.Vouchers.Create;
using Ventas.Application.Entities.Vouchers.DTOs;
using Ventas.Domain.Entities;
using Ventas.Domain.Enums;

namespace Ventas.Application.Entities.Vouchers.CloseSale
{
    public record CloseSaleServiceItemCommand(int Id, decimal Quantity, decimal Price, int TaxRateId);

    public record CloseSaleServicePaymentCommand(int Id, decimal Discount, decimal Amount);

    public record CloseSaleServiceCommand(List<CloseSaleServiceItemCommand> Items, CloseSaleServicePaymentCommand Payment, int UserId, int? CustomerId, int VoucherTypeId) : IRequest<VoucherOutput>;

    public class CloseSaleServiceHandler : IRequestHandler<CloseSaleServiceCommand, VoucherOutput>
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly ITaxRateRepository _taxRateRepository;

        public CloseSaleServiceHandler(IUnitOfWorkRepository unitOfWorkRepository, IVoucherRepository voucherRepository, ITaxRateRepository taxRateRepository)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
            _voucherRepository = voucherRepository;
            _taxRateRepository = taxRateRepository;
        }

        public async Task<VoucherOutput> Handle(CloseSaleServiceCommand request, CancellationToken cancellationToken)
        {
            var taxRates = await _taxRateRepository.GetAllAsync();

            List<VoucherDetail> voucherDetails = new List<VoucherDetail>();
            foreach(var item in request.Items)
            {
                var taxRate = taxRates.First(t => t.Id == item.TaxRateId);
                decimal amountFinal = item.Quantity * item.Price;
                voucherDetails.Add(new VoucherDetail()
                {
                    //VoucherId = ,
                    ProductId = item.Id,
                    Quantity = item.Quantity,
                    PriceUnit = item.Price,
                    AmountNet = amountFinal * (1 - taxRate.Percentage / 100),
                    Discount = request.Payment.Discount * amountFinal / 100,
                    AmountFinal = amountFinal
                });
            }

            List<VoucherPayment> voucherPayments = new List<VoucherPayment>();
            voucherPayments.Add(new VoucherPayment()
            {
                Amount = request.Payment.Amount,
                //VoucherId = ,
                PaymentMethodId = request.Payment.Id
            });

            Voucher voucher = new Voucher()
            {
                Number = ,
                AmountNet = voucherDetails.Sum(t => t.AmountNet),
                AmountVAT = voucherDetails.Sum(t => t.AmountFinal) - voucherDetails.Sum(t => t.AmountNet),
                CAE = string.Empty,
                CAEExpiration = null,
                UserId = request.UserId,
                CustomerId = request.CustomerId,
                VoucherTypeId = request.VoucherTypeId,
                DailyBoxId = null,
                DateCreation = DateTime.Today,
                StateEntity = (int)StateEntityEnum.VoucherStateEnum.,
                VoucherDetails = voucherDetails,
                VoucherPayments = voucherPayments
            };

            voucher = await _voucherRepository.CreateAsync(request.Adapt<Voucher>());
            await _unitOfWorkRepository.SaveChangesAsync();
            return voucher.Adapt<VoucherOutput>();
        }
    }
}
