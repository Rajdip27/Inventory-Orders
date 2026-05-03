using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static InventoryOrders.Core.Entities.Auth.IdentityModel;

namespace InventoryOrders.Infrastructure.Configuration.AuthConfiguration;

public class UserRoleConfiguration: IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasData(new UserRole
        {
            RoleId = 1,
            UserId = 1,
        }, new UserRole
        {
            RoleId = 2,
            UserId = 2,
        }, new UserRole
        {
            RoleId = 3,
            UserId = 3,
        });
    }
}
