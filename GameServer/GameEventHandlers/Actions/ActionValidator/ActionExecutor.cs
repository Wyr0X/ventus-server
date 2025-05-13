using System;
using System.Collections.Generic;
using System.Linq;
using VentusServer.Domain.Objects;

public class ActionExecutor
{

    public ActionExecutor()
    {
    }

    // Ejecuta la acción según el tipo de la acción
    public void TryToExecuteAction(HotbarAction action, PlayerObject player)
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
