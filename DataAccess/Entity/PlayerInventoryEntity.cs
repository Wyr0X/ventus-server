using System;
using System.Text.Json;

namespace VentusServer.DataAccess.Entities
{
    public class DbPlayerInventoryItemEntity
    {
        public Guid Id { get; set; }  // UUID de la tabla player_items
        public Guid InventoryId { get; set; }  // FK al inventario del jugador
        public int ItemId { get; set; }  // FK al ítem base

        public int Quantity { get; set; }  // Cantidad de ítems
        public int? Slot { get; set; }  // Posición en el inventario, si se usa

        public JsonDocument? CustomData { get; set; }  // JSONB con información personalizada

        public DateTime CreatedAt { get; set; }  // Fecha de creación
        public DateTime UpdatedAt { get; set; }  // Fecha de modificación
    }
}
