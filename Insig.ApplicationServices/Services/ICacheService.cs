using System;

namespace Insig.ApplicationServices.Services;

public interface ICacheService
{
    void CacheData(string key, object data, TimeSpan timeToLive);
    object GetCachedData(string key);
}
