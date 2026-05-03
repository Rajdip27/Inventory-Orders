using Mapster;
using InventoryOrders.Application.ViewModel;
using InventoryOrders.Core.Entities;

namespace InventoryOrders.Application.Mappings;

public static class MapsterConfig
{
    public static void RegisterMappings()
    {
        var config = TypeAdapterConfig.GlobalSettings;
    }
}
