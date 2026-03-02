using Mapster;
using MediatR;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Application.Entities.Vouchers.DTOs;

namespace Ventas.Application.Entities.Vouchers.Update
{
    public record VoucherUpdateCommand(int Id, int Number, decimal AmountNet, decimal AmountVAT, string CAE, DateTime CAEExpiration, int UserId, int? CustomerId, int VoucherTypeId, int? DailyBoxId, DateTime DateCreation, int StateEntityId) : IRequest<VoucherOutput>;

    public class VoucherUpdateHandler : IRequestHandler<VoucherUpdateCommand, VoucherOutput>
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public VoucherUpdateHandler(IVoucherRepository voucherRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _voucherRepository = voucherRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<VoucherOutput> Handle(VoucherUpdateCommand request, CancellationToken cancellationToken)
        {
            var existingVoucher = await _voucherRepository.GetByIdAsync(request.Id);
            if (existingVoucher == null)
            {
                throw new KeyNotFoundException($"Comprobante con Id {request.Id} no encontrado.");
            }
            existingVoucher.Number = request.Number;
            existingVoucher.AmountNet = request.AmountNet;
            existingVoucher.AmountVAT = request.AmountVAT;
            existingVoucher.CAE = request.CAE;
            existingVoucher.CAEExpiration = request.CAEExpiration;
            existingVoucher.UserId = request.UserId;
            existingVoucher.CustomerId = request.CustomerId;
            existingVoucher.VoucherTypeId = request.VoucherTypeId;
            existingVoucher.DailyBoxId = request.DailyBoxId;
            existingVoucher.DateCreation = request.DateCreation;
            existingVoucher.StateEntityId = request.StateEntityId;
            var updatedVoucher = await _voucherRepository.UpdateAsync(existingVoucher);
            await _unitOfWorkRepository.SaveChangesAsync();
            return updatedVoucher.Adapt<VoucherOutput>();
        }
    }
}
