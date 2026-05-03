

using Microsoft.AspNetCore.Identity;
using static InventoryOrders.Core.Entities.Auth.IdentityModel;

namespace InventoryOrders.Application.Helpers;

public static class RoleHelperById
{
    public static async Task<List<long>> GetAllUserIdsByRolesAsync(UserManager<User> userManager, List<string> roles)
    {
        var allUsers = new List<User>();

        foreach (var role in roles)
        {
            var usersInRole = await userManager.GetUsersInRoleAsync(role);
            allUsers.AddRange(usersInRole);
        }
        // Remove duplicates if a user is in multiple roles
        var uniqueUserIds = allUsers.Select(u => u.Id).Distinct().ToList();

        return uniqueUserIds;
    }
}
