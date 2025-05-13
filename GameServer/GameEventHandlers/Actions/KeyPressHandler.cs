using System;
using System.Threading.Tasks;
using Game.Models;
using Game.Server;
using Ventus.Network.Packets;
using VentusServer.Domain.Objects;

namespace VentusServer.GameEventHandlers
{
    public class KeyPressHandler
    {
        private readonly GameServer _gameServer;
        private readonly ActionExecutor _actionExecutor;

        public KeyPressHandler(GameServer gameServer, ActionExecutor actionExecutor, GameEventHandler gameEventHandler)
        {
            _gameServer = gameServer;
            _actionExecutor = actionExecutor;
            gameEventHandler.Subscribe(ClientPacket.KeyPressed, async (customGamEvent) => await HandleKeyPressAsync(customGamEvent));
        }

        public Task HandleKeyPressAsync(UserMessagePair userMessagePair)
        {
            var playerObject = GameEventHandler.GetPlayerIfPacketIs(userMessagePair, _gameServer, ClientPacket.KeyPressed);
            if (playerObject == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.KeyPressHandler, $"[KeyPressHandler] Player not found for account ID {userMessagePair.AccountId}.");
                return Task.CompletedTask;
            }

            var keyPressedPacket = userMessagePair.ClientMessage as KeyPressed;
            if (keyPressedPacket == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.KeyPressHandler, "Invalid event data type (expected KeyPressed)");
                return Task.CompletedTask;
            }

            var key = keyPressedPacket.Key;
            if (playerObject.PlayerModel.KeyBindings == null || key == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.KeyPressHandler, $"[KeyPressHandler] KeyBindings or key is null for player {playerObject.Id}.");
                return Task.CompletedTask;
            }

            var action = playerObject.PlayerModel.KeyBindings.GetByKey(key);
            if (action == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.KeyPressHandler, $"[KeyPressHandler] La tecla '{key}' no está asignada a ninguna acción para el jugador {playerObject.Id}.");
                return Task.CompletedTask;
            }

            LoggerUtil.Log(LoggerUtil.LogTag.KeyPressHandler, $"[KeyPressHandler] La tecla '{key}' está asignada a la acción '{action}' para el jugador {playerObject.Id}.");

            if (action.KeyType == KeyType.Hotbar)
            {
                if (int.TryParse(action.Action.Replace("hotbar_slot_", ""), out var hotbarIndex))
                {
                    if (hotbarIndex < 0 || hotbarIndex >= 11)
                    {
                        LoggerUtil.Log(LoggerUtil.LogTag.KeyPressHandler, $"[KeyPressHandler] Índice de hotbar inválido: {hotbarIndex}.");
                        return Task.CompletedTask;
                    }

                    var hotbarAction = playerObject.PlayerModel.Hotbar.GetBySlot(hotbarIndex);
                    if (hotbarAction == null)
                    {
                        LoggerUtil.Log(LoggerUtil.LogTag.KeyPressHandler, $"[KeyPressHandler] No hay acción asignada en la hotbar {hotbarIndex} para el jugador {playerObject.Id}.");
                        return Task.CompletedTask;
                    }

                    LoggerUtil.Log(LoggerUtil.LogTag.KeyPressHandler, $"[KeyPressHandler] Ejecutando acción '{hotbarAction.ActionId}' desde la hotbar {hotbarIndex} para el jugador {playerObject.Id}.");
                    _actionExecutor.TryToExecuteAction(hotbarAction, playerObject);
                }
                else
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.KeyPressHandler, $"[KeyPressHandler] No se pudo determinar el índice de la hotbar para la acción '{action}'.");
                }
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.KeyPressHandler, $"[KeyPressHandler] Ejecutando acción '{action}' para el jugador {playerObject.Id}.");
                ExecuteAction(playerObject, action);
            }

            return Task.CompletedTask;
        }

        private void ExecuteAction(PlayerObject playerObject, UserKeyAction action)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.KeyPressHandler, $"[KeyPressHandler] Ejecutando acción general: {action} para el jugador {playerObject.Id}.");
        }
    }
}
