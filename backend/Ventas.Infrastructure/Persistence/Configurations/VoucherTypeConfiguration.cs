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

            builder.HasData(
                new VoucherType()
                {
                    Id = 1,
                    Code = "OR",
                    Description = "ORDEN DE COMPRA"
                },
                new VoucherType()
                {
                    Id = 2,
                    Code = "001",
                    Description = "FACTURA A"
                },
                new VoucherType()
                {
                    Id = 3,
                    Code = "004",
                    Description = "RECIBO A"
                },
                new VoucherType()
                {
                    Id = 4,
                    Code = "006",
                    Description = "FACTURA B"
                },
                new VoucherType()
                {
                    Id = 5,
                    Code = "009",
                    Description = "RECIBO B"
                }
            );
        }
    }
}
