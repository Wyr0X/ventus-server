using VentusServer.DataAccess.DAO;
using VentusServer.Domain.Models;

public class PlayerInventoryService
{
    private readonly IPlayerInventoryDAO _inventoryDAO;
    private readonly IPlayerInventoryItemDAO _inventoryItemDAO;

    public PlayerInventoryService(IPlayerInventoryDAO inventoryDAO, IPlayerInventoryItemDAO inventoryItemDAO)
    {
        _inventoryDAO = inventoryDAO;
        _inventoryItemDAO = inventoryItemDAO;
    }

    public async Task<PlayerInventoryModel> GetInventoryByPlayerId(int playerId)
    {
        var inventory = await _inventoryDAO.GetByPlayerId(playerId);
        if (inventory != null)
        {
            inventory.Items = await _inventoryItemDAO.GetAllByInventoryId(inventory.Id); // Cargar los items
        }
        return inventory;
    }

    public async Task CreateInventory(PlayerInventoryModel inventoryModel)
    {
        await _inventoryDAO.Insert(inventoryModel);
    }

    public async Task UpdateGold(int playerId, int gold, DateTime updatedAt)
    {
        await _inventoryDAO.UpdateGold(playerId, gold, updatedAt);
    }

    public async Task DeleteInventoryByPlayerId(int playerId)
    {
        await _inventoryDAO.DeleteByPlayerId(playerId);
    }
}
