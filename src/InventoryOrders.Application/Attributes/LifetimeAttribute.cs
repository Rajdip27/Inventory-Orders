using Microsoft.Extensions.DependencyInjection;

namespace InventoryOrders.Application.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class LifetimeAttribute:Attribute
{
    public ServiceLifetime Lifetime { get; }

    public LifetimeAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }
}

//// Transient সার্ভিস
//[Lifetime(ServiceLifetime.Transient)]
//public class UserService : IUserService { }

//// Scoped সার্ভিস
//[Lifetime(ServiceLifetime.Scoped)]
//public class OrderService : IOrderService { }

//// Singleton সার্ভিস
//[Lifetime(ServiceLifetime.Singleton)]
//public class AppSettingsService : IAppSettingsService { }



//using Scrutor;
//using Microsoft.Extensions.DependencyInjection;
//using System.Linq;

//public static class ServiceCollectionExtensions
//{
//    public static IServiceCollection AddAutoServices(this IServiceCollection services)
//    {
//        services.Scan(scan => scan
//            .FromApplicationDependencies()  // সকল প্রজেক্ট/assembly scan করবে
//            .AddClasses()                   // সব ক্লাস select করবে
//            .AsImplementedInterfaces()      // যেই Interface implement করেছে সেটির সাথে register হবে
//            .ConfigureLifetime((type, interfaces, currentLifetime) =>
//            {
//                var attr = type.GetCustomAttributes(typeof(LifetimeAttribute), false)
//                               .FirstOrDefault() as LifetimeAttribute;
//                return attr?.Lifetime ?? ServiceLifetime.Transient; // default
//            })
//        );

//        return services;
//    }
//}

//builder.Services.AddAutoServices();