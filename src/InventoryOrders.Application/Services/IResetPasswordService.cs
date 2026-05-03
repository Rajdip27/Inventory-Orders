using Microsoft.AspNetCore.Identity;
using static InventoryOrders.Core.Entities.Auth.IdentityModel;
namespace InventoryOrders.Application.Services;

public interface IResetPasswordService
{
    Task ResetPasswordAsync(string email, string newPassword);
}
public sealed class ResetPasswordService : IResetPasswordService
{
    private readonly UserManager<User> _userManager;

    public ResetPasswordService(UserManager<User> userManager)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task ResetPasswordAsync(string email, string newPassword)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("New password is required.", nameof(newPassword));

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Password reset failed: {errors}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to reset password for {email}. {ex.Message}", ex);
        }
    }
}
