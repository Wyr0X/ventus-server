using System;
using System.Collections.Generic;
using Game.Models;
using Protos.Common;
using Protos.Game.Movement;
using Protos.Game.Session;
using VentusServer.Services;

public class SessionManager
{
    GameEngine _game;

    PlayerLocationService _playerLocationService;
    PlayerService _playerService;
    AccountService _accountService;

    public SessionManager(
        GameEngine game,
        PlayerLocationService playerLocationService,
        PlayerService playerService,
        AccountService accountService

    )
    {
        _game = game;
        _playerLocationService = playerLocationService;
        _playerService = playerService;
        _accountService = accountService;
    }

    public async void HandlePlayerJoin(UserMessagePair messagePair)
    {
        ClientMessage? clientMessage = (ClientMessage?)messagePair.ClientMessage;

        ClientMessageGameSession? sessionMessage = clientMessage.ClientMessageSession;

        if (sessionMessage == null)
            return;

        PlayerJoin playerJoinMessage = sessionMessage.PlayerJoin;

        PlayerLocation? playerLocation = await _playerLocationService.GetPlayerLocationAsync(
            playerJoinMessage.PlayerId
        );

        PlayerModel? playerModel = await _playerService.GetOrCreatePlayerInCacheAsync(
            playerJoinMessage.PlayerId
        );
        AccountModel? accountModel = await _accountService.GetAccountByIdAsync(messagePair.AccountId);

        if (accountModel != null)
        {
            int? currentActivePlayer = accountModel.ActivePlayer;
            if (currentActivePlayer != null)
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
            if (!playerModel.isSpawned)
            {
                playerModel.isSpawned = true;
                accountModel.ActivePlayer = playerModel.Id;

                Console.WriteLine($"Player {playerModel.Id} joined the game");
                _game.SpawnPlayer(messagePair.AccountId, playerModel, playerLocation);
            }
            await _accountService.SaveAccountAsync(accountModel);
            accountModel.PrintInfo();



        }

        //Aca te va a llegar data , vas a buscar la info necesaria y lo vas a meter en el eventsbuffer


        // Existirá un AccountService(userId, playerId) para saber si existe en la cuenta
        // Validaciones -> ban, si está logueado, si el pj es de la cuenta
        // PlayerBasicModel playerBasic = playerService.GetPlayerBasicById(playerJoinMessage.PlayerId);
        // PlayerLocation playerLocation = playerService.getPlayerLocationId(playerJoinMessage.PlayerId);
        // var playerInfo = new { playerBasic, playerLocation };
        // _game.worldManager.SpawnPlayer(playerInfo);
    }
}
