using Mapster;
using MediatR;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Application.Entities.Vouchers.DTOs;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Vouchers.Create
{
    public record VoucherCreateCommand(int Number, decimal AmountNet, decimal AmountVAT, string CAE, DateTime CAEExpiration, int UserId, int? CustomerId, int VoucherTypeId, int? DailyBoxId, DateTime DateCreation, int StateEntityId) : IRequest<VoucherOutput>;

    public class VoucherCreateHandler : IRequestHandler<VoucherCreateCommand, VoucherOutput>
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public VoucherCreateHandler(IVoucherRepository voucherRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _voucherRepository = voucherRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<VoucherOutput> Handle(VoucherCreateCommand request, CancellationToken cancellationToken)
        {
            var voucher = await _voucherRepository.CreateAsync(request.Adapt<Voucher>());
            await _unitOfWorkRepository.SaveChangesAsync();
            return voucher.Adapt<VoucherOutput>();
        }
    }
}
