public class EventBuffer
{
    private readonly Queue<GameEvent> eventQueue = new();

    // Encolar un evento en el buffer global
    public void EnqueueEvent(GameEvent gameEvent)
    {
        eventQueue.Enqueue(gameEvent);
    }

    // Desencolar el siguiente evento del buffer global
    public GameEvent? DequeueEvent()
    {
        if (eventQueue.Count > 0)
        {
            return eventQueue.Dequeue();
        }
        return null; // Si no hay eventos pendientes
    }

    // Verificar si hay eventos pendientes en el buffer global
    public bool HasEvents()
    {
        return eventQueue.Count > 0;
    }
}
