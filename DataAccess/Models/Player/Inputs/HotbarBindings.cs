using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

public class HotbarAction
{
    public required int Slot { get; set; } // Índice de la hotbar (0-10)

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required HotbarActionType ActionType { get; set; } // Tipo de acción

    public required object ActionId { get; set; } // ID del hechizo o ítem


    public static HotbarAction? GetBySlot(List<HotbarAction> actions, int slot)
    {
        return actions.FirstOrDefault(a => a.Slot == slot);
    }
}

public enum HotbarActionType
{
    Empty,      // Sin acción
    UseItem,    // Usar un ítem
    CastSpell,  // Lanzar hechizo
    Hit,        // Golpe físico
    Equip       // Equipar ítem
}

public class HotbarBindings
{
    private readonly List<HotbarAction> _actions;

    private HotbarBindings(List<HotbarAction> actions)
    {
        _actions = actions;
    }

    public static HotbarBindings CreateDefault()
    {
        var emptyHotbar = Enumerable.Range(0, 11)
            .Select(slot => new HotbarAction
            {
                Slot = slot,
                ActionType = HotbarActionType.Empty,
                ActionId = 0
            })
            .ToList();

        return new HotbarBindings(emptyHotbar);
    }

    public HotbarAction? GetBySlot(int slot)
    {
        return _actions.FirstOrDefault(a => a.Slot == slot);
    }

    public List<HotbarAction> GetAll()
    {
        return _actions;
    }

    public void SetAction(int slot, HotbarActionType type, int actionId)
    {
        var action = GetBySlot(slot);
        if (action != null)
        {
            action.ActionType = type;
            action.ActionId = actionId;
        }
    }

    public void ClearSlot(int slot)
    {
        var action = GetBySlot(slot);
        if (action != null)
        {
            action.ActionType = HotbarActionType.Empty;
            action.ActionId = 0;
        }
    }
}
