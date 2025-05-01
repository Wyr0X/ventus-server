using Ventus.Client;

public class TaskScheduler
{
    private readonly Dictionary<ClientPacket, List<Action<UserMessagePair>>> _handlers = new();
    public readonly EventBuffer eventBuffer = new();

    public void Subscribe(ClientPacket type, Action<UserMessagePair> handler)
    {
        if (!_handlers.ContainsKey(type))
            _handlers[type] = [];

        _handlers[type].Add(handler);
    }

    public void Dispatch(UserMessagePair message)
    {
        var type = (ClientPacket)message.ClientMessage.Type;

        if (_handlers.TryGetValue(type, out var handlerList))
        {
            foreach (var handler in handlerList)
            {
                handler(message);
            }
        }
        else
        {
            eventBuffer.EnqueueEvent(new GameEvent { Type = GameEventType.Packet, Data = message });
        }
    }
}
