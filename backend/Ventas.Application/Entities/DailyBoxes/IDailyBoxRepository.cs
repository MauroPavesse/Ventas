using System.Linq.Expressions;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.DailyBoxes
{
    public interface IDailyBoxRepository : IBaseRepository<DailyBox>
    {
        public Task<IEnumerable<DailyBox>> SearchAsync(
            Expression<Func<DailyBox, bool>>? predicate = null,
            IEnumerable<string>? includesString = null,
            bool disableTracking = true);
    }
}
