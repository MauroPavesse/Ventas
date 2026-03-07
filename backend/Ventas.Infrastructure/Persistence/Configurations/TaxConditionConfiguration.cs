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

            builder.HasData(
                new TaxCondition()
                {
                    Id = 1,
                    Code = 5,
                    Description = "CONSUMIDOR FINAL"
                },
                new TaxCondition()
                {
                    Id = 2,
                    Code = 1,
                    Description = "RESPONSABLE INSCRIPTO"
                },
                new TaxCondition()
                {
                    Id = 3,
                    Code = 4,
                    Description = "SUJETO EXENTO"
                },
                new TaxCondition()
                {
                    Id = 4,
                    Code = 6,
                    Description = "RESPONSABLE MONOTRIBUTO"
                },
                new TaxCondition()
                {
                    Id = 5,
                    Code = 7,
                    Description = "SUJETO NO CATEGORIZADO"
                }
            );
        }
    }
}
