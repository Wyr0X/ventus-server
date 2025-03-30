using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

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
    private int Frame = 0;
    public double Time { get; set; } = 0 ;
    private double AccTime = 0;
    private bool Running = false;
    private CancellationTokenSource? LoopCancellation;

    // Events
    protected Dictionary<string, bool> Keys = new Dictionary<string, bool>();

    protected Engine()
    {
        Preload();
    }

    private async void Run()
    {

        Running = true;
        LoopCancellation = new CancellationTokenSource();
        await GameLoop(LoopCancellation.Token);
    }

    private async Task GameLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && Running)
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

            Render();
            await Task.Delay(16); // Aproximadamente 60fps
        }
    }

    private void Stop()
    {
        Running = false;
        LoopCancellation?.Cancel();
    }

  
  


    public abstract void Preload();
    public abstract void Start();
    public abstract void Render();
    public abstract void Update();
    
    public void Destroy()
    {
        if (Running) Stop();
    }

    public static Vector2 ToTile(Vector2 pos)
    {
        return new Vector2((float)Math.Floor(pos.X / TILE_SIZE), (float)Math.Floor(pos.Y / TILE_SIZE));
    }

    private static double GetTime()
    {
        return DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalMilliseconds;
    }
}
