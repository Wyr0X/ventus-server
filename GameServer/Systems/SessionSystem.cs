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
        // Guid accountId = gameEvent.AccountId;
        // PlayerModel playerModel = gameEvent.PlayerModel;
        // PlayerLocationModel playerLocation = gameEvent.PlayerLocation;

        // Position playerPosition = new Position(playerLocation.PosX, playerLocation.PosY);
        // Character character = new Character(
        //     accountId,
        //     playerModel.Id,
        //     playerModel.Name,
        //     playerLocation.World.Id,
        //     playerLocation.Map.Id
        // );

        // WorldEntity world = (WorldEntity)_gameServer.worldManager.GetOrCreateWorld(playerLocation.World.Id);
        // Entity mapEntity = _gameServer.worldManager.GetOrCreateMap(
        //     playerLocation.Map.Id,
        //     playerLocation.World.Id
        // );

        // MapComponent? mapComponent = (MapComponent?)mapEntity.Get(typeof(MapComponent));
        // WorldComponent? worldComponent = (WorldComponent?)world.Get(typeof(WorldComponent));

        // if (worldComponent != null && mapComponent != null)
        // {
        //     Component[] components =
        //     [
        //         eventBuffer,
        //         playerPosition,
        //         character,
        //         worldComponent,
        //         mapComponent,
        //     ];

        //     PlayerEntity playerEntity = (PlayerEntity)
        //         Entities.CreateUserEntity(accountId, playerModel.Id, components);
        //     _worldManager.SpawnPlayer(
        //         worldComponent.GetWorldId(),
        //         playerEntity,
        //         character,
        //         playerPosition
        //     );
        // }
    }
}
