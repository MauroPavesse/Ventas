using System.Linq.Expressions;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.VoucherTypes
{
    public interface IVoucherTypeRepository : IBaseRepository<VoucherType>
    {
        public Task<IEnumerable<VoucherType>> SearchAsync(
            Expression<Func<VoucherType, bool>>? predicate = null,
            IEnumerable<string>? includesString = null,
            bool disableTracking = true);
    }
}
