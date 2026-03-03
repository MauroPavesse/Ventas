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
        }
    }
}
