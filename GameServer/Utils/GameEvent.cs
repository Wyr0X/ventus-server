using Ventus.Network.Packets;

public enum GameEventType
{
    ClientPacket,
    CustomGameEvent,
}

public enum CustomGameEvent
{
    PlayerSpawn,
    GetWorldData,
}
public class GameEvent
{
    public GameEventType Type { get; set; }
    public object? Data { get; set; }
}


