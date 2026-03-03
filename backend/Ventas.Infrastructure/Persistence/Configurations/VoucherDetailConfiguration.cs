using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ventas.Domain.Entities;

namespace Ventas.Infrastructure.Persistence.Configurations
{
    public class VoucherDetailConfiguration : IEntityTypeConfiguration<VoucherDetail>
    {
        public void Configure(EntityTypeBuilder<VoucherDetail> builder)
        {
            builder.Property(t => t.Quantity)
                .HasPrecision(10, 2);

            builder.Property(t => t.PriceUnit)
                .HasPrecision(18, 2);

            builder.Property(t => t.AmountNet)
                .HasPrecision(18, 2);

            builder.Property(t => t.Discount)
                .HasPrecision(18, 2);

            builder.Property(t => t.AmountFinal)
                .HasPrecision(18, 2);
        }
    }
}
