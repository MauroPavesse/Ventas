using Mapster;
using MediatR;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Application.Entities.VoucherPayments.DTOs;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.VoucherPayments.Create
{
    public record VoucherPaymentCreateCommand(
        decimal Amount,
        int VoucherId,
        int PaymentMethodId) : IRequest<VoucherPaymentOutput>;

    public class VoucherPaymentCreateHandler : IRequestHandler<VoucherPaymentCreateCommand, VoucherPaymentOutput>
    {
        private readonly IVoucherPaymentRepository _voucherPaymentRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public VoucherPaymentCreateHandler(IVoucherPaymentRepository voucherPaymentRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _voucherPaymentRepository = voucherPaymentRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<VoucherPaymentOutput> Handle(VoucherPaymentCreateCommand request, CancellationToken cancellationToken)
        {
            var voucherPayment = await _voucherPaymentRepository.CreateAsync(request.Adapt<VoucherPayment>());
            await _unitOfWorkRepository.SaveChangesAsync();
            return voucherPayment.Adapt<VoucherPaymentOutput>();
        }
    }
}
