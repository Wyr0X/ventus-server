using Microsoft.Extensions.DependencyInjection;

public static class GameStartup
{
    public static void StartGameComponents(IServiceProvider provider)
    {
        var webSocketServerController = provider.GetRequiredService<WebSocketServerController>();
        var gameEngine = provider.GetRequiredService<GameEngine>();

        Task.Run(() => webSocketServerController.StartServerAsync());
        Task.Run(() => webSocketServerController.StartLoop());
        Task.Run(() => gameEngine.Run());

        LoggerUtil.Log("SERVER", "Servidores iniciados correctamente.", ConsoleColor.Yellow);
    }
}
