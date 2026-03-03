using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ventas.Domain.Entities;

namespace Ventas.Infrastructure.Persistence.Configurations
{
    public class DailyBoxConfiguration : IEntityTypeConfiguration<DailyBox>
    {
        public void Configure(EntityTypeBuilder<DailyBox> builder)
        {
            builder.Property(t => t.Amount)
                .HasPrecision(18, 2);
        }
    }
}
