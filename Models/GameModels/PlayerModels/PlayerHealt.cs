using System;

namespace Game.Models
{
    public class PlayerHealth
    {
        public int PlayerId { get; set; }
        public int Hp { get; set; }
        public int Mp { get; set; }
        public int Sp { get; set; }
        public int MaxHp { get; set; }
        public int MaxMp { get; set; }
        public int MaxSp { get; set; }

        // Relación con Player
        public Player Player { get; set; }

        // Lógica para modificar la salud, mana y puntos de habilidad
        public void TakeDamage(int damage)
        {
            Hp = Math.Max(0, Hp - damage);  // Asegura que la salud no sea menor a 0
        }

        public void Heal(int amount)
        {
            Hp = Math.Min(MaxHp, Hp + amount);  // No puede superar el MaxHp
        }

        public void UseMana(int amount)
        {
            Mp = Math.Max(0, Mp - amount);  // Asegura que el mana no sea menor a 0
        }

        public void RestoreMana(int amount)
        {
            Mp = Math.Min(MaxMp, Mp + amount);  // No puede superar el MaxMp
        }

        public void UseSp(int amount)
        {
            Sp = Math.Max(0, Sp - amount);  // Asegura que los puntos de habilidad no sean menores a 0
        }

        public void RestoreSp(int amount)
        {
            Sp = Math.Min(MaxSp, Sp + amount);  // No puede superar el MaxSp
        }

        // Verifica si el jugador está muerto
        public bool IsDead()
        {
            return Hp == 0;
        }
    }
}
