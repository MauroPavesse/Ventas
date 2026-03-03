using Mapster;
using MediatR;
using Ventas.Application.Entities.PointOfSaleVoucherTypes.DTOs;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.PointOfSaleVoucherTypes.Update
{
    public record PointOfSaleVoucherTypeUpdateCommand(int Id, int PointOfSaleId, int VoucherTypeId, int Numeration) : IRequest<PointOfSaleVoucherTypeOutput>;

    public class PointOfSaleVoucherTypeUpdateHandler : IRequestHandler<PointOfSaleVoucherTypeUpdateCommand, PointOfSaleVoucherTypeOutput>
    {
        private readonly IPointOfSaleVoucherTypeRepository _pointOfSaleVoucherTypeRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public PointOfSaleVoucherTypeUpdateHandler(IPointOfSaleVoucherTypeRepository pointOfSaleVoucherTypeRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _pointOfSaleVoucherTypeRepository = pointOfSaleVoucherTypeRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<PointOfSaleVoucherTypeOutput> Handle(PointOfSaleVoucherTypeUpdateCommand request, CancellationToken cancellationToken)
        {
            var existingPointOfSaleVoucherType = await _pointOfSaleVoucherTypeRepository.GetByIdAsync(request.Id);
            if (existingPointOfSaleVoucherType == null)
            {
                throw new KeyNotFoundException($"Punto de venta Comprobante Tipo con Id {request.Id} no encontrado.");
            }
            existingPointOfSaleVoucherType.PointOfSaleId = request.PointOfSaleId;
            existingPointOfSaleVoucherType.VoucherTypeId = request.VoucherTypeId;
            existingPointOfSaleVoucherType.Numeration = request.Numeration;
            var updatedPointOfSaleVoucherType = await _pointOfSaleVoucherTypeRepository.UpdateAsync(existingPointOfSaleVoucherType);
            await _unitOfWorkRepository.SaveChangesAsync();
            return updatedPointOfSaleVoucherType.Adapt<PointOfSaleVoucherTypeOutput>();
        }
    }
}
