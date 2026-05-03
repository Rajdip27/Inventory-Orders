using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static InventoryOrders.Core.Entities.Auth.IdentityModel;

namespace InventoryOrders.Infrastructure.Configuration.AuthConfiguration;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        var hasher = new PasswordHasher<User>();
        builder.HasData(new User
        {
            Id = 1,
            Email = "admin@localhost.com",
            NormalizedEmail = "ADMIN@LOCALHOST.COM",
            UserName = "admin@localhost.com",
            NormalizedUserName = "ADMIN@LOCALHOST.COM",
            PasswordHash = hasher.HashPassword(null, "P@ssword1"),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        }, new User
        {
            Id = 2,
            Email = "eventmanager@localhost.com",
            NormalizedEmail = "EVENTMANAGER@LOCALHOST.COM",
            UserName = "eventmanager@localhost.com",
            NormalizedUserName = "EVENTMANAGER@LOCALHOST.COM",
            PasswordHash = hasher.HashPassword(null, "P@ssword1"),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        }, new User
        {
            Id = 3,
            Email = "student@localhost.com",
            NormalizedEmail = "STUDENT@LOCALHOST.COM",
            UserName = "student@localhost.com",
            NormalizedUserName = "STUDENT@LOCALHOST.COM",
            PasswordHash = hasher.HashPassword(null, "P@ssword1"),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        });
    }
}
