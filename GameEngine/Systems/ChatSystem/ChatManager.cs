using Protos.Common;
using Protos.Game.Chat;
using VentusServer.Services;


public class ChatManager
{
    private readonly PlayerLocationService _playerLocationService;
    private readonly PlayerService _playerService;
    GameEngine _game;
    Lazy<WebSocketServerController> _websocketServerController;

    private readonly GlobalChatService _globalChatService;
    private readonly ModerationService _moderationService;
    public ChatManager(PlayerService playerService, PlayerLocationService playerLocationService, GameEngine game, Lazy<WebSocketServerController>
    websocketServerController, GlobalChatService globalChatService, ModerationService moderationService)
    {
        _playerService = playerService;
        _playerLocationService = playerLocationService;
        _game = game;
        _websocketServerController = websocketServerController;
        _globalChatService = globalChatService;
        _moderationService = moderationService;
    }
    public void HandleChatSend(Guid senderId, ChatSend chatSend)
    {
        if (_moderationService.IsMessageBlocked(chatSend.Message))
        {
            Console.WriteLine("❌ Mensaje bloqueado por moderación.");
            return;
        }
Console.WriteLine(chatSend.ToString());


        switch (chatSend.Channel)
        {
            case ChatChannel.General:
                HandleGlobalChat(senderId, chatSend);
                break;
                // case ChatChannel.PRIVATE:
                //     _privateChatService.SendMessage(senderId, message);
                //     break;
                // case ChatChannel.PARTY:
                //     _partyChatService.SendMessage(senderId, message);
                //     break;
                // case ChatChannel.GUILD:
                //     _guildChatService.SendMessage(senderId, message);
                //     break;
                // default:
                //     Console.WriteLine("❌ Canal de chat desconocido.");
                //     break;
        }
    }

    public async void HandleGlobalChat(Guid accountId, ChatSend chatSend)
    {
        int playerId = chatSend.PlayerId;

        PlayerModel? playerModel = await _playerService.GetPlayerByIdAsync(playerId);
        PlayerLocation? playerLocation = await _playerLocationService.GetPlayerLocationAsync(playerId);
        if (playerLocation != null && playerModel != null)
        {
            List<Entity> playersInTheWorld = _game._worldManager.GetCharactersInWorld(playerLocation.World.Id);
            Guid[] accountsIdToBroadcast = [];
            foreach (var _playerEntity in playersInTheWorld)
            {
                PlayerEntity playerEntity = (PlayerEntity)_playerEntity;
                accountsIdToBroadcast.Append(playerEntity.GetAccountId());

            }
            _globalChatService.SendGlobalMessage(accountsIdToBroadcast, chatSend, playerModel.Name, _websocketServerController.Value.SendServerPacketByAccountId);

        }

    }

}
