using Game.Models;
using Game.Repositories;

namespace Game.Services
{
    public class PlayerWorldRelationService
    {
        private readonly IPlayerWorldRelationRepository _repository;

        public PlayerWorldRelationService(IPlayerWorldRelationRepository repository)
        {
            _repository = repository;
        }

        // Obtener la relación del jugador con el mundo y mapa
        public PlayerWorldRelation GetPlayerWorldRelation(int playerId)
        {
            return _repository.GetByPlayerId(playerId);
        }

        // Actualizar la relación del jugador con el mundo y mapa
        public void UpdatePlayerWorldRelation(PlayerWorldRelation relation)
        {
            _repository.Update(relation);
        }

        // Cambiar de mundo y mapa al jugador
        public void ChangePlayerWorld(int playerId, int newWorldId, int newMapId)
        {
            var relation = _repository.GetByPlayerId(playerId);
            relation.ChangeWorld(newWorldId);
            relation.ChangeMap(newMapId);
            _repository.Update(relation);
        }

        // Verificar si el jugador está en un mapa específico
        public bool IsPlayerInMap(int playerId, int mapId)
        {
            var relation = _repository.GetByPlayerId(playerId);
            return relation.IsInMap(mapId);
        }
    }
}
