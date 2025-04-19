using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.Domain.Models;

namespace VentusServer.DataAccess.DAO
{
    public interface IPlayerInventoryItemDAO
    {
        Task<List<PlayerInventoryItemModel>> GetAllByInventoryId(Guid inventoryId);
        Task<PlayerInventoryItemModel?> GetById(Guid id);
        Task Insert(PlayerInventoryItemModel item);
        Task Update(PlayerInventoryItemModel item);
        Task Delete(Guid id);
    }
}
