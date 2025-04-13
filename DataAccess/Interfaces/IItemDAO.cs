using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.Domain.Models;

namespace VentusServer.DataAccess.Interfaces
{
    public interface IItemDAO
    {
        Task<ItemModel?> GetItemByIdAsync(int id);
        Task<ItemModel?> GetItemByKeyAsync(string key);
        Task<IEnumerable<ItemModel>> GetAllItemsAsync();

        Task CreateItemAsync(ItemModel item);
        Task UpdateItemAsync(ItemModel item);
        Task DeleteItemAsync(int id);
    }
}
