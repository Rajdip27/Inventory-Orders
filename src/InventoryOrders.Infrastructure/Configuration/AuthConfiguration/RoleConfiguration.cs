using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static InventoryOrders.Core.Entities.Auth.IdentityModel;

namespace InventoryOrders.Infrastructure.Configuration.AuthConfiguration;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasData(new Role
        {
            Id = 1,
            Name = "Administrator",
            NormalizedName = "ADMINISTRATOR",

        }, new Role
        {
            Id = 2,
            Name = "User",
            NormalizedName = "USER"
        });
    }
}
