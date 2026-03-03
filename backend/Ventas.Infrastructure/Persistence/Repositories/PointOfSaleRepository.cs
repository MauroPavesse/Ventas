using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ventas.Application.Entities.PointOfSales;
using Ventas.Domain.Entities;
using Ventas.Infrastructure.Data;

namespace Ventas.Infrastructure.Persistence.Repositories
{
    public class PointOfSaleRepository : BaseRepository<PointOfSale>, IPointOfSaleRepository
    {
        public PointOfSaleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PointOfSale>> SearchAsync(
            Expression<Func<PointOfSale, bool>>? predicate = null, 
            IEnumerable<string>? includesString = null, 
            bool disableTracking = true)
        {
            List<Func<IQueryable<PointOfSale>, IQueryable<PointOfSale>>> includes = [];

            if (includesString != null && includesString.Any())
            {
                foreach (var include in includesString)
                {
                    switch (include)
                    {
                        case "Users":
                            includes.Add(i => i
                                .Include(t => t.Users));
                            break;
                        case "PointOfSaleVoucherTypes":
                            includes.Add(i => i
                                .Include(t => t.PointOfSaleVoucherTypes));
                            break;
                    }
                }
            }
            else
            {
                includes.Add(i => i
                    .Include(t => t.Users)
                    .Include(t => t.PointOfSaleVoucherTypes));
            }

            return await GetAllAsync(predicate, includes, disableTracking);
        }
    }
}
