using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ventas.Domain.Entities;

namespace Ventas.Infrastructure.Persistence.Configurations
{
    public class ConfigurationConfiguration : IEntityTypeConfiguration<Configuration>
    {
        public void Configure(EntityTypeBuilder<Configuration> builder)
        {
            builder.Property(t => t.Variable)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Description)
                .HasMaxLength(150);

            builder.Property(t => t.StringValue)
                .HasColumnType("text");

            builder.Property(t => t.NumericValue)
                .HasPrecision(10, 2);

            builder.HasData(
                new Configuration
                {
                    Id = 1,
                    Variable = "M&MID",
                    Description = "Identificador del sistema",
                    StringValue = "TEST",
                    NumericValue = 0,
                    BoolValue = false
                },
                new Configuration
                {
                    Id = 2,
                    Variable = "empresa",
                    Description = "Nombre de la empresa",
                    StringValue = "",
                    NumericValue = 0,
                    BoolValue = false
                },
                new Configuration
                {
                    Id = 3,
                    Variable = "fechaInicio",
                    Description = "Fecha inicio de la empresa",
                    StringValue = "",
                    NumericValue = 0,
                    BoolValue = false
                },
                new Configuration
                {
                    Id = 4,
                    Variable = "cuit",
                    Description = "CUIT / CUIL de la empresa",
                    StringValue = "",
                    NumericValue = 0,
                    BoolValue = false
                },
                new Configuration
                {
                    Id = 5,
                    Variable = "condicionFiscalId",
                    Description = "Condición fiscal de la empresa",
                    StringValue = "",
                    NumericValue = 0,
                    BoolValue = false
                },
                new Configuration
                {
                    Id = 6,
                    Variable = "arcaAlias",
                    Description = "Alias de ARCA",
                    StringValue = "",
                    NumericValue = 0,
                    BoolValue = false
                },
                new Configuration
                {
                    Id = 7,
                    Variable = "arcaCertificado",
                    Description = "Dirección certificado de ARCA",
                    StringValue = "",
                    NumericValue = 0,
                    BoolValue = false
                },
                new Configuration
                {
                    Id = 8,
                    Variable = "arcaClave",
                    Description = "Clave del certificado de ARCA",
                    StringValue = "",
                    NumericValue = 0,
                    BoolValue = false
                },
                new Configuration
                {
                    Id = 9,
                    Variable = "tokenSystem",
                    Description = "Token del sistema",
                    StringValue = "",
                    NumericValue = 0,
                    BoolValue = false
                },
                new Configuration
                {
                    Id = 10,
                    Variable = "cajaDiariaNumero",
                    Description = "Último número usado en la caja diaria",
                    StringValue = "",
                    NumericValue = 0,
                    BoolValue = false
                },
                new Configuration
                {
                    Id = 11,
                    Variable = "imprimeTicketDirecto",
                    Description = "true: El ticket imprime directo / false: Muestra pantalla previa",
                    StringValue = "",
                    NumericValue = 0,
                    BoolValue = false
                }
            );
        }
    }
}
