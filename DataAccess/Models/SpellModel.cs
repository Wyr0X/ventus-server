public class SpellModel
{
    // --- Identificación y requisitos ---
    public string Id { get; }
    public string Name { get; }
    public CharacterClass RequiredClass { get; } = CharacterClass.None;
    public int RequiredLevel { get; }

    // --- Costos y tiempos ---
    public int ManaCost { get; }
    public int CastTime { get; }
    public int Cooldown { get; }
    public bool IsChanneled { get; }
    public int Duration { get; }
    public SpellCastType CastType { get; }

    // --- Alcance y targeting ---
    public int Range { get; }
    public TargetType TargetType { get; }
    public bool RequiresLineOfSight { get; }
    public ITargetingStrategy? Targeting { get; }

    // --- Efectos del hechizo ---
    public IReadOnlyList<ISpellEffect> UnitEffects { get; }
    public IReadOnlyList<ITerrainEffect> TerrainEffects { get; }
    public IReadOnlyList<ISummonEffect> SummonEffects { get; }

    // --- Información visual y auditiva ---
    public string? Description { get; }
    public string? VfxCast { get; }
    public string? VfxImpact { get; }
    public string? CastSound { get; }
    public string? ImpactSound { get; }

    // --- Otros datos ---
    public int Price { get; }

    // --- Nuevos campos agregados ---
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }
    public string? CastMode { get; } // Agregado para mapear el campo `cast_mode` en la base de datos

    // --- Constructor ---
    public SpellModel(
        string id,
        string name,
        int manaCost,
        int castTime,
        int cooldown,
        int range,
        bool isChanneled,
        int duration,
        SpellCastType castType,
        ITargetingStrategy? targeting,
        IEnumerable<ISpellEffect>? unitEffects = null,
        IEnumerable<ITerrainEffect>? terrainEffects = null,
        IEnumerable<ISummonEffect>? summonEffects = null,
        TargetType targetType = TargetType.None,
        int requiredLevel = 0,
        bool requiresLineOfSight = false,
        string? description = null,
        string? castSound = null,
        string? impactSound = null,
        string? vfxCast = null,
        string? vfxImpact = null,
        int price = 0,
        CharacterClass requiredClass = CharacterClass.None,
        DateTime createdAt = default,
        DateTime updatedAt = default,
        string? castMode = null
    )
    {
        Id = id;
        Name = name;
        ManaCost = manaCost;
        CastTime = castTime;
        Cooldown = cooldown;
        Range = range;
        IsChanneled = isChanneled;
        Duration = duration;
        CastType = castType;
        Targeting = targeting;
        UnitEffects = unitEffects?.ToList() ?? new List<ISpellEffect>();
        TerrainEffects = terrainEffects?.ToList() ?? new List<ITerrainEffect>();
        SummonEffects = summonEffects?.ToList() ?? new List<ISummonEffect>();
        TargetType = targetType;
        RequiredLevel = requiredLevel;
        RequiresLineOfSight = requiresLineOfSight;
        Description = description;
        CastSound = castSound;
        ImpactSound = impactSound;
        VfxCast = vfxCast;
        VfxImpact = vfxImpact;
        Price = price;
        RequiredClass = requiredClass;
        CreatedAt = createdAt == default ? DateTime.Now : createdAt;
        UpdatedAt = updatedAt == default ? DateTime.Now : updatedAt;
        CastMode = castMode;
    }
}
