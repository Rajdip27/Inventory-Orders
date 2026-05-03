using InventoryOrders.Application.Caching;

namespace InventoryOrders.Application.Helpers;

public class RedisCacheHelper: IRedisCacheHelper
{
    private readonly IRedisCacheService _redis;

    public RedisCacheHelper(IRedisCacheService redis)
    {
        _redis = redis;
    }

    public async Task<T> GetAsync<T>(string key, CancellationToken ct)
    {
        try { return await _redis.GetDataAsync<T>(key, ct); }
        catch { return default; }
    }

    public async Task SetAsync<T>(string key, T data, CancellationToken ct)
    {
        try
        {
            // Real-time: no TTL needed; persist until manually invalidated
            await _redis.SetDataAsync(key, data, ct);
        }
        catch { }
    }

    public async Task RemoveAsync(string key, CancellationToken ct)
    {
        try { await _redis.RemoveDataAsync(key, ct); }
        catch { }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken ct)
    {
        try { await _redis.RemoveByPatternAsync(pattern, ct); }
        catch { }
    }

    // Update item and remove list caches immediately
    public async Task UpdateItemAndInvalidateListAsync<T>(string itemKey, T itemData, string listPattern, CancellationToken ct)
    {
        await SetAsync(itemKey, itemData, ct);
        await RemoveByPatternAsync(listPattern, ct);
    }

    public async Task RemoveItemAndInvalidateListAsync(string itemKey, string listPattern, CancellationToken ct)
    {
        await RemoveAsync(itemKey, ct);
        await RemoveByPatternAsync(listPattern, ct);
    }
}
