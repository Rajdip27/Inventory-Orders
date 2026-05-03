using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using InventoryOrders.Infrastructure.Data;
using InventoryOrders.Infrastructure.Healper.Acls;

namespace InventoryOrders.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Ensure logging services exist
        services.AddLogging();

        // Register DbContext
        services.AddDbContext<ApplicationDbContext>((sp, builder) =>
        {
            builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                   .ConfigureWarnings(warnings =>
                       warnings.Ignore(RelationalEventId.PendingModelChangesWarning));

            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            builder.UseLoggerFactory(loggerFactory);

            builder.LogTo(Console.WriteLine, LogLevel.Information);
        });

       

        // Scoped SignInHelper
        services.AddTransient<ISignInHelper, SignInHelper>();

        // Authorization
        services.AddAuthorizationCore(options =>
        {
            options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator"));
        });

        return services;
    }
}
