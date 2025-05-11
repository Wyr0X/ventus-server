namespace VentusServer.Domain.Objects
{
    public class ItemObject
    {
        /// <summary>
        /// Identificador único del ítem en el juego.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// El modelo del ítem, contiene los detalles del ítem.
        /// </summary>
        public ItemModel ItemModel { get; set; }

        /// <summary>
        /// Cantidad de este ítem en el inventario (en caso de ítems apilables).
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Durabilidad del ítem (si aplica, por ejemplo, para armas o armaduras).
        /// </summary>
        public float Durability { get; set; }

        /// <summary>
        /// Si el ítem está equipado por el jugador.
        /// </summary>
        public bool IsEquipped { get; set; }

        public CooldownManager CooldownManager = new CooldownManager();

        public ItemObject(ItemModel itemModel, int quantity = 1, float durability = 100f)
        {
            ItemModel = itemModel;
            Quantity = quantity;
            Durability = durability;
            IsEquipped = false; // Por defecto no está equipado
            Id = Guid.NewGuid().ToString(); // ID único generado para cada ítem en el inventario
        }

        /// <summary>
        /// Verificar si el ítem puede apilarse.
        /// </summary>
        public bool CanStack()
        {
            return ItemModel.IsStackable;
        }

        /// <summary>
        /// Usar el ítem (si es un ítem que se puede usar).
        /// </summary>
        public void Use(PlayerObject player)
        {
            if (ItemModel.Effects.Count > 0)
            {
                // Aplica los efectos del ítem al jugador
                foreach (var effect in ItemModel.Effects)
                {
                    if (effect.ActivationType == EffectActivationType.Active)
                    {
                        ApplyEffect(player, effect);
                    }
                }

                // Si el ítem tiene durabilidad, reducimos su durabilidad al usarlo
                if (Durability > 0)
                {
                    Durability -= 10f; // La cantidad de durabilidad reducida puede depender del uso
                    if (Durability <= 0)
                    {
                        // El ítem se rompe (puedes agregar más lógica aquí)
                        Destroy();
                    }
                }

                // Si el ítem es apilable, reducimos la cantidad si tiene más de 1.
                if (CanStack() && Quantity > 1)
                {
                    Quantity--;
                }
                else if (CanStack() && Quantity == 1)
                {
                    Destroy(); // El ítem se destruye si no se apila más
                }
            }
        }

        /// <summary>
        /// Aplicar un efecto al jugador.
        /// </summary>
        private void ApplyEffect(PlayerObject player, ItemEffect effect)
        {
            // switch (effect.Type)
            // {
            //     case EffectType.Heal:
            //         player.Stats.Health += (int)effect.Value;
            //         break;
            //     case EffectType.Damage:
            //         player.Stats.Health -= (int)effect.Value;
            //         break;
            //     case EffectType.SpeedBoost:
            //         player.Stats.Speed += (int)effect.Value;
            //         break;
            //         // Agregar más tipos de efectos según sea necesario
            // }
        }

        /// <summary>
        /// Equipar el ítem al jugador.
        /// </summary>
        public void Equip(PlayerObject player)
        {
            if (!IsEquipped)
            {
                IsEquipped = true;
                // Equipar el ítem en el jugador
                ApplyEquipEffects(player);
            }
        }

        /// <summary>
        /// Aplicar los efectos pasivos al jugador cuando el ítem es equipado.
        /// </summary>
        private void ApplyEquipEffects(PlayerObject player)
        {
            foreach (var effect in ItemModel.Effects)
            {
                if (effect.ActivationType == EffectActivationType.Passive)
                {
                    ApplyEffect(player, effect);
                }
            }
        }

        /// <summary>
        /// Destruir el ítem (cuando su durabilidad se acaba o cuando se elimina).
        /// </summary>
        public void Destroy()
        {
            // Aquí puedes agregar la lógica para destruir el ítem y eliminarlo del inventario
            Console.WriteLine($"El ítem {ItemModel.Name} ha sido destruido.");
        }

        /// <summary>
        /// Incrementar la cantidad de un ítem apilable.
        /// </summary>
        public void Stack(int quantity)
        {
            if (CanStack())
            {
                Quantity += quantity;
            }
        }
    }
}
