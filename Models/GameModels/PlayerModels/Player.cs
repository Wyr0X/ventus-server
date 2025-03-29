using System;
using System.Collections.Generic;

namespace Game.Models
{
    public class Player
    {
        public int Id { get; set; }
        public int AccountId { get; set; }  // Relación con la cuenta (se asume que la cuenta existe)
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Race { get; set; }
        public int Level {get; set;}
        public string Class { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public string Status { get; set; } // Ejemplo: "Online", "Offline", "Banned"
        
        // Relaciones
        public PlayerLocation Location { get; set; }  // Ubicación actual del jugador en el mundo
        public List<PlayerWorldRelation> WorldRelations { get; set; }  // Relación con el mundo

        // Métodos para gestionar la lógica del jugador
        public void SetLocation(int mapId, int posX, int posY, string direction)
        {
            // Asigna la nueva ubicación del jugador en el mapa
            Location = new PlayerLocation
            {
                PlayerId = this.Id,
                PosMap = mapId.ToString(),
                PosX = posX,
                PosY = posY,
                Direction = direction
            };
        }

        public void UpdateStatus(string newStatus)
        {
            // Actualiza el estado del jugador (Ej. de "Offline" a "Online")
            Status = newStatus;
        }

        // Verifica si el jugador tiene acceso al mundo
        public bool CanEnterWorld(World world)
        {
            // Verifica si el jugador tiene el nivel requerido para acceder al mundo
            return this.Level >= world.LevelRequirements;
        }
    }

}
