using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ventas.Application.Entities.TaxConditions;
using Ventas.Domain.Entities;
using Ventas.Infrastructure.Data;

namespace Ventas.Infrastructure.Persistence.Repositories
{
    public class TaxConditionRepository : BaseRepository<TaxCondition>, ITaxConditionRepository
    {
        public TaxConditionRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TaxCondition>> SearchAsync(
            Expression<Func<TaxCondition, bool>>? predicate = null, 
            IEnumerable<string>? includesString = null, 
            bool disableTracking = true)
        {
            List<Func<IQueryable<TaxCondition>, IQueryable<TaxCondition>>> includes = [];

            if (includesString != null && includesString.Any())
            {
                foreach (var include in includesString)
                {
                    switch (include)
                    {
                        case "Customers":
                            includes.Add(i => i
                                .Include(t => t.Customers));
                            break;
                    }
                }
            }
            else
            {
                includes.Add(i => i
                    .Include(t => t.Customers));
            }

            return await GetAllAsync(predicate, includes, disableTracking);
        }
    }
}
