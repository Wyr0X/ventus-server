using System.Net.WebSockets;

public interface ISessionManager
{
    void RegisterSession(Guid accountId, string token, WebSocket socket);
    bool IsTokenActive(Guid accountId, string token);
    void InvalidateSession(Guid accountId);
    WebSocket? GetSocket(Guid accountId);
}
