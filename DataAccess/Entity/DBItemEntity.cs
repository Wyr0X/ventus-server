using System;

namespace VentusServer.DataAccess.Entities
{
    public class DbItemEntity
    {
        public int Id { get; set; }                      // Primary Key
        public string Key { get; set; } = string.Empty;  // Internal item key, e.g., "red_potion"

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int? HpMin { get; set; }                  // Null if not applicable
        public int? HpMax { get; set; }

        public int? MP { get; set; }            // Stored as 0.05 for "5%"

        public string? Sound { get; set; }               // Optional; if server triggers it

        public DateTime CreatedAt { get; set; }
    }
}
