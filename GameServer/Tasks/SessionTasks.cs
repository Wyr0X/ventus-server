using System.Net.WebSockets;
using Game.Server;
using Ventus.Network.Packets;
using VentusServer.Services;
using static LoggerUtil;

public class SessionTasks
{
    TaskScheduler _taskScheduler;
    PlayerService _playerService;
    PlayerLocationService _playerLocationService;
    IAccountService _IAccountService;
    GameServer _gameServer;

    public SessionTasks(
        TaskScheduler taskScheduler,
        GameServer gameServer,
        PlayerService playerService,
        PlayerLocationService playerLocationService,
        IAccountService IAccountService
    )
    {
        _taskScheduler = taskScheduler;
        _playerService = playerService;
        _playerLocationService = playerLocationService;
        _IAccountService = IAccountService;
        _gameServer = gameServer;

        taskScheduler.Subscribe(ClientPacket.PlayerJoin, async (messagePair) => await HandlePlayerJoin(messagePair));
        taskScheduler.Subscribe(ClientPacket.PlayerExit, async (messagePair) => await HandlePlayerExit(messagePair));
        taskScheduler.Subscribe(ClientPacket.ClientAlive, async (messagePair) => await HandleClienAlive(messagePair));
        taskScheduler.Subscribe(ClientPacket.ClientLoaded, async (messagePair) => await HandleClientLoaded(messagePair));

    }

    private async Task<(AccountModel? Account, PlayerModel? Player)> LoadSessionContextAsync(UserMessagePair messagePair)
    {
        try
        {
            Log(LogTag.SessionTasks, $"Fetching Account and PlayerModel for AccountId={messagePair.AccountId}");

            var playerJoinMessage = (PlayerJoin)messagePair.ClientMessage;

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
        catch (Exception ex)
        {
            Log(LogTag.SessionTasks, $"❌ Error loading session context for {messagePair.AccountId}: {ex.Message}", isError: true);
            // Opcionalmente, si quieres ver la pila:
            Log(LogTag.SessionTasks, ex.StackTrace ?? string.Empty, isError: true);
            return (null, null);
        }
    }

    private Task HandleClientLoaded(UserMessagePair userMessagePair)
    {
        if (!_gameServer.playersByAccountId.TryGetValue(userMessagePair.AccountId, out var playerObject))
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler, $"[HandleUnspawnPlayer] Player not found for account ID {userMessagePair.AccountId}");
            return Task.CompletedTask;
        }
        if (playerObject == null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler, $"[HandleUnspawnPlayer] Player not found for ID {userMessagePair.AccountId}");
            return Task.CompletedTask;
        }

        playerObject.isReady = true;
        var loc = playerObject.PlayerModel.Location;
        if (loc == null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler, "[HandleSpawnPlayer] PlayerLocation is null");
            return Task.CompletedTask;
        }

        var _playersInTheWorld = _gameServer.worldManager.GetPlayersInArea(loc.WorldId, loc.MapId, loc.PosX, loc.PosY, 1000);
        foreach (var player in _playersInTheWorld)
        {
            if (player.Id != playerObject.Id)
            {
                PlayerSpawn playerSpawn = new()
                {
                    PlayerId = playerObject.Id,
                    X = loc.PosX,
                    Y = loc.PosY,
                    Name = playerObject.Name
                };
                _gameServer._webSocketServerController._outgoingQueue.Enqueue(player.AccountId, playerSpawn, ServerPacket.PlayerSpawn);
            }
        }
        return Task.CompletedTask;
    }
    private Task HandleClienAlive(UserMessagePair userMessagePair)
    {
        if (!_gameServer.playersByAccountId.TryGetValue(userMessagePair.AccountId, out var playerObject))
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler, $"[HandleUnspawnPlayer] Player not found for account ID {userMessagePair.AccountId}");
            return Task.CompletedTask;
        }
        if (playerObject == null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SessionHandler, $"[HandleUnspawnPlayer] Player not found for ID {userMessagePair.AccountId}");
            return Task.CompletedTask;
        }
        playerObject.IsActiviyConfirmed = true;
        return Task.CompletedTask;

    }
    private async Task HandlePlayerJoin(UserMessagePair messagePair)
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
