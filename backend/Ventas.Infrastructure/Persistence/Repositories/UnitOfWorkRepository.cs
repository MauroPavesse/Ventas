using Ventas.Application.Entities.UnitOfWork;
using Ventas.Infrastructure.Data;

namespace Ventas.Infrastructure.Persistence.Repositories
{
    public class UnitOfWorkRepository : IUnitOfWorkRepository
    {
        private readonly AppDbContext _context;

        public UnitOfWorkRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void ClearContext()
        {
            _context.ChangeTracker.Clear();
        }
    }
}
