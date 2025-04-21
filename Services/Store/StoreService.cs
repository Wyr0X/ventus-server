using System;
using System.Threading.Tasks;
using VentusServer.DataAccess.DAO;
using VentusServer.DataAccess.Interfaces;
using VentusServer.Domain.Models;

namespace VentusServer.Services
{
    public class StoreService
    {
        private readonly IItemDAO _itemDAO;
        private readonly IPlayerInventoryDAO _inventoryDAO;
        private readonly IPlayerInventoryDAO _inventoryItemDAO;
        private readonly IPlayerDAO _playerDAO;

        public StoreService(
            IItemDAO itemDAO,
            IPlayerInventoryDAO inventoryDAO,
            IPlayerInventoryDAO inventoryItemDAO,
            IPlayerDAO playerDAO)
        {
            _itemDAO = itemDAO;
            _inventoryDAO = inventoryDAO;
            _inventoryItemDAO = inventoryItemDAO;
            _playerDAO = playerDAO;
        }

        public async Task<BuyItemResult> BuyItemAsync(PlayerModel player, int itemId, int quantity)
        {
            // Validar cantidad
            if (quantity <= 0)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.StoreService, "Cantidad inválida en compra.", "BuyItemAsync", true);
                return BuyItemResult.FailBuild("Cantidad inválida.");
            }

            // Obtener el ítem
            var item = await _itemDAO.GetItemByIdAsync(itemId);
            if (item == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Ítem no encontrado. ItemID: {itemId}", "BuyItemAsync", true);
                return BuyItemResult.FailBuild("Ítem no encontrado.");
            }

            // Obtener inventario del jugador
            var inventory = await _inventoryDAO.GetByPlayerId(player.Id);
            if (inventory == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Inventario no encontrado para el jugador {player.Id}", "BuyItemAsync", true);
                return BuyItemResult.FailBuild("Inventario no encontrado.");
            }

            // Calcular costo total
            var totalCost = item.Price * quantity;
            if (inventory.Gold < totalCost)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Oro insuficiente. Necesitado: {totalCost}, Disponible: {inventory.Gold}", "BuyItemAsync", true);
                return BuyItemResult.FailBuild("Oro insuficiente.");
            }

            // Descontar oro
            var newGold = inventory.Gold - totalCost;
            inventory.Gold = inventory.Gold - totalCost;

            await _inventoryDAO.UpdateGold(player.Id, newGold);
            LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Oro descontado. Nuevo saldo: {newGold} ", "BuyItemAsync");

            // Agregar ítem
            var existingStack = inventory.Items.Find((item) => item.ItemId == itemId);

            if (existingStack != null)
            {
                if (item.MaxStack != null && (existingStack.Quantity + quantity) < item.MaxStack)
                {
                    existingStack.Quantity = existingStack.Quantity + quantity;
                    await _inventoryItemDAO.UpdateGold(inventory.Id, inventory.Gold);
                    LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"Ítem apilado. Cantidad nueva: {existingStack.Quantity}. ItemID: {itemId}", "BuyItemAsync");
                }
                else
                {
                    return BuyItemResult.FailBuild("No puedes comprar mas de este item.");


                }
            }
            else
            {
                var newItem = new PlayerInventoryItemModel
                {
                    ItemId = item.Id,
                    Quantity = quantity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                inventory.Items.Add(newItem);
                Console.WriteLine(inventory.Items.Count);
                LoggerUtil.Log(LoggerUtil.LogTag.StoreService, $"ítem agregado. ItemID: {itemId}, Cantidad: {quantity}", "BuyItemAsync");
            }
            await _inventoryItemDAO.UpsertAsync(inventory);

            LoggerUtil.Log(LoggerUtil.LogTag.StoreService, "Compra realizada correctamente.", "BuyItemAsync");
            return BuyItemResult.SuccessBuild("Compra realizada correctamente.");
        }
    }
}

public class BuyItemResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }

    public static BuyItemResult FailBuild(string message) => new BuyItemResult { Success = false, ErrorMessage = message };
    public static BuyItemResult SuccessBuild(string message) => new BuyItemResult { Success = true, ErrorMessage = null };
}
