using System;
using System.Collections.Generic;
using Game.Models;
using Protos.Game.Common;
using Protos.Game.Movement;
using Protos.Game.Session;
using ProtosCommon;

public class SessionManager
{
    GameEngine _game;
    SessionLogic _sessionLogic;

    public SessionManager(GameEngine game, SessionLogic sessionLogic)
    {
        _game = game;
        _sessionLogic = sessionLogic;
    }

    public void HandlePlayerJoin(UserMessagePair messagePair)
    {
        ClientMessage clientMessage = messagePair.ClientMessage;
        ClientMessageGameSession sessionMessage = clientMessage
            .MessageOauth
            .ClientMessageGame
            .MessageSession;
        PlayerJoin playerJoinMessage = sessionMessage.PlayerJoin;

        // Existirá un AccountService(userId, playerId) para saber si existe en la cuenta
        // Validaciones -> ban, si está logueado, si el pj es de la cuenta
        // PlayerBasicModel playerBasic = playerService.GetPlayerBasicById(playerJoinMessage.PlayerId);
        // PlayerLocation playerLocation = playerService.getPlayerLocationId(playerJoinMessage.PlayerId);
        // var playerInfo = new { playerBasic, playerLocation };
        // _game.worldManager.SpawnPlayer(playerInfo);
    }
}
