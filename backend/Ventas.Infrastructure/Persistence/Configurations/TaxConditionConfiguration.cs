using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ventas.Domain.Entities;

namespace Ventas.Infrastructure.Persistence.Configurations
{
    public class TaxConditionConfiguration : IEntityTypeConfiguration<TaxCondition>
    {
        public void Configure(EntityTypeBuilder<TaxCondition> builder)
        {
            builder.Property(t => t.Description)
                .HasMaxLength(50);
        }
    }
}
