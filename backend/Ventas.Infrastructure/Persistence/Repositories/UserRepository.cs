using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Ventas.Application.Entities.Users;
using Ventas.Domain.Entities;
using Ventas.Infrastructure.Data;

namespace Ventas.Infrastructure.Persistence.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<User>> SearchAsync(
            Expression<Func<User, bool>>? predicate = null, 
            IEnumerable<string>? includesString = null, 
            bool disableTracking = true)
        {
            List<Func<IQueryable<User>, IQueryable<User>>> includes = [];

            if (includesString != null && includesString.Any())
            {
                foreach (var include in includesString)
                {
                    switch (include)
                    {
                        case "Role":
                            includes.Add(i => i
                                .Include(t => t.Role));
                            break;
                        case "PointOfSale":
                            includes.Add(i => i
                                .Include(t => t.PointOfSale));
                            break;
                    }
                }
            }
            else
            {
                includes.Add(i => i
                    .Include(t => t.Role)
                    .Include(t => t.PointOfSale));
            }

            return await GetAllAsync(predicate, includes, disableTracking);
        }
    }
}
