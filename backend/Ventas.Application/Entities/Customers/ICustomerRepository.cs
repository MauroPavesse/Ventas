using System.Linq.Expressions;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Customers
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        public Task<IEnumerable<Customer>> SearchAsync(
            Expression<Func<Customer, bool>>? predicate = null,
            IEnumerable<string>? includesString = null,
            bool disableTracking = true);
    }
}
