using System;
using System.Collections.Generic;
using Game.Models;
using Protos.Game.Common;
using Protos.Game.Movement;
using Protos.Game.Session;
using ProtosCommon;
using VentusServer.Services;

public class SessionManager
{
    GameEngine _game;

    PlayerLocationService _playerLocationService;
    PlayerService _playerService;

    public SessionManager(
        GameEngine game,
        PlayerLocationService playerLocationService,
        PlayerService playerService
    )
    {
        _game = game;
        _playerLocationService = playerLocationService;
        _playerService = playerService;
    }

    public async void HandlePlayerJoin(UserMessagePair messagePair)
    {
        ClientMessageGame? clientMessageGame = messagePair.GetClientMessageGame();

        if (clientMessageGame == null)
            return;

        ClientMessageGameSession sessionMessage = clientMessageGame.MessageSession;
        PlayerJoin playerJoinMessage = sessionMessage.PlayerJoin;

        PlayerLocation? playerLocation = await _playerLocationService.GetPlayerLocationAsync(
            playerJoinMessage.PlayerId
        );

        PlayerModel? playerModel = await _playerService.GetPlayerByIdAsync(
            playerJoinMessage.PlayerId
        );

        if (playerLocation != null && playerModel != null)
        {
            Console.WriteLine($"Player {playerModel.Id} joined the game");
            _game.SpawnPlayer(messagePair.UserId, playerModel, playerLocation);
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
