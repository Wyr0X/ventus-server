using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Models;

namespace Game.Server
{
    /// <summary>
    /// Gestiona la carga de mundos y la incorporación de jugadores.
    /// </summary>
    public class WorldManager
    {
        private readonly Dictionary<int, WorldModel> _worlds = new();
        private readonly GameServer _gameServer;
        private readonly ConcurrentDictionary<int, Task<WorldModel?>> _loadingWorlds = new();

        public WorldManager(GameServer gameServer)
        {
            _gameServer = gameServer;
        }

        /// <summary>
        /// Intenta añadir y spawnear un jugador en el mundo y mapa.
        /// Si el mundo no está cargado, lo carga en segundo plano y reintenta.
        /// </summary>
        public bool AddPlayer(PlayerModel player)
        {
            var loc = player.Location;
            if (loc == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, "PlayerLocation is null");
                return false;
            }

            int worldId = loc.WorldId;

            if (_worlds.TryGetValue(worldId, out var world))
            {
                return AddPlayerToLoadedWorld(player, world);
            }

            // Evitar que múltiples tareas lo pidan al mismo tiempo
            var loadingTask = _loadingWorlds.GetOrAdd(worldId, id =>
                _gameServer._gameServiceMediator.GetWorldInfo(id)
            );

            loadingTask.ContinueWith(task =>
            {
                var loaded = task.Result;
                if (loaded != null)
                {
                    _gameServer.Schedule(() =>
                    {
                        _worlds[worldId] = loaded;
                        _loadingWorlds.TryRemove(worldId, out _);
                        AddPlayerToLoadedWorld(player, loaded);
                    });
                }
                else
                {
                    _loadingWorlds.TryRemove(worldId, out _);
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                        $"Failed to load world {worldId} for player {player.Id}");
                }
            });

            return false;
        }
        private bool AddPlayerToLoadedWorld(PlayerModel player, WorldModel world)
        {
            var loc = player.Location!;
            lock (world)
            {
                if (!world.TryAddPlayer(player.Id, player.Level))
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                        $"Cannot add player {player.Id} to world {loc.WorldId}");
                    return false;
                }

                var map = world.Maps.FirstOrDefault(m => m.Id == loc.MapId);
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                      $"Mapas: {world.Maps.Count()}");
                if (map == null)
                {
                    world.RemovePlayer(player.Id);
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                        $"Map {loc.MapId} not found in world {loc.WorldId}");
                    return false;
                }

                if (!map.AddPlayer(player.Id, player.Level))
                {
                    world.RemovePlayer(player.Id);
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                        $"Cannot add player {player.Id} to map {loc.MapId}");
                    return false;
                }

                if (!world.TrySpawnPlayer(player.Id) || !map.SpawnPlayer(player.Id))
                {
                    world.RemovePlayer(player.Id);
                    map.RemovePlayer(player.Id);
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                        $"Cannot spawn player {player.Id} in world or map");
                    return false;
                }
            }

            return true;
        }

    }
}
