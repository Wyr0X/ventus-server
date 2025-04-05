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
    private readonly PrivateChatService _privateChatService;
    private readonly ModerationService _moderationService;
    public ChatManager(PlayerService playerService, PlayerLocationService playerLocationService, GameEngine game, Lazy<WebSocketServerController>
    websocketServerController, ModerationService moderationService)
    {
        _playerService = playerService;
        _playerLocationService = playerLocationService;
        _game = game;
        _websocketServerController = websocketServerController;
        _globalChatService = new GlobalChatService();
        _privateChatService = new PrivateChatService();
        _moderationService = moderationService;
    }
    public void HandleChatSend(Guid senderId, ChatSend chatSend)
    {
        if (_moderationService.IsMessageBlocked(chatSend.Message))
        {
            Console.WriteLine("❌ Mensaje bloqueado por moderación.");
            return;
        }
        Console.WriteLine(chatSend.Channel);


        switch (chatSend.Channel)
        {
            case "GLOBAL":
                HandleGlobalChat(senderId, chatSend);
                break;
            case "PRIVATE":
                HandlePrivateChatService(senderId, chatSend);
                break;
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

    public async void HandlePrivateChatService(Guid accountId, ChatSend chatSend)
    {
        int playerId = chatSend.PlayerId;

        PlayerModel? playerModel = await _playerService.GetPlayerByIdAsync(playerId);
        string? nickNameToSend = chatSend.NickNameToSend;
        if (nickNameToSend != null)
        {
            PlayerLocation? playerLocation = await _playerLocationService.GetPlayerLocationAsync(playerId);
            PlayerModel? playerToSend = await _playerService.GetPlayerByName(nickNameToSend);
            if (playerLocation != null && playerModel != null && playerToSend != null && playerToSend.isSpawned)
            {

                _privateChatService.SendPrivateMessage(playerToSend.AccountId, chatSend, playerModel.Name, _websocketServerController.Value.SendServerPacketByAccountId);

            }

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
            List<Guid> accountsIdToBroadcast = [];
            Console.WriteLine($"playersInTheWorld {playersInTheWorld}");

            foreach (var _playerEntity in playersInTheWorld)
            {
                PlayerEntity playerEntity = (PlayerEntity)_playerEntity;
                Console.WriteLine($"playerEntity.GetAccountId() {playerEntity.GetAccountId()}");

                accountsIdToBroadcast.Add(playerEntity.GetAccountId());

            }
            Console.WriteLine($"accountsIdToBroadcast {accountsIdToBroadcast} {accountsIdToBroadcast.Count()}");
            _globalChatService.SendGlobalMessage(accountsIdToBroadcast, chatSend, playerModel.Name, _websocketServerController.Value.SendServerPacketByAccountId);

        }

    }

}
