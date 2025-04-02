using System;
using System.Collections.Generic;

namespace Game.Models
{
    public class PlayerBasicModel
    {
        public int Id { get; set; }
        public int AccountId { get; set; } // Relación con la cuenta (se asume que la cuenta existe)
        public required string Name { get; set; }
        public required string Gender { get; set; }
        public required string Race { get; set; }
        public int Level { get; set; }
        public required string Class { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public required string Status { get; set; } // Ejemplo: "Online", "Offline", "Banned"

        public void UpdateStatus(string newStatus)
        {
            // Actualiza el estado del jugador (Ej. de "Offline" a "Online")
            Status = newStatus;
        }
    }
}
