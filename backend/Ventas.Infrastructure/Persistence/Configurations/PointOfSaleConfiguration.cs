using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ventas.Domain.Entities;

namespace Ventas.Infrastructure.Persistence.Configurations
{
    public class PointOfSaleConfiguration : IEntityTypeConfiguration<PointOfSale>
    {
        public void Configure(EntityTypeBuilder<PointOfSale> builder)
        {
            builder.Property(t => t.Name)
                .HasMaxLength(50);

            builder.Property(t => t.Number)
                .HasMaxLength(10);

            builder.Property(t => t.Address)
                .HasMaxLength(70);

            builder.Property(t => t.City)
                .HasMaxLength(50);

            builder.Property(t => t.Provincie)
                .HasMaxLength(50);

            builder.Property(t => t.Name)
                .HasMaxLength(10);
        }
    }
}
