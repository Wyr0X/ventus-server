using Game.Server;
using Ventus.Network.Packets;

public class SessionSystem
{
    private readonly GameServer _gameServer;


    public SessionSystem(GameServer gameServer, SystemHandler taskHandler)
    {
        _gameServer = gameServer;
        taskHandler.Subscribe(CustomGameEvent.PlayerSpawn, HandleSpawnPlayer);
    }

    public void HandleSpawnPlayer(dynamic gameEvent)
    {
        // Validar que los datos del evento sean correctos
        LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem, $"EMTRA SESSION SYSTEM {gameEvent}");

        if (!(gameEvent.PlayerModel is PlayerModel playerModel) ||
            !(gameEvent.AccountModel is AccountModel accountModel))
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem, "Invalid event data for PlayerSpawn");
            return;
        }
        // Evitar doble spawn
        if (playerModel.isSpawned)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem,
                $"Player {playerModel.Id} is already spawned.");
            return;
        }

        // Verificar ubicación del jugador
        var loc = playerModel.Location;
        if (loc == null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem, "PlayerLocation is null");
            return;
        }

        // Intentar agregar al mundo y mapa
        bool canSpawn = _gameServer.worldManager.AddPlayer(playerModel);
        if (!canSpawn)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem,
                $"Failed to spawn player {playerModel.Id} in world {loc.WorldId}, map {loc.MapId}");
            return;
        }
        // Marcar estado y registrar en memoria
        playerModel.isSpawned = true;
        accountModel.ActivePlayerId = playerModel.Id;
        _gameServer.PlayerModels[playerModel.Id] = playerModel;

        PlayerSpawn playerJoin = new PlayerSpawn
        {
            PlayerId = playerModel.Id,
        };

        _gameServer._webSocketServerController._outgoingQueue.Enqueue(accountModel.AccountId, playerJoin, ServerPacket.PlayerSpawn);

        LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem,
            $"Player {playerModel.Id} spawned successfully in world {loc.WorldId}, map {loc.MapId}");

    }
}
