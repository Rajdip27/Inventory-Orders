using InventoryOrders.Application.Imports;
using InventoryOrders.Application.Logging;
using InventoryOrders.Application.Repositories;
using InventoryOrders.Application.Repositories.Auth;
using InventoryOrders.Application.Services;
using InventoryOrders.Application.Services.Pdf;
using InventoryOrders.Infrastructure.Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryOrders.Application;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();
        services.AddScoped<IResetPasswordService, ResetPasswordService>();
        services.AddScoped<IPdfService, PdfService>();
        services.AddScoped<IExcelImportService, ExcelImportService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
    

    }
}
