public class MovementPlayerInput
{
    public Direction Direction { get; set; }
    public long Timestamp { get; set; }
    public int SequenceNumber { get; set; }
}
public class PlayerObject : Character
{
    private const float TicksPerSecond = 1000f; // Si Timestamp está en milisegundos
    public float X { get; set; }
    public float Y { get; set; }
    public Direction CurrentDirection { get; set; }
    public bool IsMoving { get; set; }
    public long LastProcessedInputTimestamp { get; set; } = 0;
    public PlayerModel PlayerModel { get; set; }
    public bool IsActiviyConfirmed { get; set; } = false;

    public int LastSequenceNumberProcessed = 0;
    private Queue<MovementPlayerInput> inputsToProcess = new Queue<MovementPlayerInput>();
    private readonly object _inputLock = new object();

    public PlayerObject(int id, Vec2 position, string name, PlayerModel playerModel)
        : base(id, position, name)
    {
        PlayerModel = playerModel;
        IsActiviyConfirmed = true;
    }

    private bool CheckCollision()
    {
        bool collision = IsPositionBlocked(X, Y);
        if (collision)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.PlayerEntity,
                $"[CheckCollision] Collision detected for Player {Id} at position ({X}, {Y})");
        }
        return collision;
    }

    private void RevertMovement(Direction direction)
    {
        float moveDistance = Speed;
        float prevX = X;
        float prevY = Y;

        switch (direction)
        {
            case Direction.Up: Y += moveDistance; break;
            case Direction.Down: Y -= moveDistance; break;
            case Direction.Left: X += moveDistance; break;
            case Direction.Right: X -= moveDistance; break;
        }

        LoggerUtil.Log(LoggerUtil.LogTag.PlayerEntity,
            $"[RevertMovement] Player {Id} reverted movement from ({prevX}, {prevY}) back to ({X}, {Y}) due to collision.");
    }

    private bool IsPositionBlocked(float x, float y)
    {
        return (x < 0 || x >= 100 || y < 0 || y >= 100);
    }

    public void ProcessInputs()
    {
        lock (_inputLock)
        {
            while (inputsToProcess.Count > 0)
            {
                var input = inputsToProcess.Dequeue();

                if (input.SequenceNumber <= LastSequenceNumberProcessed)
                    continue;

                LoggerUtil.Log(LoggerUtil.LogTag.PlayerEntity,
                    $"[ProcessInputs] Processing input for Player {Id} with direction {input.Direction}, timestamp {input.Timestamp}, sequence #{input.SequenceNumber}");

                ApplyInputToPlayer(input);
                LastSequenceNumberProcessed = input.SequenceNumber;
                LastProcessedInputTimestamp = input.Timestamp;
            }
        }
    }

    private void ApplyInputToPlayer(MovementPlayerInput input)
    {
        if (input.Direction == Direction.None)
            return;

        // Calcular deltaTime entre inputs (en segundos)
        float deltaTime = 0.016f; // fallback por si es el primer input

        if (LastProcessedInputTimestamp > 0 && input.Timestamp > LastProcessedInputTimestamp)
        {
            deltaTime = (input.Timestamp - LastProcessedInputTimestamp) / TicksPerSecond;
        }

        // Calcular desplazamiento
        Vec2 directionVec = Vec2.DirectionToVector(input.Direction);
        Vec2 move = Vec2.Scale(directionVec, this.Speed);

        this.Position.Add(move);

        LoggerUtil.Log(LoggerUtil.LogTag.PlayerEntity,
            $"[ApplyInputToPlayer] Player {Id} moved to ({Position.X:F3}, {Position.Y:F3}) using deltaTime {deltaTime:F4}s, speed {Speed}, direction {input.Direction}");
    }

    public Task EnqueueInput(PlayerInput input)
    {
        lock (_inputLock)
        {
            inputsToProcess.Enqueue(new MovementPlayerInput
            {
                Direction = input.Direction,
                Timestamp = input.Timestamp,
                SequenceNumber = input.SequenceNumber
            });
        }

        return Task.CompletedTask;
    }
}
