public abstract class BaseCachedService<TModel, TId>
    where TModel : class
{
    private class CacheEntry
    {
        public TModel Model { get; set; } = default!;
        public DateTime Expiration { get; set; }
    }

    private readonly Dictionary<TId, CacheEntry> _cache = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly TimeSpan _ttl;

    protected BaseCachedService(TimeSpan? ttl = null)
    {
        _ttl = ttl ?? TimeSpan.FromMinutes(5); // TTL por defecto de 5 minutos
    }

    protected abstract Task<TModel?> LoadModelAsync(TId id);

    public TModel? GetIfLoaded(TId id)
    {
        if (_cache.TryGetValue(id, out var entry))
        {
            if (entry.Expiration > DateTime.UtcNow)
            {
                return entry.Model;
            }
            else
            {
                _cache.Remove(id);
            }
        }
        return null;
    }

    public async Task<TModel?> GetOrLoadAsync(TId id)
    {
        if (_cache.TryGetValue(id, out var entry) && entry.Expiration > DateTime.UtcNow)
        {
            return entry.Model;
        }

        await _semaphore.WaitAsync();
        try
        {
            // Verificar otra vez por si otro hilo ya cargó
            if (_cache.TryGetValue(id, out entry) && entry.Expiration > DateTime.UtcNow)
            {
                return entry.Model;
            }

            var model = await LoadModelAsync(id);
            if (model != null)
            {
                _cache[id] = new CacheEntry
                {
                    Model = model,
                    Expiration = DateTime.UtcNow.Add(_ttl)
                };
            }

            return model;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Set(TId id, TModel model)
    {
        _cache[id] = new CacheEntry
        {
            Model = model,
            Expiration = DateTime.UtcNow.Add(_ttl)
        };
    }

    public void Invalidate(TId id)
    {
        _cache.Remove(id);
    }

    public void Clear()
    {
        _cache.Clear();
    }
}
