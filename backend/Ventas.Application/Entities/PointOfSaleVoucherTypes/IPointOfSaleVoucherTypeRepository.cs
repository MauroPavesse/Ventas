using System.Linq.Expressions;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.PointOfSaleVoucherTypes
{
    public interface IPointOfSaleVoucherTypeRepository : IBaseRepository<PointOfSaleVoucherType>
    {
        public Task<IEnumerable<PointOfSaleVoucherType>> SearchAsync(
            Expression<Func<PointOfSaleVoucherType, bool>>? predicate = null,
            IEnumerable<string>? includesString = null,
            bool disableTracking = true);
    }
}
