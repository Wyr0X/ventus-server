using System;
using System.Collections.Generic;

namespace VentusServer.Domain.Models
{
    public class PlayerInventoryModel
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int MaxSlots { get; set; }
        public int Gold { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Lista de ítems en el inventario
        public List<PlayerInventoryItemModel> Items { get; set; } = new();
        public int Slots = 10;
    }
}
