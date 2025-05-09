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
    private readonly TimeSpan _activityCheckInterval = TimeSpan.FromSeconds(30); // Intervalo para comprobar la actividad de los jugadores
    private readonly TimeSpan _playerTimeout = TimeSpan.FromSeconds(10); // Tiempo máximo para que el jugador responda


    public SessionHandler(GameServer gameServer, GameEventHandler gameEventHandler)
    {
        _gameServer = gameServer;
        gameEventHandler.Subscribe(CustomGameEvent.PlayerSpawn, async (customGamEvent) => await HandleSpawnPlayer(customGamEvent));
        gameEventHandler.Subscribe(ClientPacket.TryToDespawnPlayer, HandleTryToDespawnPlayer);


    }

    public async Task MonitorPlayerActivityAsync(CancellationToken externalToken)
    {
        LoggerUtil.Log(LoggerUtil.LogTag.GameServer, "Iniciando monitoreo de actividad de jugadores...");
        var tasks = new List<Task>();

        while (!externalToken.IsCancellationRequested)
        {
            // Itera sobre todos los jugadores activos
            foreach (var player in _gameServer.playersInTheGame.Values)
            {
                // Enviar un paquete de solicitud de actividad al jugador
                SendActivityRequest(player);
                player.IsActiviyConfirmed = false;
                // Espera la respuesta del jugador
                tasks.Add(MonitorPlayerActivityForSinglePlayerAsync(player, externalToken));


            }

            // Esperar el siguiente ciclo
            await Task.Delay(_activityCheckInterval, externalToken);
        }
    }

    /// <summary>
    /// Envía una solicitud de actividad al jugador.
    /// </summary>
    private void SendActivityRequest(PlayerObject player)
    {
        LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"Enviando solicitud de actividad al jugador {player.PlayerModel.Id}...");
        VerifyPlayerConnection verifyPlayerConnection = new();
        _gameServer._webSocketServerController._outgoingQueue.Enqueue(player.PlayerModel.AccountId, verifyPlayerConnection, ServerPacket.VerifyPlayerConnection);

    }

    /// <summary>
    /// Espera la respuesta de actividad del jugador dentro del tiempo límite.
    /// </summary>
    public async Task MonitorPlayerActivityForSinglePlayerAsync(PlayerObject player, CancellationToken externalToken)
    {
        LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"Esperando confirmación del jugador {player.PlayerModel.Id}...");

        // Esperamos el tiempo determinado (por ejemplo, 10 segundos)
        await Task.Delay(_playerTimeout, externalToken);

        // Comprobamos si la propiedad 'IsConfirmed' sigue siendo falsa después del tiempo
        if (!player.IsActiviyConfirmed)
        {
            // Si el jugador no ha confirmado dentro del tiempo, eliminarlo
            _gameServer.RemovePlayerFromGame(player.PlayerModel.Id);
            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"Jugador {player.PlayerModel.Id} despawneado por inactividad.");
        }
        else
        {
            LoggerUtil.Log(LoggerUtil.LogTag.GameServer, $"Jugador {player.PlayerModel.Id} confirmado.");
        }
    }
    public async Task HandleSpawnPlayer(dynamic gameDataPacket)
    {
        LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler, $"[HandleSpawnPlayer] Incoming event: {gameDataPacket.ToString()}");


        var data = gameDataPacket as SpawnPlayerData;
        if (data == null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler, "Invalid event data type (expected SpawnPlayerData)");
            return;
        }
        // Validar que los datos del evento sean correctos
        PlayerModel? playerSpawnedModel = null;
        if (!(data.PlayerModel is PlayerModel playerModel) ||
        (data.PlayerSpawnedModel != null && data.PlayerSpawnedModel is not PlayerModel) ||
            !(data.AccountModel is AccountModel accountModel))
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler, "Invalid event data for PlayerSpawn");
            return;
        }
        LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler, $"[HandleSpawnPlayer] PlayerModel: {playerModel.ToString()}");


        // Evitar doble spawn
        if (playerModel.isSpawned)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler,
                $"[HandleSpawnPlayer] Player {playerModel.Id} is already spawned, removing from world...");
            _gameServer.RemovePlayerFromGame(playerModel.Id);
            playerModel.isSpawned = false;
            accountModel.ActivePlayerId = null;
        }
        if (playerSpawnedModel != null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler,
                $"[HandleSpawnPlayer] Player {accountModel.ActivePlayerId} is already spawned, removing from world...");
            _gameServer.RemovePlayerFromGame(playerSpawnedModel.Id);

            playerSpawnedModel.isSpawned = false;
            accountModel.ActivePlayerId = null;
        }

        // Verificar ubicación del jugador
        var loc = playerModel.Location;
        if (loc == null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler, "[HandleSpawnPlayer] PlayerLocation is null");
            return;
        }

        // Intentar agregar al mundo y mapa
        _ = _gameServer.worldManager.AddPlayerToWorldAsync(playerModel, canSpawn =>
        {
            if (!canSpawn)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler,
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
            LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler,
                $"[HandleSpawnPlayer] Player {playerModel.Id} spawned successfully in world {loc.WorldId}, map {loc.MapId}, current player active {accountModel.ActivePlayerId}");
        });
    }

    public Task HandleTryToDespawnPlayer(UserMessagePair userMessagePair)
    {
        try
        {
            var despawnPlayerPacket = userMessagePair.ClientMessage as TryToDespawnPlayer;
            if (despawnPlayerPacket == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler, "Invalid event data type (expected TryToDespawnPlayer)");
                return Task.CompletedTask;
            }
            LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler, $"[HandleUnspawnPlayer] Incoming AccountId: {userMessagePair.AccountId}");


            if (!_gameServer.playersByAccountId.TryGetValue(userMessagePair.AccountId, out var playerObject))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler, $"[HandleUnspawnPlayer] Player not found for account ID {userMessagePair.AccountId}");
                return Task.CompletedTask;
            }


            if (playerObject == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler, $"[HandleUnspawnPlayer] Player not found for ID {userMessagePair.AccountId}");
                return Task.CompletedTask;
            }
            LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler, $"[HandleUnspawnPlayer] Try to remove from the game ID: {playerObject.Id}");

            _gameServer.RemovePlayerFromGame(playerObject.Id); // Eliminar el jugador del juego

            return Task.CompletedTask;

        }
        catch (Exception ex)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler,
                $"[HandleUnspawnPlayer] Exception occurred: {ex.Message}\n{ex.StackTrace}");
            return Task.CompletedTask;

        }
    }


    public void HandleTimeSyncRequest(UserMessagePair userMessagePair)
    {
        TimeSyncRequest? request = userMessagePair.ClientMessage as TimeSyncRequest;
        if (request == null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler, "Invalid TimeSyncRequest packet received.");
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
