using Game.Server;
using Ventus.Network.Packets;
using static LoggerUtil;

public class GameEventHandler
{
    private readonly GameServer _gameServer;
    private readonly Dictionary<ClientPacket, List<Action<UserMessagePair>>> _clientPacketHandlers = new();
    private readonly Dictionary<CustomGameEvent, List<Action<dynamic>>> _customGameEventHandlers = new();

    public GameEventHandler(GameServer gameServer)
    {
        _gameServer = gameServer;
        Log(LogTag.GameEventHandler, "GameEventHandler initialized");
    }

    public void Subscribe(ClientPacket type, Action<UserMessagePair> handler)
    {
        if (!_clientPacketHandlers.ContainsKey(type))
        {
            _clientPacketHandlers[type] = new List<Action<UserMessagePair>>();
            Log(LogTag.GameEventHandler, $"Subscribed first handler for packet type: {type}", "Subscribe");
        }

        _clientPacketHandlers[type].Add(handler);
        Log(LogTag.GameEventHandler, $"Added handler for packet type: {type}", "Subscribe");
    }

    public void Subscribe(CustomGameEvent type, Action<dynamic> handler)
    {
        if (!_customGameEventHandlers.ContainsKey(type))
        {
            _customGameEventHandlers[type] = new List<Action<dynamic>>();
            Log(LogTag.GameEventHandler, $"Subscribed first handler for custom game event type: {type}", "SubscribeCustom");
        }

        _customGameEventHandlers[type].Add(handler);
        Log(LogTag.GameEventHandler, $"Added handler for custom game event type: {type}", "SubscribeCustom");
    }

    public void HandlePacket(UserMessagePair userMessagePair)
    {
        var type = userMessagePair.PacketType;
        if (_clientPacketHandlers.TryGetValue(type, out var handlerList))
        {
            Log(LogTag.GameEventHandler, $"Handling packet type: {type} with {handlerList.Count} handler(s)", "HandlePacket");
            foreach (var handler in handlerList)
            {
                handler(userMessagePair);
            }
        }
        else
        {
            Log(LogTag.GameEventHandler, $"No handler found for packet type: {type}", "HandlePacket", isError: true);
        }
    }

    public void HandlePacket(GameEvent gameEvent)
    {
        switch (gameEvent.PacketType)
        {
            case GameEventType.ClientPacket:
                if (gameEvent.Data is UserMessagePair userMessagePair)
                {
                    var type = userMessagePair.PacketType;
                    if (_clientPacketHandlers.TryGetValue(type, out var userHandlers))
                    {
                        Log(LogTag.GameEventHandler, $"Dispatching ClientPacket of type: {type} to {userHandlers.Count} handler(s)", "HandleGameEvent");
                        foreach (var handler in userHandlers)
                        {
                            handler(userMessagePair);
                        }
                    }
                    else
                    {
                        Log(LogTag.GameEventHandler, $"No handler registered for ClientPacket of type: {type}", "HandleGameEvent", isError: true);
                    }
                }
                else
                {
                    Log(LogTag.GameEventHandler, "Invalid data type for ClientPacket in GameEvent", "HandleGameEvent", isError: true);
                }
                break;

            case GameEventType.CustomGameEvent:
                CustomGameEvent eventType = (CustomGameEvent)gameEvent.Type;

                if (_customGameEventHandlers.TryGetValue(eventType, out var eventHandlers))
                {
                    Log(LogTag.GameEventHandler, $"Dispatching CustomGameEvent of type: {eventType} to {eventHandlers.Count} handler(s)", "HandleGameEvent");
                    foreach (var handler in eventHandlers)
                    {
                        if (gameEvent.Data != null)
                        {
                            handler(gameEvent.Data);
                        }
                        else
                        {
                            Log(LogTag.GameEventHandler, "GameEvent data is null, skipping handler invocation", "HandleGameEvent", isError: true);
                        }
                    }
                }
                else
                {
                    Log(LogTag.GameEventHandler, $"No handler registered for CustomGameEvent of type: {eventType}", "HandleGameEvent", isError: true);
                }
                break;

            default:
                Log(LogTag.GameEventHandler, $"Unhandled GameEvent packet type: {gameEvent.PacketType}", "HandleGameEvent", isError: true);
                break;
        }
    }
}
