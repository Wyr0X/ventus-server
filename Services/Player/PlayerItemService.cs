using VentusServer.DataAccess.DAO;
using VentusServer.Domain.Models;

public class PlayerItemService
{
    private readonly IPlayerInventoryItemDAO _inventoryItemDAO;

    public PlayerItemService(IPlayerInventoryItemDAO inventoryItemDAO)
    {
        _inventoryItemDAO = inventoryItemDAO;
    }

    public async Task<List<PlayerInventoryItemModel>> GetItemsByInventoryId(Guid inventoryId)
    {
        return await _inventoryItemDAO.GetAllByInventoryId(inventoryId);
    }

    public async Task AddItemToInventory(PlayerInventoryItemModel itemModel)
    {
        await _inventoryItemDAO.Insert(itemModel);
    }

    public async Task UpdateItem(PlayerInventoryItemModel itemModel)
    {
        await _inventoryItemDAO.Update(itemModel);
    }

    public async Task RemoveItem(Guid itemId)
    {
        await _inventoryItemDAO.Delete(itemId);
    }
}
