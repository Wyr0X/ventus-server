using System;
using System.Collections.Generic;

namespace Game.Models
{
    public class PlayerBasicModel
    {
        public int Id { get; set; }
        public int AccountId { get; set; } // Relación con la cuenta (se asume que la cuenta existe)
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Race { get; set; }
        public int Level { get; set; }
        public string Class { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public string Status { get; set; } // Ejemplo: "Online", "Offline", "Banned"

        public void UpdateStatus(string newStatus)
        {
            // Actualiza el estado del jugador (Ej. de "Offline" a "Online")
            Status = newStatus;
        }
    }
}
