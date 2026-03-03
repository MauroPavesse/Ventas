using System.Linq.Expressions;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Products
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        public Task<IEnumerable<Product>> SearchAsync(
            Expression<Func<Product, bool>>? predicate = null,
            IEnumerable<string>? includesString = null,
            bool disableTracking = true);
    }
}
