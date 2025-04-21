public class DbItemEntity
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string NameJson { get; set; }
    public string DescriptionJson { get; set; }
    public string Type { get; set; }
    public string Rarity { get; set; }
    public int? RequiredLevel { get; set; }
    public int? Price { get; set; }
    public int? Quantity { get; set; }
    public int? MaxStack { get; set; }
    public bool IsTradable { get; set; }
    public bool IsDroppable { get; set; }
    public bool IsUsable { get; set; }
    public string IconPath { get; set; }
    public int[]? Sprite { get; set; }
    public string? Sound { get; set; }
    public string? DataJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
