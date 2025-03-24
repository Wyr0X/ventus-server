public class EventBus
{
    private readonly Dictionary<string, Action<object>> _eventHandlers = new();

    public void Subscribe(string eventType, Action<object> handler)
    {
        _eventHandlers[eventType] = handler;
    }

    public void Publish(string eventType, object eventData)
    {
        if (_eventHandlers.TryGetValue(eventType, out var handler))
        {
            handler(eventData);
        }
    }
}
