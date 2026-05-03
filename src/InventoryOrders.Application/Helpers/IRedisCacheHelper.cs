namespace InventoryOrders.Application.Helpers;

public interface IRedisCacheHelper
{
    Task<T> GetAsync<T>(string key, CancellationToken ct);
    Task SetAsync<T>(string key, T data, CancellationToken ct);
    Task RemoveAsync(string key, CancellationToken ct);
    Task RemoveByPatternAsync(string pattern, CancellationToken ct);
    Task UpdateItemAndInvalidateListAsync<T>(string itemKey, T itemData, string listPattern, CancellationToken ct);
    Task RemoveItemAndInvalidateListAsync(string itemKey, string listPattern, CancellationToken ct);
}
