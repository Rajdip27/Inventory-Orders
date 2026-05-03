using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using InventoryOrders.Infrastructure.Healper.Acls;

public class SignInHelper : ISignInHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SignInHelper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long? UserId => long.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
    public string Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    public string Username => _httpContextAccessor.HttpContext?.User?.Identity?.Name;
    public List<string> Roles => _httpContextAccessor.HttpContext?.User?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList() ?? new();
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    public string AccessToken => _httpContextAccessor.HttpContext?.Request?.Headers["Authorization"].ToString()?.Split(' ')[1];
    public DateTimeOffset JwtExpiresAt => DateTimeOffset.UtcNow; // calculate from token if needed
    public string RequestOrigin => _httpContextAccessor.HttpContext?.Request?.Headers["Origin"].ToString();

    public string Fullname => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.GivenName);
    public string MobileNumber => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.MobilePhone);

}
