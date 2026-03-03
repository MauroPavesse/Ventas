using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ventas.Application.Entities.Categories;
using Ventas.Domain.Entities;
using Ventas.Infrastructure.Data;

namespace Ventas.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> SearchAsync(
            Expression<Func<Category, bool>>? predicate = null, 
            IEnumerable<string>? includesString = null, 
            bool disableTracking = true)
        {
            List<Func<IQueryable<Category>, IQueryable<Category>>> includes = [];

            if (includesString != null && includesString.Any())
            {
                foreach (var include in includesString)
                {
                    switch (include)
                    {
                        case "Products":
                            includes.Add(i => i
                                .Include(t => t.Products));
                            break;
                    }
                }
            }
            else
            {
                includes.Add(i => i
                    .Include(t => t.Products));
            }

            return await GetAllAsync(predicate, includes, disableTracking);
        }
    }
}
