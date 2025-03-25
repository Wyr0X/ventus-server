using Microsoft.EntityFrameworkCore;
using VentusServer.Database;
using System;
using System.Linq.Expressions;
using System.Reflection;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly PostgresDbContext _context;

    public Repository(PostgresDbContext context)
    {
        _context = context;
    }

    public async Task<TEntity> GetByPlayerIdAsync(int playerId)
    {
        var entity = await _context.Set<TEntity>()
            .FirstOrDefaultAsync(e => GetPlayerIdProperty(e) == playerId);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Entidad con PlayerId {playerId} no encontrada.");
        }

        return entity;
    }

    public async Task AddAsync(TEntity entity)
    {
        try
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al agregar la entidad", ex);
        }
    }

    public void Update(TEntity entity)
    {
        try
        {
            _context.Set<TEntity>().Update(entity);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al actualizar la entidad", ex);
        }
    }

    public async Task DeleteAsync(TEntity entity)
    {
        try
        {
            _context.Set<TEntity>().Remove(entity);
        }
        catch (Exception ex)
        {
            throw new Exception("Error al eliminar la entidad", ex);
        }
    }

    public async Task SaveChangesAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error al guardar los cambios en la base de datos", ex);
        }
    }

    // Método para obtener dinámicamente el valor de PlayerId
    private int GetPlayerIdProperty(TEntity entity)
    {
        var playerIdProperty = typeof(TEntity).GetProperties()
            .FirstOrDefault(p => p.Name.Equals("PlayerId", StringComparison.OrdinalIgnoreCase));

        if (playerIdProperty == null)
        {
            throw new InvalidOperationException("La propiedad 'PlayerId' no se encuentra en la entidad.");
        }

        var playerIdValue = playerIdProperty.GetValue(entity);
        if (playerIdValue == null)
        {
            throw new InvalidOperationException("El valor de 'PlayerId' es nulo.");
        }

        return (int)playerIdValue;
    }
}
