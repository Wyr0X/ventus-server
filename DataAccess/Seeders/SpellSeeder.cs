using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.DataAccess.Interfaces;
using Game.Models;

namespace VentusServer.Seeders
{
    public class SpellSeeder
    {
        private readonly ISpellDAO _spellDao;

        public SpellSeeder(ISpellDAO spellDao)
        {
            _spellDao = spellDao;
        }

        public async Task SeedAsync()
        {
            var spells = new List<SpellModel>
            {
                new SpellModel(
                    id: "heal_light",
                    name: "Curar Heridas Leves",
                    manaCost: 15,
                    castTime: 1000,
                    cooldown: 3000,
                    range: 6,
                    isChanneled: false,
                    duration: 0,
                    castType: SpellCastType.Instant,
                    targeting: new SingleTargetStrategy(),
                    unitEffects: new List<ISpellEffect>
                    {
                        new HealEffect { Amount = 40 }
                    },
                    terrainEffects: new List<ITerrainEffect>(),
                    summonEffects: new List<ISummonEffect>(),
                    targetType: TargetType.Ally,
                    requiredLevel: 1,
                    requiresLineOfSight: true,
                    description: "Restaura una pequeña cantidad de salud.",
                    castSound: "sounds/heal.wav",
                    impactSound: "sounds/impact_heal.wav",
                    vfxCast: "vfx/heal_cast.vfx",
                    vfxImpact: "vfx/heal_impact.vfx",
                    price: 0, // Aseguramos que todos los campos están definidos
                    requiredClass: CharacterClass.None // Asegúrate que el CharacterClass esté definido correctamente
                ),
                new SpellModel(
                    id: "lightning_storm",
                    name: "Tormenta Eléctrica",
                    manaCost: 40,
                    castTime: 2500,
                    cooldown: 10000,
                    range: 8,
                    isChanneled: true,
                    duration: 0,
                    castType: SpellCastType.CastTime,
                    targeting: new AreaOfEffectStrategy(),
                    unitEffects: new List<ISpellEffect>
                    {
                        new DamageEffect { Element = "electric", Amount = 80 }
                    },
                    terrainEffects: new List<ITerrainEffect>(),
                    summonEffects: new List<ISummonEffect>(),
                    targetType: TargetType.Area,
                    requiredLevel: 8,
                    requiresLineOfSight: true,
                    description: "Inflige daño eléctrico en un área.",
                    castSound: "sounds/storm.wav",
                    impactSound: null,
                    vfxCast: "vfx/storm_cast.vfx",
                    vfxImpact: "vfx/storm_impact.vfx",
                    price: 0, // Aseguramos que todos los campos están definidos
                    requiredClass: CharacterClass.None
                ),
                new SpellModel(
                    id: "invisibility",
                    name: "Invisibilidad",
                    manaCost: 25,
                    castTime: 1200,
                    cooldown: 20000,
                    range: 6,
                    isChanneled: false,
                    duration: 10,
                    castType: SpellCastType.CastTime,
                    targeting: new SingleTargetStrategy(),
                    unitEffects: new List<ISpellEffect>
                    {
                        new InvisibleEffect(10)
                    },
                    terrainEffects: new List<ITerrainEffect>(),
                    summonEffects: new List<ISummonEffect>(),
                    targetType: TargetType.Self,
                    requiredLevel: 5,
                    requiresLineOfSight: false,
                    description: "Vuelve invisible al objetivo temporalmente.",
                    castSound: "sounds/invisibility.wav",
                    impactSound: null,
                    vfxCast: "vfx/invisible_cast.vfx",
                    vfxImpact: null,
                    price: 0, // Aseguramos que todos los campos están definidos
                    requiredClass: CharacterClass.None
                )
            };

            // Verificamos si cada hechizo existe en la base de datos
            foreach (var spell in spells)
            {
                var exists = await _spellDao.SpellExistsAsync(spell.Id);
                if (!exists)
                {
                    // Si no existe, lo creamos
                    await _spellDao.CreateSpellAsync(spell);
                }
            }
        }
    }
}
