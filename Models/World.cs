using System;
using System.Collections.Generic;

namespace Game.Models
{
    public class Map
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Coordinates { get; set; }  // Ejemplo: "X: 100, Y: 200"
        public int MinLevel { get; set; }
        public int MaxPlayers { get; set; }
        public string Features { get; set; }  // Ejemplo: "PvP, Recursos Especiales"
        public int WorldId { get; set; } // Relación con el mundo al que pertenece

        // Relación con World
        public World World { get; set; }

        // Lista de jugadores en el mapa
        public List<PlayerWorldRelation> PlayerWorldRelations { get; set; }  // Relación entre el jugador y el mapa

        // Lógica de negocio

        // Verifica si un jugador puede acceder a este mapa
        public bool CanAccess(int playerLevel)
        {
            return playerLevel >= MinLevel;
        }

        // Verifica si hay espacio disponible para más jugadores
        public bool HasSpace()
        {
            return PlayerWorldRelations.Count < MaxPlayers;
        }

        // Método para agregar un jugador al mapa si hay espacio y puede acceder
        public bool TryAddPlayer(int playerLevel)
        {
            if (CanAccess(playerLevel) && HasSpace())
            {
                return true; // El jugador puede entrar
            }
            return false; // El jugador no puede entrar
        }

        // Método para obtener una descripción del mapa
        public string GetMapDescription()
        {
            return $"{Name} - {Coordinates} - {Features}";
        }
    }
}
}