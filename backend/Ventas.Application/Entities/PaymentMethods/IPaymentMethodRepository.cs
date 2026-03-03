using System.Linq.Expressions;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.PaymentMethods
{
    public interface IPaymentMethodRepository : IBaseRepository<PaymentMethod>
    {
        public Task<IEnumerable<PaymentMethod>> SearchAsync(
            Expression<Func<PaymentMethod, bool>>? predicate = null,
            IEnumerable<string>? includesString = null,
            bool disableTracking = true);
    }
}
