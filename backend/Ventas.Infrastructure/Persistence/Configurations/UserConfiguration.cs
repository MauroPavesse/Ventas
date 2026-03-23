using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ventas.Domain.Entities;

namespace Ventas.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(t => t.Username)
                .HasMaxLength(50);

            builder.Property(t => t.Password)
                .HasMaxLength(150);

            builder.HasData(
                new User()
                {
                    Id = 1,
                    Username = "admin",
                    Password = "$2a$11$DTFShXgUa2qdWrsowwU41ue5BFG7MElT3pGWZFZkYCI9lBB2gxERG"
                }
            );
        }
    }
}
