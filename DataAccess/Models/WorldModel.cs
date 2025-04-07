using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Models
{
    public class WorldModel  : BaseModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int MaxMaps { get; set; }
        public int MaxPlayers { get; set; }
        public int LevelRequirements { get; set; }

        // Relaciones
        public List<MapModel> Maps { get; set; } = new List<MapModel>();
        public List<PlayerLocation> PlayersLocation { get; set; } = new List<PlayerLocation>();
        public List<PlayerLocation> spawnedPlayers { get; set; } = new List<PlayerLocation>();
        public void AddMap(MapModel map)
        {
            if (Maps.Count < MaxMaps)
            {
                Maps.Add(map);
            }
            else
            {
                throw new InvalidOperationException("No se pueden agregar más mapas, se ha alcanzado el límite de mapas.");
            }
        }

        /// <summary>
        /// Elimina un mapa del mundo por su ID.
        /// </summary>
        /// <param name="mapId">ID del mapa a eliminar.</param>
        public void RemoveMap(int mapId)
        {
            var map = Maps.FirstOrDefault(m => m.Id == mapId);
            if (map != null)
            {
                Maps.Remove(map);
            }
            else
            {
                throw new InvalidOperationException("El mapa con el ID proporcionado no existe.");
            }
        }

        /// <summary>
        /// Verifica si un jugador cumple con los requisitos de nivel para acceder al mundo.
        /// </summary>
        /// <param name="playerLevel">El nivel del jugador.</param>
        /// <returns>Verdadero si el jugador puede acceder, de lo contrario falso.</returns>
        public bool CanPlayerAccess(int playerLevel)
        {
            return playerLevel >= LevelRequirements;
        }

        /// <summary>
        /// Verifica si hay espacio suficiente en el mundo para más jugadores.
        /// </summary>
        /// <param name="currentPlayerCount">El número actual de jugadores en el mundo.</param>
        /// <returns>Verdadero si hay espacio disponible para más jugadores, de lo contrario falso.</returns>
        public bool HasSpace(int currentPlayerCount)
        {
            return currentPlayerCount < MaxPlayers;
        }
        public void RemovePlayer(int playerId)
        {
            var playerLocation = PlayersLocation.FirstOrDefault(p => p.Player.Id == playerId);
            if (playerLocation != null)
            {
                PlayersLocation.Remove(playerLocation);
            }
            else
            {
                throw new InvalidOperationException("El jugador con el ID proporcionado no existe en este mundo.");
            }
        }
   

    }
}