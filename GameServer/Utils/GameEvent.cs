public enum GameEventType
{
    ClientPacket,
    CustomGameEvent,
}

public class GameEvent
{
    public GameEventType Type { get; set; }
    public object? Data { get; set; }
}
