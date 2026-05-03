using InventoryOrders.Application;
using InventoryOrders.Application.Mappings;
using InventoryOrders.Infrastructure;
using InventoryOrders.Infrastructure.Data;
using InventoryOrders.Web.Logging;
using InventoryOrders.Web.Middlewares;
using InventoryOrders.Web.Pdf;
using Microsoft.AspNetCore.Identity;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using static InventoryOrders.Core.Entities.Auth.IdentityModel;
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    EnvironmentName = Environments.Development
});
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.With<TraceIdEnricher>()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

builder.Host.UseSerilog(); 
builder.Services.AddDinkToPdf(builder.Environment.ContentRootPath);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllersWithViews();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault().AddService("UniversityEventsMVC"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter();
    });

// Register Identity
builder.Services.AddIdentity<User, Role>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddHttpContextAccessor();

// Mapster
MapsterConfig.RegisterMappings();

// Distributed cache + session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// HttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseMiddleware<ErrorHandlingMiddleware>(); 
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); 
DateTimeExtensions.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<BlockDirectLoginMiddleware>();
app.UseMiddleware<RouteLoggingMiddleware>();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);
app.Run();