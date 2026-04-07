using Ventas.Application.Entities.AfipTokens.DTOs;
using Ventas.Application.Entities.Externas.Afip.DTOs;

namespace Ventas.Application.Entities.Externas.Afip
{
    public interface IAfipAuthService
    {
        public Task<AfipResultOutput<AfipTokenOutput>> GetToken(string pathCert, string password);
    }
}
