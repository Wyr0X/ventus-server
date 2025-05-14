using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VentusServer.Models;

public class SpellDTO
{
    [Required]
    public string Name { get; set; }

    [Range(0, int.MaxValue)]
    public int ManaCost { get; set; }

    [Range(0, int.MaxValue)]
    public int CastTime { get; set; }

    [Range(0, int.MaxValue)]
    public int Cooldown { get; set; }

    [Range(0, int.MaxValue)]
    public int Range { get; set; }

    public bool IsChanneled { get; set; }

    [Range(0, int.MaxValue)]
    public int Duration { get; set; }

    public SpellCastType CastType { get; set; }
    [Required]
    public ITargetingStrategy Targeting { get; set; }

    [Required]
    public List<ISpellEffect> Effects { get; set; } = new();

    public List<ITerrainEffect> TerrainEffects { get; set; } = new();
    public List<ISummonEffect> SummonEffects { get; set; } = new();

    public bool RequiresLineOfSight { get; set; }

    [Range(0, int.MaxValue)]
    public int RequiredLevel { get; set; }

    [Required]
    public TargetType TargetType { get; set; }

    public string Description { get; set; }
    public string CastSound { get; set; }
    public string ImpactSound { get; set; }
    public string VfxCast { get; set; }
    public string VfxImpact { get; set; }
}