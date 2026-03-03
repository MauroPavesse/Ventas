using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ventas.Domain.Entities;

namespace Ventas.Infrastructure.Persistence.Configurations
{
    public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.Property(t => t.Name)
                .HasMaxLength(70);

            builder.Property(t => t.DescountPercentage)
                .HasPrecision(6, 2);

            builder.Property(t => t.IncreasePercentage)
                .HasPrecision(6, 2);

            builder.Property(t => t.Color)
                .HasMaxLength(15);
        }
    }
}
