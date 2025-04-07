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
        LoggerUtil.Log("BaseCachedService", "Servicio de caché inicializado con TTL: " + _ttl, ConsoleColor.Cyan);
    }

    protected abstract Task<TModel?> LoadModelAsync(TId id);

    public TModel? GetIfLoaded(TId id)
    {
        LoggerUtil.Log("BaseCachedService", $"Intentando obtener el modelo con ID: {id}", ConsoleColor.Yellow);

        if (_cache.TryGetValue(id, out var entry))
        {
            if (entry.Expiration > DateTime.UtcNow)
            {
                LoggerUtil.Log("BaseCachedService", $"Modelo con ID {id} encontrado en caché y válido.", ConsoleColor.Green);
                return entry.Model;
            }
            else
            {
                LoggerUtil.Log("BaseCachedService", $"Modelo con ID {id} expiró, eliminando de caché.", ConsoleColor.Red);
                _cache.Remove(id);
            }
        }
        else
        {
            LoggerUtil.Log("BaseCachedService", $"Modelo con ID {id} no encontrado en caché.", ConsoleColor.Red);
        }

        return null;
    }

    public async Task<TModel?> GetOrLoadAsync(TId id)
    {
        LoggerUtil.Log("BaseCachedService", $"Verificando caché para obtener el modelo con ID: {id}", ConsoleColor.Yellow);

        if (_cache.TryGetValue(id, out var entry) && entry.Expiration > DateTime.UtcNow)
        {
            LoggerUtil.Log("BaseCachedService", $"Modelo con ID {id} encontrado en caché y válido.", ConsoleColor.Green);
            return entry.Model;
        }

        LoggerUtil.Log("BaseCachedService", $"Cargando modelo con ID {id} desde la fuente externa...", ConsoleColor.Magenta);

        await _semaphore.WaitAsync();
        try
        {
            // Verificar otra vez por si otro hilo ya cargó
            if (_cache.TryGetValue(id, out entry) && entry.Expiration > DateTime.UtcNow)
            {
                LoggerUtil.Log("BaseCachedService", $"Modelo con ID {id} ya cargado por otro hilo. Usando el valor de caché.", ConsoleColor.Green);
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
                LoggerUtil.Log("BaseCachedService", $"Modelo con ID {id} cargado y almacenado en caché.", ConsoleColor.Green);
            }
            else
            {
                LoggerUtil.Log("BaseCachedService", $"No se pudo cargar el modelo con ID {id} desde la fuente externa.", ConsoleColor.Red);
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
        LoggerUtil.Log("BaseCachedService", $"Estableciendo modelo con ID {id} en caché.", ConsoleColor.Cyan);
        _cache[id] = new CacheEntry
        {
            Model = model,
            Expiration = DateTime.UtcNow.Add(_ttl)
        };
        LoggerUtil.Log("BaseCachedService", $"Modelo con ID {id} almacenado en caché con TTL de {_ttl}.", ConsoleColor.Cyan);
    }

    public void Invalidate(TId id)
    {
        LoggerUtil.Log("BaseCachedService", $"Invalidando caché para el modelo con ID {id}.", ConsoleColor.Yellow);
        _cache.Remove(id);
        LoggerUtil.Log("BaseCachedService", $"Caché invalidado para el modelo con ID {id}.", ConsoleColor.Yellow);
    }

    public void Clear()
    {
        LoggerUtil.Log("BaseCachedService", "Limpiando todo el caché.", ConsoleColor.Yellow);
        _cache.Clear();
        LoggerUtil.Log("BaseCachedService", "Caché limpiado.", ConsoleColor.Yellow);
    }
}
