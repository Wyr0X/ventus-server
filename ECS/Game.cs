public class Game : Engine
{
    public WorldManager worldManager = new WorldManager();
    private readonly WebSocketServerController _webSocketServerController;
    private bool userHasInteracted = false;

    public Game(WebSocketServerController webSocketServerController)
    {
        _webSocketServerController = webSocketServerController;
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
        _webSocketServerController.ProcessQueue();

        worldManager.Update();
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

    public override void Preload()
    {
        throw new NotImplementedException();
    }
}
