using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ventas.Domain.Entities;

namespace Ventas.Infrastructure.Persistence.Configurations
{
    public class PointOfSaleVoucherTypeConfiguration : IEntityTypeConfiguration<PointOfSaleVoucherType>
    {
        public void Configure(EntityTypeBuilder<PointOfSaleVoucherType> builder)
        {
            
        }
    }
}
