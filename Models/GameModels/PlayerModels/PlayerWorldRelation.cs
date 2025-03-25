using System;

namespace Game.Models
{
    public class PlayerWorldRelation
    {
        public int PlayerId { get; set; }
        public int WorldId { get; set; }
        public int MapId { get; set; }

        // Relaciones
        public Player Player { get; set; }
        public World World { get; set; }
        public Map Map { get; set; }

        // Lógica de negocio

        // Verifica si el jugador está en el mundo especificado
        public bool IsInWorld(int worldId)
        {
            return WorldId == worldId;
        }

        // Verifica si el jugador está en el mapa especificado
        public bool IsInMap(int mapId)
        {
            return MapId == mapId;
        }

        // Actualiza la relación del jugador con un nuevo mapa
        public void ChangeMap(int newMapId)
        {
            MapId = newMapId;
        }

        // Cambia el mundo en el que está el jugador
        public void ChangeWorld(int newWorldId)
        {
            WorldId = newWorldId;
        }
    }
}
