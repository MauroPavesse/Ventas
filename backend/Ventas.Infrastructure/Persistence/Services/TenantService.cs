using Ventas.Domain.Others;

namespace Ventas.Infrastructure.Persistence.Services
{
    public class TenantService : ITenantService
    {
        public string? TenantId { get; set; }
        public string? ConnectionString { get; set; }
    }
}
