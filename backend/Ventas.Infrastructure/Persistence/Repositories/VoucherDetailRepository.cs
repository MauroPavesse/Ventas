using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ventas.Application.Entities.VoucherDetails;
using Ventas.Domain.Entities;
using Ventas.Infrastructure.Data;

namespace Ventas.Infrastructure.Persistence.Repositories
{
    public class VoucherDetailRepository : BaseRepository<VoucherDetail>, IVoucherDetailRepository
    {
        public VoucherDetailRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<VoucherDetail>> SearchAsync(
            Expression<Func<VoucherDetail, bool>>? predicate = null, 
            IEnumerable<string>? includesString = null, 
            bool disableTracking = true)
        {
            List<Func<IQueryable<VoucherDetail>, IQueryable<VoucherDetail>>> includes = [];

            if (includesString != null && includesString.Any())
            {
                foreach (var include in includesString)
                {
                    switch (include)
                    {
                        case "Voucher":
                            includes.Add(i => i
                                .Include(t => t.Voucher));
                            break;
                        case "Product":
                            includes.Add(i => i
                                .Include(t => t.Product));
                            break;
                    }
                }
            }
            else
            {
                includes.Add(i => i
                    .Include(t => t.Voucher)
                    .Include(t => t.Product));
            }

            return await GetAllAsync(predicate, includes, disableTracking);
        }
    }
}
