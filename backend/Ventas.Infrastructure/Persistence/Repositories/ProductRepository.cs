using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ventas.Application.Entities.Products;
using Ventas.Domain.Entities;
using Ventas.Infrastructure.Data;

namespace Ventas.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> SearchAsync(
            Expression<Func<Product, bool>>? predicate = null, 
            IEnumerable<string>? includesString = null, 
            bool disableTracking = true)
        {
            List<Func<IQueryable<Product>, IQueryable<Product>>> includes = [];

            if (includesString != null && includesString.Any())
            {
                foreach (var include in includesString)
                {
                    switch (include)
                    {
                        case "Category":
                            includes.Add(i => i
                                .Include(t => t.Category));
                            break;
                        case "TaxRate":
                            includes.Add(i => i
                                .Include(t => t.TaxRate));
                            break;
                    }
                }
            }
            else
            {
                includes.Add(i => i
                    .Include(t => t.Category)
                    .Include(t => t.TaxRate));
            }

            return await GetAllAsync(predicate, includes, disableTracking);
        }
    }
}
