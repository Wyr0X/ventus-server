namespace VentusServer.Models
{
    public class ItemDTO
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? HpMin { get; set; }
        public int? HpMax { get; set; }
        public int? MP { get; set; }
        public int[] Sprite { get; set; } = Array.Empty<int>();
        public string? Sound { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
