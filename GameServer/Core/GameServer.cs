using Ventus.Client;

public class GameServer
{
    private CancellationTokenSource? LoopCancellation;

    public WorldManager worldManager;
    public SyncSystem _syncSystem;
    private readonly SessionSystem _sessionSystem;
    public readonly TaskScheduler taskScheduler;
    public readonly PacketHandler packetHandler;

    public GameServer(Lazy<MessageSender> messageSender, TaskScheduler taskScheduler)
    {
        worldManager = new WorldManager(this.Entities, _syncSystem);
        _syncSystem = new SyncSystem(this.Entities, messageSender);
        this.taskScheduler = taskScheduler;
        _sessionSystem = new SessionSystem(this);
        packetHandler = new PacketHandler(this);
    }

    public async Task Run()
    {
        LoopCancellation = new CancellationTokenSource();
        await Loop(LoopCancellation.Token);
    }

    private async Task Loop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Update();
            await Task.Delay(16); // Aproximadamente 60fps
        }
    }

    public void Stop()
    {
        LoopCancellation?.Cancel();
    }

    private void Update()
    {
        // Manejar eventos
        HandleEvents();
        // Actualizar entidades
        _worldManager.Update();
        // Flush packets
    }

    private void HandleEvents()
    {
        var gameEvent = taskScheduler.eventBuffer.DequeueEvent();
        while (gameEvent != null)
        {
            switch (gameEvent.Type)
            {
                case GameEventType.CustomGameEvent:
                    _sessionSystem.HandleSpawnPlayer(gameEvent.Data!);
                    break;
                case GameEventType.ClientPacket:
                    packetHandler.HandlePacket((UserMessagePair)gameEvent.Data);
                    break;
            }
            gameEvent = taskScheduler.eventBuffer.DequeueEvent();
        }
    }

    public void EnqueuEvent(GameEvent gameEvent)
    {
        Entity? playerEntity = Entities.GetPlayerByAccountId(gameEvent.GetAccountId());
        if (playerEntity != null)
        {
            EventBuffer? eventBuffer = (EventBuffer?)playerEntity.Get(typeof(EventBuffer));
            if (eventBuffer != null)
            {
                eventBuffer.EnqueueEvent(gameEvent);
            }
        }
    }

    public void UnSpawnPlayer(
        Guid accountId,
        PlayerModel playerModel,
        PlayerLocation playerLocation
    )
    {
        PlayerEntity? playerEntity = (PlayerEntity?)Entities.GetPlayerByAccountId(accountId);
        if (playerEntity != null)
        {
            Entities.Remove(playerEntity);
            _worldManager.UnSpawnPlayer(playerLocation.World.Id);
        }
    }
}
