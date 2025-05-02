using System.Collections.Concurrent;
using Ventus.Network.Packets;

public class TaskScheduler
{
    private readonly ConcurrentDictionary<ClientPacket, List<Action<UserMessagePair>>> _handlers = new();
    public readonly EventBuffer eventBuffer = new();

    public void Subscribe(ClientPacket type, Action<UserMessagePair> handler)
    {
        LoggerUtil.Log(LoggerUtil.LogTag.TaskScheduler, $"Subscribing handler to packet: {type}");

        _handlers.AddOrUpdate(
            type,
            (_) =>
            {
                LoggerUtil.Log(LoggerUtil.LogTag.SessionSystem, $"Created new handler list for packet: {type}");
                return new List<Action<UserMessagePair>> { handler };
            },
            (_, existingList) =>
            {
                lock (existingList)
                {
                    existingList.Add(handler);
                    LoggerUtil.Log(LoggerUtil.LogTag.TaskScheduler, $"Added handler to existing list for packet: {type}");
                    return existingList;
                }
            });
    }

    public void Dispatch(UserMessagePair message)
    {
        var type = message.PacketType;

        if (_handlers.TryGetValue(type, out var handlerList))
        {
            LoggerUtil.Log(LoggerUtil.LogTag.TaskScheduler, $"Dispatching packet of type: {type} to {handlerList.Count} handler(s)");

            List<Action<UserMessagePair>> snapshot;
            lock (handlerList)
            {
                snapshot = handlerList.ToList();
            }

            foreach (var handler in snapshot)
            {
                _ = Task.Run(() =>
                {
                    try
                    {
                        handler(message);
                    }
                    catch (Exception ex)
                    {
                        LoggerUtil.Log(LoggerUtil.LogTag.TaskScheduler, $"Exception in handler for packet {type}: {ex.Message}", isError: true);
                    }
                });
            }
        }
        else
        {
            LoggerUtil.Log(LoggerUtil.LogTag.TaskScheduler, $"No handler found for packet {type}, adding to event buffer.");
            eventBuffer.EnqueueEvent(new GameEvent { Type = GameEventType.ClientPacket, Data = message });
        }
    }

    public void GetWorldData()
    {
        LoggerUtil.Log(LoggerUtil.LogTag.TaskScheduler, "Called GetWorldData() - Not yet implemented.");
    }
}
