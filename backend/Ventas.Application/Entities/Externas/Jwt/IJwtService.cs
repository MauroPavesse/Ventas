using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Externas.Jwt
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
