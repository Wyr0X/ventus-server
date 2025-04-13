using System;
using System.IO;
using System.Threading.Tasks;
using VentusServer.Services;

namespace VentusServer.Seeding
{
    public class ItemSeeder
    {
        private readonly ItemService _itemService;

        public ItemSeeder(ItemService itemService)
        {
            _itemService = itemService;
        }

        public async Task SeedFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"[ItemSeeder] JSON file not found: {filePath}");
                return;
            }

            try
            {
                var jsonContent = await File.ReadAllTextAsync(filePath);
                await _itemService.CreateMultipleItemsFromJsonAsync(jsonContent);
                Console.WriteLine("[ItemSeeder] Items seeding completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ItemSeeder] Error while seeding items: {ex.Message}");
            }
        }
    }
}
