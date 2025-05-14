using Newtonsoft.Json;

public static class SpellMapper
{
    // Mapea un SpellDBEntity a un SpellModel
    public static SpellModel ToModel(SpellDBEntity entity)
    {
        // Convertimos JSONB a objetos correspondientes
        var targetingStrategy = string.IsNullOrEmpty(entity.Targeting) ? null : JsonConvert.DeserializeObject<ITargetingStrategy>(entity.Targeting);
        var unitEffects = string.IsNullOrEmpty(entity.UnitEffects) ? new List<ISpellEffect>() : JsonConvert.DeserializeObject<List<ISpellEffect>>(entity.UnitEffects);
        var terrainEffects = string.IsNullOrEmpty(entity.TerrainEffects) ? new List<ITerrainEffect>() : JsonConvert.DeserializeObject<List<ITerrainEffect>>(entity.TerrainEffects);
        var summonEffects = string.IsNullOrEmpty(entity.SummonEffects) ? new List<ISummonEffect>() : JsonConvert.DeserializeObject<List<ISummonEffect>>(entity.SummonEffects);

        // Creamos el modelo de dominio a partir de la entidad DB
        return new SpellModel(
            id: entity.Id,
            name: entity.Name,
            manaCost: entity.ManaCost,
            castTime: entity.CastTime,
            cooldown: entity.Cooldown,
            range: entity.Range,
            isChanneled: entity.IsChanneled,
            duration: entity.Duration,
            castType: (SpellCastType)Enum.Parse(typeof(SpellCastType), entity.CastMode ?? "Instant"),
            targeting: targetingStrategy,
            unitEffects: unitEffects,
            terrainEffects: terrainEffects,
            summonEffects: summonEffects,
            targetType: string.IsNullOrEmpty(entity.TargetType) ? TargetType.None : (TargetType)Enum.Parse(typeof(TargetType), entity.TargetType),
            requiredLevel: entity.RequiredLevel,
            requiresLineOfSight: entity.RequiresLineOfSight,
            description: entity.Description,
            castSound: entity.CastSound,
            impactSound: entity.ImpactSound,
            vfxCast: entity.VfxCast,
            vfxImpact: entity.VfxImpact,
            price: entity.Price,
            requiredClass: string.IsNullOrEmpty(entity.RequiredClass) ? CharacterClass.None : (CharacterClass)Enum.Parse(typeof(CharacterClass), entity.RequiredClass)
        );
    }

    // Mapea un SpellModel a un SpellDBEntity
    public static SpellDBEntity ToDBEntity(SpellModel model)
    {
        // Serializamos las listas de efectos y el targeting a JSONB
        var targetingJson = model.Targeting != null ? JsonConvert.SerializeObject(model.Targeting) : null;
        var unitEffectsJson = model.UnitEffects != null ? JsonConvert.SerializeObject(model.UnitEffects) : "[]";
        var terrainEffectsJson = model.TerrainEffects != null ? JsonConvert.SerializeObject(model.TerrainEffects) : "[]";
        var summonEffectsJson = model.SummonEffects != null ? JsonConvert.SerializeObject(model.SummonEffects) : "[]";

        // Creamos la entidad de base de datos a partir del modelo
        return new SpellDBEntity
        {
            Id = model.Id,
            Name = model.Name,
            ManaCost = model.ManaCost,
            CastTime = model.CastTime,
            Cooldown = model.Cooldown,
            Range = model.Range,
            Price = model.Price,
            IsChanneled = model.IsChanneled,
            Duration = model.Duration,
            Targeting = targetingJson,
            UnitEffects = unitEffectsJson,
            TerrainEffects = terrainEffectsJson,
            SummonEffects = summonEffectsJson,
            TargetType = model.TargetType.ToString(),
            RequiredClass = model.RequiredClass.ToString(),
            RequiredLevel = model.RequiredLevel,
            RequiresLineOfSight = model.RequiresLineOfSight,
            Description = model.Description,
            CastSound = model.CastSound,
            ImpactSound = model.ImpactSound,
            VfxCast = model.VfxCast,
            VfxImpact = model.VfxImpact,
            CastMode = model.CastType.ToString(),
            CreatedAt = DateTime.UtcNow, // Por ejemplo, si se está creando
            UpdatedAt = DateTime.UtcNow  // También se puede ajustar dependiendo de la lógica
        };
    }
}
