using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ventas.Domain.Entities;

namespace Ventas.Infrastructure.Persistence.Configurations
{
    public class VoucherTypeConfiguration : IEntityTypeConfiguration<VoucherType>
    {
        public void Configure(EntityTypeBuilder<VoucherType> builder)
        {
            builder.Property(t => t.Code)
                .HasMaxLength(5);

            builder.Property(t => t.Description)
                .HasMaxLength(30);
        }
    }
}
