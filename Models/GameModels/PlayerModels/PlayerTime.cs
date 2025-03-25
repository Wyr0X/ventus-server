using System;

namespace Game.Models
{
    public class PlayerTime
    {
        public int PlayerId { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Deaths { get; set; }

        // Relación con Player
        public Player Player { get; set; }

        // Lógica de negocio

        // Actualiza la fecha del último login
        public void UpdateLastLogin()
        {
            LastLogin = DateTime.UtcNow;
        }

        // Incrementa el contador de muertes
        public void IncrementDeaths()
        {
            Deaths++;
        }

        // Verifica si el jugador tiene un tiempo razonable desde su última conexión
        public bool HasRecentlyLoggedIn(TimeSpan threshold)
        {
            return DateTime.UtcNow - LastLogin < threshold;
        }
    }
}
