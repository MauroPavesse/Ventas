using Mapster;
using MediatR;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Application.Entities.VoucherDetails.DTOs;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.VoucherDetails.Create
{
    public record VoucherDetailCreateCommand(int VoucherId,
        int ProductId,
        decimal Quantity,
        decimal PriceUnit,
        decimal AmountNet,
        decimal Discount,
        decimal AmountFinal) : IRequest<VoucherDetailOutput>;

    public class VoucherDetailCreateHandler : IRequestHandler<VoucherDetailCreateCommand, VoucherDetailOutput>
    {
        private readonly IVoucherDetailRepository _voucherDetailRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public VoucherDetailCreateHandler(IVoucherDetailRepository voucherDetailRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _voucherDetailRepository = voucherDetailRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<VoucherDetailOutput> Handle(VoucherDetailCreateCommand request, CancellationToken cancellationToken)
        {
            var voucherDetail = await _voucherDetailRepository.CreateAsync(request.Adapt<VoucherDetail>());
            await _unitOfWorkRepository.SaveChangesAsync();
            return voucherDetail.Adapt<VoucherDetailOutput>();
        }
    }
}
