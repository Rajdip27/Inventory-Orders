using InventoryOrders.Application.Logging;
using InventoryOrders.Application.Repositories.Auth;
using InventoryOrders.Application.ViewModel.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static InventoryOrders.Core.Entities.Auth.IdentityModel;

namespace InventoryOrders.Web.Controllers.Auth;

public class AccountController(
    SignInManager<User> _signInManager,
    IAppLogger<AccountController> _logger,
    IAuthService _authService) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl)
    {
        _logger.LogInfo($"Login page accessed. ReturnUrl: {returnUrl}");
        return View(new LoginViewModel { ReturnUrl = returnUrl ?? "/" });
    }
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Invalid login details.";
            TempData["AlertType"] = "error";
            _logger.LogWarning($"Login attempt failed due to invalid model state. Email: {model.Email}");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

        if (result.Succeeded)
        {
            var user = await _signInManager.UserManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                TempData["AlertMessage"] = "Login successful!";
                TempData["AlertType"] = "success";
                _logger.LogInfo($"User {model.Email} logged in successfully.");
                return RedirectToAction("Index", "Dashboard");
            }
        }

        TempData["AlertMessage"] = "Invalid email or password.";
        TempData["AlertType"] = "error";
        _logger.LogWarning($"Invalid login attempt for email: {model.Email}");
        return View(model);
    }
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["AlertMessage"] = "Please fill all required fields correctly.";
            TempData["AlertType"] = "error";
            _logger.LogWarning("Invalid registration model state.");
            return View(model);
        }
        _logger.LogInfo($"Registration attempt started for Email: {model.Email}");
        var result = await _authService.Register(model);

        if (!result.Success)
        {
            result.Errors.ForEach(e => ModelState.AddModelError("", e));
            TempData["AlertMessage"] = string.Join(", ", result.Errors);
            TempData["AlertType"] = "error";
            _logger.LogWarning($"Registration failed for Email: {model.Email}. Errors: {string.Join(", ", result.Errors)}");
            return View(model);
        }
        var user = await _signInManager.UserManager.FindByIdAsync(result.UserId.ToString());
        if (user != null)
        {
            await _signInManager.SignInAsync(user, false);
            TempData["AlertMessage"] = "Registration successful!";
            TempData["AlertType"] = "success";
            _logger.LogInfo($"User {user.Email} registered and logged in successfully.");
            var roles = await _signInManager.UserManager.GetRolesAsync(user);
            if (roles.Contains("Administrator")) return LocalRedirect("/Dashboard");
            if (roles.Contains("Student")) return LocalRedirect("/Home");

            return LocalRedirect("/");
        }

        TempData["AlertMessage"] = "User created but failed to log in.";
        TempData["AlertType"] = "warning";
        _logger.LogWarning($"User created but failed to log in. Email: {model.Email}");
        return LocalRedirect("/");
    }
   

   
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        TempData["AlertMessage"] = "Logged out successfully.";
        TempData["AlertType"] = "info";
        return RedirectToAction("Index", "Home");
    }
}
