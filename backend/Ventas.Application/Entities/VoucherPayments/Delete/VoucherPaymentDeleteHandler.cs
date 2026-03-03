using MediatR;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.VoucherPayments.Delete
{
    public record VoucherPaymentDeleteCommand(int Id) : IRequest<bool>;

    public class VoucherPaymentDeleteHandler : IRequestHandler<VoucherPaymentDeleteCommand, bool>
    {
        private readonly IVoucherPaymentRepository _voucherPaymentRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public VoucherPaymentDeleteHandler(IVoucherPaymentRepository voucherPaymentRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _voucherPaymentRepository = voucherPaymentRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<bool> Handle(VoucherPaymentDeleteCommand request, CancellationToken cancellationToken)
        {
            var existingVoucherPayment = await _voucherPaymentRepository.GetByIdAsync(request.Id);
            if (existingVoucherPayment == null)
            {
                throw new KeyNotFoundException($"Comprobante pago con ID {request.Id} no encontrado.");
            }
            existingVoucherPayment.Deleted = 1;
            existingVoucherPayment.Active = 0;
            var result = await _voucherPaymentRepository.UpdateAsync(existingVoucherPayment);
            await _unitOfWorkRepository.SaveChangesAsync();
            return true;
        }
    }
}
