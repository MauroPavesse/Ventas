using Mapster;
using MediatR;
using Ventas.Application.Entities.PointOfSales.DTOs;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.PointOfSales.Create
{
    public record PointOfSaleCreateCommand(string Name, string Number, string Address, string City, string Provincie, string PostalCode) : IRequest<PointOfSaleOutput>;

    public class PointOfSaleCreateHandler : IRequestHandler<PointOfSaleCreateCommand, PointOfSaleOutput>
    {
        private readonly IPointOfSaleRepository _pointOfSaleRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public PointOfSaleCreateHandler(IPointOfSaleRepository pointOfSaleRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _pointOfSaleRepository = pointOfSaleRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<PointOfSaleOutput> Handle(PointOfSaleCreateCommand request, CancellationToken cancellationToken)
        {
            var pointOfSale = await _pointOfSaleRepository.CreateAsync(request.Adapt<PointOfSale>());
            await _unitOfWorkRepository.SaveChangesAsync();
            return pointOfSale.Adapt<PointOfSaleOutput>();
        }
    }
}
