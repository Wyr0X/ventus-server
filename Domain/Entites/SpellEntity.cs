using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    /// <summary>
    /// Representa un hechizo dentro del juego.
    /// </summary>
    public class SpellEntity
    {
        // --- Identificación y requisitos ---

        /// <summary>Identificador único del hechizo.</summary>
        public string Id { get; }

        /// <summary>Nombre del hechizo mostrado al jugador.</summary>
        public string Name { get; }

        /// <summary>Clase requerida para poder aprender/lanzar el hechizo.</summary>
        public CharacterClass RequiredClass { get; } = CharacterClass.None;

        /// <summary>Nivel mínimo requerido para lanzar el hechizo.</summary>
        public int RequiredLevel { get; }

        // --- Costos y tiempos ---

        /// <summary>Costo de maná para lanzar el hechizo.</summary>
        public int ManaCost { get; }

        /// <summary>Tiempo que tarda en lanzarse el hechizo (en milisegundos o ticks).</summary>
        public int CastTime { get; }

        /// <summary>Tiempo de reutilización antes de poder volver a usar el hechizo.</summary>
        public int Cooldown { get; }

        /// <summary>Indica si el hechizo es canalizado (continúa aplicando mientras se mantiene).</summary>
        public bool IsChanneled { get; }

        /// <summary>Duración total del efecto (en milisegundos o ticks, si aplica).</summary>
        public int Duration { get; }

        /// <summary>Tipo de lanzamiento del hechizo (instantáneo, canalizado, con casteo, etc.).</summary>
        public SpellCastType CastType { get; }

        // --- Alcance y targeting ---

        /// <summary>Rango máximo del hechizo en unidades del mundo.</summary>
        public int Range { get; }

        /// <summary>Tipo de objetivo permitido (enemigo, aliado, área, etc.).</summary>
        public TargetType TargetType { get; }

        /// <summary>Indica si el objetivo debe estar en línea de visión para lanzar el hechizo.</summary>
        public bool RequiresLineOfSight { get; }

        /// <summary>Estrategia utilizada para seleccionar objetivos al lanzar el hechizo.</summary>
        public ITargetingStrategy? Targeting { get; }

        // --- Efectos del hechizo ---

        /// <summary>Lista de efectos que se aplican a unidades (daño, curación, estados, etc.).</summary>
        public IReadOnlyList<ISpellEffect> UnitEffects { get; }

        /// <summary>Lista de efectos que se aplican sobre el terreno (áreas persistentes, trampas, etc.).</summary>
        public IReadOnlyList<ITerrainEffect> TerrainEffects { get; }

        /// <summary>Lista de efectos que invocan criaturas, tótems o entidades en el mundo.</summary>
        public IReadOnlyList<ISummonEffect> SummonEffects { get; }

        // --- Información visual y auditiva ---

        /// <summary>Descripción del hechizo mostrada en la UI.</summary>
        public string? Description { get; }

        /// <summary>Ruta o clave del efecto visual al lanzar el hechizo.</summary>
        public string? VfxCast { get; }

        /// <summary>Ruta o clave del efecto visual al impactar el hechizo.</summary>
        public string? VfxImpact { get; }

        /// <summary>Sonido reproducido al lanzar el hechizo.</summary>
        public string? CastSound { get; }

        /// <summary>Sonido reproducido al impactar el hechizo.</summary>
        public string? ImpactSound { get; }

        // --- Otros datos ---

        /// <summary>Precio en monedas para comprar o desbloquear el hechizo.</summary>
        public int Price { get; }

        // --- Constructor ---

        /// <summary>
        /// Constructor para inicializar un nuevo hechizo.
        /// </summary>
        public SpellEntity(
            string id,
            string name,
            int manaCost,
            int castTime,
            int cooldown,
            int range,
            bool isChanneled,
            int duration,
            SpellCastType castType,
            ITargetingStrategy? targeting,
            IEnumerable<ISpellEffect>? unitEffects = null,
            IEnumerable<ITerrainEffect>? terrainEffects = null,
            IEnumerable<ISummonEffect>? summonEffects = null,
            TargetType targetType = TargetType.None,
            int requiredLevel = 0,
            bool requiresLineOfSight = false,
            string? description = null,
            string? castSound = null,
            string? impactSound = null,
            string? vfxCast = null,
            string? vfxImpact = null,
            int price = 0,
            CharacterClass requiredClass = CharacterClass.None
        )
        {
            Id = id;
            Name = name;
            ManaCost = manaCost;
            CastTime = castTime;
            Cooldown = cooldown;
            Range = range;
            IsChanneled = isChanneled;
            Duration = duration;
            CastType = castType;
            Targeting = targeting;
            UnitEffects = unitEffects?.ToList() ?? new List<ISpellEffect>();
            TerrainEffects = terrainEffects?.ToList() ?? new List<ITerrainEffect>();
            SummonEffects = summonEffects?.ToList() ?? new List<ISummonEffect>();
            TargetType = targetType;
            RequiredLevel = requiredLevel;
            RequiresLineOfSight = requiresLineOfSight;
            Description = description;
            CastSound = castSound;
            ImpactSound = impactSound;
            VfxCast = vfxCast;
            VfxImpact = vfxImpact;
            Price = price;
            RequiredClass = requiredClass;
        }
    }
}
