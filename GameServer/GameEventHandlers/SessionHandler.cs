using Game.Server;
using Google.Protobuf;
using Ventus.Network.Packets;
public class SpawnPlayerData
{
    public required PlayerModel PlayerModel { get; set; }
    public required PlayerModel? PlayerSpawnedModel { get; set; }
    public required AccountModel AccountModel { get; set; }

    public override string ToString()
    {
        return $"[SpawnPlayerData] PlayerId={PlayerModel?.Id}, AccountId={AccountModel?.AccountId}";
    }
}

public class SessionHandler
{
    private readonly GameServer _gameServer;

    public SessionHandler(GameServer gameServer, GameEventHandler gameEventHandler)
    {
        _gameServer = gameServer;
        gameEventHandler.Subscribe(CustomGameEvent.PlayerSpawn, async (customGamEvent) => await HandleSpawnPlayer(customGamEvent));
        gameEventHandler.Subscribe(CustomGameEvent.PlayerExit, async (customGamEvent) => await HandleUnspawnPlayer(customGamEvent));

    }

    public async Task HandleSpawnPlayer(dynamic gameDataPacket)
    {
        LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem, $"[HandleSpawnPlayer] Incoming event: {gameDataPacket.ToString()}");


        var data = gameDataPacket as SpawnPlayerData;
        if (data == null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem, "Invalid event data type (expected SpawnPlayerData)");
            return;
        }
        // Validar que los datos del evento sean correctos
        PlayerModel? playerSpawnedModel = null;
        if (!(data.PlayerModel is PlayerModel playerModel) ||
        (data.PlayerSpawnedModel != null && data.PlayerSpawnedModel is not PlayerModel) ||
            !(data.AccountModel is AccountModel accountModel))
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem, "Invalid event data for PlayerSpawn");
            return;
        }
        LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem, $"[HandleSpawnPlayer] PlayerModel: {playerModel.ToString()}");


        // Evitar doble spawn
        if (playerModel.isSpawned)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem,
                $"[HandleSpawnPlayer] Player {playerModel.Id} is already spawned, removing from world...");
            _gameServer.worldManager.RemovePlayerFromWorld(playerModel, false);
            playerModel.isSpawned = false;
            accountModel.ActivePlayerId = null;
        }
        if (playerSpawnedModel != null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem,
                $"[HandleSpawnPlayer] Player {accountModel.ActivePlayerId} is already spawned, removing from world...");
            _gameServer.worldManager.RemovePlayerFromWorld(playerSpawnedModel, false);
            playerSpawnedModel.isSpawned = false;
            accountModel.ActivePlayerId = null;
        }

        // Verificar ubicación del jugador
        var loc = playerModel.Location;
        if (loc == null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem, "[HandleSpawnPlayer] PlayerLocation is null");
            return;
        }

        // Intentar agregar al mundo y mapa
        _ = _gameServer.worldManager.AddPlayerToWorldAsync(playerModel, canSpawn =>
        {
            if (!canSpawn)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem,
                    $"[HandleSpawnPlayer] Failed to spawn player {playerModel.Id} in world {loc.WorldId}, map {loc.MapId}");

                PlayerSpawnError playerSpawnError = new()
                {
                    PlayerId = playerModel.Id,
                };
                _gameServer._webSocketServerController._outgoingQueue.Enqueue(accountModel.AccountId, playerSpawnError, ServerPacket.PlayerSpawnError);

                return;
            }

            playerModel.isSpawned = true;
            accountModel.ActivePlayerId = playerModel.Id;
            _gameServer.PlayerModels[playerModel.Id] = playerModel;
            var _playersInTheWorld = _gameServer.worldManager.GetPlayersInArea(loc.WorldId, loc.MapId, loc.PosX, loc.PosY, 1000);

            SelfSpawn selfSpawnPacket = new()
            {
                PlayerId = playerModel.Id,
                X = loc.PosX,
                Y = loc.PosY,
                Name = playerModel.Name
            };
            selfSpawnPacket.Players.AddRange(_playersInTheWorld.Select(p => new PlayerUpdateData
            {
                PlayerId = p.Id,
                X = p.Location!.PosX,
                Y = p.Location!.PosY,
                Name = p.Name
            }));

            _gameServer._webSocketServerController._outgoingQueue.Enqueue(accountModel.AccountId, selfSpawnPacket, ServerPacket.SelfSpawn);

            foreach (var player in _playersInTheWorld)
            {
                if (player.Id != playerModel.Id)
                {
                    PlayerSpawn playerSpawn = new()
                    {
                        PlayerId = playerModel.Id,
                        X = loc.PosX,
                        Y = loc.PosY,
                        Name = playerModel.Name
                    };
                    _gameServer._webSocketServerController._outgoingQueue.Enqueue(player.AccountId, playerSpawn, ServerPacket.PlayerSpawn);
                }
            }
            LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem,
                $"[HandleSpawnPlayer] Player {playerModel.Id} spawned successfully in world {loc.WorldId}, map {loc.MapId}, current player active {accountModel.ActivePlayerId}");
        });
    }

    public async Task HandleUnspawnPlayer(dynamic gameEvent)
    {
        LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem, $"[HandleUnspawnPlayer] Incoming event: {gameEvent}");

        // Validar los datos del evento
        if (!(gameEvent.PlayerModel is PlayerModel playerModel) ||
            !(gameEvent.AccountModel is AccountModel accountModel))
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem, "Invalid event data for PlayerUnspawn");
            return;
        }

        // Verificar ubicación del jugador
        var loc = playerModel.Location;
        if (loc == null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem, "[HandleUnspawnPlayer] PlayerLocation is null");
            return;
        }

        // Intentar remover al jugador del mundo
        bool removed = _gameServer.worldManager.RemovePlayerFromWorld(playerModel, fullExit: false);
        if (!removed)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem,
                $"[HandleUnspawnPlayer] Failed to unspawn player {playerModel.Id} from world {loc.WorldId}, map {loc.MapId}");
            return;
        }

        // Actualizar el estado del jugador y de la cuenta
        playerModel.isSpawned = false;
        accountModel.ActivePlayerId = null;

        // Notificar que el jugador fue des-spawneado correctamente
        LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem,
            $"[HandleUnspawnPlayer] Player {playerModel.Id} unspawned successfully from world {loc.WorldId}, map {loc.MapId}");
    }

    public void HandleTimeSyncRequest(UserMessagePair userMessagePair)
    {
        TimeSyncRequest? request = userMessagePair.ClientMessage as TimeSyncRequest;
        if (request == null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem, "Invalid TimeSyncRequest packet received.");
            return;
        }
        var clientSendTime = request.ClientSendTime;
        var serverTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond; // Tiempo del servidor en ms

        var timeSyncResponse = new TimeSyncResponse
        {
            ClientSendTime = clientSendTime,
            ServerTime = serverTime,
        };

        _gameServer._webSocketServerController._outgoingQueue.Enqueue(userMessagePair.AccountId, request, ServerPacket.TimeSyncResponse);
    }

}
