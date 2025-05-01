using System.Net.WebSockets;
using Ventus.Network.Packets;
using VentusServer.Services;

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
    }

    public async void HandlePlayerJoin(UserMessagePair messagePair)
    {
        // ClientMessage clientMessage = messagePair.ClientMessage;

        // PlayerJoin playerJoinMessage = clientMessage.PlayerJoin;

        // PlayerLocationModel? playerLocation = await _playerLocationService.GetPlayerLocationAsync(
        //     playerJoinMessage.PlayerId
        // );

        // PlayerModel? playerModel = await _playerService.GetOrCreatePlayerInCacheAsync(
        //     playerJoinMessage.PlayerId
        // );
        // AccountModel? accountModel = await _accountService.GetOrLoadAsync(messagePair.AccountId);

        // if (accountModel == null)
        // {
        //     Console.WriteLine("AccountModel is null");
        //     return;
        // }
        // if (playerModel == null)
        // {
        //     Console.WriteLine("PlayerModel is null");
        //     return;
        // }
        // if (playerLocation == null)
        // {
        //     Console.WriteLine("PlayerLocation is null");
        //     return;
        // }
        // // TODO: Considerar echarlo del juego
        // if (playerModel.isSpawned)
        // {
        //     Console.WriteLine("Player is already spawned");
        //     return;
        // }

        // int? currentActivePlayer = accountModel.ActivePlayerId;
        // playerModel.isSpawned = true;
        // accountModel.ActivePlayerId = playerModel.Id;

        // _taskScheduler.eventBuffer.EnqueueEvent(
        //     new GameEvent
        //     {
        //         Type = GameEventType.CustomGameEvent,
        //         Data = new
        //         {
        //             messagePair.AccountId,
        //             Type = "PlayerSpawn",
        //             PlayerModel = playerModel,
        //             PlayerLocation = playerLocation,
        //             currentActivePlayer,
        //         },
        //     }
        // );

        // await _accountService.SaveAccountAsync(accountModel);
        // accountModel.PrintInfo();
    }
}
