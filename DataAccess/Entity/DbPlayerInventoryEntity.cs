using System;
using System.Text.Json;

namespace VentusServer.DataAccess.Entities
{
    public class DbPlayerInventoryEntity
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int Gold { get; set; }
        public int MaxSlots { get; set; }

        public JsonDocument? Items { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
