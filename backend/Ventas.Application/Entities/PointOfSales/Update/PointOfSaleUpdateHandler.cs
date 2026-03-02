using Mapster;
using MediatR;
using Ventas.Application.Entities.PointOfSales.DTOs;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.PointOfSales.Update
{
    public record PointOfSaleUpdateCommand(int Id, string Name, string Number, string Address, string City, string Provincie, string PostalCode) : IRequest<PointOfSaleOutput>;

    public class PointOfSaleUpdateHandler : IRequestHandler<PointOfSaleUpdateCommand, PointOfSaleOutput>
    {
        private readonly IPointOfSaleRepository _pointOfSaleRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public PointOfSaleUpdateHandler(IPointOfSaleRepository pointOfSaleRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _pointOfSaleRepository = pointOfSaleRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<PointOfSaleOutput> Handle(PointOfSaleUpdateCommand request, CancellationToken cancellationToken)
        {
            var existingPointOfSale = await _pointOfSaleRepository.GetByIdAsync(request.Id);
            if (existingPointOfSale == null)
            {
                throw new KeyNotFoundException($"Punto de venta con Id {request.Id} no encontrado.");
            }
            existingPointOfSale.Name = request.Name;
            existingPointOfSale.Number = request.Number;
            existingPointOfSale.Address = request.Address;
            existingPointOfSale.City = request.City;
            existingPointOfSale.Provincie = request.Provincie;
            existingPointOfSale.PostalCode = request.PostalCode;
            var updatedPointOfSale = await _pointOfSaleRepository.UpdateAsync(existingPointOfSale);
            await _unitOfWorkRepository.SaveChangesAsync();
            return updatedPointOfSale.Adapt<PointOfSaleOutput>();
        }
    }
}
