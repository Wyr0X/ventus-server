using System;
using System.Collections.Generic;
using Game.Models;
using Protos.Game.Client.Session;
using Ventus.Client;
using VentusServer.Services;

public class SessionManager
{
    GameEngine _game;

    PlayerLocationService _playerLocationService;
    PlayerService _playerService;
    AccountService _accountService;
    SystemChatService _systemService = new SystemChatService();
    Lazy<WebSocketServerController> _websocketServerController;

    public SessionManager(
        GameEngine game,
        PlayerLocationService playerLocationService,
        PlayerService playerService,
        AccountService accountService,
         Lazy<WebSocketServerController> websocketServerController

    )
    {
        _game = game;
        _playerLocationService = playerLocationService;
        _playerService = playerService;
        _accountService = accountService;
        _websocketServerController = websocketServerController;



    }

    public async void HandlePlayerJoin(UserMessagePair messagePair)
    {
        ClientMessage clientMessage = messagePair.ClientMessage;


        PlayerJoin playerJoinMessage = clientMessage.PlayerJoin;

        PlayerLocationModel? playerLocation = await _playerLocationService.GetPlayerLocationAsync(
            playerJoinMessage.PlayerId
        );
        Console.WriteLine("Entra acaaaaaaaaaaaa");

        PlayerModel? playerModel = await _playerService.GetOrCreatePlayerInCacheAsync(
            playerJoinMessage.PlayerId
        );
        AccountModel? accountModel = await _accountService.GetOrLoadAsync(messagePair.AccountId);

        if (accountModel != null)
        {
            int? currentActivePlayer = accountModel.ActivePlayerId;
            if (currentActivePlayer != null && playerModel != null && playerLocation != null)
            {
                _game.UnSpawnPlayer(messagePair.AccountId, playerModel, playerLocation);
                playerModel.isSpawned = false;

            }

            if (playerLocation != null && playerModel != null)
            {
                playerModel.PrintInfo();

                if (playerModel.isSpawned)
                {
                    _game.UnSpawnPlayer(messagePair.AccountId, playerModel, playerLocation);
                    playerModel.isSpawned = false;
                }

                await _playerService.SavePlayerAsync(playerModel);

            }
            if (playerModel != null && !playerModel.isSpawned && playerLocation != null)
            {
                playerModel.isSpawned = true;
                accountModel.ActivePlayerId = playerModel.Id;

                _game.SpawnPlayer(messagePair.AccountId, playerModel, playerLocation);
                List<Entity> _playersInTheWorld = _game._worldManager.GetCharactersInWorld(playerLocation.World.Id);

                List<Guid> accountsIdInTheWorld = new List<Guid>();
                foreach (var entityC in _playersInTheWorld)
                {

                    Character? character = (Character?)entityC.Get(typeof(Character));

                    if (character == null) continue;

                    accountsIdInTheWorld.Add(character.AccountId);
                }

                _systemService.BroadcastInfo(accountsIdInTheWorld, $"El jugador {playerModel.Name} ha entrado al Mundo", _websocketServerController.Value.SendServerPacketByAccountId);
            }
            await _accountService.SaveAccountAsync(accountModel);
            accountModel.PrintInfo();



        }

        //Aca te va a llegar data , vas a buscar la info necesaria y lo vas a meter en el eventsbuffer


        // Existirá un AccountService(userId, playerId) para saber si existe en la cuenta
        // Validaciones -> ban, si está logueado, si el pj es de la cuenta
        // PlayerBasicModel playerBasic = playerService.GetPlayerBasicById(playerJoinMessage.PlayerId);
        // PlayerLocationModel playerLocation = playerService.getPlayerLocationId(playerJoinMessage.PlayerId);
        // var playerInfo = new { playerBasic, playerLocation };
        // _game.worldManager.SpawnPlayer(playerInfo);
    }
}
