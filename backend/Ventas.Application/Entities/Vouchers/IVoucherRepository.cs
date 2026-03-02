using System.Linq.Expressions;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Vouchers
{
    public interface IVoucherRepository : IBaseRepository<Voucher>
    {
        public Task<IEnumerable<Voucher>> SearchAsync(
            Expression<Func<Voucher, bool>>? predicate = null,
            IEnumerable<string>? includes = null,
            bool disableTracking = true);
    }
}
