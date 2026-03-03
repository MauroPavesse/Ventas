using System.Linq.Expressions;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.VoucherPayments
{
    public interface IVoucherPaymentRepository : IBaseRepository<VoucherPayment>
    {
        public Task<IEnumerable<VoucherPayment>> SearchAsync(
            Expression<Func<VoucherPayment, bool>>? predicate = null,
            IEnumerable<string>? includesString = null,
            bool disableTracking = true);
    }
}
