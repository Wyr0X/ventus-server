using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Models;
using Ventus.Network.Packets;

namespace Game.Server
{
    /// <summary>
    /// Gestiona la carga de mundos y la incorporación de jugadores.
    /// </summary>
    public class WorldManager
    {
        private readonly ConcurrentDictionary<int, WorldModel> _worlds = new();
        private readonly ConcurrentDictionary<int, ConcurrentBag<PlayerObject>> _playersByWorldId = new();
        private readonly ConcurrentDictionary<int, Task<WorldModel?>> _loadingWorlds = new();

        private readonly GameServer _gameServer;

        public WorldManager(GameServer gameServer)
        {
            _gameServer = gameServer;
        }

        /// <summary>
        /// Versión asincrónica de AddPlayerToWorld, con async/await.
        /// </summary>
        public async Task AddPlayerToWorldAsync(PlayerModel player, Action<bool> onFinished)
        {
            var loc = player.Location;
            if (loc == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                    $"[AddPlayerAsync] Player {player.Id} has null location.");
                onFinished(false);
                return;
            }

            int worldId = loc.WorldId;
            LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                $"[AddPlayerAsync] Player {player.Id} requests World {worldId}, MapId={loc.MapId}");

            // Intentamos sacar el mundo de la caché
            if (!_worlds.TryGetValue(worldId, out var world))
            {
                // Si no está, esperamos la carga (o reutilizamos la existente)
                var loadTask = _loadingWorlds.GetOrAdd(worldId, id =>
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                        $"[AddPlayerAsync] Starting async load for world {id}...");
                    return _gameServer._gameServiceMediator.GetWorldInfo(id);
                });

                WorldModel? loaded;
                try
                {
                    loaded = await loadTask;
                }
                catch (Exception ex)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                        $"[AddPlayerAsync] Exception loading world {worldId}: {ex.Message}");
                    _loadingWorlds.TryRemove(worldId, out _);
                    onFinished(false);
                    return;
                }

                _loadingWorlds.TryRemove(worldId, out _);

                if (loaded == null)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                        $"[AddPlayerAsync] Failed to load world {worldId}.");
                    onFinished(false);
                    return;
                }

                // Cacheamos el mundo cargado
                _worlds[worldId] = loaded;
                world = loaded;
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager, $"[AddPlayerAsync] World {worldId} loaded and cached.");
                _gameServer.Schedule(() =>
                {
                    AddPlayerToLoadedWorld(player, world, onFinished);

                });
                return;

            }

            // Ya tenemos el mundo, delegamos al método sincrónico
            AddPlayerToLoadedWorld(player, world, onFinished);
        }

        /// <summary>
        /// Elimina un jugador del mundo, y si queda vacío también quita el mundo.
        /// </summary>
        public bool RemovePlayerFromWorld(PlayerModel player, bool fullExit = true)
        {
            var loc = player.Location;
            if (loc == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                    $"[RemovePlayer] Player {player.Id} has null location.");
                return false;
            }

            int worldId = loc.WorldId;
            if (!_worlds.TryGetValue(worldId, out var world))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                    $"[RemovePlayer] World {worldId} not found.");
                return false;
            }

            lock (world)
            {
                // Des-spawn
                var map = world.Maps.FirstOrDefault(m => m.Id == loc.MapId);
                if (map != null)
                {
                    map.RemovePlayer(player.Id);
                    world.RemovePlayer(player.Id);
                    LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                        $"[RemovePlayer] Player {player.Id} removed from World {worldId}, Map {loc.MapId}.");
                }

                // Quitar de la bolsa
                if (_playersByWorldId.TryGetValue(worldId, out var bag))
                {
                    var filtered = new ConcurrentBag<PlayerObject>(bag.Where(p => p.Id != player.Id));
                    _playersByWorldId[worldId] = filtered;

                    // Si ya no quedan jugadores, limpiamos todo
                    if (filtered.IsEmpty)
                    {
                        _worlds.TryRemove(worldId, out _);
                        _playersByWorldId.TryRemove(worldId, out _);
                        LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                            $"[RemovePlayer] World {worldId} is now empty—removed from cache.");
                    }
                }
            }


            return true;
        }

        /// <summary>
        /// Lógica común de spawn + registro de jugador en estructuras hilo‐seguras.
        /// </summary>
        private void AddPlayerToLoadedWorld(PlayerModel player, WorldModel world, Action<bool> onFinished)
        {
            int worldId = world.Id;

            // Aseguramos que esté cacheado
            _worlds.AddOrUpdate(worldId, world, (_, __) => world);

            bool spawned;
            try
            {
                lock (world)
                {
                    spawned = TrySpawnPlayerInWorldAndMap(player, world);
                }
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                    $"[AddPlayerToLoadedWorld] Exception: {ex.Message}");
                onFinished(false);
                return;
            }

            if (!spawned)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                    $"[AddPlayer] Failed to spawn Player {player.Id} in World {worldId}.");
                onFinished(false);
                return;
            }

            // Registro en playersInTheGame
            var playerEntity = new PlayerObject(player.Id, new Vec2(player.Location!.PosX, player.Location!.PosY), player.Name, player);
            _gameServer.playersInTheGame[player.Id] = playerEntity;
            _gameServer.playersByAccountId[player.AccountId] = playerEntity;

            // Añadir al bag
            var bag = _playersByWorldId.GetOrAdd(worldId, _ => new ConcurrentBag<PlayerObject>());
            if (!bag.Any(p => p.Id == player.Id))
                bag.Add(playerEntity);

            LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                $"[AddPlayer] Player {player.Id} registered in World {worldId}. Total now: {bag.Count}");

            onFinished(true);
        }

        private bool TrySpawnPlayerInWorldAndMap(PlayerModel player, WorldModel world)
        {
            var loc = player.Location!;
            var map = world.Maps.FirstOrDefault(m => m.Id == loc.MapId);

            //Borrar codigo este

            if (world.ContainsPlayer(player.Id))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.WorldManager,
                    $"[TrySpawnPlayer] Player {player.Id} already in World {world.Id}.");
                world.RemovePlayer(player.Id);
            }
            if (map == null || player.Stats == null)
                return false;

            if (!world.TrySpawnPlayer(player.Id))
                return false;

            if (!map.AddPlayer(player.Id, player.Stats.Level))
            {
                world.RemovePlayer(player.Id);
                return false;
            }

            return true;
        }

        public List<PlayerModel> GetPlayersInArea(int worldId, int mapId, float centerX, float centerY, float radius)
        {
            if (!_worlds.TryGetValue(worldId, out var world))
                return new List<PlayerModel>();

            var map = world.Maps.FirstOrDefault(m => m.Id == mapId);
            if (map == null)
                return new List<PlayerModel>();

            float r2 = radius * radius;
            var result = new List<PlayerModel>();
            foreach (var pid in map.SpawnedPlayerIds)
            {
                if (!_gameServer.PlayerModels.TryGetValue(pid, out var other)) continue;
                var ol = other.Location;
                if (ol == null || ol.MapId != mapId) continue;

                float dx = ol.PosX - centerX, dy = ol.PosY - centerY;
                if (dx * dx + dy * dy <= r2)
                    result.Add(other);
            }
            return result;
        }

        public Task UpdateWorlds()
        {
            foreach (var kv in _worlds)
            {
                int worldId = kv.Key;
                if (!_playersByWorldId.TryGetValue(worldId, out var bag)) continue;

                var players = bag.ToArray();
                var pkt = new UpdateWorld();
                pkt.Players.AddRange(players.Select(p => new PlayerState
                {
                    Id = p.Id,
                    X = p.Position.X,
                    Y = p.Position.Y,
                    Direction = p.CurrentDirection,
                    LastSequenceNumberProcessed = p.LastSequenceNumberProcessed,
                }));

                foreach (var p in players)
                {
                    _gameServer._webSocketServerController
                        ._outgoingQueue.Enqueue(p.PlayerModel.AccountId, pkt, ServerPacket.UpdateWorld);
                }
            }
            return Task.CompletedTask;
        }

    }
}
