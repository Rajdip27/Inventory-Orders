namespace InventoryOrders.Web.Middlewares;

public class BlockDirectLoginMiddleware(RequestDelegate next, ILogger<BlockDirectLoginMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<BlockDirectLoginMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();

        if (path == "/account/login")
        {
            var referer = context.Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                _logger.LogWarning("Blocked direct access to login page from {IP}", context.Connection.RemoteIpAddress);
                context.Response.Redirect("/Home/AccessDenied");
                return;
            }
        }

        await _next(context);
    }
}
