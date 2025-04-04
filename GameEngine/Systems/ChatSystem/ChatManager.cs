using Protos.Common;
using Protos.Game.Chat;
using VentusServer.Services;


public class ChatManager
{
    private readonly PlayerLocationService _playerLocationService;
    private readonly PlayerService _playerService;
    GameEngine _game;
    Lazy<WebSocketServerController> _websocketServerController;
    public ChatManager(PlayerService playerService, PlayerLocationService playerLocationService, GameEngine game,  Lazy<WebSocketServerController> websocketServerController)
    {
        _playerService = playerService;
        _playerLocationService = playerLocationService;
        _game = game;
        _websocketServerController = websocketServerController;
    }

    public async void HandleChatSend(Guid accountId, ChatSend chatSend)
    {
        int playerId = chatSend.PlayerId;

        PlayerModel? playerModel = await _playerService.GetPlayerByIdAsync(playerId);
        PlayerLocation? playerLocation = await _playerLocationService.GetPlayerLocationAsync(playerId);
        if (playerLocation != null && playerModel != null)
        {
            List<Entity> playersInTheWorld = _game._worldManager.GetCharactersInWorld(playerLocation.World.Id);
            foreach (var _playerEntity in playersInTheWorld)
            {
                PlayerEntity playerEntity = (PlayerEntity)_playerEntity;
                Guid accountPlayerId = playerEntity.GetAccountId();
                    OutgoingChatMessage outgoingChatMessage = new OutgoingChatMessage
                    {
                        Message = chatSend.Message,
                        PlayerId = chatSend.PlayerId,
                        PlayerName = playerModel.Name,
                        TimestampMs = chatSend.TimestampMs
                    };

                    ServerMessageChat serverMessageChat = new ServerMessageChat
                    {
                        OutgoingChatMessage = outgoingChatMessage
                    };
                    ServerMessage serverMessage = new ServerMessage
                    {
                        ServerMessageChat = serverMessageChat
                    };
                    Console.WriteLine($"Send message {accountId}");

                    _websocketServerController.Value.SendServerPacketByAccountId(accountPlayerId, serverMessage);

            }
        }

    }

}
