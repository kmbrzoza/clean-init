using System;
using Insig.ApplicationServices.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Insig.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void CacheData(string key, object data, TimeSpan timeToLive)
    {
        if (data is null)
        {
            return;
        }

        _memoryCache.Set(key, data, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = timeToLive
        });
    }

    public object GetCachedData(string key)
    {
        return _memoryCache.Get(key);
    }
}
