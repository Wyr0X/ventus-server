public class PacketHandler
{
    private readonly GameServer _gameServer;
    private readonly Dictionary<ClientPacket, List<Action<UserMessagePair>>> _handlers = new();

    public PacketHandler(GameServer gameServer)
    {
        _gameServer = gameServer;
        new ChatHandler(gameServer);
    }

    public void Subscribe(ClientPacket type, Action<UserMessagePair> handler)
    {
        if (!_handlers.ContainsKey(type))
            _handlers[type] = [];

        _handlers[type].Add(handler);
    }

    public void HandlePacket(UserMessagePair message)
    {
        var type = (ClientPacket)message.ClientMessage.Type;

        if (_handlers.TryGetValue(type, out var handlerList))
        {
            foreach (var handler in handlerList)
            {
                handler(message);
            }
        }
    }
}
