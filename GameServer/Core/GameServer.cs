
public class GameServer
{
    private CancellationTokenSource? LoopCancellation;

    private readonly SessionSystem _sessionSystem;
    public readonly SystemHandler systemHandler;
    public readonly TaskScheduler taskScheduler;
    public readonly PacketHandler packetHandler;

    public GameServer(Lazy<MessageSender> messageSender, TaskScheduler taskScheduler)
    {
        // worldManager = new WorldManager(this.Entities, _syncSystem);
        // _syncSystem = new SyncSystem(this.Entities, messageSender);
        this.taskScheduler = taskScheduler;
        this.systemHandler = new SystemHandler();
        _sessionSystem = new SessionSystem(this, systemHandler);
        packetHandler = new PacketHandler(this);
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        await Loop(cancellationToken);
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
        // _worldManager.Update();
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
                    var data = gameEvent.Data;

                    systemHandler.HandlePacket(data);
                    break;
                case GameEventType.ClientPacket:


                    if (gameEvent.Data is UserMessagePair userMessagePair)
                    {
                        packetHandler.HandlePacket(userMessagePair);
                    }

                    break;
            }
            gameEvent = taskScheduler.eventBuffer.DequeueEvent();
        }
    }

    // public void EnqueuEvent(GameEvent gameEvent)
    // {
    //     Entity? playerEntity = Entities.GetPlayerByAccountId(gameEvent.GetAccountId());
    //     if (playerEntity != null)
    //     {
    //         EventBuffer? eventBuffer = (EventBuffer?)playerEntity.Get(typeof(EventBuffer));
    //         if (eventBuffer != null)
    //         {
    //             eventBuffer.EnqueueEvent(gameEvent);
    //         }
    //     }
    // }

    // public void UnSpawnPlayer(
    //     Guid accountId,
    //     PlayerModel playerModel,
    //     PlayerLocation playerLocation
    // )
    // {
    //     PlayerEntity? playerEntity = (PlayerEntity?)Entities.GetPlayerByAccountId(accountId);
    //     if (playerEntity != null)
    //     {
    //         Entities.Remove(playerEntity);
    //         _worldManager.UnSpawnPlayer(playerLocation.World.Id);
    //     }
    // }
}
