public class ItemModel
{
    /// <summary>
    /// Identificador único del ítem.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Nombre del ítem.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Descripción del ítem.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Categoría del ítem (por ejemplo, "Armadura", "Arma", "Poción").
    /// </summary>
    public ItemType ItemType { get; set; }

    /// <summary>
    /// Rareza del ítem (por ejemplo, común, raro, épico).
    /// </summary>
    public ItemRarity Rarity { get; set; }

    /// <summary>
    /// Costo del ítem.
    /// </summary>
    public int Cost { get; set; }

    /// <summary>
    /// Propiedades del ítem que pueden modificar las estadísticas del jugador.
    /// </summary>
    public ItemStats Stats { get; set; }

    /// <summary>
    /// Efectos especiales del ítem.
    /// </summary>
    public List<ItemEffect> Effects { get; set; }

    /// <summary>
    /// Ubicación en el cuerpo donde el ítem se puede equipar (por ejemplo, cabeza, manos, etc.).
    /// Solo aplica si IsEquippable es true.
    /// </summary>
    public EquipLocation EquipLocation { get; set; }
    /// <summary>
    /// Indica si el ítem es equipable (por ejemplo, es un arma o armadura).
    /// </summary>
    public bool IsEquippable { get; set; }
    /// <summary>
    /// Indica si este ítem, una vez equipado, puede ser reemplazado por otro ítem en la misma ranura.
    /// </summary>
    public bool CanBeReplaced { get; set; } = true;
    /// <summary>
    /// Si el ítem puede apilarse en el inventario.
    /// </summary>
    public bool IsStackable { get; set; } = false; // Default es false
    public ItemModel(string id, string name, string description, ItemType itemType, ItemRarity rarity, int cost, ItemStats stats, List<ItemEffect> effects)
    {
        Id = id;
        Name = name;
        Description = description;
        ItemType = itemType;
        Rarity = rarity;
        Cost = cost;
        Stats = stats;
        Effects = effects ?? new List<ItemEffect>();
    }
}

public class ItemStats
{
    /// <summary>
    /// Aumento de la salud.
    /// </summary>
    public int Health { get; set; }

    /// <summary>
    /// Aumento de maná.
    /// </summary>
    public int Mana { get; set; }

    /// <summary>
    /// Aumento de daño.
    /// </summary>
    public int Damage { get; set; }

    /// <summary>
    /// Aumento de armadura.
    /// </summary>
    public int Armor { get; set; }

    /// <summary>
    /// Aumento de velocidad.
    /// </summary>
    public int Speed { get; set; }

    /// <summary>
    /// Aumento de regeneración de maná.
    /// </summary>
    public int ManaRegen { get; set; }

    /// <summary>
    /// Aumento de regeneración de salud.
    /// </summary>
    public int HealthRegen { get; set; }

    public ItemStats(int health = 0, int mana = 0, int damage = 0, int armor = 0, int speed = 0, int manaRegen = 0, int healthRegen = 0)
    {
        Health = health;
        Mana = mana;
        Damage = damage;
        Armor = armor;
        Speed = speed;
        ManaRegen = manaRegen;
        HealthRegen = healthRegen;
    }
}
public class ItemEffect
{
    public string EffectId { get; set; }

    public string EffectName { get; set; }

    public string EffectDescription { get; set; }

    /// <summary>
    /// Tipo de efecto (daño, curación, etc.).
    /// </summary>
    public EffectType Type { get; set; }

    /// <summary>
    /// Duración del efecto (aplicable si es un efecto activo).
    /// </summary>
    public float Duration { get; set; }

    /// <summary>
    /// Magnitud del efecto (ejemplo: curación, daño, aumento de velocidad, etc.).
    /// </summary>
    public float Value { get; set; }

    /// <summary>
    /// Tipo de activación del efecto (activo o pasivo).
    /// </summary>
    public EffectActivationType ActivationType { get; set; }

    public ItemEffect(string effectId, string effectName, string effectDescription, EffectType type, float duration, float value, EffectActivationType activationType)
    {
        EffectId = effectId;
        EffectName = effectName;
        EffectDescription = effectDescription;
        Type = type;
        Duration = duration;
        Value = value;
        ActivationType = activationType;
    }
}

/// <summary>
/// Tipo de activación del efecto: activo (requiere uso) o pasivo (se aplica siempre que el ítem esté equipado).
/// </summary>
public enum EffectActivationType
{
    Active,     // Efecto que requiere ser activado por el jugador (por ejemplo, al usar un ítem)
    Passive     // Efecto que se aplica automáticamente al tener el ítem equipado
}