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
        private readonly Dictionary<int, PlayerModel> _playersInTheWorld = new();
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
        public void AddPlayerToWorld(PlayerModel player, Action<bool> onFinished)
        {
            var loc = player.Location;
            if (loc == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayer] Player {player.Id} has null location.");
                onFinished(false);
                return;
            }

            int worldId = loc.WorldId;
            LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayer] Player {player.Id} requests World {worldId}. Location: MapId={loc.MapId}");

            if (_worlds.TryGetValue(worldId, out var world))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayer] World {worldId} already loaded. Proceeding to add player...");
                _gameServer.Schedule(() =>
                {
                    bool result = AddPlayerToLoadedWorld(player, world);
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayer] Add to loaded world result for player {player.Id}: {result}"); // NUEVO LOG
                    onFinished(result);
                });
                return;
            }

            LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayer] World {worldId} not loaded. Requesting load...");

            var loadingTask = _loadingWorlds.GetOrAdd(worldId, id =>
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayer] Starting async load for world {id}..."); // NUEVO LOG
                return _gameServer._gameServiceMediator.GetWorldInfo(id);
            });

            loadingTask.ContinueWith(task =>
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayer] Finished loading task for world {worldId}. Success: {task.Result != null}"); // NUEVO LOG
                var loaded = task.Result;
                _gameServer.Schedule(() =>
                {
                    if (loaded != null)
                    {
                        LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayer] World {worldId} loaded successfully. Caching and adding player...");
                        _worlds[worldId] = loaded;
                        _loadingWorlds.TryRemove(worldId, out _);
                        bool result = AddPlayerToLoadedWorld(player, loaded);
                        LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayer] Add to newly loaded world result for player {player.Id}: {result}"); // NUEVO LOG
                        if (result)
                        {
                            _playersInTheWorld[player.Id] = player;
                        }
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
        public bool RemovePlayerFromWorld(PlayerModel player, bool fullExit = true)
        {
            var loc = player.Location;
            if (loc == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[RemovePlayerFromWorld] Player {player.Id} has null location.");
                return false;
            }

            int worldId = loc.WorldId;
            if (!_worlds.TryGetValue(worldId, out var world))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[RemovePlayerFromWorld] World {worldId} not found.");
                return false;
            }

            lock (world)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[RemovePlayerFromWorld] Attempting to remove player {player.Id} from world {worldId}.");

                var map = world.Maps.FirstOrDefault(m => m.Id == loc.MapId);
                if (map != null)
                {
                    if (!fullExit)
                    {
                        LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[RemovePlayerFromWorld] Unspawning player {player.Id} from world {loc.WorldId}, map {loc.MapId}.");
                        map?.RemovePlayer(player.Id);
                        world.RemovePlayer(player.Id);
                    }
                    else
                    {
                        LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[RemovePlayerFromWorld] Fully removing player {player.Id} from world {loc.WorldId}, map {loc.MapId}.");
                        map?.RemovePlayer(player.Id);

                        world.RemovePlayer(player.Id);
                    }
                }
                else
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[RemovePlayerFromWorld] Map {loc.MapId} not found in world {worldId}.");
                }

                world.RemovePlayer(player.Id);

                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[RemovePlayerFromWorld] Player {player.Id} removed and despawned from world {worldId}.");
            }

            return true;
        }

        public void RemovePlayer(PlayerModel player)
        {

        }
        private bool AddPlayerToLoadedWorld(PlayerModel player, WorldModel world)
        {
            var loc = player.Location!;
            LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayerToLoadedWorld] Trying to add player {player.Id} to world {loc.WorldId} (MapId={loc.MapId})...");

            lock (world)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayerToLoadedWorld] Lock acquired. Maps in world {loc.WorldId}: {world.Maps.Count}.");


                if (!TrySpawnPlayerInWorldAndMap(player, world))
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayerToLoadedWorld] Failed to spawn player {player.Id} in world {loc.WorldId} or map {loc.MapId}.");
                    return false;
                }

                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayerToLoadedWorld] Player {player.Id} successfully added and spawned in world {loc.WorldId}, map {loc.MapId}.");
                return true;
            }
        }


        private bool TrySpawnPlayerInWorldAndMap(PlayerModel player, WorldModel world)
        {
            var loc = player.Location!;
            LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[TrySpawnPlayer] Spawning player {player.Id} in world {loc.WorldId}, map {loc.MapId}"); // NUEVO LOG

            var map = world.Maps.FirstOrDefault(m => m.Id == loc.MapId);
            if (map == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[TrySpawnPlayer] Map {loc.MapId} not found."); // NUEVO LOG
                return false;
            }

            if (player.Stats == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[TrySpawnPlayer] Player {player.Id} has null stats."); // NUEVO LOG
                return false;
            }

            if (!world.TrySpawnPlayer(player.Id))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[TrySpawnPlayer] World failed to spawn player {player.Id}."); // NUEVO LOG
                return false;
            }

            if (!map.AddPlayer(player.Id, player.Stats.Level))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[TrySpawnPlayer] Map {loc.MapId} failed to add player {player.Id}."); // NUEVO LOG
                world.RemovePlayer(player.Id);
                return false;
            }

            LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[TrySpawnPlayer] Player {player.Id} successfully spawned."); // NUEVO LOG
            return true;
        }
        public List<PlayerModel> GetPlayersInArea(int worldId, int mapId, float centerX, float centerY, float radius)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[GetPlayersInArea] Called with WorldId={worldId}, MapId={mapId}, Center=({centerX},{centerY}), Radius={radius}");

            if (!_worlds.TryGetValue(worldId, out var world))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[GetPlayersInArea] World {worldId} not found.");
                return new List<PlayerModel>();
            }

            var map = world.Maps.FirstOrDefault(m => m.Id == mapId);
            if (map == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[GetPlayersInArea] Map {mapId} not found in World {worldId}.");
                return new List<PlayerModel>();
            }

            var playersInArea = new List<PlayerModel>();
            float radiusSquared = radius * radius;

            foreach (var playerId in map.SpawnedPlayerIds)
            {
                if (!_gameServer.PlayerModels.TryGetValue(playerId, out var otherPlayer))
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[GetPlayersInArea] Player {playerId} not found in PlayerModels.");
                    continue;
                }

                var otherLoc = otherPlayer.Location;
                if (otherLoc == null || otherLoc.MapId != mapId)
                    continue;

                float dx = otherLoc.PosX - centerX;
                float dy = otherLoc.PosY - centerY;
                float distanceSquared = dx * dx + dy * dy;

                if (distanceSquared <= radiusSquared)
                {
                    playersInArea.Add(otherPlayer);
                }
            }

            LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[GetPlayersInArea] Found {playersInArea.Count} players in area.");
            return playersInArea;
        }

    }
}
