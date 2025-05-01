public class EventsSystem
{

    EntityManager _entityManager;
    public EventsSystem(EntityManager entityManager)
    {
        _entityManager = entityManager;
    }

    public void processQueues()
    {
        Dictionary<Guid, PlayerEntity> playerEntities = _entityManager.GetPlayersEntity();
        foreach (var kvp in playerEntities)
        {
            Guid key = kvp.Key; // La clave del diccionario
            PlayerEntity player = kvp.Value; // El valor asociado (PlayerEntity)
            EventBuffer? eventBuffer = player.Get(typeof(EventBuffer)) as EventBuffer;
            if (eventBuffer == null) continue;
            while (eventBuffer.HasEvents())
            {
                GameEvent? gameEvent = eventBuffer.DequeueEvent();
                if (gameEvent != null)
                {
                    switch (gameEvent)
                    {
                        case InputsKeyEvent inputsKeyEvent:
                            HasEventsandleInputsKey(inputsKeyEvent);
                            break;
                    }
                }
            }
        }
    }

    private void HasEventsandleInputsKey(InputsKeyEvent inputsKeyEvent)
    {

    }
}