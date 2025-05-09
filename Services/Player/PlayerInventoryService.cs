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
        var now = TimeProvider.UtcNow(); // Almacenar TimeProvider.now() en una variable local

        PlayerInventoryModel inventoryModel = new PlayerInventoryModel
        {
            CreatedAt = now,
            Gold = 0,
            Items = [],
            PlayerId = playerModel.Id,
            MaxSlots = 22,
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
        await _inventoryDAO.DeleteByPlayerId(playerId); ;
    }

    public async Task<bool> BuyItemAsync(PlayerModel player, ItemModel itemToBuy)
    {
        if (itemToBuy.Price <= 0)
            throw new InvalidOperationException("Este ítem no se puede comprar.");

        var inventory = await GetInventoryByPlayerId(player.Id).ConfigureAwait(false);
        if (inventory == null)
            throw new InvalidOperationException("Inventario no encontrado.");

        if (inventory.Gold < itemToBuy.Price)
            return false;

        var now = TimeProvider.UtcNow(); // Almacenar TimeProvider.now() en una variable local

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

            existingItem.UpdatedAt = now;
            await _inventoryDAO.UpsertAsync(inventory).ConfigureAwait(false);
        }
        else
        {
            int nextSlot = inventory.Items.Count;

            var newItem = new PlayerInventoryItemModel
            {
                ItemId = itemToBuy.Id,
                Quantity = 1,
                Slot = nextSlot,
                isEquipped = false,
                CreatedAt = now,
                UpdatedAt = now,
                Name = itemToBuy.Name.Es,
                Icon = itemToBuy.IconPath,
            };

            inventory.Items.Add(newItem);
            await _inventoryDAO.UpsertAsync(inventory).ConfigureAwait(false);
        }

        inventory.Gold -= itemToBuy.Price;
        inventory.UpdatedAt = now;
        await _inventoryDAO.UpdateGold(inventory.PlayerId, inventory.Gold).ConfigureAwait(false);

        return true;
    }

    public async Task SaveInventoryAsync(PlayerInventoryModel inventory)
    {
        if (inventory == null)
        {
            throw new ArgumentNullException(nameof(inventory), "El inventario no puede ser nulo.");
        }
        await _inventoryDAO.UpsertAsync(inventory).ConfigureAwait(false);
    }

    public async Task<bool> MoveItemAsync(int playerId, int fromSlot, int toSlot)
    {
        var inventory = await _inventoryDAO.GetByPlayerId(playerId).ConfigureAwait(false);

        if (inventory == null)
        {
            throw new InvalidOperationException("Inventario no encontrado.");
        }

        var itemToMove = inventory.Items.FirstOrDefault(i => i.Slot == fromSlot);

        if (itemToMove == null)
        {
            return false;
        }

        var destinationItem = inventory.Items.FirstOrDefault(i => i.Slot == toSlot);

        var now = TimeProvider.UtcNow(); // Almacenar TimeProvider.now() en una variable local

        if (destinationItem != null)
        {
            int tempSlot = itemToMove.Slot;
            itemToMove.Slot = destinationItem.Slot;
            destinationItem.Slot = tempSlot;
        }
        else
        {
            itemToMove.Slot = toSlot;
        }

        itemToMove.UpdatedAt = now;

        await _inventoryDAO.UpsertAsync(inventory).ConfigureAwait(false);

        return true;
    }
}
