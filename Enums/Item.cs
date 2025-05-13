
public enum ItemType
{
    Weapon,      // Arma
    Armor,       // Armadura
    Potion,      // Poción
    Accessory,   // Accesorio
    Material,     // Material para crafting,
    Consumable
}

public enum ItemRarity
{
    Common,      // Común
    Uncommon,    // Poco común
    Rare,        // Raro
    Epic,        // Épico
    Legendary    // Legendario
}
public enum EffectType
{
    Heal,
    Damage,
    SpeedBoost,
    ArmorBoost,
    ManaBoost,
    ManaRegen,
    HealthRegen,
    Stun,          // Efecto que puede aturdir al jugador
    Freeze,        // Efecto que congela a un jugador
    Burn,          // Efecto que causa daño por quemaduras a lo largo del tiempo
    Poison,        // Efecto de veneno que inflige daño por envenenamiento
    Buff,          // Buffs de estadísticas
    Debuff         // Debuffs de estadísticas
}
public enum EquipLocation
{
    Head,        // Casco, gafas, etc.
    Chest,       // Armaduras de torso
    Legs,        // Pantalones, grebas
    Hands,       // Guantes, armas
    Feet,        // Botas
    Accessory,   // Anillos, collares
    Weapon,      // Armas (espadas, hachas, etc.)
    Shield,      // Escudos
    None         // Ítems que no se equipan en el cuerpo (como pociones)
}
