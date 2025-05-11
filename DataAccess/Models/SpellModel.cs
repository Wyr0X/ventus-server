using System.Collections.Generic;

namespace Ventus.GameModel.Spells
{
    // --- Interfaces base para los distintos tipos de efectos ---

    /// <summary>
    /// Efecto que aplica algo a unidades (jugadores, NPCs, etc.).
    /// </summary>
    public interface ISpellEffect { }

    /// <summary>
    /// Efecto que modifica el terreno (crea fuego, niebla, etc.).
    /// </summary>
    public interface ITerrainEffect { }

    /// <summary>
    /// Efecto que invoca criaturas u objetos (por ejemplo, invocar una criatura).
    /// </summary>
    public interface ISummonEffect { }

    /// <summary>
    /// Define cómo se seleccionan los objetivos del hechizo (área, en línea recta, objetivo único, etc.).
    /// </summary>
    public interface ITargetingStrategy { }

    // --- Modelo principal del hechizo ---

    /// <summary>
    /// Representa un hechizo dentro del juego.
    /// </summary>
    public class SpellModel
    {
        // --- Propiedades básicas ---

        /// <summary>
        /// Identificador único del hechizo.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Nombre del hechizo.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Costo de maná para lanzar el hechizo.
        /// </summary>
        public int ManaCost { get; }

        /// <summary>
        /// Tiempo de lanzamiento del hechizo (en milisegundos).
        /// </summary>
        public int CastTime { get; }

        /// <summary>
        /// Tiempo de reutilización del hechizo (en milisegundos).
        /// </summary>
        public int Cooldown { get; }

        /// <summary>
        /// Rango del hechizo (en unidades de distancia).
        /// </summary>
        public int Range { get; }

        /// <summary>
        /// Indica si el hechizo es canalizado (requiere mantener la tecla presionada o estar en un estado de canalización).
        /// </summary>
        public bool IsChanneled { get; }

        /// <summary>
        /// Duración del hechizo (en segundos), si aplica.
        /// </summary>
        public int Duration { get; }

        // --- Estrategia de selección de objetivos ---

        /// <summary>
        /// Estrategia para seleccionar los objetivos del hechizo (área, en línea recta, objetivo único, etc.).
        /// </summary>
        public ITargetingStrategy Targeting { get; }

        // --- Efectos del hechizo ---

        /// <summary>
        /// Lista de efectos que afectan a las unidades (jugadores, NPCs, etc.).
        /// </summary>
        public List<ISpellEffect> UnitEffects { get; }

        /// <summary>
        /// Lista de efectos que modifican el terreno (como crear fuego, niebla, etc.).
        /// </summary>
        public List<ITerrainEffect> TerrainEffects { get; }

        /// <summary>
        /// Lista de efectos que invocan criaturas u objetos.
        /// </summary>
        public List<ISummonEffect> SummonEffects { get; }
        public bool RequiresLineOfSight { get; }
        // --- Constructor ---

        /// <summary>
        /// Constructor para inicializar un nuevo hechizo.
        /// </summary>
        /// <param name="id">Identificador único del hechizo.</param>
        /// <param name="name">Nombre del hechizo.</param>
        /// <param name="manaCost">Costo de maná.</param>
        /// <param name="castTime">Tiempo de lanzamiento.</param>
        /// <param name="cooldown">Tiempo de reutilización.</param>
        /// <param name="range">Rango del hechizo.</param>
        /// <param name="isChanneled">Indica si es canalizado.</param>
        /// <param name="duration">Duración del hechizo.</param>
        /// <param name="targeting">Estrategia de selección de objetivos.</param>
        /// <param name="unitEffects">Lista de efectos que afectan a las unidades.</param>
        /// <param name="terrainEffects">Lista de efectos que afectan al terreno.</param>
        /// <param name="summonEffects">Lista de efectos que invocan criaturas u objetos.</param>
        public SpellModel(
            string id,
            string name,
            int manaCost,
            int castTime,
            int cooldown,
            int range,
            bool isChanneled,
            int duration,
            ITargetingStrategy targeting,
            List<ISpellEffect> unitEffects = null,
            List<ITerrainEffect> terrainEffects = null,
            List<ISummonEffect> summonEffects = null
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
            Targeting = targeting;

            // Si no se pasa ninguna lista de efectos, se inicializa como lista vacía
            UnitEffects = unitEffects ?? new List<ISpellEffect>();
            TerrainEffects = terrainEffects ?? new List<ITerrainEffect>();
            SummonEffects = summonEffects ?? new List<ISummonEffect>();
        }
    }
}
