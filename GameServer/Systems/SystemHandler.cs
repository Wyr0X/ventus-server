using Ventus.Network.Packets;

public class SystemHandler
{
    private readonly Dictionary<CustomGameEvent, List<Action<UserMessagePair>>> _handlers = new();

    public void Subscribe(CustomGameEvent type, Action<UserMessagePair> handler)
    {
        if (!_handlers.ContainsKey(type))
            _handlers[type] = [];

        _handlers[type].Add(handler);
    }

    public void HandlePacket(dynamic message)
    {

        var type = (CustomGameEvent)message.type;
        if (_handlers.TryGetValue(type, out var handlerList))
        {
            foreach (var handler in handlerList)
            {
                handler(message);
            }
        }
    }
}
