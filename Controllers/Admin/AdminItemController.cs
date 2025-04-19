using Microsoft.AspNetCore.Mvc;
using VentusServer.Auth;
using VentusServer.DataAccess.Mappers;
using VentusServer.Domain.Models;
using VentusServer.Models;
using VentusServer.Services;
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

    [HttpGet]
    public async Task<IActionResult> GetAllItems()
    {
        var items = await _itemService.GetAllItemsAsync();
        var itemDTOs = items.Select(MapToDTO).ToList();
        return Ok(itemDTOs);
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetItem(string key)
    {
        var item = await _itemService.GetItemByKeyAsync(key);
        if (item == null)
            return NotFound("Item no encontrado.");

        return Ok(MapToDTO(item));
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateItem([FromBody] ItemCreateDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Key) || string.IsNullOrWhiteSpace(dto.Name.Es))
            return BadRequest("Key y Name.Es son obligatorios.");

        var exists = await _itemService.GetItemByKeyAsync(dto.Key);
        if (exists != null)
            return Conflict("Ya existe un ítem con esa clave.");

        var item = new ItemModel
        {
            Key = dto.Key,
            Name = new TranslatedTextModel
            {
                Es = dto.Name.Es,
                En = dto.Name.En
            },
            Description = new TranslatedTextModel
            {
                Es = dto.Description.Es,
                En = dto.Description.En
            },
            Type = dto.Type,
            Rarity = dto.Rarity,
            Sound = dto.Sound,
            Damage = dto.Damage,
            Defense = dto.Defense,
            ManaBonus = dto.ManaBonus,
            StrengthBonus = dto.StrengthBonus,
            SpeedBonus = dto.SpeedBonus,
            MaxStack = dto.MaxStack,
            IconPath = dto.IconPath,
            Sprite = dto.Sprite ?? Array.Empty<int>(),
            IsTradable = dto.IsTradable,
            IsDroppable = dto.IsDroppable,
            IsUsable = dto.IsUsable,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _itemService.CreateItemAsync(item);
        return Ok(new { message = "Item creado exitosamente." });
    }

    [HttpPut("{key}/update")]
    public async Task<IActionResult> UpdateItem(string key, [FromBody] ItemUpdateDTO dto)
    {
        var item = await _itemService.GetItemByKeyAsync(key);
        if (item == null)
            return NotFound("Item no encontrado.");

        item.Name = dto.Name != null ? new TranslatedTextModel
        {
            Es = dto.Name.Es,
            En = dto.Name.En
        } : item.Name;
        item.Description = dto.Description != null ? new TranslatedTextModel
        {
            Es = dto.Description.Es,
            En = dto.Description.En
        } : item.Description;
        item.Type = dto.Type ?? item.Type;
        item.Rarity = dto.Rarity ?? item.Rarity;
        item.Sound = dto.Sound ?? item.Sound;
        item.Damage = dto.Damage ?? item.Damage;
        item.Defense = dto.Defense ?? item.Defense;
        item.ManaBonus = dto.ManaBonus ?? item.ManaBonus;
        item.StrengthBonus = dto.StrengthBonus ?? item.StrengthBonus;
        item.SpeedBonus = dto.SpeedBonus ?? item.SpeedBonus;
        item.MaxStack = dto.MaxStack ?? item.MaxStack;
        item.IconPath = dto.IconPath ?? item.IconPath;
        item.Sprite = dto.Sprite ?? item.Sprite;
        item.IsTradable = dto.IsTradable ?? item.IsTradable;
        item.IsDroppable = dto.IsDroppable ?? item.IsDroppable;
        item.IsUsable = dto.IsUsable ?? item.IsUsable;
        item.UpdatedAt = DateTime.UtcNow;

        await _itemService.UpdateItemAsync(item);
        return Ok("Item actualizado.");
    }

    [HttpDelete("{key}/delete")]
    public async Task<IActionResult> DeleteItem(string key)
    {
        var item = await _itemService.GetItemByKeyAsync(key);
        if (item == null)
            return NotFound("Item no encontrado.");

        await _itemService.DeleteItemAsync(item.Id);
        return Ok("Item eliminado.");
    }

    // Mapper auxiliar
    private static ItemDTO MapToDTO(ItemModel item)
    {
        return new ItemDTO
        {
            Id = item.Id,
            Key = item.Key,
            Name = new TranslatedTextDTO { Es = item.Name.Es, En = item.Name.En },
            Description = new TranslatedTextDTO { Es = item.Description.Es, En = item.Description.En },
            Type = item.Type,
            Rarity = item.Rarity,
            Sound = item.Sound,
            Damage = item.Damage,
            Defense = item.Defense,
            ManaBonus = item.ManaBonus,
            StrengthBonus = item.StrengthBonus,
            SpeedBonus = item.SpeedBonus,
            MaxStack = item.MaxStack,
            IconPath = item.IconPath,
            Sprite = item.Sprite,
            IsTradable = item.IsTradable,
            IsDroppable = item.IsDroppable,
            IsUsable = item.IsUsable,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}
