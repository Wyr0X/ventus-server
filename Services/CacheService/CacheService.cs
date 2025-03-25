using System;
using System.Runtime.Caching;

public class CacheService : ICacheService
{
    private readonly ObjectCache _cache;

    public CacheService()
    {
        _cache = MemoryCache.Default; // Usando la memoria local del servidor
    }

    public T Get<T>(string key) where T : class
    {
        return _cache[key] as T;
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        var policy = new CacheItemPolicy
        {
            AbsoluteExpiration = DateTimeOffset.Now.Add(expiration ?? TimeSpan.FromMinutes(30)) // Expiración por defecto en 30 minutos
        };
        _cache.Set(key, value, policy);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }

    public bool Exists(string key)
    {
        return _cache.Contains(key);
    }
}
