using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.Auth;
using VentusServer.Domain.Models;
using VentusServer.Services;

namespace VentusServer.Controllers
{
    [JwtAuthRequired]
    [ApiController]

    [Route("items")]
    public class ItemController : ControllerBase
    {
        private readonly ItemService _itemService;

        public ItemController(ItemService itemService)
        {
            _itemService = itemService;
        }
        [JwtAuthRequired]

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemModel>>> GetAllItems()
        {
            var items = await _itemService.GetAllItemsAsync();
            return Ok(items);
        }
        [JwtAuthRequired]

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ItemModel>> GetItemById(int id)
        {
            var item = await _itemService.GetItemByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [JwtAuthRequired]
        [HttpGet("key/{key}")]
        public async Task<ActionResult<ItemModel>> GetItemByKey(string key)
        {
            var item = await _itemService.GetItemByKeyAsync(key);
            if (item == null)
                return NotFound();

            return Ok(item);
        }
        [JwtAuthRequired]

        [HttpPost]
        public async Task<ActionResult> CreateItem([FromBody] ItemModel item)
        {
            await _itemService.CreateItemAsync(item);
            return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
        }

        [JwtAuthRequired]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateItem(int id, [FromBody] ItemModel item)
        {
            if (id != item.Id)
                return BadRequest("El ID del path no coincide con el del objeto.");

            await _itemService.UpdateItemAsync(item);
            return NoContent();
        }
        [JwtAuthRequired]

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteItem(int id)
        {
            await _itemService.DeleteItemAsync(id);
            return NoContent();
        }
    }
}
