using Game.Models;
using System;

namespace Game.Services
{
    public class PlayerStatusService  : PlayerModuleService<PlayerEconomy, PlayerEconomyEntity>
    {
        private readonly PlayerStatusRepository _repository;

        public PlayerStatusService(PlayerStatusRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        // Obtener el estado del jugador por ID
        public PlayerStatus GetPlayerStatus(int playerId)
        {
            var entity = _repository.GetByPlayerId(playerId);
            if (entity == null)
            {
                throw new InvalidOperationException($"No se encontró el estado del jugador con ID {playerId}");
            }

            // Mapear la entidad a modelo de dominio
            return GenericMapper.ToDomainModel<PlayerStatusEntity, PlayerStatus>(entity);
        }

        // Actualizar el estado del jugador
        public void UpdatePlayerStatus(PlayerStatus playerStatus)
        {
            var entity = GenericMapper.ToEntity<PlayerStatus, PlayerStatusEntity>(playerStatus);

            // Realizamos las validaciones o cambios de lógica si es necesario
            if (playerStatus.IsBanned && playerStatus.IsPoisoned)
            {
                // Ejemplo de lógica de negocio adicional
                throw new InvalidOperationException("Un jugador no puede estar envenenado y baneado simultáneamente.");
            }

            _repository.Update(entity);
        }

        // Crear un nuevo estado para el jugador
        public void CreatePlayerStatus(PlayerStatus playerStatus)
        {
            var entity = GenericMapper.ToEntity<PlayerStatus, PlayerStatusEntity>(playerStatus);
            _repository.Add(entity);
        }

        // Borrar el estado del jugador
        public void DeletePlayerStatus(int playerId)
        {
            _repository.Delete(playerId);
        }

        // Actualizar estado de envenenamiento
        public void SetPoisoned(int playerId, bool isPoisoned)
        {
            var playerStatus = GetPlayerStatus(playerId);
            playerStatus.IsPoisoned = isPoisoned;
            UpdatePlayerStatus(playerStatus);
        }

        // Actualizar estado de fuego
        public void SetOnFire(int playerId, bool isOnFire)
        {
            var playerStatus = GetPlayerStatus(playerId);
            playerStatus.IsOnFire = isOnFire;
            UpdatePlayerStatus(playerStatus);
        }

        // Actualizar estado de congelamiento
        public void SetFrozen(int playerId, bool isFrozen)
        {
            var playerStatus = GetPlayerStatus(playerId);
            playerStatus.IsFrozen = isFrozen;
            UpdatePlayerStatus(playerStatus);
        }

        // Actualizar estado de baneo
        public void SetBanned(int playerId, bool isBanned)
        {
            var playerStatus = GetPlayerStatus(playerId);
            playerStatus.IsBanned = isBanned;
            UpdatePlayerStatus(playerStatus);
        }
    }
}
