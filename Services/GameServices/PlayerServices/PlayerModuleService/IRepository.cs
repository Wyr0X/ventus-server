public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> GetByPlayerIdAsync(int playerId); // Obtener una entidad por ID de jugador
    Task AddAsync(TEntity entity); // Agregar una nueva entidad
    void Update(TEntity entity); // Actualizar una entidad existente
    Task DeleteAsync(TEntity entity); // Eliminar una entidad
    Task SaveChangesAsync(); // Guardar los cambios en la base de datos
}
