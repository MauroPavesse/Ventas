using System.Linq.Expressions;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Roles
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        public Task<IEnumerable<Role>> SearchAsync(
            Expression<Func<Role, bool>>? predicate = null,
            IEnumerable<string>? includes = null,
            bool disableTracking = true);
    }
}
