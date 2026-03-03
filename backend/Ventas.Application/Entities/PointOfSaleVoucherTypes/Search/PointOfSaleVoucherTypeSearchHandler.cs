using Mapster;
using MediatR;
using System.Linq.Expressions;
using Ventas.Application.Entities.PointOfSaleVoucherTypes.DTOs;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.PointOfSaleVoucherTypes.Search
{
    public record PointOfSaleVoucherTypeSearchCommand(SearchCommand Search) : IRequest<IEnumerable<PointOfSaleVoucherTypeOutput>>;

    public class PointOfSaleVoucherTypeSearchHandler : IRequestHandler<PointOfSaleVoucherTypeSearchCommand, IEnumerable<PointOfSaleVoucherTypeOutput>>
    {
        private readonly IPointOfSaleVoucherTypeRepository _pointOfSaleVoucherTypeRepository;

        public PointOfSaleVoucherTypeSearchHandler(IPointOfSaleVoucherTypeRepository pointOfSaleVoucherTypeRepository)
        {
            _pointOfSaleVoucherTypeRepository = pointOfSaleVoucherTypeRepository;
        }

        public async Task<IEnumerable<PointOfSaleVoucherTypeOutput>> Handle(PointOfSaleVoucherTypeSearchCommand request, CancellationToken cancellationToken)
        {
            var search = request.Search;

            Expression<Func<PointOfSaleVoucherType, bool>> predicate = t => t.Deleted == 0;

            if (search.Id > 0)
            {
                predicate = t => t.Id == search.Id;
            }
            else
            {
                var pointOfSaleIdFilter = search.Filters.FirstOrDefault(t => t.Field == "PointOfSaleId");
                if (pointOfSaleIdFilter != null)
                {
                    predicate = predicate.And(t => t.PointOfSaleId == Convert.ToInt32(pointOfSaleIdFilter.Value));
                }

                var voucherTypeIdFilter = search.Filters.FirstOrDefault(t => t.Field == "VoucherTypeId");
                if (voucherTypeIdFilter != null)
                {
                    predicate = predicate.And(t => t.VoucherTypeId == Convert.ToInt32(voucherTypeIdFilter.Value));
                }
            }

            var pointOfSaleVoucherTypes = await _pointOfSaleVoucherTypeRepository.SearchAsync(predicate, search.Includes, search.DisableTracking);

            return pointOfSaleVoucherTypes.Adapt<IEnumerable<PointOfSaleVoucherTypeOutput>>();
        }
    }
}
