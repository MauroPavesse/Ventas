using MediatR;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.Vouchers.Delete
{
    public record VoucherDeleteCommand(int Id) : IRequest<bool>;

    public class VoucherDeleteHandler : IRequestHandler<VoucherDeleteCommand, bool>
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public VoucherDeleteHandler(IVoucherRepository voucherRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _voucherRepository = voucherRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<bool> Handle(VoucherDeleteCommand request, CancellationToken cancellationToken)
        {
            var existingVoucher = await _voucherRepository.GetByIdAsync(request.Id);
            if (existingVoucher == null)
            {
                throw new KeyNotFoundException($"Comprobante con ID {request.Id} no encontrado.");
            }
            existingVoucher.Deleted = 1;
            existingVoucher.Active = 0;
            var result = await _voucherRepository.UpdateAsync(existingVoucher);
            await _unitOfWorkRepository.SaveChangesAsync();
            return true;
        }
    }
}
