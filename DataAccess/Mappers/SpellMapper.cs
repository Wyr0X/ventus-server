using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public static class SpellMapper
{
    public static SpellModel Map(dynamic dbEntity)
    {
        Console.WriteLine($"[SpellMapper] Procesando spell ID: {dbEntity.id}, effects: {dbEntity.effects}");

        // 1) Deserializar effects
        var effects = new List<SpellEffect>();
        if (dbEntity.effects != null)
        {
            var effectsString = dbEntity.effects.ToString();
            if (!string.IsNullOrWhiteSpace(effectsString) && effectsString != "[]")
            {
                if (IsValidJson(effectsString))
                {
                    try
                    {
                        effects = JsonConvert.DeserializeObject<List<SpellEffect>>(effectsString)
                                  ?? new List<SpellEffect>();
                        Console.WriteLine("[SpellMapper] Deserialización exitosa de 'effects': "
                                          + JsonConvert.SerializeObject(effects));
                    }
                    catch (JsonReaderException ex)
                    {
                        Console.WriteLine($"[SpellMapper] Error al deserializar 'effects' para spell ID {dbEntity.id}: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("[SpellMapper] 'effects' no es un JSON válido.");
                }
            }
        }

        // 2) Deserializar area
        AreaOfEffect? area = null;
        if (dbEntity.area != null)
        {
            var areaString = dbEntity.area.ToString();
            if (!string.IsNullOrWhiteSpace(areaString) && IsValidJson(areaString))
            {
                try
                {
                    area = JsonConvert.DeserializeObject<AreaOfEffect>(areaString);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SpellMapper] Error al deserializar 'area' para spell ID {dbEntity.id}: {ex.Message}");
                }
            }
        }

        // 3) Deserializar tags
        var tags = new List<string>();
        if (dbEntity.tags != null)
        {
            try
            {
                var token = JToken.Parse(dbEntity.tags.ToString());
                if (token.Type == JTokenType.Array)
                    tags = token.ToObject<List<string>>() ?? new List<string>();
            }
            catch
            {
                try
                {
                    var arr = dbEntity.tags as IEnumerable<object>;
                    if (arr != null)
                        tags = arr.Select(o => o?.ToString() ?? string.Empty).ToList();
                }
                catch { }
            }
        }

        // 4) Construir el modelo con valores predeterminados para bool
        return new SpellModel
        {
            Id = dbEntity.id,
            Name = dbEntity.name,
            Price = dbEntity.price,
            Description = dbEntity.description,
            Icon = dbEntity.icon,
            Animation = dbEntity.animation,
            ManaCost = dbEntity.mana_cost,
            Cooldown = dbEntity.cooldown,
            CastTime = dbEntity.cast_time,
            Range = dbEntity.range,
            Area = area,
            School = dbEntity.school,
            RequiredLevel = dbEntity.required_level,
            RequiredClass = dbEntity.required_class,
            TargetType = Enum.TryParse<TargetType>(dbEntity.target_type?.ToString(), out TargetType t) ? t : TargetType.Self,
            CastMode = Enum.TryParse<CastMode>(dbEntity.cast_mode?.ToString(), out CastMode c) ? c : CastMode.Instant,
            Effects = effects,
            CanCrit = (bool?)(dbEntity.can_crit) ?? false,
            IsReflectable = (bool?)(dbEntity.is_reflectable) ?? false,
            RequiresLineOfSight = (bool?)(dbEntity.requires_line_of_sight) ?? false,
            Interruptible = (bool?)(dbEntity.interruptible) ?? false,
            CastSound = dbEntity.cast_sound,
            ImpactSound = dbEntity.impact_sound,
            VfxCast = dbEntity.vfx_cast,
            VfxImpact = dbEntity.vfx_impact,
            Tags = tags,
            IsUltimate = (bool?)(dbEntity.is_ultimate) ?? false,
            UnlockedByQuest = dbEntity.unlocked_by_quest,
            CreatedAt = dbEntity.created_at,
            UpdatedAt = dbEntity.updated_at
        };
    }

    private static bool IsValidJson(string str)
    {
        try
        {
            JToken.Parse(str);
            return true;
        }
        catch (JsonReaderException)
        {
            return false;
        }
    }

    public static object ToDbParameters(SpellModel spell)
    {
        return new
        {
            spell.Id,
            spell.Name,
            spell.Price,
            spell.Description,
            spell.Icon,
            spell.Animation,
            spell.ManaCost,
            spell.Cooldown,
            spell.CastTime,
            spell.Range,
            Area = spell.Area != null ? JsonConvert.SerializeObject(spell.Area) : null,
            spell.School,
            spell.RequiredLevel,
            spell.RequiredClass,
            TargetType = spell.TargetType.ToString(),
            CastMode = spell.CastMode.ToString(),
            Effects = spell.Effects != null ? JsonConvert.SerializeObject(spell.Effects) : null,
            spell.CanCrit,
            spell.IsReflectable,
            spell.RequiresLineOfSight,
            spell.Interruptible,
            spell.CastSound,
            spell.ImpactSound,
            spell.VfxCast,
            spell.VfxImpact,
            Tags = spell.Tags != null && spell.Tags.Any() ? spell.Tags.ToArray() : null,
            spell.IsUltimate,
            spell.UnlockedByQuest,
            spell.CreatedAt,
            spell.UpdatedAt
        };
    }
}