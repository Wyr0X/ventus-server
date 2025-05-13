public class DbSpellEntity
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int ManaCost { get; set; }
    public int CastTime { get; set; }
    public int Cooldown { get; set; }
    public int Range { get; set; }
    public bool IsChanneled { get; set; }
    public int Duration { get; set; }
    public string CastType { get; set; }
    public string? TargetingJson { get; set; }
    public string? EffectsJson { get; set; }
    public string? TerrainEffectsJson { get; set; }
    public string? SummonEffectsJson { get; set; }
    public string TargetType { get; set; }
    public int RequiredLevel { get; set; }
    public bool RequiresLineOfSight { get; set; }
    public string? Description { get; set; }
    public string? VfxCast { get; set; }
    public string? VfxImpact { get; set; }
    public string? CastSound { get; set; }
    public string? ImpactSound { get; set; }
    public int Price { get; set; }
    public string RequiredClass { get; set; }
}