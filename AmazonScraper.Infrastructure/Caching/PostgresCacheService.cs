using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace AmazonScraper.Infrastructure.Caching
{
    public static class PostgresCacheService
    {
        public static async Task SetCacheAsync<T>(this IDistributedCache cache, string CacheKey, T data, int expireMin)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireMin)
            };

            var jsonData = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(CacheKey, jsonData, options);
        }

        public static async Task<T?> GetCacheAsync<T>(this IDistributedCache cache, string CacheKey)
        {
            var jsonData = await cache.GetStringAsync(CacheKey);

            if (jsonData is null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }

    }
}
