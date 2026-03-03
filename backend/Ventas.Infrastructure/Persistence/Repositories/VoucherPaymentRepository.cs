using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ventas.Application.Entities.VoucherPayments;
using Ventas.Domain.Entities;
using Ventas.Infrastructure.Data;

namespace Ventas.Infrastructure.Persistence.Repositories
{
    public class VoucherPaymentRepository : BaseRepository<VoucherPayment>, IVoucherPaymentRepository
    {
        public VoucherPaymentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<VoucherPayment>> SearchAsync(
            Expression<Func<VoucherPayment, bool>>? predicate = null, 
            IEnumerable<string>? includesString = null, 
            bool disableTracking = true)
        {
            List<Func<IQueryable<VoucherPayment>, IQueryable<VoucherPayment>>> includes = [];

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
                        case "PaymentMethod":
                            includes.Add(i => i
                                .Include(t => t.PaymentMethod));
                            break;
                    }
                }
            }
            else
            {
                includes.Add(i => i
                    .Include(t => t.Voucher)
                    .Include(t => t.PaymentMethod));
            }

            return await GetAllAsync(predicate, includes, disableTracking);
        }
    }
}
