using System.Linq.Expressions;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.PointOfSales
{
    public interface IPointOfSaleRepository : IBaseRepository<PointOfSale>
    {
        public Task<IEnumerable<PointOfSale>> SearchAsync(
            Expression<Func<PointOfSale, bool>>? predicate = null,
            IEnumerable<string>? includesString = null,
            bool disableTracking = true);
    }
}
