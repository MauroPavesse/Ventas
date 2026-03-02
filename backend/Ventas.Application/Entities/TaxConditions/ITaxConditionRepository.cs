using System.Linq.Expressions;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.TaxConditions
{
    public interface ITaxConditionRepository : IBaseRepository<TaxCondition>
    {
        public Task<IEnumerable<TaxCondition>> SearchAsync(
            Expression<Func<TaxCondition, bool>>? predicate = null,
            IEnumerable<string>? includes = null,
            bool disableTracking = true);
    }
}
