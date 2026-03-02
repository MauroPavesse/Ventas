using System.Linq.Expressions;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.TaxRates
{
    public interface ITaxRateRepository : IBaseRepository<TaxRate>
    {
        public Task<IEnumerable<TaxRate>> SearchAsync(
            Expression<Func<TaxRate, bool>>? predicate = null,
            IEnumerable<string>? includes = null,
            bool disableTracking = true);
    }
}
