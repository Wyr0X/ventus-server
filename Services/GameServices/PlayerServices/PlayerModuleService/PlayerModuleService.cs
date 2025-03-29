public class PlayerModuleService<TEntity> : IPlayerModuleService<TEntity>
    where TEntity : class, IHasPlayerId
{
    private readonly IRepository<TEntity> _repository;
    private readonly ICacheService _cache;

    public PlayerModuleService(IRepository<TEntity> repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    // Obtiene la entidad cacheada del jugador
    public TEntity GetPlayerModuleFromCache(int playerId)
    {
        // Verificar si el módulo está en caché
        var cachedEntity = _cache.Get<TEntity>($"playerModule_{playerId}_{typeof(TEntity).Name}");
        if (cachedEntity != null)
        {
            return cachedEntity; // Retorna si se encuentra en caché
        }

        // Si no está en caché, cargar desde la base de datos
        var entityFromDb = _repository.GetByPlayerIdAsync(playerId).Result;
        if (entityFromDb == null)
        {
            throw new KeyNotFoundException($"Módulo de jugador con ID {playerId} no encontrado.");
        }

        // Guardar la entidad en caché para futuras consultas
        _cache.Set($"playerModule_{playerId}_{typeof(TEntity).Name}", entityFromDb);

        return entityFromDb;
    }

    // Guarda la entidad del jugador en caché
    public void CachePlayerModule(int playerId, TEntity entity)
    {
        _cache.Set($"playerModule_{playerId}_{typeof(TEntity).Name}", entity);
    }

    // Actualiza la entidad del jugador en la base de datos
    public void UpdatePlayerModule(TEntity entity)
    {
        // Actualizar la base de datos
        _repository.Update(entity);

        // Actualizar la caché también
        _cache.Set($"playerModule_{entity.PlayerId}_{typeof(TEntity).Name}", entity);
    }

    // Elimina la entidad del jugador de la base de datos
    public async Task DeletePlayerModuleAsync(int playerId)
    {
        // Obtener la entidad del jugador por ID
        var entity = await _repository.GetByPlayerIdAsync(playerId);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Módulo de jugador con ID {playerId} no encontrado.");
        }

        // Eliminar la entidad de la base de datos
        await _repository.DeleteAsync(entity);
        await _repository.SaveChangesAsync();

        // Eliminar la caché relacionada
        _cache.Remove($"playerModule_{playerId}_{typeof(TEntity).Name}");
    }

    // Guarda o actualiza la entidad del jugador en la base de datos y caché
    public async Task SavePlayerModuleAsync(TEntity entity)
    {
        // Verificar si la entidad ya existe en la base de datos
        var existingEntity = await _repository.GetByPlayerIdAsync(entity.PlayerId);

        if (existingEntity != null)
        {
            // Si existe, actualizar la entidad
            _repository.Update(entity);
        }
        else
        {
            // Si no existe, agregar una nueva entidad
            await _repository.AddAsync(entity);
        }

        // Guardar los cambios en la base de datos
        await _repository.SaveChangesAsync();

        // También guardar la entidad en caché
        _cache.Set($"playerModule_{entity.PlayerId}_{typeof(TEntity).Name}", entity);
    }
}
