using Mapster;
using MediatR;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Application.Entities.VoucherPayments.DTOs;

namespace Ventas.Application.Entities.VoucherPayments.Update
{
    public record VoucherPaymentUpdateCommand(
        int Id,
        decimal Amount,
        int VoucherId,
        int PaymentMethodId) : IRequest<VoucherPaymentOutput>;

    public class VoucherPaymentUpdateHandler : IRequestHandler<VoucherPaymentUpdateCommand, VoucherPaymentOutput>
    {
        private readonly IVoucherPaymentRepository _voucherPaymentRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public VoucherPaymentUpdateHandler(IVoucherPaymentRepository voucherPaymentRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _voucherPaymentRepository = voucherPaymentRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<VoucherPaymentOutput> Handle(VoucherPaymentUpdateCommand request, CancellationToken cancellationToken)
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
