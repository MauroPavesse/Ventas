using MediatR;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.VoucherDetails.Delete
{
    public record VoucherDetailDeleteCommand(int Id) : IRequest<bool>;

    public class VoucherDetailDeleteHandler : IRequestHandler<VoucherDetailDeleteCommand, bool>
    {
        private readonly IVoucherDetailRepository _voucherDetailRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public VoucherDetailDeleteHandler(IVoucherDetailRepository voucherDetailRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _voucherDetailRepository = voucherDetailRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<bool> Handle(VoucherDetailDeleteCommand request, CancellationToken cancellationToken)
        {
            var existingVoucherDetail = await _voucherDetailRepository.GetByIdAsync(request.Id);
            if (existingVoucherDetail == null)
            {
                throw new KeyNotFoundException($"Comprobante detalle con ID {request.Id} no encontrado.");
            }
            existingVoucherDetail.Deleted = 1;
            existingVoucherDetail.Active = 0;
            var result = await _voucherDetailRepository.UpdateAsync(existingVoucherDetail);
            await _unitOfWorkRepository.SaveChangesAsync();
            return true;
        }
    }
}
