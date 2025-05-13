using System;
using System.Linq;
using VentusServer.Domain.Objects;

namespace Server.Logic.Validators
{
    public static class ActionValidator
    {
        /// <summary>
        /// Valida si el jugador puede realizar cualquier acción general.
        /// </summary>
        public static ValidationResult CanPerformGeneralAction(PlayerObject player)
        {
            if (player == null || player.Stats == null)
                return ValidationResult.Fail("Jugador no válido.");

            if (player.Stats.IsDead)
                return ValidationResult.Fail("No puedes realizar acciones mientras estás muerto.");

            if (player.Stats.IsStunned || player.Stats.IsRooted || player.Stats.IsChanneling)
                return ValidationResult.Fail("No puedes realizar acciones en este momento.");

            return ValidationResult.Success();
        }


        /// <summary>
        /// Valida si el jugador puede lanzar un hechizo.
        /// </summary>
        public static ValidationResult CanCastSpell(PlayerObject player, SpellObject spell)
        {
            var generalValidation = CanPerformGeneralAction(player);
            if (!generalValidation.IsValid)
                return generalValidation;

            if (spell == null)
                return ValidationResult.Fail("Hechizo no válido.");

            if (player.Stats.Mana < spell.Model.ManaCost)
                return ValidationResult.Fail("No tienes suficiente maná para lanzar este hechizo.");

            if (spell.IsOnCooldown)
                return ValidationResult.Fail("El hechizo está en cooldown.");

            return ValidationResult.Success();
        }

        /// <summary>
        /// Valida si el jugador puede usar un ítem.
        /// </summary>
        public static ValidationResult CanUseItem(PlayerObject player, string itemId)
        {
            // var generalValidation = CanPerformGeneralAction(player);
            // if (!generalValidation.IsValid)
            //     return generalValidation;

            // var item = player.Inventory.GetItem(itemId);
            // if (item == null || item.Quantity <= 0)
            //     return ValidationResult.Fail("El ítem no es válido o no tienes suficientes cargas.");

            return ValidationResult.Success();
        }

        /// <summary>
        /// Valida si el jugador puede equipar un ítem.
        /// </summary>
        public static ValidationResult CanEquipItem(PlayerObject player, string itemId)
        {
            // var generalValidation = CanPerformGeneralAction(player);
            // if (!generalValidation.IsValid)
            //     return generalValidation;

            // if (string.IsNullOrEmpty(itemId))
            //     return ValidationResult.Fail("El ID del ítem no es válido.");

            // var item = player.Inventory.GetItem(itemId);
            // if (item == null)
            //     return ValidationResult.Fail("El ítem no existe en el inventario.");

            // if (!item.ItemModel.IsEquippable)
            //     return ValidationResult.Fail("El ítem no es equipable.");

            // var slot = item.ItemModel.EquipLocation;
            // var currentlyEquipped = player.Inventory.GetEquippedItem(slot);
            // if (currentlyEquipped != null && !player.Inventory.CanReplaceEquipped(slot))
            //     return ValidationResult.Fail("No puedes reemplazar el ítem equipado en esta ranura.");

            // return ValidationResult.Success();
            return ValidationResult.Success();
        }

        /// <summary>
        /// Valida si el jugador puede aplicar un efecto de estado.
        /// </summary>
        public static ValidationResult CanApplyStatusEffect(PlayerObject player, string effectId)
        {
            if (string.IsNullOrEmpty(effectId))
                return ValidationResult.Success();

            if (player.Stats.ActiveEffects.Any(e => e.Value.Id == effectId))
                return ValidationResult.Fail("El efecto ya está activo en el jugador.");

            return ValidationResult.Success();
        }



    }
}
