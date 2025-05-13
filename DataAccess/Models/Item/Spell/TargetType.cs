public enum TargetType
{
    Self,        // El hechizo solo puede lanzarse sobre el propio jugador
    Ally,        // El hechizo puede lanzarse sobre aliados
    Enemy,       // El hechizo puede lanzarse sobre enemigos
    Any,         // El hechizo puede lanzarse sobre cualquier unidad (aliada o enemiga)
    Area,        // El hechizo se lanza en un área (puede afectar múltiples objetivos)
    Ground,      // El hechizo se lanza sobre una posición en el terreno
    None,
}
