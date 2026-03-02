using System.Linq.Expressions;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.PaymentMethods
{
    public interface IPaymentMethodRepository : IBaseRepository<PaymentMethod>
    {
        public Task<IEnumerable<PaymentMethod>> SearchAsync(
            Expression<Func<PaymentMethod, bool>>? predicate = null,
            IEnumerable<string>? includes = null,
            bool disableTracking = true);
    }
}
