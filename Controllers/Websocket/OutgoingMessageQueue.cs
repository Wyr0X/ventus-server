using Google.Protobuf;
using Ventus.Network.Packets;

public class OutgoingMessageQueue
{
    private readonly Dictionary<Guid, Queue<OutgoingPacket>> _queues = new();
    private readonly object _lock = new();

    public void Enqueue(Guid accountId, IMessage message, ServerPacket packetType)
    {
        lock (_lock)
        {
            if (!_queues.TryGetValue(accountId, out var queue))
            {
                queue = new Queue<OutgoingPacket>();
                _queues[accountId] = queue;
            }
            queue.Enqueue(new OutgoingPacket(message, packetType));
        }
    }

    public bool TryDequeue(Guid accountId, out OutgoingPacket? packet)
    {
        lock (_lock)
        {
            packet = null;
            if (_queues.TryGetValue(accountId, out var queue) && queue.Count > 0)
            {
                packet = queue.Dequeue();
                return true;
            }
            return false;
        }
    }

    public List<Guid> GetAccountIdsWithMessages()
    {
        lock (_lock)
        {
            return _queues.Where(kvp => kvp.Value.Count > 0).Select(kvp => kvp.Key).ToList();
        }
    }

    public void Clear(Guid accountId)
    {
        lock (_lock)
        {
            _queues.Remove(accountId);
        }
    }
}

public class OutgoingPacket
{
    public IMessage Message { get; }
    public ServerPacket PacketType { get; }

    public OutgoingPacket(IMessage message, ServerPacket packetType)
    {
        Message = message;
        PacketType = packetType;
    }
}
