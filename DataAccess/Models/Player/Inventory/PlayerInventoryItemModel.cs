using System;
using System.Text.Json;

namespace VentusServer.Domain.Models
{
    public class PlayerInventoryItemModel
    {
        public Guid Id { get; set; }
        public Guid InventoryId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public int? Slot { get; set; } // nullable, opcional si se usan slots

        // Datos personalizados como durabilidad, mejoras, etc
        public JsonDocument? CustomData { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Info del ítem base (puede venir del JOIN con `items`)
        public string? Name { get; set; }
        public string? Icon { get; set; }
        public string? ItemType { get; set; }
    }
}
