using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.AfipTokens
{
    public interface IAfipTokenRepository : IBaseRepository<AfipToken>
    {
        public Task<AfipToken?> GetLatest(int pointOfSaleId);
    }
}
