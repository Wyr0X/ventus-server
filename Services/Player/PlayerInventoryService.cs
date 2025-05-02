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

    public async Task SaveInventoryAsync(PlayerInventoryModel inventory)
    {
        if (inventory == null)
        {
            throw new ArgumentNullException(nameof(inventory), "El inventario no puede ser nulo.");
        }
        await _inventoryDAO.UpsertAsync(inventory);
    }
    public async Task<bool> MoveItemAsync(int playerId, int fromSlot, int toSlot)
    {
        var inventory = await _inventoryDAO.GetByPlayerId(playerId);

        if (inventory == null)
        {
            throw new InvalidOperationException("Inventario no encontrado.");
        }

        var itemToMove = inventory.Items.FirstOrDefault(i => i.Slot == fromSlot);

        if (itemToMove == null)
        {
            return false; // No hay ítem en el slot de origen
        }

        var destinationItem = inventory.Items.FirstOrDefault(i => i.Slot == toSlot);

        if (destinationItem != null)
        {
            // Hay un ítem en el destino → intercambio de posiciones
            int tempSlot = itemToMove.Slot;
            itemToMove.Slot = destinationItem.Slot;
            destinationItem.Slot = tempSlot;
        }
        else
        {
            // El slot de destino está vacío → mover directamente
            itemToMove.Slot = toSlot;
        }

        itemToMove.UpdatedAt = DateTime.Now;

        // Actualizar en la base de datos
        await _inventoryDAO.UpsertAsync(inventory);

        return true;
    }

}
