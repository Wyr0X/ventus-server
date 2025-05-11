using System.Collections.Generic;
using System.Linq;

public enum KeyType { Hotbar, Attack, Move, QuickAction }
public class UserKeyAction
{
    public required string Action { get; set; }      // Ej: "hotbar_slot_0"

    public required KeyType KeyType { get; set; }
    public required string Key { get; set; }  // Ej: "1"
}

public class UserKeyBindings
{
    private readonly List<UserKeyAction> _bindings;

    private UserKeyBindings(List<UserKeyAction> bindings)
    {
        _bindings = bindings;
    }

    public static UserKeyBindings CreateDefault()
    {
        var defaultBindings = new List<UserKeyAction>
        {
            new() { Action = "hotbar_slot_0", Key = "1" , KeyType = KeyType.Hotbar },
            new() { Action = "hotbar_slot_1", Key = "2" , KeyType = KeyType.Hotbar  },
            new() { Action = "hotbar_slot_2", Key = "3" , KeyType = KeyType.Hotbar  },
            new() { Action = "hotbar_slot_3", Key = "4" , KeyType = KeyType.Hotbar  },
            new() { Action = "hotbar_slot_4", Key = "5" , KeyType = KeyType.Hotbar  },
            new() { Action = "hotbar_slot_5", Key = "6" , KeyType = KeyType.Hotbar  },
            new() { Action = "hotbar_slot_6", Key = "7" , KeyType = KeyType.Hotbar  },
            new() { Action = "hotbar_slot_7", Key = "8" , KeyType = KeyType.Hotbar  },
            new() { Action = "hotbar_slot_8", Key = "9" , KeyType = KeyType.Hotbar  },
            new() { Action = "hotbar_slot_9", Key = "q" , KeyType = KeyType.QuickAction  },
            new() { Action = "hotbar_slot_10", Key = "e" , KeyType = KeyType.QuickAction},
            new() { Action = "basic_attack", Key = "ctrl", KeyType = KeyType.Attack }
        };

        return new UserKeyBindings(defaultBindings);
    }

    public UserKeyAction? GetByAction(string action)
    {
        return _bindings.FirstOrDefault(b => b.Action == action);
    }

    public UserKeyAction? GetByKey(string key)
    {
        return _bindings.FirstOrDefault(b => b.Key == key);
    }

    public List<UserKeyAction> GetAll()
    {
        return _bindings;
    }
}
