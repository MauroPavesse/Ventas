using Microsoft.EntityFrameworkCore;
using Ventas.Application.Entities.AfipTokens;
using Ventas.Domain.Entities;
using Ventas.Infrastructure.Data;

namespace Ventas.Infrastructure.Persistence.Repositories
{
    public class AfipTokenRepository : BaseRepository<AfipToken>, IAfipTokenRepository
    {
        public AfipTokenRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private readonly AppDbContext _context;

        public async Task<AfipToken?> GetLatest(int pointOfSaleId)
        {
            return await _context.Set<AfipToken>()
                .Where(x => x.PointOfSaleId == pointOfSaleId)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
        }
    }
}
