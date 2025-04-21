using System;
using System.Text.Json;

namespace VentusServer.Domain.Models
{
    public class PlayerInventoryItemModel
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }

        public bool isEquipped { get; set; }
        public int Slot { get; set; } // nullable, opcional si se usan slots

        // Datos personalizados como durabilidad, mejoras, etc
        public JsonDocument? CustomData { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Info del ítem base (puede venir del JOIN con `items`)
        public string? Name { get; set; }
        public string? Icon { get; set; }
    }
}
