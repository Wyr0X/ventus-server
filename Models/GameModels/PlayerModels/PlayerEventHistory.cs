using System;
using System.Collections.Generic;

namespace Game.Models
{
    public class PlayerEventHistory
    {
        public int PlayerId { get; set; }
        public int KilledNpcs { get; set; }
        public int KilledUsers { get; set; }
        public List<string> Events { get; set; } = new List<string>();

        // Relación con Player
        public Player Player { get; set; }

        // Lógica para manejar eventos
        public void AddEvent(string newEvent)
        {
            if (!string.IsNullOrEmpty(newEvent) && !Events.Contains(newEvent))
            {
                Events.Add(newEvent);
            }
        }

        public void IncrementNpcKill()
        {
            KilledNpcs++;
        }

        public void IncrementUserKill()
        {
            KilledUsers++;
        }

        // Método que revisa si el jugador ha completado todos los eventos necesarios
        public bool HasCompletedEvent(string eventName)
        {
            return Events.Contains(eventName);
        }
    }
}
