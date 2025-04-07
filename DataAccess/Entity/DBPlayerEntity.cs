using System;

namespace VentusServer.DataAccess.Entities
{
    public class DbPlayerEntity
    {
        public int Id { get; set; }
        public Guid AccountId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Race { get; set; } = string.Empty;
        public int Level { get; set; } = 1;
        public string Class { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public string Status { get; set; } = "active";
    }
}
