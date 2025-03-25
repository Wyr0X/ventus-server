using System;

namespace Game.Models
{
    public class PlayerEconomy
    {
        public int PlayerId { get; set; }
        public decimal Gold { get; set; }
        public decimal BankGold { get; set; }
        public int Hunger { get; set; }
        public int Thirst { get; set; }

        // Relación con Player
        public Player Player { get; set; }

        // Método para añadir oro
        public void AddGold(decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("La cantidad de oro a añadir debe ser positiva.");
            }
            
            Gold += amount;
        }

        // Método para restar oro
        public void SubtractGold(decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("La cantidad de oro a restar debe ser positiva.");
            }
            
            if (Gold < amount)
            {
                throw new InvalidOperationException("No tienes suficiente oro.");
            }

            Gold -= amount;
        }

        // Método para transferir oro entre el oro disponible y el banco
        public void TransferGoldToBank(decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("La cantidad de oro a transferir debe ser positiva.");
            }

            if (Gold < amount)
            {
                throw new InvalidOperationException("No tienes suficiente oro en tu inventario.");
            }

            Gold -= amount;
            BankGold += amount;
        }

        public void TransferGoldFromBank(decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("La cantidad de oro a transferir debe ser positiva.");
            }

            if (BankGold < amount)
            {
                throw new InvalidOperationException("No tienes suficiente oro en el banco.");
            }

            BankGold -= amount;
            Gold += amount;
        }

        // Método para aumentar hambre
        public void IncreaseHunger(int amount)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("La cantidad de hambre a incrementar debe ser positiva.");
            }

            Hunger += amount;

            // Limitar el hambre a un máximo
            if (Hunger > 100)
            {
                Hunger = 100;
            }
        }

        // Método para disminuir hambre
        public void DecreaseHunger(int amount)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("La cantidad de hambre a disminuir debe ser positiva.");
            }

            Hunger -= amount;

            // No permitir que el hambre sea negativa
            if (Hunger < 0)
            {
                Hunger = 0;
            }
        }

        // Método para aumentar sed
        public void IncreaseThirst(int amount)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("La cantidad de sed a incrementar debe ser positiva.");
            }

            Thirst += amount;

            // Limitar la sed a un máximo
            if (Thirst > 100)
            {
                Thirst = 100;
            }
        }

        // Método para disminuir sed
        public void DecreaseThirst(int amount)
        {
            if (amount <= 0)
            {
                throw new InvalidOperationException("La cantidad de sed a disminuir debe ser positiva.");
            }

            Thirst -= amount;

            // No permitir que la sed sea negativa
            if (Thirst < 0)
            {
                Thirst = 0;
            }
        }

        // Método para aumentar o disminuir hambre y sed por el tiempo
        public void UpdateNeeds(int hungerIncrease, int thirstIncrease)
        {
            IncreaseHunger(hungerIncrease);
            IncreaseThirst(thirstIncrease);
        }

        // Método para regenerar automáticamente hambre y sed (por ejemplo, por cada hora de juego)
        public void AutoRegenerateNeeds()
        {
            // Por cada "regeneración", incrementar la sed y hambre en 5 puntos
            IncreaseHunger(5);
            IncreaseThirst(5);
        }

        // Método para determinar si el jugador tiene suficiente comida y bebida para continuar
        public bool CanContinue()
        {
            return Hunger < 100 && Thirst < 100;
        }

        // Método para resetear la economía del jugador (por ejemplo, al reiniciar o al inicio del juego)
        public void ResetEconomy()
        {
            Gold = 0;
            BankGold = 0;
            Hunger = 0;
            Thirst = 0;
        }
    }
}
