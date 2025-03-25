using System;

namespace Game.Models
{
    public class PlayerProgression
    {
        public int PlayerId { get; set; }
        public int Level { get; set; }
        public int Xp { get; set; }
        public int FreeSkillPoints { get; set; }
        public int KilledNpcs { get; set; }
        public int KilledUsers { get; set; }
        public int Deaths { get; set; }
        public int Hp { get; set; }
        public int Mp { get; set; }
        public int Sp { get; set; }

        // Relación con Player
        public Player Player { get; set; }

        // Método para ganar experiencia
        public void GainExperience(int amount)
        {
            Xp += amount;
            CheckLevelUp();
        }

        // Método para verificar si el jugador sube de nivel
        private void CheckLevelUp()
        {
            // Ejemplo de lógica de nivelación
            int xpForNextLevel = Level * 1000; // Definir la cantidad de XP necesaria por nivel
            if (Xp >= xpForNextLevel)
            {
                Level++;
                FreeSkillPoints += 3; // Otorga 3 puntos de habilidad por nivel
                Xp -= xpForNextLevel; // Resta la experiencia gastada para el siguiente nivel
            }
        }

        // Método para distribuir puntos de habilidad
        public void DistributeSkillPoints(int points)
        {
            if (FreeSkillPoints >= points)
            {
                FreeSkillPoints -= points;
                Sp += points; // Aumenta los puntos de habilidad (puedes tener más lógica aquí para distribuir entre atributos específicos)
            }
            else
            {
                throw new InvalidOperationException("No tienes suficientes puntos de habilidad.");
            }
        }

        // Método para contabilizar muertes
        public void IncrementDeathCount()
        {
            Deaths++;
            Hp = 100; // Restablece la vida al morir (esto puede cambiar dependiendo de tu lógica)
            Mp = 50;  // Restablece el mana si aplica
        }

        // Método para incrementar NPCs muertos
        public void IncrementKilledNpcs()
        {
            KilledNpcs++;
        }

        // Método para incrementar jugadores muertos
        public void IncrementKilledUsers()
        {
            KilledUsers++;
        }

        // Método para revivir al jugador
        public void Revive()
        {
            Hp = 100; // Restablecer vida al revivir
            Mp = 50;  // Restablecer mana si aplica
        }

        // Método de reinicio (si es necesario)
        public void ResetProgression()
        {
            Level = 1;
            Xp = 0;
            FreeSkillPoints = 0;
            KilledNpcs = 0;
            KilledUsers = 0;
            Deaths = 0;
            Hp = 100;
            Mp = 50;
            Sp = 0;
        }
    }
}
