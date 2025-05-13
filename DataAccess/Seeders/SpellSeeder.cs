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
                    targeting: new SingleTargetStrategy(),
                    unitEffects: new List<ISpellEffect>
                    {
                        new HealEffect { Amount = 40 }
                    },
                    terrainEffects: new List<ITerrainEffect>(),
                    summonEffects: new List<ISummonEffect>(),
                    requiresLineOfSight: true,
                    requiredLevel: 1,
                    targetType: TargetType.Ally,
                    description: "Restaura una pequeña cantidad de salud.",
                    castSound: "sounds/heal.wav",
                    impactSound: "sounds/impact_heal.wav",
                    vfxCast: "vfx/heal_cast.vfx",
                    vfxImpact: "vfx/heal_impact.vfx"
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
                    targeting: new AreaOfEffectStrategy(),
                    unitEffects: new List<ISpellEffect>
                    {
                        new DamageEffect { Element = "electric", Amount = 80 }
                    },
                    terrainEffects: new List<ITerrainEffect>(),
                    summonEffects: new List<ISummonEffect>(),
                    requiresLineOfSight: true,
                    requiredLevel: 8,
                    targetType: TargetType.Area,
                    description: "Inflige daño eléctrico en un área.",
                    castSound: "sounds/storm.wav",
                    impactSound: null,
                    vfxCast: "vfx/storm_cast.vfx",
                    vfxImpact: "vfx/storm_impact.vfx"
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
                    targeting: new SingleTargetStrategy(),
                   unitEffects: new List<ISpellEffect>
{
    new InvisibleEffect(10)
},
                    terrainEffects: new List<ITerrainEffect>(),
                    summonEffects: new List<ISummonEffect>(),
                    requiresLineOfSight: false,
                    requiredLevel: 5,
                    targetType: TargetType.Self,
                    description: "Vuelve invisible al objetivo temporalmente.",
                    castSound: "sounds/invisibility.wav",
                    impactSound: null,
                    vfxCast: "vfx/invisible_cast.vfx",
                    vfxImpact: null
                )
            };

            foreach (var spell in spells)
            {
                var exists = await _spellDao.SpellExistsAsync(spell.Id);
                if (!exists)
                {
                    await _spellDao.CreateSpellAsync(spell);
                }
            }
        }
    }
}
