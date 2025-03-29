using System;
using System.Threading.Tasks;
using Game.Models;
using VentusServer.Database;
using VentusServer.Models;

namespace Game.Services
{
    public class PlayerLocationService(IRepository<PlayerLocation> repository, ICacheService cache) : PlayerModuleService<PlayerLocation, PlayerLocationEntity>(repository, cache)
    {
        private readonly PostgresDbContext _context;

        // Obtiene la ubicación del jugador por su PlayerId
        public async Task<PlayerLocation> GetPlayerLocationAsync(int playerId)
        {
            var entity = await _context.PlayerLocations
                .FirstOrDefaultAsync(pl => pl.PlayerId == playerId);

            if (entity == null)
                return null;

            // Mapeamos la entidad a un modelo de dominio usando GenericMapper
            var playerLocation = GenericMapper.ToDomainModel<PlayerLocationEntity, PlayerLocation>(entity);

            // Mapear las relaciones
            var player = await _context.Players.FindAsync(entity.PlayerId);
            var playerWorldRelation = await _context.PlayerWorldRelations.FindAsync(entity.PlayerWorldRelationId);

            playerLocation.Player = player;
            playerLocation.PlayerWorldRelation = playerWorldRelation;

            return playerLocation;
        }

        // Actualiza la ubicación del jugador
        public async Task<bool> UpdatePlayerLocationAsync(int playerId, string newMap, int newPosX, int newPosY, string newDirection)
        {
            var entity = await _context.PlayerLocations
                .FirstOrDefaultAsync(pl => pl.PlayerId == playerId);

            if (entity == null)
                return false;

            // Actualizamos la entidad
            entity.PosMap = newMap;
            entity.PosX = newPosX;
            entity.PosY = newPosY;
            entity.Direction = newDirection;

            // Guardamos los cambios en la base de datos
            _context.PlayerLocations.Update(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        // Mueve al jugador a una nueva ubicación
        public async Task<bool> MovePlayerAsync(int playerId, int newPosX, int newPosY, string newDirection)
        {
            var entity = await _context.PlayerLocations
                .FirstOrDefaultAsync(pl => pl.PlayerId == playerId);

            if (entity == null)
                return false;

            // Realizamos el movimiento
            entity.PosX = newPosX;
            entity.PosY = newPosY;
            entity.Direction = newDirection;

            // Guardamos los cambios
            _context.PlayerLocations.Update(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        // Verifica si el jugador está en la misma ubicación
        public bool IsPlayerInSameLocation(PlayerLocation currentLocation, int checkPosX, int checkPosY)
        {
            return currentLocation.IsInSameLocation(checkPosX, checkPosY);
        }
    }
}
