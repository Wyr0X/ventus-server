using System.Net.WebSockets;
using Ventus.Network.Packets;
using VentusServer.Services;
using static LoggerUtil;

public class SessionTasks
{
    TaskScheduler _taskScheduler;
    PlayerService _playerService;
    PlayerLocationService _playerLocationService;
    IAccountService _IAccountService;

    public SessionTasks(
        TaskScheduler taskScheduler,
        PlayerService playerService,
        PlayerLocationService playerLocationService,
        IAccountService IAccountService
    )
    {
        _taskScheduler = taskScheduler;
        _playerService = playerService;
        _playerLocationService = playerLocationService;
        _IAccountService = IAccountService;

        taskScheduler.Subscribe(ClientPacket.PlayerJoin, async (messagePair) => await HandlePlayerJoin(messagePair));
        taskScheduler.Subscribe(ClientPacket.PlayerExit, async (messagePair) => await HandlePlayerExit(messagePair));

    }
    private async Task<(AccountModel? Account, PlayerModel? Player)> LoadSessionContextAsync(UserMessagePair messagePair)
    {
        Log(LogTag.SessionTasks, $"Fetching Account and PlayerModel for AccountId={messagePair.AccountId}");

        PlayerJoin playerJoinMessage = (PlayerJoin)messagePair.ClientMessage;

        var playerModel = await _playerService.GetPlayerByIdAsync(
            playerJoinMessage.PlayerId,
            new PlayerModuleOptions
            {
                IncludeInventory = true,
                IncludeLocation = true,
                IncludeStats = true,
                IncludeSpells = true,
            });

        var accountModel = await _IAccountService.GetOrCreateAccountInCacheAsync(messagePair.AccountId);

        if (accountModel == null)
        {
            Log(LogTag.SessionTasks, "AccountModel is null", isError: true);
        }

        if (playerModel == null)
        {
            Log(LogTag.SessionTasks, "PlayerModel is null", isError: true);
        }

        if (playerModel?.Location == null)
        {
            Log(LogTag.SessionTasks, "PlayerLocation is null", isError: true);
        }

        return (accountModel, playerModel);
    }

    public async Task HandlePlayerJoin(UserMessagePair messagePair)
    {
        Log(LogTag.SessionTasks, $"Handling PlayerJoin for AccountId={messagePair.AccountId}");

        var (accountModel, playerModel) = await LoadSessionContextAsync(messagePair);
        if (accountModel == null || playerModel == null)
        {
            Log(LogTag.SessionTasks, "AccountModel or PlayerModel is null", isError: true);
            return;
        }

        if (playerModel.Location == null)
        {
            Log(LogTag.SessionTasks, "PlayerLocation is null", isError: true);
            return;
        }

        Log(LogTag.SessionTasks, $"Spawning player {playerModel.Id} for account {accountModel.AccountId} current player active {accountModel.ActivePlayerId}");
        PlayerModel? playerSpawnedModel = null;
        if (accountModel.ActivePlayerId != null)
        {
            Log(LogTag.SessionTasks, $"Account {accountModel.AccountId} already has an active player {accountModel.ActivePlayerId}");

            playerSpawnedModel = await _playerService.GetPlayerByIdAsync(
                accountModel.ActivePlayerId ?? 0,
                new PlayerModuleOptions
                {
                    IncludeInventory = true,
                    IncludeLocation = true,
                    IncludeStats = true,
                    IncludeSpells = true,
                });
        }

        _taskScheduler.eventBuffer.EnqueueEvent(
            new GameEvent
            {
                PacketType = GameEventType.CustomGameEvent,
                Type = CustomGameEvent.PlayerSpawn,
                Data = new SpawnPlayerData
                {
                    PlayerModel = playerModel,
                    AccountModel = accountModel,
                    PlayerSpawnedModel = playerSpawnedModel
                },
            }
        );



        Log(LogTag.SessionTasks, $"Enqueued PlayerSpawn event for player {playerModel.Id}");
        accountModel.PrintInfo();
    }

    public async Task HandlePlayerExit(UserMessagePair messagePair)
    {
        Log(LogTag.SessionTasks, $"Handling PlayerExit for AccountId={messagePair.AccountId}");
        var (accountModel, playerModel) = await LoadSessionContextAsync(messagePair);
        if (accountModel == null || playerModel == null)
        {
            Log(LogTag.SessionTasks, "AccountModel or PlayerModel is null", isError: true);
            return;
        }

        if (playerModel.Location == null)
        {
            Log(LogTag.SessionTasks, "PlayerLocation is null", isError: true);
            return;
        }

        _taskScheduler.eventBuffer.EnqueueEvent(
            new GameEvent
            {
                PacketType = GameEventType.CustomGameEvent,
                Type = CustomGameEvent.PlayerExit,
                Data = new SpawnPlayerData
                {
                    PlayerModel = playerModel,
                    AccountModel = accountModel,
                    PlayerSpawnedModel = null,
                },
            }
        );

        Log(LogTag.SessionTasks, $"Enqueued PlayerExit event for player {playerModel.Id}");
    }


}
