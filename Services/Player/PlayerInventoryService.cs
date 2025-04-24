using VentusServer.DataAccess.DAO;
using VentusServer.Domain.Models;

public class PlayerInventoryService
{
    private readonly IPlayerInventoryDAO _inventoryDAO;

    public PlayerInventoryService(IPlayerInventoryDAO inventoryDAO)
    {
        _inventoryDAO = inventoryDAO;
    }

    public async Task<PlayerInventoryModel?> GetInventoryByPlayerId(int playerId)
    {
        var inventory = await _inventoryDAO.GetByPlayerId(playerId);
        return inventory;
    }

    public async Task<PlayerInventoryModel?> LoadPlayerInventoryInModule(PlayerModel playerModel)
    {
        var inventory = await _inventoryDAO.GetByPlayerId(playerModel.Id);

        if (inventory != null)
        {
            playerModel.Inventory = inventory;
        }

        return inventory;
    }

    public async Task CreateInventory(PlayerInventoryModel inventoryModel)
    {
        await _inventoryDAO.UpsertAsync(inventoryModel);
    }

    public async Task<PlayerInventoryModel> CreateDefaultInventory(PlayerModel playerModel)
    {
        PlayerInventoryModel inventoryModel = new PlayerInventoryModel
        {
            CreatedAt = DateTime.Now,
            Gold = 0,
            Items = [],
            PlayerId = playerModel.Id
        };

        await CreateInventory(inventoryModel);
        return inventoryModel;
    }

    public async Task UpdateGold(int playerId, int gold)
    {
        await _inventoryDAO.UpdateGold(playerId, gold);
    }

    public async Task DeleteInventoryByPlayerId(int playerId)
    {
        await _inventoryDAO.DeleteByPlayerId(playerId);
    }

    public async Task<bool> BuyItemAsync(PlayerModel player, ItemModel itemToBuy)
    {
        // Verificar que el ítem sea comprable
        if (itemToBuy.Price <= 0)
            throw new InvalidOperationException("Este ítem no se puede comprar.");

        var inventory = await GetInventoryByPlayerId(player.Id);
        if (inventory == null)
            throw new InvalidOperationException("Inventario no encontrado.");

        if (inventory.Gold < itemToBuy.Price)
            return false; // No tiene suficiente oro

        // Verificar si ya tiene el ítem (si es stackeable)
        var existingItem = inventory.Items.FirstOrDefault(i => i.ItemId == itemToBuy.Id);

        if (existingItem != null)
        {
            if (itemToBuy.MaxStack != null && existingItem.Quantity + 1 <= itemToBuy.MaxStack)
            {
                existingItem.Quantity += 1;
            }
            else if (itemToBuy.MaxStack == null)
            {
                existingItem.Quantity += 1;
            }
            else
            {
                return false;
            }

            existingItem.UpdatedAt = DateTime.Now;
            await _inventoryDAO.UpsertAsync(inventory);
        }
        else
        {
            // Buscar slot libre (esto podría ser manejado por la base de datos)
            int nextSlot = inventory.Items.Count;

            var newItem = new PlayerInventoryItemModel
            {
                ItemId = itemToBuy.Id,
                Quantity = 1,
                Slot = nextSlot,
                isEquipped = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Name = itemToBuy.Name.Es,
                Icon = itemToBuy.IconPath,
            };

            inventory.Items.Add(newItem);
            await _inventoryDAO.UpsertAsync(inventory);
        }

        // Restar el oro
        inventory.Gold -= itemToBuy.Price;
        inventory.UpdatedAt = DateTime.Now;
        await _inventoryDAO.UpdateGold(inventory.PlayerId, inventory.Gold);

        return true;
    }

    public async Task UpdateInventory(PlayerInventoryModel inventory)
    {
        if (inventory == null)
        {
            throw new ArgumentNullException(nameof(inventory), "El inventario no puede ser nulo.");
        }
        await _inventoryDAO.UpsertAsync(inventory);
    }
}
