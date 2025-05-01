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
        Log.Log(Log.LogTag.BaseCachedService, "Servicio de caché inicializado con TTL: " + _ttl);
    }

    protected abstract Task<TModel?> LoadModelAsync(TId id);

    public TModel? GetIfLoaded(TId id)
    {
        Log.Log(Log.LogTag.BaseCachedService, $"Intentando obtener el modelo con ID: {id}");

        if (_cache.TryGetValue(id, out var entry))
        {
            if (entry.Expiration > DateTime.UtcNow)
            {
                Log.Log(Log.LogTag.BaseCachedService, $"Modelo con ID {id} encontrado en caché y válido.");
                return entry.Model;
            }
            else
            {
                Log.Log(Log.LogTag.BaseCachedService, $"Modelo con ID {id} expiró, eliminando de caché.");
                _cache.Remove(id);
            }
        }
        else
        {
            Log.Log(Log.LogTag.BaseCachedService, $"Modelo con ID {id} no encontrado en caché.");
        }

        return null;
    }

    public async Task<TModel?> GetOrLoadAsync(TId id)
    {
        Log.Log(Log.LogTag.BaseCachedService, $"Verificando caché para obtener el modelo con ID: {id}");

        if (_cache.TryGetValue(id, out var entry) && entry.Expiration > DateTime.UtcNow)
        {
            Log.Log(Log.LogTag.BaseCachedService, $"Modelo con ID {id} encontrado en caché y válido.");
            return entry.Model;
        }

        Log.Log(Log.LogTag.BaseCachedService, $"Cargando modelo con ID {id} desde la fuente externa...");

        await _semaphore.WaitAsync();
        try
        {
            // Verificar otra vez por si otro hilo ya cargó
            if (_cache.TryGetValue(id, out entry) && entry.Expiration > DateTime.UtcNow)
            {
                Log.Log(Log.LogTag.BaseCachedService, $"Modelo con ID {id} ya cargado por otro hilo. Usando el valor de caché.");
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
                Log.Log(Log.LogTag.BaseCachedService, $"Modelo con ID {id} cargado y almacenado en caché.");
            }
            else
            {
                Log.Log(Log.LogTag.BaseCachedService, $"No se pudo cargar el modelo con ID {id} desde la fuente externa.");
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
        Log.Log(Log.LogTag.BaseCachedService, $"Estableciendo modelo con ID {id} en caché.");
        _cache[id] = new CacheEntry
        {
            Model = model,
            Expiration = DateTime.UtcNow.Add(_ttl)
        };
        Log.Log(Log.LogTag.BaseCachedService, $"Modelo con ID {id} almacenado en caché con TTL de {_ttl}.");
    }

    public void Invalidate(TId id)
    {
        Log.Log(Log.LogTag.BaseCachedService, $"Invalidando caché para el modelo con ID {id}.");
        _cache.Remove(id);
        Log.Log(Log.LogTag.BaseCachedService, $"Caché invalidado para el modelo con ID {id}.");
    }

    public void Clear()
    {
        Log.Log(Log.LogTag.BaseCachedService, "Limpiando todo el caché.");
        _cache.Clear();
        Log.Log(Log.LogTag.BaseCachedService, "Caché limpiado.");
    }
}
