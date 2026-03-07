using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ventas.Domain.Entities;

namespace Ventas.Infrastructure.Persistence.Configurations
{
    public class TaxRateConfiguration : IEntityTypeConfiguration<TaxRate>
    {
        public void Configure(EntityTypeBuilder<TaxRate> builder)
        {
            builder.Property(t => t.Description)
                .HasMaxLength(50);

            builder.Property(t => t.Percentage)
                .HasPrecision(6, 2);

            builder.Property(t => t.Code)
                .HasMaxLength(5);

            builder.HasData(
                new TaxRate()
                {
                    Id = 1,
                    Description = "IVA 21%",
                    Percentage = 21,
                    Code = "0005"
                },
                new TaxRate()
                {
                    Id = 2,
                    Description = "IVA 10,5%",
                    Percentage = 10.5m,
                    Code = "0004"
                },
                new TaxRate()
                {
                    Id = 3,
                    Description = "IVA 0%",
                    Percentage = 0,
                    Code = "0003"
                },
                new TaxRate()
                {
                    Id = 4,
                    Description = "IVA 27%",
                    Percentage = 27,
                    Code = "0006"
                },
                new TaxRate()
                {
                    Id = 5,
                    Description = "IVA 5%",
                    Percentage = 5,
                    Code = "0008"
                },
                new TaxRate()
                {
                    Id = 6,
                    Description = "IVA 2,5%",
                    Percentage = 2.5m,
                    Code = "0009"
                }
            );
        }
    }
}
