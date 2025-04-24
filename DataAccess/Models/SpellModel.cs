public enum CastMode { Instant, Channeled, Charge }
public enum TargetType { Self, Ally, Enemy, Area }
public class SpellEffect
{
    public string Type { get; set; } = string.Empty;  // Siempre tiene un valor por defecto
    public string? Element { get; set; } = null;      // Se puede dejar como null si no se usa
    public string? Status { get; set; } = null;       // Lo mismo aquí
    public string? Target { get; set; } = null;       // Lo mismo para Target
    public float Value { get; set; } = 0.0f;          // Asegúrate de que tenga valor
    public float Duration { get; set; } = 0.0f;       // Al igual que Duration, lo mismo
    public Dictionary<string, float>? Scaling { get; set; } = null;  // Esto puede ser null si no se usa
}


public class AreaOfEffect
{
    public string Shape { get; set; } = "circle"; // "circle", "cone", etc.
    public float Radius { get; set; }
}

public class SpellModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Price { get; set; } = 0;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Animation { get; set; } = string.Empty;

    public int ManaCost { get; set; }
    public float Cooldown { get; set; }
    public float CastTime { get; set; }
    public float Range { get; set; }
    public AreaOfEffect? Area { get; set; }
    public string School { get; set; } = string.Empty;
    public int RequiredLevel { get; set; }
    public string? RequiredClass { get; set; }

    public TargetType TargetType { get; set; }
    public CastMode CastMode { get; set; }

    public List<SpellEffect> Effects { get; set; } = new();
    public bool? CanCrit { get; set; }
    public bool? IsReflectable { get; set; }
    public bool? RequiresLineOfSight { get; set; }
    public bool? Interruptible { get; set; }

    public string CastSound { get; set; } = string.Empty;
    public string ImpactSound { get; set; } = string.Empty;
    public string VfxCast { get; set; } = string.Empty;
    public string VfxImpact { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new();
    public bool IsUltimate { get; set; }
    public string? UnlockedByQuest { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Fecha de creación
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;  //
}
