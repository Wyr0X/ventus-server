using Ventus.Client;

public class MessageDispatcher
{
    private readonly Dictionary<ClientMessage.PayloadOneofCase, List<Action<UserMessagePair>>> _handlers = new();

    public void Subscribe(ClientMessage.PayloadOneofCase type, Action<UserMessagePair> handler)
    {

        if (!_handlers.ContainsKey(type))
            _handlers[type] = new List<Action<UserMessagePair>>();

        _handlers[type].Add(handler);
    }

    public void Dispatch(UserMessagePair message)
    {
        var type = message.ClientMessage.PayloadCase;

        if (_handlers.TryGetValue(type, out var handlerList))
        {
            foreach (var handler in handlerList)
            {

                handler(message);
            }
        }
        else
        {
            // LoggerUtil.Log(LoggerUtil.LogTag.Dispatcher, $"❌ No hay handler para el mensaje {type}");
        }
    }
}
