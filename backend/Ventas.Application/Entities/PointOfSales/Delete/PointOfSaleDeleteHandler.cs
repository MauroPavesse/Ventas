using MediatR;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.PointOfSales.Delete
{
    public record PointOfSaleDeleteCommand(int Id) : IRequest<bool>;

    public class PointOfSaleDeleteHandler : IRequestHandler<PointOfSaleDeleteCommand, bool>
    {
        private readonly IPointOfSaleRepository _pointOfSaleRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public PointOfSaleDeleteHandler(IPointOfSaleRepository pointOfSaleRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _pointOfSaleRepository = pointOfSaleRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<bool> Handle(PointOfSaleDeleteCommand request, CancellationToken cancellationToken)
        {
            var existingPointOfSale = await _pointOfSaleRepository.GetByIdAsync(request.Id);
            if (existingPointOfSale == null)
            {
                throw new KeyNotFoundException($"Punto de venta con ID {request.Id} no encontrado.");
            }
            existingPointOfSale.Deleted = 1;
            existingPointOfSale.Active = 0;
            var result = await _pointOfSaleRepository.UpdateAsync(existingPointOfSale);
            await _unitOfWorkRepository.SaveChangesAsync();
            return true;
        }
    }
}
