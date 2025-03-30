public class World
{
    public int Id { get; set; }
    //private Maps
    private EntityManager Entities = new EntityManager();

    public World(int id)
    {
        Id = id;

        List<ISystem> systems =
        [
            //new MoveWithUserInputSystem(this),
            new UpdateMovableSystem(),
            new FollowEntitySystem(),
            new BroadcastWorldSystem(this),
        ];

        Entities.AddSystems(SystemTrigger.Update, systems);
    }

    public void Update() {
        Entities.RunSystems(SystemTrigger.Update);
    }

    public void SpawnPlayer(PlayerLocation playerLocation) {
        // instanciamos
        Entities.Create(
            new Position(playerLocation.PosX, playerLocation.PosY, playerLocation.Map.id),
            new PlayerStats(),
            new PlayerNetworking(),
            //new FollowComponent(this, playerLocation)
        );
    }
}
