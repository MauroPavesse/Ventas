using Mapster;
using MediatR;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Application.Entities.VoucherPayments.DTOs;

namespace Ventas.Application.Entities.VoucherPayments.Update
{
    public record VoucherPaymentCreateCommand(
        int Id,
        decimal Amount,
        int VoucherId,
        int PaymentMethodId) : IRequest<VoucherPaymentOutput>;

    public class VoucherPaymentUpdateHandler : IRequestHandler<VoucherPaymentCreateCommand, VoucherPaymentOutput>
    {
        private readonly IVoucherPaymentRepository _voucherPaymentRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public VoucherPaymentUpdateHandler(IVoucherPaymentRepository voucherPaymentRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _voucherPaymentRepository = voucherPaymentRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<VoucherPaymentOutput> Handle(VoucherPaymentCreateCommand request, CancellationToken cancellationToken)
        {
            var existingVoucherPayment = await _voucherPaymentRepository.GetByIdAsync(request.Id);
            if (existingVoucherPayment == null)
            {
                throw new KeyNotFoundException($"Comprobante pago con Id {request.Id} no encontrado.");
            }
            existingVoucherPayment.Amount = request.Amount;
            existingVoucherPayment.VoucherId = request.VoucherId;
            existingVoucherPayment.PaymentMethodId = request.PaymentMethodId;
            var updatedVoucherPayment = await _voucherPaymentRepository.UpdateAsync(existingVoucherPayment);
            await _unitOfWorkRepository.SaveChangesAsync();
            return updatedVoucherPayment.Adapt<VoucherPaymentOutput>();
        }
    }
}
