using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;
using Ventas.Application.Entities.Externas.Licences;

namespace Ventas.Infrastructure.Persistence.Services
{
    public record LicenseValidationResponse(bool IsActive);

    public class LicenseService : ILicenseService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        public LicenseService(HttpClient httpClient, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<bool> IsLicenseValidAsync(string domain)
        {
            string cacheKey = $"lic_{domain}";

            // 1. Si ya lo validamos hace poco, lo tomamos de la memoria de la VPS (Rápido)
            if (_cache.TryGetValue(cacheKey, out bool isValid))
            {
                return isValid;
            }

            try
            {
                // 2. Si no está en caché, le preguntamos a la API de tu sistema de Licencias
                // Apunta al endpoint de tu backend de licencias (ej: /api/subscriptions/validate)
                var response = await _httpClient.GetAsync($"api/subscriptions/validate?domain={domain}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LicenseValidationResponse>();
                    bool isConfiguredValid = result?.IsActive ?? false;

                    // Guardamos en caché por 15 minutos para no saturar el servidor
                    _cache.Set(cacheKey, isConfiguredValid, TimeSpan.FromMinutes(15));
                    return isConfiguredValid;
                }
            }
            catch
            {
                // Si el sistema de licencias se cae por alguna razón, podés decidir si dejar pasar al cliente o no.
                return false;
            }

            return false;
        }
    }
}
