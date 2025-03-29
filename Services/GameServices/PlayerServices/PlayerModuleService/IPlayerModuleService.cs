public interface IPlayerModuleService<TEntity> where TEntity : class, IHasPlayerId
{
    // Obtiene el módulo del jugador desde la base de datos o caché
    TEntity GetPlayerModuleAsync(int playerId);
    
    // Guarda la entidad del jugador en la caché
    void CachePlayerModule(int playerId, TEntity entity);

    // Actualiza la entidad del jugador en la base de datos y caché
    void UpdatePlayerModule(TEntity entity);

    // Guarda o actualiza la entidad del jugador en la base de datos y caché
    Task SavePlayerModuleAsync(TEntity entity);

    // Elimina la entidad del jugador de la base de datos y caché
    Task DeletePlayerModuleAsync(int playerId);
}
