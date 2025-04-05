using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;

public class WebSocketConnectionManager
{
    private readonly ConcurrentDictionary<string, WebSocket> _pendingConnections = new();
    private readonly ConcurrentDictionary<Guid, WebSocket> _websocketsByAccountId = new();
    private readonly ConcurrentDictionary<string, Guid> _connectionsByAccountId = new();

    public void AddPendingConnection(string connectionId, WebSocket socket)
    {
        _pendingConnections[connectionId] = socket;
        LoggerUtil.Log("WebSocket", $"Pending connection added: {connectionId}", ConsoleColor.Yellow);
    }

    public bool TryAuthenticateConnection(string connectionId, Guid accountId, out WebSocket socket)
    {
        socket = null!;
        if (_pendingConnections.TryRemove(connectionId, out var pending))
        {
            _connectionsByAccountId[connectionId] = accountId;
            _websocketsByAccountId[accountId] = pending;
            socket = pending;

            LoggerUtil.Log("WebSocket", $"Connection {connectionId} authenticated for Account {accountId}", ConsoleColor.Green);
            return true;
        }

        LoggerUtil.Log("WebSocket", $"Authentication failed for Connection {connectionId}", ConsoleColor.Red);
        return false;
    }

    public bool TryGetAccountId(string connectionId, out Guid accountId)
    {
        var result = _connectionsByAccountId.TryGetValue(connectionId, out accountId);
        LoggerUtil.Log("WebSocket", result
            ? $"Retrieved AccountId {accountId} for ConnectionId {connectionId}"
            : $"Failed to retrieve AccountId for ConnectionId {connectionId}", ConsoleColor.Cyan);
        return result;
    }

    public bool TryGetSocket(Guid accountId, out WebSocket socket)
    {
        var result = _websocketsByAccountId.TryGetValue(accountId, out socket);
        LoggerUtil.Log("WebSocket", result
            ? $"Retrieved socket for Account {accountId}"
            : $"Failed to retrieve socket for Account {accountId}", ConsoleColor.Cyan);
        return result;
    }

    public void RemoveConnection(string connectionId, Guid accountId)
    {
        _connectionsByAccountId.TryRemove(connectionId, out _);
        _websocketsByAccountId.TryRemove(accountId, out _);
        _pendingConnections.TryRemove(connectionId, out _);

        LoggerUtil.Log("WebSocket", $"Removed connection {connectionId} and Account {accountId}", ConsoleColor.DarkMagenta);
    }
}
