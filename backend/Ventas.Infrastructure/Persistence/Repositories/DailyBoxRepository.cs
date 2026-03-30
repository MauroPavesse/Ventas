using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ventas.Application.Entities.DailyBoxes;
using Ventas.Domain.Entities;
using Ventas.Infrastructure.Data;

namespace Ventas.Infrastructure.Persistence.Repositories
{
    public class DailyBoxRepository : BaseRepository<DailyBox>, IDailyBoxRepository
    {
        public DailyBoxRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DailyBox>> SearchAsync(
            Expression<Func<DailyBox, bool>>? predicate = null, 
            IEnumerable<string>? includesString = null, 
            bool disableTracking = true)
        {
            List<Func<IQueryable<DailyBox>, IQueryable<DailyBox>>> includes = [];

            if (includesString != null && includesString.Any())
            {
                foreach (var include in includesString)
                {
                    switch (include)
                    {
                        case "Vouchers":
                            includes.Add(i => i
                                .Include(t => t.Vouchers));
                            break;
                        case "Vouchers.VoucherDetails":
                            includes.Add(i => i
                                .Include(t => t.Vouchers)
                                .ThenInclude(t => t.VoucherDetails));
                            break;
                        case "Voucher.User.PointOfSale":
                            includes.Add(i => i
                                .Include(t => t.Vouchers)
                                .ThenInclude(t => t.User!.PointOfSale));
                            break;
                    }
                }
            }
            else
            {
                includes.Add(i => i
                    .Include(t => t.Vouchers)
                    .ThenInclude(t => t.VoucherDetails)
                    .Include(t => t.Vouchers)
                    .ThenInclude(t => t.User!.PointOfSale)
                    .Include(t => t.Vouchers)
                    .ThenInclude(t => t.VoucherType));
            }

            return await GetAllAsync(predicate, includes, disableTracking);
        }
    }
}
