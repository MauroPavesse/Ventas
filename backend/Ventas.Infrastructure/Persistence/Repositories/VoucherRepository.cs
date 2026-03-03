using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ventas.Application.Entities.Vouchers;
using Ventas.Domain.Entities;
using Ventas.Infrastructure.Data;

namespace Ventas.Infrastructure.Persistence.Repositories
{
    public class VoucherRepository : BaseRepository<Voucher>, IVoucherRepository
    {
        public VoucherRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Voucher>> SearchAsync(
            Expression<Func<Voucher, bool>>? predicate = null, 
            IEnumerable<string>? includesString = null, 
            bool disableTracking = true)
        {
            List<Func<IQueryable<Voucher>, IQueryable<Voucher>>> includes = [];

            if (includesString != null && includesString.Any())
            {
                foreach (var include in includesString)
                {
                    switch (include)
                    {
                        case "User":
                            includes.Add(i => i
                                .Include(t => t.User));
                            break;
                        case "Customer":
                            includes.Add(i => i
                                .Include(t => t.Customer));
                            break;
                        case "VoucherType":
                            includes.Add(i => i
                                .Include(t => t.VoucherType));
                            break;
                        case "DailyBox":
                            includes.Add(i => i
                                .Include(t => t.DailyBox));
                            break;
                        case "StateEntity":
                            includes.Add(i => i
                                .Include(t => t.StateEntity));
                            break;
                        case "VoucherDetails":
                            includes.Add(i => i
                                .Include(t => t.VoucherDetails));
                            break;
                        case "VoucherPayments":
                            includes.Add(i => i
                                .Include(t => t.VoucherPayments));
                            break;
                    }
                }
            }
            else
            {
                includes.Add(i => i
                    .Include(t => t.User)
                    .Include(t => t.Customer)
                    .Include(t => t.VoucherType)
                    .Include(t => t.DailyBox)
                    .Include(t => t.StateEntity)
                    .Include(t => t.VoucherDetails)
                    .Include(t => t.VoucherPayments));
            }

            return await GetAllAsync(predicate, includes, disableTracking);
        }
    }
}
