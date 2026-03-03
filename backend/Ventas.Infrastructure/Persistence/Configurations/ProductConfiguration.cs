using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ventas.Domain.Entities;

namespace Ventas.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(t => t.Name)
                .HasMaxLength(70);

            builder.Property(t => t.Description)
                .HasMaxLength(200);

            builder.Property(t => t.ImagePath)
                .HasMaxLength(200);

            builder.Property(t => t.Price)
                .HasPrecision(18, 2);

            builder.Property(t => t.CodeBar)
                .HasMaxLength(40);
        }
    }
}
