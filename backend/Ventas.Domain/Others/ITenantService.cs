namespace Ventas.Domain.Others
{
    public interface ITenantService
    {
        string? TenantId { get; set; }
        string? ConnectionString { get; set; }
    }
}
