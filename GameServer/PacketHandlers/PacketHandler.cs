using Game.Server;
using Ventus.Network.Packets;

public class PacketHandler
{
    private readonly GameServer _gameServer;
    private readonly Dictionary<ClientPacket, List<Action<UserMessagePair>>> _handlers = new();

    public PacketHandler(GameServer gameServer)
    {
        _gameServer = gameServer;
    }

    public void Subscribe(ClientPacket type, Action<UserMessagePair> handler)
    {
        if (!_handlers.ContainsKey(type))
            _handlers[type] = [];

        _handlers[type].Add(handler);
    }

    public void HandlePacket(UserMessagePair userMessagePair)
    {
        var type = userMessagePair.PacketType;
        if (_handlers.TryGetValue(type, out var handlerList))
        {
            foreach (var handler in handlerList)
            {
                handler(userMessagePair);
            }
        }
    }
}
