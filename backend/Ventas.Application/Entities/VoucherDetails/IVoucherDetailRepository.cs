using System.Linq.Expressions;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.VoucherDetails
{
    public interface IVoucherDetailRepository : IBaseRepository<VoucherDetail>
    {
        public Task<IEnumerable<VoucherDetail>> SearchAsync(
            Expression<Func<VoucherDetail, bool>>? predicate = null,
            IEnumerable<string>? includesString = null,
            bool disableTracking = true);
    }
}
