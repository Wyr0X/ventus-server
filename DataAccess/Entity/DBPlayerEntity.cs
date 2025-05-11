using System;

namespace VentusServer.DataAccess.Entities
{
    public class DbPlayerEntity
    {
        public int Id { get; set; }
        public Guid AccountId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Gender { get; set; } = 0;
        public int Race { get; set; } = 0;
        public int Level { get; set; } = 1;
        public int Class { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public string Status { get; set; } = "active";
        public string InputBindings { get; set; } = "[]";  // Usando JSONB en la base de datos
        public string Hotbar { get; set; } = "[]";  // Usando JSONB en la base de datos
    }
}
