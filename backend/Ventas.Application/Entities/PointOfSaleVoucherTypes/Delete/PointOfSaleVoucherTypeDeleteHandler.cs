using MediatR;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.PointOfSaleVoucherTypes.Delete
{
    public record PointOfSaleVoucherTypeDeleteCommand(int Id) : IRequest<bool>;

    public class PointOfSaleVoucherTypeDeleteHandler : IRequestHandler<PointOfSaleVoucherTypeDeleteCommand, bool>
    {
        private readonly IPointOfSaleVoucherTypeRepository _pointOfSaleVoucherTypeRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public PointOfSaleVoucherTypeDeleteHandler(IPointOfSaleVoucherTypeRepository pointOfSaleVoucherTypeRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _pointOfSaleVoucherTypeRepository = pointOfSaleVoucherTypeRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<bool> Handle(PointOfSaleVoucherTypeDeleteCommand request, CancellationToken cancellationToken)
        {
            var existingPointOfSaleVoucherType = await _pointOfSaleVoucherTypeRepository.GetByIdAsync(request.Id);
            if (existingPointOfSaleVoucherType == null)
            {
                throw new KeyNotFoundException($"Punto de venta Comprobante Tipo con ID {request.Id} no encontrado.");
            }
            existingPointOfSaleVoucherType.Deleted = 1;
            existingPointOfSaleVoucherType.Active = 0;
            var result = await _pointOfSaleVoucherTypeRepository.UpdateAsync(existingPointOfSaleVoucherType);
            await _unitOfWorkRepository.SaveChangesAsync();
            return true;
        }
    }
}
