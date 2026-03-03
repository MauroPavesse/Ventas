using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ventas.Application.Entities.VoucherTypes;
using Ventas.Domain.Entities;
using Ventas.Infrastructure.Data;

namespace Ventas.Infrastructure.Persistence.Repositories
{
    public class VoucherTypeRepository : BaseRepository<VoucherType>, IVoucherTypeRepository
    {
        public VoucherTypeRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<VoucherType>> SearchAsync(
            Expression<Func<VoucherType, bool>>? predicate = null, 
            IEnumerable<string>? includesString = null, 
            bool disableTracking = true)
        {
            List<Func<IQueryable<VoucherType>, IQueryable<VoucherType>>> includes = [];

            if (includesString != null && includesString.Any())
            {
                foreach (var include in includesString)
                {
                    switch (include)
                    {
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
                    .Include(t => t.PointOfSaleVoucherTypes));
            }

            return await GetAllAsync(predicate, includes, disableTracking);
        }
    }
}
