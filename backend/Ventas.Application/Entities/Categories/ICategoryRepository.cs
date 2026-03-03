using System.Linq.Expressions;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Categories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        public Task<IEnumerable<Category>> SearchAsync(
            Expression<Func<Category, bool>>? predicate = null,
            IEnumerable<string>? includesString = null,
            bool disableTracking = true);
    }
}
