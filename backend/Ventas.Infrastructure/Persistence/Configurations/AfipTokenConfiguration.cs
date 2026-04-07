using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ventas.Domain.Entities;

namespace Ventas.Infrastructure.Persistence.Configurations
{
    public class AfipTokenConfiguration : IEntityTypeConfiguration<AfipToken>
    {
        public void Configure(EntityTypeBuilder<AfipToken> builder)
        {
            builder.Property(t => t.Token)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(t => t.Sign)
                .IsRequired()
                .HasMaxLength(255);
        }
    }
}
