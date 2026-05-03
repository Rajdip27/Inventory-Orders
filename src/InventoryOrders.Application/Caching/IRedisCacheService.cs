using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace InventoryOrders.Application.Caching;

public interface IRedisCacheService
{
    Task<T> GetDataAsync<T>(string key, CancellationToken ct);
    Task SetDataAsync<T>(string key, T data, CancellationToken ct);
    Task RemoveDataAsync(string key, CancellationToken ct);
    Task RemoveByPatternAsync(string pattern, CancellationToken ct);
    Task RefreshDataAsync<T>(string key, T data, CancellationToken ct);
}

public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redis;

    public RedisCacheService(IDistributedCache cache, IConnectionMultiplexer redis)
    {
        _cache = cache;
        _redis = redis;
    }

    public async Task<T> GetDataAsync<T>(string key, CancellationToken ct)
    {
        var json = await _cache.GetStringAsync(key, ct);
        return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetDataAsync<T>(string key, T data, CancellationToken ct)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30) // 30s cache for near-real-time
        };
        var json = JsonSerializer.Serialize(data);
        await _cache.SetStringAsync(key, json, options, ct);
    }

    public async Task RemoveDataAsync(string key, CancellationToken ct)
    {
        await _cache.RemoveAsync(key, ct);
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken ct)
    {
        if (_redis is null) return;

        var endpoints = _redis.GetEndPoints();
        foreach (var endpoint in endpoints)
        {
            var server = _redis.GetServer(endpoint);
            var db = _redis.GetDatabase();

            await foreach (var key in server.KeysAsync(pattern: $"{pattern}*"))
            {
                await db.KeyDeleteAsync(key);
            }
        }
    }

    public async Task RefreshDataAsync<T>(string key, T data, CancellationToken ct)
    {
        await RemoveDataAsync(key, ct);
        await SetDataAsync(key, data, ct);
    }
}
