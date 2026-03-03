using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ventas.Domain.Entities;

namespace Ventas.Infrastructure.Persistence.Configurations
{
    public class VoucherConfiguration : IEntityTypeConfiguration<Voucher>
    {
        public void Configure(EntityTypeBuilder<Voucher> builder)
        {
            builder.Property(t => t.AmountNet)
                .HasPrecision(18, 2);

            builder.Property(t => t.AmountVAT)
                .HasPrecision(18, 2);

            builder.Property(t => t.CAE)
                .HasMaxLength(50);
        }
    }
}
