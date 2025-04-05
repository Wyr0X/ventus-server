using System.Net.WebSockets;

public class InMemorySessionManager : ISessionManager
{
    private readonly Dictionary<Guid, (string Token, WebSocket Socket)> _sessions = new();

    public void RegisterSession(Guid accountId, string token, WebSocket socket)
    {
        // Cerrar sesión previa si existe
        if (_sessions.TryGetValue(accountId, out var oldSession))
        {
            try
            {
                oldSession.Socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Nueva sesión iniciada", CancellationToken.None);
            }
            catch {}
        }

        _sessions[accountId] = (token, socket);
    }

    public bool IsTokenActive(Guid accountId, string token)
    {
        return _sessions.TryGetValue(accountId, out var session) && session.Token == token;
    }

    public void InvalidateSession(Guid accountId)
    {
        if (_sessions.TryGetValue(accountId, out var session))
        {
            try
            {
                session.Socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Sesión invalidada", CancellationToken.None);
            }
            catch {}
            _sessions.Remove(accountId);
        }
    }

    public WebSocket? GetSocket(Guid accountId)
    {
        return _sessions.TryGetValue(accountId, out var session) ? session.Socket : null;
    }
}
