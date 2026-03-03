using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ventas.Application.Entities.PointOfSaleVoucherTypes;
using Ventas.Domain.Entities;
using Ventas.Infrastructure.Data;

namespace Ventas.Infrastructure.Persistence.Repositories
{
    public class PointOfSaleVoucherTypeRepository : BaseRepository<PointOfSaleVoucherType>, IPointOfSaleVoucherTypeRepository
    {
        public PointOfSaleVoucherTypeRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PointOfSaleVoucherType>> SearchAsync(
            Expression<Func<PointOfSaleVoucherType, bool>>? predicate = null, 
            IEnumerable<string>? includesString = null, 
            bool disableTracking = true)
        {
            List<Func<IQueryable<PointOfSaleVoucherType>, IQueryable<PointOfSaleVoucherType>>> includes = [];

            if (includesString != null && includesString.Any())
            {
                foreach (var include in includesString)
                {
                    switch (include)
                    {
                        case "PointOfSale":
                            includes.Add(i => i
                                .Include(t => t.PointOfSale));
                            break;
                        case "VoucherType":
                            includes.Add(i => i
                                .Include(t => t.VoucherType));
                            break;
                    }
                }
            }
            else
            {
                includes.Add(i => i
                    .Include(t => t.PointOfSale)
                    .Include(t => t.VoucherType));
            }

            return await GetAllAsync(predicate, includes, disableTracking);
        }
    }
}
