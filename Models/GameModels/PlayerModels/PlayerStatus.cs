using System;

namespace Game.Models
{
    public class PlayerStatus
    {
        public int PlayerId { get; set; }
        public bool IsPoisoned { get; set; }
        public bool IsOnFire { get; set; }
        public bool IsFrozen { get; set; }
        public bool IsBanned { get; set; }

        // Relación con Player
        public Player Player { get; set; }

        // Activar veneno
        public void ActivatePoison()
        {
            if (IsPoisoned)
            {
                throw new InvalidOperationException("El jugador ya está envenenado.");
            }
            
            IsPoisoned = true;
        }

        // Desactivar veneno
        public void DeactivatePoison()
        {
            if (!IsPoisoned)
            {
                throw new InvalidOperationException("El jugador no está envenenado.");
            }
            
            IsPoisoned = false;
        }

        // Activar fuego
        public void ActivateFire()
        {
            if (IsOnFire)
            {
                throw new InvalidOperationException("El jugador ya está en fuego.");
            }
            
            IsOnFire = true;
        }

        // Desactivar fuego
        public void DeactivateFire()
        {
            if (!IsOnFire)
            {
                throw new InvalidOperationException("El jugador no está en fuego.");
            }
            
            IsOnFire = false;
        }

        // Activar congelación
        public void ActivateFreeze()
        {
            if (IsFrozen)
            {
                throw new InvalidOperationException("El jugador ya está congelado.");
            }
            
            IsFrozen = true;
        }

        // Desactivar congelación
        public void DeactivateFreeze()
        {
            if (!IsFrozen)
            {
                throw new InvalidOperationException("El jugador no está congelado.");
            }
            
            IsFrozen = false;
        }

        // Activar baneo
        public void ActivateBan()
        {
            if (IsBanned)
            {
                throw new InvalidOperationException("El jugador ya está baneado.");
            }
            
            IsBanned = true;
        }

        // Desactivar baneo
        public void DeactivateBan()
        {
            if (!IsBanned)
            {
                throw new InvalidOperationException("El jugador no está baneado.");
            }
            
            IsBanned = false;
        }

        // Verificar si el jugador está afectado por algún estado
        public bool IsAffectedByAnyState()
        {
            return IsPoisoned || IsOnFire || IsFrozen || IsBanned;
        }

        // Método que activa todos los efectos negativos en el jugador (veneno, fuego, congelación, baneo)
        public void ApplyNegativeEffects()
        {
            ActivatePoison();
            ActivateFire();
            ActivateFreeze();
            ActivateBan();
        }

        // Método para resetear el estado (útil para limpiar todos los efectos)
        public void ResetStatus()
        {
            IsPoisoned = false;
            IsOnFire = false;
            IsFrozen = false;
            IsBanned = false;
        }

        // Método para comprobar si el jugador está envenenado
        public bool IsPlayerPoisoned()
        {
            return IsPoisoned;
        }

        // Método para comprobar si el jugador está quemado
        public bool IsPlayerOnFire()
        {
            return IsOnFire;
        }

        // Método para comprobar si el jugador está congelado
        public bool IsPlayerFrozen()
        {
            return IsFrozen;
        }

        // Método para comprobar si el jugador está baneado
        public bool IsPlayerBanned()
        {
            return IsBanned;
        }
    }
}
