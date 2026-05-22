using Ventas.Application.Entities.Externas.Licences;

namespace Ventas.Api.Middlewares
{
    public class LicenseCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public LicenseCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILicenseService licenseService)
        {
            var host = context.Request.Host.Host;

            if (host == "localhost" || host == "127.0.0.1")
            {
                await _next(context);
                return;
            }

            var partes = host.Split('.');
            var subdominio = partes.Length > 1 ? partes[0] : "default";

            // Excluir rutas que no requieren licencia (por ejemplo: webhooks de pago, login del admin central, etc.)
            if (context.Request.Path.StartsWithSegments("/api/admin-central"))
            {
                await _next(context);
                return;
            }

            // 3. Validar la licencia
            bool hasValidLicense = await licenseService.IsLicenseValidAsync(subdominio);

            if (!hasValidLicense)
            {
                // Si no tiene licencia paga o expiró, cortamos la petición acá
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { error = "Su suscripción ha expirado o el subdominio no es válido." });
                return;
            }

            // Si está todo bien, continúa el flujo normal hacia los Controllers de Ventas
            await _next(context);
        }
    }
}
