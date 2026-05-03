using System.Net;

namespace InventoryOrders.Web.Middlewares;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
            {
                _logger.LogWarning("404 Not Found: {Path}", context.Request.Path);
                context.Response.Redirect("/Home/PageNotFound");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception at {Path}", context.Request.Path);
            context.Response.Redirect("/Home/Error");
        }
    }
}
