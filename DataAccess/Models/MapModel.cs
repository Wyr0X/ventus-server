using System;
using System.Collections.Generic;

namespace Game.Models
{
    /// <summary>
    /// Representa un mapa dentro de un mundo, con lógica de capacidad y gestión de jugadores.
    /// Solo mantiene la lista de jugadores presentes y los jugadores spawneados.
    /// </summary>
    public class MapModel : BaseModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int MinLevel { get; set; }
        public int MaxPlayers { get; set; }
        public int WorldId { get; set; } // Relación con el mundo al que pertenece

        public WorldModel? WorldModel { get; set; }

        // Jugadores actualmente en el mapa
        private readonly HashSet<int> _playerIds = new();
        public IReadOnlyCollection<int> PlayersIds => _playerIds;

        // Jugadores spawneados (activos) en el mapa
        private readonly HashSet<int> _spawnedPlayerIds = new();
        public IReadOnlyCollection<int> SpawnedPlayerIds => _spawnedPlayerIds;

        /// <summary>
        /// Verifica si hay espacio para más jugadores.
        /// </summary>
        public bool HasSpace() => _playerIds.Count < MaxPlayers;

        /// <summary>
        /// Verifica si hay espacio para spawnear jugadores activos.
        /// </summary>
        public bool HasSpawnSpace() => _spawnedPlayerIds.Count < MaxPlayers;

        /// <summary>
        /// Agrega un jugador al mapa si cumple el nivel mínimo y hay espacio.
        /// </summary>
        public bool AddPlayer(int playerId, int playerLevel)
        {
            if (playerLevel < MinLevel || !HasSpace())
                return false;

            return _playerIds.Add(playerId);
        }

        /// <summary>
        /// Spawnea (activa) un jugador en el mapa si ya está presente y hay espacio.
        /// </summary>
        public bool SpawnPlayer(int playerId)
        {
            if (!_playerIds.Contains(playerId) || !HasSpawnSpace())
                return false;

            return _spawnedPlayerIds.Add(playerId);
        }

        /// <summary>
        /// Remueve un jugador del mapa (tanto de la lista de presentes como de spawneados).
        /// </summary>
        public bool RemovePlayer(int playerId)
        {
            bool removed = _playerIds.Remove(playerId);
            bool despawned = _spawnedPlayerIds.Remove(playerId);
            return removed || despawned;
        }

        /// <summary>
        /// Número actual de jugadores en el mapa.
        /// </summary>
        public int PlayerCount => _playerIds.Count;

        /// <summary>
        /// Número actual de jugadores spawneados en el mapa.
        /// </summary>
        public int SpawnedCount => _spawnedPlayerIds.Count;

        public override string ToString()
            => $"Map(Id={Id}, Name={Name}, Players={PlayerCount}/{MaxPlayers}, Spawned={SpawnedCount}/{MaxPlayers})";
    }
}