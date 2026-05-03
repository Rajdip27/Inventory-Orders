using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using InventoryOrders.Core.Entities.EntityLogs;
using InventoryOrders.Infrastructure.Data;
using InventoryOrders.Infrastructure.Healper.Acls;

namespace InventoryOrders.Web.Middlewares
{
    public class RouteLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RouteLoggingMiddleware> _logger;
        private readonly IServiceProvider _services;
        private readonly ISignInHelper _signInHelper;

        public RouteLoggingMiddleware(RequestDelegate next, ILogger<RouteLoggingMiddleware> logger, IServiceProvider services, ISignInHelper signInHelper)
        {
            _next = next;
            _logger = logger;
            _services = services;
            _signInHelper = signInHelper;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var rd = context.GetRouteData();
                var controller = rd.Values["controller"]?.ToString();
                var action = rd.Values["action"]?.ToString();

                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Logout handling
                if (controller?.Equals("Account", StringComparison.OrdinalIgnoreCase) == true &&
                    action?.Equals("Logout", StringComparison.OrdinalIgnoreCase) == true &&
                    _signInHelper.UserId != null)
                {
                    var last = db.RouteLogs
                        .Where(l => l.UserId == _signInHelper.UserId.ToString() && l.LoginStatus == "LoggedIn")
                        .OrderByDescending(l => l.CreatedDate)
                        .FirstOrDefault();

                    if (last != null)
                    {
                        last.LoggedOutDateTimeUtc = DateTime.UtcNow.ToString("u");
                        last.LoginStatus = "LoggedOut";
                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    // Normal route logging
                    db.RouteLogs.Add(new RouteLog
                    {
                        Area = rd.Values["area"]?.ToString(),
                        ControllerName = controller,
                        ActionName = action,
                        RoleId = _signInHelper.Roles.Any() ? string.Join(",", _signInHelper.Roles) : "Anonymous",
                        UserId = _signInHelper.UserId?.ToString() ?? "Guest",
                        IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                        PageAccessed = context.Request.Path,
                        UrlReferrer = context.Request.Headers["Referer"].ToString(),
                        SessionId = context.Session?.Id,
                        LoginStatus = _signInHelper.IsAuthenticated ? "LoggedIn" : "Guest",
                        LoggedInDateTimeUtc = _signInHelper.IsAuthenticated ? DateTime.UtcNow.ToString("u") : null
                    });
                    await db.SaveChangesAsync();
                }
            }
            catch { }

            await _next(context);
        }
    }

    
}
