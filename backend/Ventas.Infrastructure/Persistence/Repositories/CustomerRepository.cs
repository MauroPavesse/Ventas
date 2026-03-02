using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ventas.Application.Entities.Customers;
using Ventas.Domain.Entities;
using Ventas.Infrastructure.Data;

namespace Ventas.Infrastructure.Persistence.Repositories
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Customer>> SearchAsync(
            Expression<Func<Customer, bool>>? predicate = null, 
            IEnumerable<string>? includesString = null, 
            bool disableTracking = true)
        {
            List<Func<IQueryable<Customer>, IQueryable<Customer>>> includes = [];

            if (includesString != null && includesString.Any())
            {
                foreach(var include in includesString)
                {
                    switch (include)
                    {
                        case "TaxCondition":
                            includes.Add(i => i
                                .Include(t => t.TaxCondition));
                            break;
                    }
                }
            }
            else
            {
                includes.Add(i => i
                    .Include(t => t.TaxCondition));
            }

            return await GetAllAsync(predicate, includes, disableTracking);
        }
    }
}
