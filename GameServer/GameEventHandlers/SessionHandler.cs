using Game.Server;
using Ventus.Network.Packets;
public class SpawnPlayerData
{
    public required PlayerModel PlayerModel { get; set; }
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
        gameEventHandler.Subscribe(CustomGameEvent.PlayerSpawn, HandleSpawnPlayer);
        gameEventHandler.Subscribe(CustomGameEvent.PlayerExit, HandleUnspawnPlayer);
    }

    public void HandleSpawnPlayer(dynamic gameDataPacket)
    {
        LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem, $"[HandleSpawnPlayer] Incoming event: {gameDataPacket.ToString()}");



        var data = gameDataPacket as SpawnPlayerData;
        if (data == null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem, "Invalid event data type (expected SpawnPlayerData)");
            return;
        }
        // Validar que los datos del evento sean correctos
        if (!(data.PlayerModel is PlayerModel playerModel) ||
            !(data.AccountModel is AccountModel accountModel))
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem, "Invalid event data for PlayerSpawn");
            return;
        }

        // Evitar doble spawn
        if (playerModel.isSpawned)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem,
                $"[HandleSpawnPlayer] Player {playerModel.Id} is already spawned, removing from world...");
            _gameServer.worldManager.RemovePlayerFromWorld(playerModel, false);
            playerModel.isSpawned = false;
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
        _gameServer.worldManager.AddPlayerToWorld(playerModel, canSpawn =>
        {
            if (!canSpawn)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem,
                    $"[HandleSpawnPlayer] Failed to spawn player {playerModel.Id} in world {loc.WorldId}, map {loc.MapId}");
                return;
            }

            playerModel.isSpawned = true;
            accountModel.ActivePlayerId = playerModel.Id;
            _gameServer.PlayerModels[playerModel.Id] = playerModel;

            PlayerSpawn playerJoin = new PlayerSpawn
            {
                PlayerId = playerModel.Id,
                X = loc.PosX,
                Y = loc.PosY,
                Name = playerModel.Name
            };

            _gameServer._webSocketServerController._outgoingQueue.Enqueue(accountModel.AccountId, playerJoin, ServerPacket.PlayerSpawn);

            LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem,
                $"[HandleSpawnPlayer] Player {playerModel.Id} spawned successfully in world {loc.WorldId}, map {loc.MapId}");
        });
    }

    public void HandleUnspawnPlayer(dynamic gameEvent)
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
}
