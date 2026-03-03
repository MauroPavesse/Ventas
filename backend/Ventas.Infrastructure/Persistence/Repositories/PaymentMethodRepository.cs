using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ventas.Application.Entities.PaymentMethods;
using Ventas.Domain.Entities;
using Ventas.Infrastructure.Data;

namespace Ventas.Infrastructure.Persistence.Repositories
{
    public class PaymentMethodRepository : BaseRepository<PaymentMethod>, IPaymentMethodRepository
    {
        public PaymentMethodRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PaymentMethod>> SearchAsync(
            Expression<Func<PaymentMethod, bool>>? predicate = null, 
            IEnumerable<string>? includesString = null, 
            bool disableTracking = true)
        {
            List<Func<IQueryable<PaymentMethod>, IQueryable<PaymentMethod>>> includes = [];

            if (includesString != null && includesString.Any())
            {
                foreach (var include in includesString)
                {
                    switch (include)
                    {
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
                    .Include(t => t.VoucherPayments));
            }

            return await GetAllAsync(predicate, includes, disableTracking);
        }
    }
}
