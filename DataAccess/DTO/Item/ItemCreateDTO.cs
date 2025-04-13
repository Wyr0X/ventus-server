namespace VentusServer.Models
{
    public class ItemCreateDTO
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? HpMin { get; set; }
        public int? HpMax { get; set; }
        public int? MP { get; set; }
        public int[]? Sprite { get; set; }
        public string? Sound { get; set; }
    }
}
