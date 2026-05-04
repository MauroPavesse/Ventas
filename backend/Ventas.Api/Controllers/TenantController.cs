using Microsoft.AspNetCore.Mvc;
using Ventas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ventas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TenantController> _logger;

        public TenantController(AppDbContext context, ILogger<TenantController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("setup-database")]
        public async Task<IActionResult> SetupDatabase()
        {
            try
            {
                _logger.LogInformation("Iniciando migración para el tenant actual...");

                // Este comando hace la magia: 
                // 1. Crea la base de datos si no existe.
                // 2. Crea la tabla __EFMigrationsHistory.
                // 3. Aplica todas las migraciones pendientes (tablas, índices, etc.)
                await _context.Database.MigrateAsync();

                return Ok(new { message = "Base de datos creada y actualizada correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la base de datos");
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}