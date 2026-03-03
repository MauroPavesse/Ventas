using Mapster;
using MediatR;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Application.Entities.VoucherDetails.DTOs;

namespace Ventas.Application.Entities.VoucherDetails.Update
{
    public record VoucherDetailUpdateCommand(
        int Id,
        int VoucherId,
        int ProductId,
        decimal Quantity,
        decimal PriceUnit,
        decimal AmountNet,
        decimal Discount,
        decimal AmountFinal) : IRequest<VoucherDetailOutput>;

    public class VoucherDetailUpdateHandler : IRequestHandler<VoucherDetailUpdateCommand, VoucherDetailOutput>
    {
        private readonly IVoucherDetailRepository _voucherDetailRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public VoucherDetailUpdateHandler(IVoucherDetailRepository voucherDetailRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _voucherDetailRepository = voucherDetailRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<VoucherDetailOutput> Handle(VoucherDetailUpdateCommand request, CancellationToken cancellationToken)
        {
            var existingVoucherDetail = await _voucherDetailRepository.GetByIdAsync(request.Id);
            if (existingVoucherDetail == null)
            {
                throw new KeyNotFoundException($"Comprobante detalle con Id {request.Id} no encontrado.");
            }
            existingVoucherDetail.VoucherId = request.VoucherId;
            existingVoucherDetail.ProductId = request.ProductId;
            existingVoucherDetail.Quantity = request.Quantity;
            existingVoucherDetail.PriceUnit = request.PriceUnit;
            existingVoucherDetail.AmountNet = request.AmountNet;
            existingVoucherDetail.Discount = request.Discount;
            existingVoucherDetail.AmountFinal = request.AmountFinal;
            var updatedVoucherDetail = await _voucherDetailRepository.UpdateAsync(existingVoucherDetail);
            await _unitOfWorkRepository.SaveChangesAsync();
            return updatedVoucherDetail.Adapt<VoucherDetailOutput>();
        }
    }
}
