using Ventus.Network.Packets;

public enum GameEventType
{
    ClientPacket,
    CustomGameEvent,
}

public enum CustomGameEvent
{
    PlayerSpawn,
    PlayerExit,
    GetWorldData,
}
public class GameEvent
{
    public GameEventType PacketType { get; set; }
    public CustomGameEvent Type { get; set; }
    public object? Data { get; set; }
}


