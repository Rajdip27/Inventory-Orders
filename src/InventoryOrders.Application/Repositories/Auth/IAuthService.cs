using Microsoft.AspNetCore.Identity;
using InventoryOrders.Application.CommonModel;
using InventoryOrders.Application.Services;
using InventoryOrders.Application.ViewModel.Auth;
using static InventoryOrders.Core.Entities.Auth.IdentityModel;

namespace InventoryOrders.Application.Repositories.Auth;

public interface IAuthService
{
    Task<RegistrationResponse> Register(RegisterViewModel model);

}

public class AuthService( UserManager<User> _userManager) : IAuthService
{
    public async Task<RegistrationResponse> Register(RegisterViewModel request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new RegistrationResponse
            {
                Success = false,
                Errors = new() { $"Email '{request.Email}' is already registered." }
            };
        }

        var user = new User
        {
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber,
            FullName = request.FullName,
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };
        
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return new RegistrationResponse
            {
                Success = false,
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        await _userManager.AddToRoleAsync(user, "User");

        return new RegistrationResponse
        {
            Success = true,
            UserId = user.Id
        };
    }
}


