using System.Collections.Concurrent;
using Ventus.Network.Packets;

public class TaskScheduler
{
    private readonly ConcurrentDictionary<ClientPacket, List<Func<UserMessagePair, Task>>> _handlers = new();
    public readonly EventBuffer eventBuffer = new();

    public void Subscribe(ClientPacket type, Func<UserMessagePair, Task> handler)
    {
        LoggerUtil.Log(LoggerUtil.LogTag.TaskScheduler, $"Subscribing handler to packet: {type}");

        _handlers.AddOrUpdate(
            type,
            (_) =>
            {
                LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler, $"Created new handler list for packet: {type}");
                return new List<Func<UserMessagePair, Task>> { handler };
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
            if (type != ClientPacket.PlayerInput)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.TaskScheduler, $"Dispatching packet {type} to {handlerList.Count} handler(s)");
            }

            List<Func<UserMessagePair, Task>> snapshot;
            lock (handlerList)
            {
                snapshot = handlerList.ToList();
            }

            foreach (var handler in snapshot)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await handler(message);
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
            eventBuffer.EnqueueEvent(new GameEvent { PacketType = GameEventType.ClientPacket, Data = message });
        }
    }
}
