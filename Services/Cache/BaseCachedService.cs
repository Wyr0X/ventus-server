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
        LoggerUtil.Log(LoggerUtil.LogTag.BaseCachedService, "Servicio de caché inicializado con TTL: " + _ttl);
    }

    protected abstract Task<TModel?> LoadModelAsync(TId id);

    public TModel? GetIfLoaded(TId id)
    {
        LoggerUtil.Log(LoggerUtil.LogTag.BaseCachedService, $"Intentando obtener el modelo con ID: {id}");

        if (_cache.TryGetValue(id, out var entry))
        {
            if (entry.Expiration > DateTime.UtcNow)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.BaseCachedService, $"Modelo con ID {id} encontrado en caché y válido.");
                return entry.Model;
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.BaseCachedService, $"Modelo con ID {id} expiró, eliminando de caché.");
                _cache.Remove(id);
            }
        }
        else
        {
            LoggerUtil.Log(LoggerUtil.LogTag.BaseCachedService, $"Modelo con ID {id} no encontrado en caché.");
        }

        return null;
    }

    public async Task<TModel?> GetOrLoadAsync(TId id)
    {
        LoggerUtil.Log(LoggerUtil.LogTag.BaseCachedService, $"Verificando caché para obtener el modelo con ID: {id}");

        if (_cache.TryGetValue(id, out var entry) && entry.Expiration > DateTime.UtcNow)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.BaseCachedService, $"Modelo con ID {id} encontrado en caché y válido.");
            return entry.Model;
        }

        LoggerUtil.Log(LoggerUtil.LogTag.BaseCachedService, $"Cargando modelo con ID {id} desde la fuente externa...");

        await _semaphore.WaitAsync();
        try
        {
            // Verificar otra vez por si otro hilo ya cargó
            if (_cache.TryGetValue(id, out entry) && entry.Expiration > DateTime.UtcNow)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.BaseCachedService, $"Modelo con ID {id} ya cargado por otro hilo. Usando el valor de caché.");
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
                LoggerUtil.Log(LoggerUtil.LogTag.BaseCachedService, $"Modelo con ID {id} cargado y almacenado en caché.");
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.BaseCachedService, $"No se pudo cargar el modelo con ID {id} desde la fuente externa.");
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
        LoggerUtil.Log(LoggerUtil.LogTag.BaseCachedService, $"Estableciendo modelo con ID {id} en caché.");
        _cache[id] = new CacheEntry
        {
            Model = model,
            Expiration = DateTime.UtcNow.Add(_ttl)
        };
        LoggerUtil.Log(LoggerUtil.LogTag.BaseCachedService, $"Modelo con ID {id} almacenado en caché con TTL de {_ttl}.");
    }

    public void Invalidate(TId id)
    {
        LoggerUtil.Log(LoggerUtil.LogTag.BaseCachedService, $"Invalidando caché para el modelo con ID {id}.");
        _cache.Remove(id);
        LoggerUtil.Log(LoggerUtil.LogTag.BaseCachedService, $"Caché invalidado para el modelo con ID {id}.");
    }

    public void Clear()
    {
        LoggerUtil.Log(LoggerUtil.LogTag.BaseCachedService, "Limpiando todo el caché.");
        _cache.Clear();
        LoggerUtil.Log(LoggerUtil.LogTag.BaseCachedService, "Caché limpiado.");
    }
}
