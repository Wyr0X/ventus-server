using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Newtonsoft.Json;
using VentusServer.DataAccess.Interfaces;
using VentusServer.DataAccess.Queries;
using Game.Models;

#region Type Handler for JSON Fields
/// <summary>
/// Allows Dapper to automatically serialize/deserialize JSON fields.
/// </summary>
public class JsonTypeHandler<T> : SqlMapper.TypeHandler<T>
{
    public override void SetValue(IDbDataParameter parameter, T value)
    {
        parameter.Value = JsonConvert.SerializeObject(value);
        parameter.DbType = DbType.String;
    }

    public override T Parse(object value)
    {
        return JsonConvert.DeserializeObject<T>(value.ToString())!;
    }
}
#endregion

#region DTO: Database Entity Representation
/// <summary>
/// Represents the raw fields of the 'spells' table.
/// </summary>
public class DbSpellEntity
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int ManaCost { get; set; }
    public int CastTime { get; set; }
    public int Cooldown { get; set; }
    public int Range { get; set; }
    public int Price { get; set; }
    public bool IsChanneled { get; set; }
    public int Duration { get; set; }
    public List<ISpellEffect> UnitEffects { get; set; } = new();
    public List<ITerrainEffect> TerrainEffects { get; set; } = new();
    public List<ISummonEffect> SummonEffects { get; set; } = new();
    public string TargetType { get; set; } = string.Empty;
    public string RequiredClass { get; set; } = string.Empty;
    public int RequiredLevel { get; set; }
    public bool RequiresLineOfSight { get; set; }
    public string? Description { get; set; }
    public string? CastSound { get; set; }
    public string? ImpactSound { get; set; }
    public string? VfxCast { get; set; }
    public string? VfxImpact { get; set; }
    public string CastMode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
#endregion

#region Mapper: DbSpellEntity ↔ SpellModel
/// <summary>
/// Maps between the database DTO and the domain SpellModel.
/// </summary>
public static class SpellMapper
{
    public static SpellModel Map(DbSpellEntity e)
    {
        // Parse enums
        var targetType = Enum.Parse<TargetType>(e.TargetType);
        var castMode = Enum.Parse<SpellCastType>(e.CastMode);
        var reqClass = Enum.Parse<CharacterClass>(e.RequiredClass);

        return new SpellModel(
            id: e.Id,
            name: e.Name,
            manaCost: e.ManaCost,
            castTime: e.CastTime,
            cooldown: e.Cooldown,
            range: e.Range,
            isChanneled: e.IsChanneled,
            duration: e.Duration,
            castType: castMode,
            targeting: null, // implement if serialized
            unitEffects: e.UnitEffects,
            terrainEffects: e.TerrainEffects,
            summonEffects: e.SummonEffects,
            targetType: targetType,
            requiredLevel: e.RequiredLevel,
            requiresLineOfSight: e.RequiresLineOfSight,
            description: e.Description,
            castSound: e.CastSound,
            impactSound: e.ImpactSound,
            vfxCast: e.VfxCast,
            vfxImpact: e.VfxImpact,
            price: e.Price,
            requiredClass: reqClass
        );
    }

    public static object ToDbParameters(SpellModel spell)
    {
        return new
        {
            spell.Id,
            spell.Name,
            spell.ManaCost,
            spell.CastTime,
            spell.Cooldown,
            spell.Range,
            spell.Price,
            spell.IsChanneled,
            spell.Duration,
            UnitEffects = JsonConvert.SerializeObject(spell.UnitEffects),
            TerrainEffects = JsonConvert.SerializeObject(spell.TerrainEffects),
            SummonEffects = JsonConvert.SerializeObject(spell.SummonEffects),
            TargetType = spell.TargetType.ToString(),
            RequiredClass = spell.RequiredClass.ToString(),
            spell.RequiredLevel,
            spell.RequiresLineOfSight,
            spell.Description,
            spell.CastSound,
            spell.ImpactSound,
            spell.VfxCast,
            spell.VfxImpact,
            CastMode = spell.CastType.ToString(),
            spell.CreatedAt,
            spell.UpdatedAt
        };
    }
}
#endregion

#region Initialization: Register TypeHandlers
/// <summary>
/// Call this once at application startup.
/// </summary>
public static class DapperConfig
{
    public static void Configure()
    {
        SqlMapper.AddTypeHandler(new JsonTypeHandler<List<ISpellEffect>>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<List<ITerrainEffect>>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<List<ISummonEffect>>());
        // Add other JSON-backed types as needed
    }
}
#endregion

#region DAO: Example Usage in DapperSpellDAO
public class DapperSpellDAO : BaseDAO, ISpellDAO
{
    public DapperSpellDAO(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }

    public async Task<SpellModel?> GetSpellByIdAsync(string spellId)
    {
        using var conn = GetConnection();
        var dto = await conn.QuerySingleOrDefaultAsync<DbSpellEntity>(SpellQueries.SelectById, new { Id = spellId });
        return dto == null ? null : SpellMapper.Map(dto);
    }

    public async Task CreateSpellAsync(SpellModel spell)
    {
        using var conn = GetConnection();
        await conn.ExecuteAsync(SpellQueries.Insert, SpellMapper.ToDbParameters(spell));
    }

    // ... implementar Update, Delete, etc. de forma similar ...
}
#endregion
