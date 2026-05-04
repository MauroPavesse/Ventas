using Ventas.Domain.Others;

namespace Ventas.Api.Middlewares
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
        {
            var host = context.Request.Host.Host;

            if (host == "localhost" || host == "127.0.0.1")
            {
                tenantService.TenantId = "dev_test";
                tenantService.ConnectionString = "Server=localhost\\SQLEXPRESS;Database=ventasDb;Trusted_Connection=True;TrustServerCertificate=True;";
            }
            else
            {
                var partes = host.Split('.');
                var subdominio = partes.Length > 1 ? partes[0] : "default";

                tenantService.TenantId = subdominio;

                tenantService.ConnectionString = $"Server=sql_server_shared;Database=Ventas_{subdominio};User Id=sa;Password=43041735Mau#;TrustServerCertificate=True;";

            }

            await _next(context);
        }
    }
}
