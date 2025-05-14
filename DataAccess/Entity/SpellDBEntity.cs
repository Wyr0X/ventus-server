public class SpellDBEntity
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int ManaCost { get; set; }
    public int CastTime { get; set; }
    public int Cooldown { get; set; }
    public int Range { get; set; }
    public int Price { get; set; }
    public bool IsChanneled { get; set; }
    public int Duration { get; set; }
    public string Targeting { get; set; } // JSONB como string
    public string UnitEffects { get; set; } // JSONB como string
    public string TerrainEffects { get; set; } // JSONB como string
    public string SummonEffects { get; set; } // JSONB como string
    public string TargetType { get; set; }
    public string RequiredClass { get; set; }
    public int RequiredLevel { get; set; }
    public bool RequiresLineOfSight { get; set; }
    public string Description { get; set; }
    public string CastSound { get; set; }
    public string ImpactSound { get; set; }
    public string VfxCast { get; set; }
    public string VfxImpact { get; set; }
    public string CastMode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
