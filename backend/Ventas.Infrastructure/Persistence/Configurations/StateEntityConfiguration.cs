using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ventas.Domain.Entities;
using Ventas.Domain.Enums;

namespace Ventas.Infrastructure.Persistence.Configurations
{
    public class StateEntityConfiguration : IEntityTypeConfiguration<StateEntity>
    {
        public void Configure(EntityTypeBuilder<StateEntity> builder)
        {
            builder.Property(t => t.State)
                .HasMaxLength(50);

            builder.HasData(
                new StateEntity()
                {
                    Id = 1,
                    State = "INICIADO",
                    EntityId = (int)EntityEnum.VOUCHER
                },
                new StateEntity()
                {
                    Id = 2,
                    State = "ES ESPERA",
                    EntityId = (int)EntityEnum.VOUCHER
                },
                new StateEntity()
                {
                    Id = 3,
                    State = "FINALIZADO",
                    EntityId = (int)EntityEnum.VOUCHER
                },
                new StateEntity()
                {
                    Id = 4,
                    State = "CANCELADO",
                    EntityId = (int)EntityEnum.VOUCHER
                }
            );
        }
    }
}
