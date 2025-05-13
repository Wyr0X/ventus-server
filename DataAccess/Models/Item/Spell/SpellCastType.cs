/// <summary>
/// Define las distintas formas de lanzar un hechizo.
/// </summary>
public enum SpellCastType
{
    /// <summary>Se lanza de forma instantánea sin animación ni tiempo de casteo.</summary>
    Instant,

    /// <summary>Requiere un tiempo de lanzamiento antes de activarse.</summary>
    CastTime,

    /// <summary>Canaliza continuamente mientras se mantiene el hechizo activo.</summary>
    Channeled,

    /// <summary>Se lanza como un proyectil que tarda en llegar al objetivo.</summary>
    Projectile,

    /// <summary>Afecta una zona seleccionada del terreno.</summary>
    GroundTargeted,

    /// <summary>Afecta al lanzador directamente (buff, autocuración, etc.).</summary>
    Self,

    /// <summary>Se activa en respuesta a una condición (hechizos reactivos).</summary>
    Reactive
}
