using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ventas.Application.Entities.TaxRates;
using Ventas.Domain.Entities;
using Ventas.Infrastructure.Data;

namespace Ventas.Infrastructure.Persistence.Repositories
{
    public class TaxRateRepository : BaseRepository<TaxRate>, ITaxRateRepository
    {
        public TaxRateRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TaxRate>> SearchAsync(
            Expression<Func<TaxRate, bool>>? predicate = null, 
            IEnumerable<string>? includesString = null, 
            bool disableTracking = true)
        {
            List<Func<IQueryable<TaxRate>, IQueryable<TaxRate>>> includes = [];

            if (includesString != null && includesString.Any())
            {
                foreach (var include in includesString)
                {
                    switch (include)
                    {
                        case "Products":
                            includes.Add(i => i
                                .Include(t => t.Products));
                            break;
                    }
                }
            }
            else
            {
                includes.Add(i => i
                    .Include(t => t.Products));
            }

            return await GetAllAsync(predicate, includes, disableTracking);
        }
    }
}
