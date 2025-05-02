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
        public void AddPlayer(PlayerModel player, Action<bool> onFinished)
        {
            var loc = player.Location;
            if (loc == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayer] Player {player.Id} has null location.");
                onFinished(false);
                return;
            }

            int worldId = loc.WorldId;
            LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayer] Player {player.Id} requests World {worldId}.");

            if (_worlds.TryGetValue(worldId, out var world))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayer] World {worldId} already loaded.");
                _gameServer.Schedule(() =>
                {
                    bool result = AddPlayerToLoadedWorld(player, world);
                    onFinished(result);
                });
                return;
            }

            LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayer] World {worldId} not loaded. Requesting load...");

            var loadingTask = _loadingWorlds.GetOrAdd(worldId, id =>
                _gameServer._gameServiceMediator.GetWorldInfo(id)
            );

            loadingTask.ContinueWith(task =>
            {
                var loaded = task.Result;
                _gameServer.Schedule(() =>
                {
                    if (loaded != null)
                    {
                        LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayer] World {worldId} loaded successfully.");
                        _worlds[worldId] = loaded;
                        _loadingWorlds.TryRemove(worldId, out _);
                        bool result = AddPlayerToLoadedWorld(player, loaded);
                        onFinished(result);
                    }
                    else
                    {
                        LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayer] Failed to load world {worldId}.");
                        _loadingWorlds.TryRemove(worldId, out _);
                        onFinished(false);
                    }
                });
            });
        }


        private bool AddPlayerToLoadedWorld(PlayerModel player, WorldModel world)
        {
            var loc = player.Location!;
            LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayerToLoadedWorld] Trying to add player {player.Id} to world {loc.WorldId}...");

            lock (world)
            {
                if (!world.TryAddPlayer(player.Id, player.Level))
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayerToLoadedWorld] Cannot add player {player.Id} to world {loc.WorldId} (maybe full or low level).");
                    return false;
                }

                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayerToLoadedWorld] Player {player.Id} added to world {loc.WorldId}.");

                var map = world.Maps.FirstOrDefault(m => m.Id == loc.MapId);
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayerToLoadedWorld] Available maps in world {loc.WorldId}: {world.Maps.Count}.");

                if (map == null)
                {
                    world.RemovePlayer(player.Id);
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayerToLoadedWorld] Map {loc.MapId} not found in world {loc.WorldId}.");
                    return false;
                }

                if (!map.AddPlayer(player.Id, player.Level))
                {
                    world.RemovePlayer(player.Id);
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayerToLoadedWorld] Cannot add player {player.Id} to map {loc.MapId} (maybe full or low level).");
                    return false;
                }

                if (!world.TrySpawnPlayer(player.Id) || !map.SpawnPlayer(player.Id))
                {
                    world.RemovePlayer(player.Id);
                    map.RemovePlayer(player.Id);
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayerToLoadedWorld] Failed to spawn player {player.Id} in world or map.");
                    return false;
                }

                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayerToLoadedWorld] Player {player.Id} successfully added and spawned in world {loc.WorldId}, map {loc.MapId}.");
            }

            return true;
        }
    }
}
