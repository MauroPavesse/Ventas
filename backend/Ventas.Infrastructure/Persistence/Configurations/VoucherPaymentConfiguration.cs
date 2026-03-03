using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ventas.Domain.Entities;

namespace Ventas.Infrastructure.Persistence.Configurations
{
    public class VoucherPaymentConfiguration : IEntityTypeConfiguration<VoucherPayment>
    {
        public void Configure(EntityTypeBuilder<VoucherPayment> builder)
        {
            builder.Property(t => t.Amount)
                .HasPrecision(18, 2);
        }
    }
}
