using System.Net.WebSockets;
using Ventus.Network.Packets;
using VentusServer.Services;
using static LoggerUtil;

public class SessionTasks
{
    TaskScheduler _taskScheduler;
    PlayerService _playerService;
    PlayerLocationService _playerLocationService;
    AccountService _accountService;

    public SessionTasks(
        TaskScheduler taskScheduler,
        PlayerService playerService,
        PlayerLocationService playerLocationService,
        AccountService accountService
    )
    {
        _taskScheduler = taskScheduler;
        _playerService = playerService;
        _playerLocationService = playerLocationService;
        _accountService = accountService;

        taskScheduler.Subscribe(ClientPacket.PlayerJoin, this.HandlePlayerJoin);
        Log(LogTag.SessionTasks, "Subscribed to PlayerJoin message");
    }

    public async void HandlePlayerJoin(UserMessagePair messagePair)
    {
        Log(LogTag.SessionTasks, $"Handling PlayerJoin for AccountId={messagePair.AccountId}");

        PlayerJoin playerJoinMessage = (PlayerJoin)messagePair.ClientMessage;
        Log(LogTag.SessionTasks, $"Received PlayerJoin for PlayerId={playerJoinMessage.PlayerId}");

        PlayerModel? playerModel = await _playerService.GetPlayerByIdAsync(
            playerJoinMessage.PlayerId,
            new PlayerModuleOptions
            {
                IncludeInventory = true,
                IncludeLocation = true,
                IncludeStats = true,
                IncludeSpells = true,
            }
        );
        AccountModel? accountModel = await _accountService.GetOrLoadAsync(messagePair.AccountId);

        if (accountModel == null)
        {
            Log(LogTag.SessionTasks, "AccountModel is null", isError: true);
            return;
        }
        if (playerModel == null)
        {
            Log(LogTag.SessionTasks, "PlayerModel is null", isError: true);
            return;
        }
        if (playerModel.Location == null)
        {
            Log(LogTag.SessionTasks, "PlayerLocation is null", isError: true);
            return;
        }

        if (playerModel.isSpawned)
        {
            Log(LogTag.SessionTasks, $"Player {playerModel.Id} is already spawned", isError: true);
            return;
        }

        Log(LogTag.SessionTasks, $"Spawning player {playerModel.Id} for account {accountModel.AccountId}");


        _taskScheduler.eventBuffer.EnqueueEvent(
            new GameEvent
            {
                Type = GameEventType.CustomGameEvent,
                Data = new
                {
                    AccountModel = accountModel,
                    Type = CustomGameEvent.PlayerSpawn,
                    PlayerModel = playerModel,
                },
            }
        );

        Log(LogTag.SessionTasks, $"Enqueued PlayerSpawn event for player {playerModel.Id}");


        accountModel.PrintInfo();
    }
}
