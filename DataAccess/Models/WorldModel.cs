using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Models
{
    /// <summary>
    /// Representa un mundo con mapas, jugadores y lógica de acceso y capacidad.
    /// </summary>
    public class WorldModel : BaseModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int MaxMaps { get; set; }
        public int MaxPlayers { get; set; }
        public int LevelRequirement { get; set; }

        // Mapas contenidos en el mundo
        private List<MapModel> _maps = new();
        public IReadOnlyList<MapModel> Maps => _maps;

        // Jugadores que pueden aparecer en el mundo (históricos o conectados)
        private readonly HashSet<int> _playerIds = new();
        public IReadOnlyCollection<int> PlayerIds => _playerIds;

        // Jugadores activos/spawneados en el mundo
        private readonly HashSet<int> _spawnedPlayerIds = new();
        public IReadOnlyCollection<int> SpawnedPlayerIds => _spawnedPlayerIds;

        /// <summary>
        /// Añade un nuevo mapa al mundo si no supera el límite.
        /// </summary>
        public void AddMap(MapModel map)
        {
            if (_maps.Count >= MaxMaps)
                throw new InvalidOperationException("No se pueden agregar más mapas, se ha alcanzado el límite de mapas.");

            if (map.WorldModel != null)
            {
                map.WorldModel.Id = Id;
            }
            _maps.Add(map);
        }
        /// <summary>
        /// Añade múltiples mapas al mundo respetando el límite máximo.
        /// </summary>
        public void AddMaps(IEnumerable<MapModel> maps)
        {
            foreach (var map in maps)
            {
                if (_maps.Count >= MaxMaps)
                    throw new InvalidOperationException("No se pueden agregar más mapas, se ha alcanzado el límite de mapas.");

                if (map.WorldModel != null)
                {
                    map.WorldModel.Id = Id;
                }
                _maps.Add(map);
            }
        }
        /// <summary>
        /// Elimina un mapa del mundo por su ID.
        /// </summary>
        public void RemoveMap(int mapId)
        {
            var map = _maps.FirstOrDefault(m => m.Id == mapId);
            if (map == null)
                throw new InvalidOperationException("El mapa con el ID proporcionado no existe en este mundo.");

            _maps.Remove(map);
        }

        /// <summary>
        /// Verifica si un jugador cumple los requisitos de nivel para acceder al mundo.
        /// </summary>
        public bool CanPlayerAccess(int playerLevel) => playerLevel >= LevelRequirement;

        /// <summary>
        /// Verifica si hay espacio disponible para nuevos jugadores en el mundo.
        /// </summary>
        public bool HasSpace() => _playerIds.Count < MaxPlayers;

        /// <summary>
        /// Intenta añadir un jugador al mundo si cumple requisitos y hay espacio.
        /// </summary>
        public bool TryAddPlayer(int playerId, int playerLevel)
        {
            // if (!CanPlayerAccess(playerLevel) || !HasSpace())
            //     return false;

            return _playerIds.Add(playerId);
        }

        /// <summary>
        /// Intenta spawnear (activar) un jugador en el mundo.
        /// </summary>
        public bool TrySpawnPlayer(int playerId)
        {
            if (!_playerIds.Contains(playerId) || _spawnedPlayerIds.Count >= MaxPlayers)
                return false;

            return _spawnedPlayerIds.Add(playerId);
        }

        /// <summary>
        /// Remueve un jugador del mundo y de sus spawns.
        /// </summary>
        public bool RemovePlayer(int playerId)
        {
            bool removed = _playerIds.Remove(playerId);
            bool despawned = _spawnedPlayerIds.Remove(playerId);
            return removed || despawned;
        }

        /// <summary>
        /// Limpia todos los jugadores y spawns del mundo.
        /// </summary>
        public void ClearPlayers()
        {
            _spawnedPlayerIds.Clear();
            _playerIds.Clear();
        }

        /// <summary>
        /// Número de players registrados en el mundo.
        /// </summary>
        public int PlayerCount => _playerIds.Count;

        /// <summary>
        /// Número de players spawneados actualmente.
        /// </summary>
        public int SpawnedCount => _spawnedPlayerIds.Count;

        public override string ToString()
            => $"World(Id={Id}, Name={Name}, Maps={_maps.Count}/{MaxMaps}, Players={PlayerCount}/{MaxPlayers}, Spawned={SpawnedCount}/{MaxPlayers})";
    }
}
