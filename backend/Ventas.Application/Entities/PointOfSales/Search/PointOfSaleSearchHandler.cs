using Mapster;
using MediatR;
using System.Linq.Expressions;
using Ventas.Application.Entities.PointOfSales.DTOs;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.PointOfSales.Search
{
    public record PointOfSaleSearchCommand(SearchCommand Search) : IRequest<IEnumerable<PointOfSaleOutput>>;

    public class PointOfSaleSearchHandler : IRequestHandler<PointOfSaleSearchCommand, IEnumerable<PointOfSaleOutput>>
    {
        private readonly IPointOfSaleRepository _pointOfSaleRepository;

        public PointOfSaleSearchHandler(IPointOfSaleRepository pointOfSaleRepository)
        {
            _pointOfSaleRepository = pointOfSaleRepository;
        }

        public async Task<IEnumerable<PointOfSaleOutput>> Handle(PointOfSaleSearchCommand request, CancellationToken cancellationToken)
        {
            var search = request.Search;

            Expression<Func<PointOfSale, bool>> predicate = t => t.Deleted == 0;

            if (search.Id > 0)
            {
                predicate = t => t.Id == search.Id;
            }

            var pointOfSales = await _pointOfSaleRepository.SearchAsync(predicate, search.Includes, search.DisableTracking);

            return pointOfSales.Adapt<IEnumerable<PointOfSaleOutput>>();
        }
    }
}
