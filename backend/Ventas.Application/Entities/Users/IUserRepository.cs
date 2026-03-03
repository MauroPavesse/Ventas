using System.Linq.Expressions;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Users
{
    public interface IUserRepository : IBaseRepository<User>
    {
        public Task<IEnumerable<User>> SearchAsync(
            Expression<Func<User, bool>>? predicate = null,
            IEnumerable<string>? includesString = null,
            bool disableTracking = true);
    }
}
