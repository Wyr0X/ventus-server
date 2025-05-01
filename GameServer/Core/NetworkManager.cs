public class NetworkManager
{
    private WebSocketServerController webSocketServer;

    public NetworkManager(WebSocketServerController server)
    {
        this.webSocketServer = server;
    }

    public void Broadcast(string userId)
    {
       // string jsonData = JsonConvert.SerializeObject(syncData);
    }
}
