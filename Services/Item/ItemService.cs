using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.DataAccess.Interfaces;
using VentusServer.Domain.Models;

namespace VentusServer.Services
{
    public class ItemService
    {
        private readonly IItemDAO _itemDAO;

        public ItemService(IItemDAO itemDAO)
        {
            _itemDAO = itemDAO;
        }

        public Task<ItemModel?> GetItemByIdAsync(int id)
        {
            return _itemDAO.GetItemByIdAsync(id);
        }

        public Task<ItemModel?> GetItemByKeyAsync(string key)
        {
            return _itemDAO.GetItemByKeyAsync(key);
        }

        public Task<IEnumerable<ItemModel>> GetAllItemsAsync()
        {
            return _itemDAO.GetAllItemsAsync();
        }

        public Task CreateItemAsync(ItemModel item)
        {
            return _itemDAO.CreateItemAsync(item);
        }

        public Task UpdateItemAsync(ItemModel item)
        {
            return _itemDAO.UpdateItemAsync(item);
        }

        public Task DeleteItemAsync(int id)
        {
            return _itemDAO.DeleteItemAsync(id);
        }

        public async Task CreateMultipleItemsFromJsonAsync(string json)
        {
            var parsedItems = ItemJsonParser.ParseItemsFromJson(json);

            foreach (var item in parsedItems)
            {
                var existingItem = await _itemDAO.GetItemByKeyAsync(item.Key ?? string.Empty);
                if (existingItem == null)
                {
                    await _itemDAO.CreateItemAsync(item);
                }
                else
                {
                    await _itemDAO.UpdateItemAsync(item); // o ignorar, según lo que prefieras
                }
            }
        }
    }
}
