using Game.Models;
using VentusServer.Domain.Objects;
using Server.Logic.Validators;

namespace VentusServer.GameExecution
{
    public class ActionExecutor
    {
        private readonly ActionValidator _validator;

        public ActionExecutor(
            ActionValidator validator
         )
        {
            _validator = validator;
        }

        /// <summary>
        /// Ejecuta una acción contenida en la hotbar del jugador.
        /// </summary>
        public void Execute(PlayerObject player, HotbarAction hotbarAction)
        {
            if (player == null || hotbarAction == null)
                return;

            if (this._validator.CanPerformAction(player, hotbarAction))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.ActionExecutor, $"[ActionExecutor] No se puede realizar la acción.");
                return;
            }

            switch (hotbarAction.ActionType)
            {
                case HotbarActionType.Empty:
                    return;

                case HotbarActionType.CastSpell:
                    HandleTryToCastSpell(player, hotbarAction);
                    break;

                case HotbarActionType.UseItem:
                    HandleTryToUseItem(player, hotbarAction);
                    break;

                case HotbarActionType.Hit:
                    HandleTryToHit(player);
                    break;

                case HotbarActionType.Equip:
                    HandleTryToEquip(player, hotbarAction);
                    break;

                default:
                    LoggerUtil.Log(LoggerUtil.LogTag.ActionExecutor, $"[ActionExecutor] Acción desconocida: {hotbarAction.ActionType}");
                    break;
            }
        }

        private void HandleTryToCastSpell(PlayerObject player, HotbarAction hotbarAction)
        {
            var spellId = (string)hotbarAction.ActionId;
            var spell = player.GetSpellById(spellId);

            if (spell == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.ActionExecutor, $"[CastSpell] Hechizo '{spellId}' no encontrado.");
                return;
            }

            if (!_validator.CanCastSpell(player, spell))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.ActionExecutor, $"[CastSpell] No se puede lanzar el hechizo '{spellId}' en este momento.");
                return;
            }

            _spellService.PrepareSpellTargeting(player, spell);
        }

        private void HandleTryToUseItem(PlayerObject player, HotbarAction hotbarAction)
        {
            var itemId = (string)hotbarAction.ActionId;

            if (!_validator.CanUseItem(player, itemId))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.ActionExecutor, $"[UseItem] No se puede usar el ítem '{itemId}' ahora.");
                return;
            }

            _itemService.UseItem(player, itemId);
        }

        private void HandleTryToHit(PlayerObject player)
        {
            if (!_validator.CanHit(player))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.ActionExecutor, "[Hit] No puede atacar ahora.");
                return;
            }

            _combatService.TryAttack(player);
        }

        private void HandleTryToEquip(PlayerObject player, HotbarAction hotbarAction)
        {
            var itemId = (string)hotbarAction.ActionId;

            if (!_validator.CanEquipItem(player, itemId))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.ActionExecutor, $"[Equip] No se puede equipar el ítem '{itemId}'.");
                return;
            }

            _itemService.EquipItem(player, itemId);
        }
    }
}
