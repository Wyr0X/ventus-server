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


        // Jugadores spawneados (activos) en el mapa
        private readonly HashSet<int> _spawnedPlayerIds = new();
        public IReadOnlyCollection<int> SpawnedPlayerIds => _spawnedPlayerIds;

        /// <summary>
        /// Verifica si hay espacio para más jugadores.
        /// </summary>
        public bool HasSpace() => SpawnedPlayerIds.Count < MaxPlayers;

        /// <summary>
        /// Verifica si hay espacio para spawnear jugadores activos.
        /// </summary>
        public bool HasSpawnSpace() => _spawnedPlayerIds.Count < MaxPlayers;

        /// <summary>
        /// Agrega un jugador al mapa si cumple el nivel mínimo y hay espacio.
        /// </summary>
        public bool AddPlayer(int playerId, int playerLevel)
        {
            // if (playerLevel < MinLevel || !HasSpace())
            //     return false;

            if (_spawnedPlayerIds.Contains(playerId) || !HasSpawnSpace())
                return false;

            return _spawnedPlayerIds.Add(playerId);
        }


        /// <summary>
        /// Remueve un jugador del mapa (tanto de la lista de presentes como de spawneados).
        /// </summary>
        public bool RemovePlayer(int playerId)
        {
            bool despawned = _spawnedPlayerIds.Remove(playerId);
            return despawned;
        }

        /// <summary>
        /// Número actual de jugadores en el mapa.
        /// </summary>
        public int PlayerCount => SpawnedPlayerIds.Count;

        /// <summary>
        /// Número actual de jugadores spawneados en el mapa.
        /// </summary>
        public int SpawnedCount => _spawnedPlayerIds.Count;

        public override string ToString()
            => $"Map(Id={Id}, Name={Name}, Players={PlayerCount}/{MaxPlayers}, Spawned={SpawnedCount}/{MaxPlayers})";

        /// <summary>
        /// Verifica si un jugador está registrado en este mundo.
        /// </summary>
        public bool ContainsPlayer(int playerId)
        {
            return SpawnedPlayerIds.Contains(playerId);
        }
        /// <summary>
        /// Verifica si un jugador cumple los requisitos de nivel para acceder al mundo.
        /// </summary>
        public bool CanPlayerAccess(int playerLevel) => playerLevel >= MinLevel;

        public bool AddPlayers(List<PlayerModel> players)
        {
            // Verifica si el mundo tiene espacio para los jugadores adicionales
            if (SpawnedPlayerIds.Count + players.Count() > MaxPlayers)
                return false;  // No hay espacio suficiente para todos los jugadores

            // Verifica que cada jugador cumpla con los requisitos de nivel antes de agregarlo
            foreach (var player in players)
            {
                if (player.Stats == null || !CanPlayerAccess(player.Stats.Level))
                    return false;  // Si algún jugador no cumple el nivel o Stats es nulo, no se agrega ningún jugador

                _spawnedPlayerIds.Add(player.Id);  // Agrega el jugador al mundo
            }

            return true;  // Todos los jugadores han sido añadidos exitosamente
        }
    }
}