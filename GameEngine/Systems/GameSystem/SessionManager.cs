using System;
using Protos.Game.Movement;
using Protos.Game.Common;
using System.Collections.Generic;
using ProtosCommon;
using Protos.Game.Session;

public class SessionManager
{
    SessionLogic _sessionLogic;
    public SessionManager(SessionLogic sessionLogic)
    {
        _sessionLogic = sessionLogic;
    }

    public void HandlePlayerJoin(UserMessagePair messagePair)
    {
        ClientMessage clientMessage = messagePair.ClientMessage;
        ClientMessageGameSession sessionMessage = clientMessage.MessageOauth.ClientMessageGame.MessageSession;
        PlayerJoin playerJoinMessage = sessionMessage.PlayerJoin;

        string playerId = playerJoinMessage.PlayerId;
        // Recuperar Jugador del modelo - COMPLETAR
        string Player = _sessionLogic.GetPlayerById(playerId);
        // Corroborar que no este baneado,
        // Corroborar que no este logeado y resolver

       // Recuperar PlayerWorldPosition del jugador - Completar
        string PlayerWorldPosition = _sessionLogic.GetPlayerWorldPositionById(playerId);

 
        // Recuperar World - Completar
        string World = _sessionLogic.GetPlayerWorldPositionById(playerId);
        //Chequear que no este en el maximo de jugadores para el world
        //Chequear que el world permita entrar al usuario por nivel o permisos

        // Recuperar Map
        //Chequear que no este en el maximo de jugadores para el Map
        string Map = _sessionLogic.GetMapById(playerId);


    }

}
