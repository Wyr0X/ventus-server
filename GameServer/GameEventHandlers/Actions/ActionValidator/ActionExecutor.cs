using System;
using System.Collections.Generic;
using System.Linq;
using Game.Server;
using Ventus.Network.Packets;
using VentusServer.Domain.Objects;

public class ActionExecutor
{
    GameServer _gameServer;
    public ActionExecutor(GameServer gameServer)
    {
        GameServer _gameServer = gameServer;
    }

    // Ejecuta la acción según el tipo de la acción
    public Task TryToExecuteAction(HotbarAction action, PlayerObject player, int sequenceNumber, string Key)
    {
        var status = false;
        switch (action.ActionType)
        {
            case HotbarActionType.Empty:
                // Si no hay acción, no hacer nada
                break;

            case HotbarActionType.UseItem:
                // ExecuteUseItem(action, player);
                break;

            case HotbarActionType.CastSpell:
                status = ExecuteCastSpell(action, player);
                break;

            case HotbarActionType.Hit:
                // ExecuteHit(action, player);
                break;

            case HotbarActionType.Equip:
                // ExecuteEquip(action, player);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
        KeyPressAck keyPressAck = new KeyPressAck { Key = Key, SequenceNumber = sequenceNumber, Result = status };
        _gameServer._webSocketServerController._outgoingQueue.Enqueue(player.PlayerModel.AccountId, keyPressAck, ServerPacket.KeyPressAck);
        return Task.CompletedTask;
    }

    // Ejecutar acción de usar un ítem
    private bool ExecuteUseItem(HotbarAction action, PlayerObject player)
    {
        return true;
    }

    // Ejecutar acción de lanzar un hechizo
    private bool ExecuteCastSpell(HotbarAction action, PlayerObject player)
    {
        return player.TryToCastSpell((string)action.ActionId);


    }

    // Ejecutar acción de golpear
    private bool ExecuteHit(HotbarAction action, PlayerObject player)
    {
        // Validar la acción de golpe físico
        Console.WriteLine($"El jugador {player.Name} está realizando un golpe físico.");

        // Aquí se puede agregar la lógica para realizar el golpe físico
        return true;
        // Ejemplo: player.PerformHit();
    }

    // Ejecutar acción de equipar ítem
    private bool ExecuteEquip(HotbarAction action, PlayerObject player)
    {
        return true;

    }
}
