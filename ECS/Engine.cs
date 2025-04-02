using System.Numerics;

public abstract class Engine
{
    public const int TILE_SIZE = 32;
    public const double TIME_STEP = 1000.0 / 60.0; // 60fps
    public const double MAX_DELTA_TIME = 50;

    // Engine vars
    protected EntityManager Entities = new EntityManager();

    //  protected Items Items = new Items();
    //protected GameWorld World = new GameWorld();

    // Loop vars
    public double Time { get; set; } = 0;
    private double AccTime = 0;
    private CancellationTokenSource? LoopCancellation;

    // Events
    protected Dictionary<string, bool> Keys = new Dictionary<string, bool>();

    protected Engine()
    {
    }

    public async Task Run()
    {
        LoopCancellation = new CancellationTokenSource();
        await GameLoop(LoopCancellation.Token);
    }

    private async Task GameLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            double newTime = GetTime();
            double dt = Math.Min(newTime - Time, MAX_DELTA_TIME);
            AccTime += dt;
            Time = newTime;

            while (AccTime > TIME_STEP)
            {
                Update();
                AccTime -= TIME_STEP;
            }

            await Task.Delay(16); // Aproximadamente 60fps
        }
    }

    public void Stop()
    {
        LoopCancellation?.Cancel();
    }

    public abstract void Start();
    public abstract void Update();

    public static Vector2 ToTile(Vector2 pos)
    {
        return new Vector2(
            (float)Math.Floor(pos.X / TILE_SIZE),
            (float)Math.Floor(pos.Y / TILE_SIZE)
        );
    }

    private static double GetTime()
    {
        return DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalMilliseconds;
    }
}
