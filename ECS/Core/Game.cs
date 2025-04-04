public class GameEngine : Engine
{
    public WorldManager _worldManager;
    public SyncSystem _syncSystem;

    private bool userHasInteracted = false;

    public GameEngine(Lazy<MessageSender> messageSender)
    {
        _syncSystem = new SyncSystem(this.Entities, messageSender);
        _worldManager = new WorldManager(this.Entities, _syncSystem);
    }

    public override void Start()
    {


        // Entities.AddListeners<Character>(new CharacterRenderingSystem(this),
        //     EntityManager.ComponentAdded
        // );



        // var player = Player.Create(this, Vec2Utils.Zero);
        // Entities.AddComponents(player, new MoveWithUserInput());

        // Entities.Create(
        //     new Position(0, 0),
        //     new Camera(Vec2.DivF(mainView.Resolution, 2)),
        //     new FollowEntity(player)
        // );
    }

    public override void Update()
    {

        // worldManager.Update();
        _worldManager.UpdateWorld();
    }

    private void HandleMouseDown(int X, int Y)
    {
        Position pos = new Position(X, Y);
    }

    private void HandleMouseUp(object sender, EventArgs e) { }

    private void HandleMouseMove(int X, int Y)
    {
        Position pos = new Position(X, Y);
    }

    private void HandleMouseLeave(object sender, EventArgs e) { }

    private void UserInteracted()
    {
        userHasInteracted = true;
    }

    private void HandleFocus(object sender, EventArgs e)
    {
        if (!userHasInteracted)
            return;
    }

    private void HandleBlur(object sender, EventArgs e)
    {
        HandleMouseLeave(sender, e);
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
    public void UnSpawnPlayer(Guid accountId, PlayerModel playerModel, PlayerLocation playerLocation)
    {
        PlayerEntity? playerEntity = (PlayerEntity?)Entities.GetPlayerByAccountId(accountId);
        if (playerEntity != null){
                        Console.WriteLine($"Player {playerModel.Id} unspawn");

            Entities.Remove(playerEntity);
            _worldManager.UnSpawnPlayer(playerLocation.World.Id);

        }

    }
    public void SpawnPlayer(Guid accountId, PlayerModel playerModel, PlayerLocation playerLocation)
    {

        EventBuffer eventBuffer = new EventBuffer();
        Position playerPosition = new Position(playerLocation.PosX, playerLocation.PosY);
        Character character = new Character(accountId, playerModel.Id, playerLocation.World.Id, playerLocation.Map.Id);


        WorldEntity world = (WorldEntity)_worldManager.GetOrCreateWorld(playerLocation.World.Id);
        Entity mapEntity = _worldManager.GetOrCreateMap(playerLocation.Map.Id, playerLocation.World.Id);

        MapComponent? mapComponent = (MapComponent?)mapEntity.Get(typeof(MapComponent));
        WorldComponent? worldComponent = (WorldComponent?)world.Get(typeof(WorldComponent));

        if (worldComponent != null && mapComponent != null)
        {
            Component[] components = [
                eventBuffer,
                playerPosition,
                character,
                worldComponent,
                mapComponent
            ];

            PlayerEntity playerEntity = (PlayerEntity)Entities.CreateUserEntity(accountId, components);
            _worldManager.SpawnPlayer(worldComponent.GetWorldId(), playerEntity, character, playerPosition);
        }

    }

}
