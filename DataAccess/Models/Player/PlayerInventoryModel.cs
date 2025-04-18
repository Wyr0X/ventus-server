using VentusServer.Domain.Models;

public class PlayerInventory
{
    public int Id { get; set; }
    public required int PlayerId { get; set; } // Referencia al jugador

    public required List<PlayerInventoryItemModel> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
