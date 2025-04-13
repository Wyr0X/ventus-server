using Microsoft.AspNetCore.Mvc;
using VentusServer.Auth;
using VentusServer.Domain.Models;
using VentusServer.Models;
using VentusServer.Services;

namespace VentusServer.Controllers.Admin
{
    [ApiController]
    [Route("admin/items")]
    [JwtAuthRequired]
    [RequirePermission]
    public class AdminItemController : ControllerBase
    {
        private readonly ItemService _itemService;

        public AdminItemController(ItemService itemService)
        {
            _itemService = itemService;
        }

        // Obtener todos los items
        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _itemService.GetAllItemsAsync();
            var itemDTOs = new List<ItemDTO>();

            foreach (var item in items)
            {
                itemDTOs.Add(new ItemDTO
                {
                    Id = item.Id,
                    Key = item.Key,
                    Name = item.Name,
                    Description = item.Description,
                    HpMin = item.HpMin,
                    HpMax = item.HpMax,
                    MP = item.MP,
                    Sprite = item.Sprite,
                    Sound = item.Sound,
                    CreatedAt = item.CreatedAt,
                });
            }

            return Ok(itemDTOs);
        }

        // Obtener un item por Key
        [HttpGet("{key}")]
        public async Task<IActionResult> GetItem(string key)
        {
            var item = await _itemService.GetItemByKeyAsync(key);
            if (item == null)
                return NotFound("Item no encontrado.");

            var itemDTO = new ItemDTO
            {
                Id = item.Id,
                Key = item.Key,
                Name = item.Name,
                Description = item.Description,
                HpMin = item.HpMin,
                HpMax = item.HpMax,
                MP = item.MP,
                Sprite = item.Sprite,
                Sound = item.Sound,
                CreatedAt = item.CreatedAt,
            };

            return Ok(itemDTO);
        }

        // Eliminar un item
        [HttpDelete("{key}/delete")]
        public async Task<IActionResult> DeleteItem(string key)
        {
            var item = await _itemService.GetItemByKeyAsync(key);
            if (item == null)
                return NotFound("Item no encontrado.");

            await _itemService.DeleteItemAsync(item.Id);
            return Ok("Item eliminado.");
        }

        // Actualizar un item
        [HttpPut("{key}/update")]
        public async Task<IActionResult> UpdateItem(string key, [FromBody] ItemUpdateDTO updateRequest)
        {
            var item = await _itemService.GetItemByKeyAsync(key);
            if (item == null)
                return NotFound("Item no encontrado.");

            // Actualizar los campos proporcionados en el DTO
            if (!string.IsNullOrEmpty(updateRequest.Name))
                item.Name = updateRequest.Name;

            if (!string.IsNullOrEmpty(updateRequest.Description))
                item.Description = updateRequest.Description;

            if (updateRequest.HpMin.HasValue)
                item.HpMin = updateRequest.HpMin.Value;

            if (updateRequest.HpMax.HasValue)
                item.HpMax = updateRequest.HpMax.Value;

            if (updateRequest.MP.HasValue)
                item.MP = updateRequest.MP.Value;

            if (updateRequest.Sprite != null)
                item.Sprite = updateRequest.Sprite;

            if (!string.IsNullOrEmpty(updateRequest.Sound))
                item.Sound = updateRequest.Sound;

            await _itemService.UpdateItemAsync(item);

            return Ok("Item actualizado.");
        }
        // Crear nuevo item
        // Crear nuevo item
        [HttpPost("create")]
        public async Task<IActionResult> CreateItem([FromBody] ItemCreateDTO createRequest)
        {
            // Validación mínima
            if (string.IsNullOrWhiteSpace(createRequest.Key) || string.IsNullOrWhiteSpace(createRequest.Name))
                return BadRequest("Key y Name son obligatorios.");

            // Verificar si ya existe un ítem con la misma Key
            var existingItem = await _itemService.GetItemByKeyAsync(createRequest.Key);
            if (existingItem != null)
            {
                return Conflict("Ya existe un ítem con la misma clave.");
            }

            var newItem = new ItemModel
            {
                Key = createRequest.Key,
                Name = createRequest.Name,
                Description = createRequest.Description,
                HpMin = createRequest.HpMin,
                HpMax = createRequest.HpMax,
                MP = createRequest.MP,
                Sprite = createRequest.Sprite ?? Array.Empty<int>(),
                Sound = createRequest.Sound,
                CreatedAt = DateTime.UtcNow,
            };

            await _itemService.CreateItemAsync(newItem);

            return Ok(new { message = "Item creado exitosamente." });
        }


    }
}
