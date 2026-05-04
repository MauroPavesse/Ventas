using Microsoft.EntityFrameworkCore;
using Ventas.Domain.Others;

namespace Ventas.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ITenantService _tenantService;

        public AppDbContext(DbContextOptions options, ITenantService tenantService) : base(options)
        {
            _tenantService = tenantService;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Si la cadena está vacía (como en el arranque), usamos una temporal 
            // o simplemente no configuramos nada para que no explote el constructor.
            var connString = _tenantService.ConnectionString;

            if (!string.IsNullOrEmpty(connString))
            {
                optionsBuilder.UseSqlServer(connString);
            }
            else
            {
                // Esto evita el error de "ConnectionString not initialized" durante el arranque del SDK
                optionsBuilder.UseSqlServer("Server=sql_server_shared;Database=dummy;User Id=sa;Password=tu_pass;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
