using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;

public class WebSocketConnectionManager
{
    private readonly ConcurrentDictionary<string, WebSocket> _pendingConnections = new();
    private readonly ConcurrentDictionary<Guid, WebSocket> _websocketsByAccountId = new();
    private readonly ConcurrentDictionary<string, Guid> _accountIdsByConnectionId = new();
    private readonly ConcurrentDictionary<Guid, string> _connectionIdsByAccountId = new();

    public void AddPendingConnection(string connectionId, WebSocket socket)
    {
        _pendingConnections[connectionId] = socket;
        LoggerUtil.Log(LoggerUtil.LogTag.WebSocketConnectionManager, $"Pending connection added: {connectionId}");
    }

    public bool TryAuthenticateConnection(string connectionId, Guid accountId, string currentToken, out WebSocket socket)
    {
        socket = null!;
        if (_pendingConnections.TryRemove(connectionId, out var pending))
        {
            _accountIdsByConnectionId[connectionId] = accountId;
            _connectionIdsByAccountId[accountId] = connectionId;
            _websocketsByAccountId[accountId] = pending;

            socket = pending;

            LoggerUtil.Log(LoggerUtil.LogTag.WebSocketConnectionManager, $"Connection {connectionId} authenticated for Account {accountId}");
            return true;
        }

        LoggerUtil.Log(LoggerUtil.LogTag.WebSocketConnectionManager, $"Authentication failed for Connection {connectionId}");
        return false;
    }

    public bool TryGetAccountId(string connectionId, out Guid accountId)
    {
        var result = _accountIdsByConnectionId.TryGetValue(connectionId, out accountId);
        LoggerUtil.Log(LoggerUtil.LogTag.WebSocketConnectionManager, result
            ? $"Retrieved AccountId {accountId} for ConnectionId {connectionId}"
            : $"Failed to retrieve AccountId for ConnectionId {connectionId}");
        return result;
    }

    public bool TryGetSocket(Guid accountId, out WebSocket socket)
    {
        var result = _websocketsByAccountId.TryGetValue(accountId, out socket);
       
        return result;
    }

    public void RemoveConnection(string connectionId, Guid accountId)
    {
        _accountIdsByConnectionId.TryRemove(connectionId, out _);
        _websocketsByAccountId.TryRemove(accountId, out _);
        _pendingConnections.TryRemove(connectionId, out _);
        _connectionIdsByAccountId.TryRemove(accountId, out _);


        LoggerUtil.Log(LoggerUtil.LogTag.WebSocketConnectionManager, $"Removed connection {connectionId} and Account {accountId}");
    }
    public async Task RemoveConnectionByAccountId(Guid accountId)
    {
        var result = _connectionIdsByAccountId.TryGetValue(accountId, out var connectionId);
        if (connectionId != null){
            RemoveConnection(connectionId, accountId);
        }
    }
   
}
