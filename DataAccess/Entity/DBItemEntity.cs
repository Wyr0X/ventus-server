namespace VentusServer.DataAccess.Entities
{
    public class DbItemEntity
    {
        public int Id { get; set; }  // Identificador único del ítem
        public required string Key { get; set; }  // Clave del ítem
        public required string NameJson { get; set; }  // Nombre en formato JSONB
        public required string DescriptionJson { get; set; }  // Descripción en formato JSONB
        public required string Type { get; set; }  // Tipo del ítem (Arma, Armadura, etc.)
        public required string Rarity { get; set; }  // Rareza del ítem (Common, Rare, Epic, etc.)
        public int? Damage { get; set; }  // Daño del ítem (si aplica)
        public int? Defense { get; set; }  // Defensa del ítem (si aplica)
        public int? ManaBonus { get; set; }  // Bonificación de mana (si aplica)
        public int? StrengthBonus { get; set; }  // Bonificación de fuerza (si aplica)
        public int? SpeedBonus { get; set; }  // Bonificación de velocidad (si aplica)
        public int MaxStack { get; set; }  // Cantidad máxima de apilamiento (1 si no se apila)
        public string? IconPath { get; set; }  // Ruta del ícono (si aplica)
        public required int[] Sprite { get; set; }  // Array de sprites en el juego
        public string[]? KeyArray { get; set; }  // Array de claves del ítem
        public bool IsTradable { get; set; }  // Si el ítem es comerciable
        public bool IsDroppable { get; set; }  // Si el ítem puede ser soltado
        public bool IsUsable { get; set; }  // Si el ítem es usable
        public string? Sound { get; set; }  // Ruta al sonido del ítem (si aplica)
        public DateTime CreatedAt { get; set; }  // Fecha de creación del ítem
        public DateTime UpdatedAt { get; set; }  // Fecha de la última actualización del ítem
    }
}
