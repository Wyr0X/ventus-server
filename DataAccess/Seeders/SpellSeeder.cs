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
                new SpellModel
                {
                    Id = "heal_light",
                    Name = "Curar Heridas Leves",
                    Price = 100,
                    Description = "Restaura una pequeña cantidad de salud.",
                    ManaCost = 15,
                    Cooldown = 3f,
                    CastTime = 1f,
                    Range = 6,
                    TargetType = TargetType.Ally,
                    CastMode = CastMode.Instant,
                    Effects = new List<SpellEffect>
                    {
                        new SpellEffect
                        {
                            Type = "heal",
                            Value = 40,
                            Duration = 0
                        }
                    },
                    School = "Restauración",
                    RequiredLevel = 1,
                    Icon = "icons/heal_light.png",
                    CastSound = "sounds/heal.wav",
                    ImpactSound = "sounds/impact_heal.wav",
                    VfxCast = "vfx/heal_cast.vfx",
                    VfxImpact = "vfx/heal_impact.vfx"
                },
                new SpellModel
                {
                    Id = "lightning_storm",
                    Price = 150,

                    Name = "Tormenta Eléctrica",
                    Description = "Inflige daño eléctrico en un área.",
                    ManaCost = 40,
                    Cooldown = 10f,
                    CastTime = 2.5f,
                    Range = 8,
                    TargetType = TargetType.Area,
                    CastMode = CastMode.Channeled,
                    Area = new AreaOfEffect
                    {
                        Shape = "circle",
                        Radius = 3
                    },
                    Effects = new List<SpellEffect>
                    {
                        new SpellEffect
                        {
                            Type = "damage",
                            Element = "electric",
                            Value = 80,
                            Duration = 0
                        }
                    },
                    School = "Magia",
                    RequiredLevel = 8,
                    Icon = "icons/lightning_storm.png",
                    CastSound = "sounds/storm.wav",
                    VfxCast = "vfx/storm_cast.vfx",
                    VfxImpact = "vfx/storm_impact.vfx"
                },

                new SpellModel
                {
                    Id = "invisibility",
                    Name = "Invisibilidad",
                    Description = "Vuelve invisible al objetivo temporalmente.",
                    ManaCost = 25,
                    Price = 250,

                    Cooldown = 20f,
                    CastTime = 1.2f,
                    Range = 6,
                    TargetType = TargetType.Self,
                    CastMode = CastMode.Instant,
                    Effects = new List<SpellEffect>
                    {
                        new SpellEffect
                        {
                            Type = "status",
                            Status = "invisible",
                            Duration = 10f
                        }
                    },
                    School = "Ilusión",
                    RequiredLevel = 5,
                    Icon = "icons/invisibility.png",
                    CastSound = "sounds/invisibility.wav",
                    VfxCast = "vfx/invisible_cast.vfx"
                }
            };
            Console.WriteLine("Llega");

            foreach (var spell in spells)
            {
                var exists = await _spellDao.SpellExistsAsync(spell.Id);
                if (!exists)
                {
                    Console.WriteLine("Llega 3");

                    await _spellDao.CreateSpellAsync(spell);
                }
            }
        }
    }
}
