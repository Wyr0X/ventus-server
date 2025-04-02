using System;
using System.Collections.Generic;
using Game.Models;
using Google.Protobuf.Collections;
using Protos.Game.Common;
using Protos.Game.Movement;
using Protos.Game.Session;
using ProtosCommon;
using VentusServer.Services;
using static GameUtils;

public class MovementManager
{
    GameEngine _game;

    PlayerLocationService _playerLocationService;
    PlayerService _playerService;


    public MovementManager(GameEngine game,
    PlayerLocationService playerLocationService, PlayerService playerService)
    {
     

    }

    public async void HandlePlayerInput(string userId, PlayerInput playerInput)
    {
        RepeatedField<uint> protoKeys = playerInput.Keys; // Ejemplo de entrada

        List<KeyEnum> keys = GameUtils.ParseKeys(protoKeys);
       
       int playerId = 0;
       InputsKeyEvent InputsKeyEvent = new InputsKeyEvent(userId, playerId, keys);
       _game.EnqueuEvent(InputsKeyEvent);
    }

}
