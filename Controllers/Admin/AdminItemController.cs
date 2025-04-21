// -----------------------------
// Controllers/AdminItemController.cs
// -----------------------------
using Microsoft.AspNetCore.Mvc;
using VentusServer.Auth;
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
        var dtos = items.Select(MapToDTO).ToList();
        return Ok(dtos);
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetItem(string key)
    {
        var item = await _itemService.GetItemByKeyAsync(key);
        if (item == null) return NotFound("Item no encontrado.");
        return Ok(MapToDTO(item));
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateItem([FromBody] ItemCreateDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Key) || string.IsNullOrWhiteSpace(dto.Name?.Es))
            return BadRequest("Key y Name.Es son obligatorios.");

        if (await _itemService.GetItemByKeyAsync(dto.Key) != null)
            return Conflict("Ya existe un ítem con esa clave.");

        var item = new ItemModel
        {
            Key = dto.Key,
            Name = new TranslatedTextModel { Es = dto.Name.Es, En = dto.Name.En },
            Description = new TranslatedTextModel { Es = dto.Description.Es, En = dto.Description.En },
            Type = dto.Type,
            Rarity = dto.Rarity,
            MaxStack = dto.MaxStack,
            RequiredLevel = dto.RequiredLevel,
            Price = dto.Price ?? 0,
            Quantity = dto.Quantity,
            IsTradable = dto.IsTradeable,
            IsDroppable = dto.IsDroppable,
            IsUsable = dto.IsUsable,
            WeaponData = dto.WeaponData == null ? null : new WeaponStats
            {
                WeaponType = dto.WeaponData.WeaponType,
                MinDamage = dto.WeaponData.MinDamage,
                MaxDamage = dto.WeaponData.MaxDamage,
                AttackSpeed = dto.WeaponData.AttackSpeed,
                Range = dto.WeaponData.Range,
                IsTwoHanded = dto.WeaponData.IsTwoHanded,
                ManaCost = dto.WeaponData.ManaCost
            },
            ArmorData = dto.ArmorData == null ? null : new ArmorStats
            {
                Slot = dto.ArmorData.Slot,
                Defense = dto.ArmorData.Defense,
                MagicResistance = dto.ArmorData.MagicResistance,
                Durability = dto.ArmorData.Durability
            },
            ConsumableData = dto.ConsumableData == null ? null : new ConsumableEffect
            {
                Type = dto.ConsumableData.Type,
                Amount = dto.ConsumableData.Amount,
                Duration = dto.ConsumableData.Duration,
                EffectName = dto.ConsumableData.EffectName
            },
            Sprite = dto.Sprite,
            Sound = dto.Sound,
            IconPath = dto.IconPath,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _itemService.CreateItemAsync(item);
        return Ok(new { message = "Item creado exitosamente." });
    }

    [HttpPut("{key}/update")]
    public async Task<IActionResult> UpdateItem(string key, [FromBody] ItemUpdateDTO dto)
    {
        var item = await _itemService.GetItemByKeyAsync(key);
        if (item == null) return NotFound("Item no encontrado.");

        // Solo actualiza si el DTO trae valor no nulo
        if (dto.Name != null)
            item.Name = new TranslatedTextModel { Es = dto.Name.Es, En = dto.Name.En };
        if (dto.Description != null)
            item.Description = new TranslatedTextModel { Es = dto.Description.Es, En = dto.Description.En };
        if (dto.Type.HasValue) item.Type = dto.Type.Value;
        if (dto.Rarity.HasValue) item.Rarity = dto.Rarity.Value;
        if (dto.MaxStack.HasValue) item.MaxStack = dto.MaxStack;
        if (dto.RequiredLevel.HasValue) item.RequiredLevel = dto.RequiredLevel;
        if (dto.Price.HasValue) item.Price = dto.Price ?? 0;
        if (dto.Quantity.HasValue) item.Quantity = dto.Quantity;
        if (dto.IsTradable.HasValue) item.IsTradable = dto.IsTradable.Value;
        if (dto.IsDroppable.HasValue) item.IsDroppable = dto.IsDroppable.Value;
        if (dto.IsUsable.HasValue) item.IsUsable = dto.IsUsable.Value;
        if (dto.WeaponData != null)
            item.WeaponData = new WeaponStats
            {
                WeaponType = dto.WeaponData.WeaponType,
                MinDamage = dto.WeaponData.MinDamage,
                MaxDamage = dto.WeaponData.MaxDamage,
                AttackSpeed = dto.WeaponData.AttackSpeed,
                Range = dto.WeaponData.Range,
                IsTwoHanded = dto.WeaponData.IsTwoHanded,
                ManaCost = dto.WeaponData.ManaCost
            };
        if (dto.ArmorData != null)
            item.ArmorData = new ArmorStats
            {
                Slot = dto.ArmorData.Slot,
                Defense = dto.ArmorData.Defense,
                MagicResistance = dto.ArmorData.MagicResistance,
                Durability = dto.ArmorData.Durability
            };
        if (dto.ConsumableData != null)
            item.ConsumableData = new ConsumableEffect
            {
                Type = dto.ConsumableData.Type,
                Amount = dto.ConsumableData.Amount,
                Duration = dto.ConsumableData.Duration,
                EffectName = dto.ConsumableData.EffectName
            };
        if (dto.Sprite != null) item.Sprite = dto.Sprite;
        if (dto.Sound != null) item.Sound = dto.Sound;
        if (dto.IconPath != null) item.IconPath = dto.IconPath;

        item.UpdatedAt = DateTime.UtcNow;
        await _itemService.UpdateItemAsync(item);
        return Ok("Item actualizado.");
    }

    [HttpDelete("{key}/delete")]
    public async Task<IActionResult> DeleteItem(string key)
    {
        var item = await _itemService.GetItemByKeyAsync(key);
        if (item == null) return NotFound("Item no encontrado.");

        await _itemService.DeleteItemAsync(item.Id);
        return Ok("Item eliminado.");
    }

    private static ItemDTO MapToDTO(ItemModel item)
    {
        return new ItemDTO
        {
            Id = item.Id,
            Key = item.Key,
            Name = new VentusServer.Models.TranslatedTextDTO { Es = item.Name.Es, En = item.Name.En },
            Description = new VentusServer.Models.TranslatedTextDTO { Es = item.Description.Es, En = item.Description.En },
            Type = item.Type,
            Rarity = item.Rarity,
            MaxStack = item.MaxStack,
            RequiredLevel = item.RequiredLevel,
            Price = item.Price,
            Quantity = item.Quantity,
            IsTradable = item.IsTradable,
            IsDroppable = item.IsDroppable,
            IsUsable = item.IsUsable,
            WeaponData = item.WeaponData == null ? null : new WeaponStatsDTO
            {
                WeaponType = item.WeaponData.WeaponType,
                MinDamage = item.WeaponData.MinDamage,
                MaxDamage = item.WeaponData.MaxDamage,
                AttackSpeed = item.WeaponData.AttackSpeed,
                Range = item.WeaponData.Range,
                IsTwoHanded = item.WeaponData.IsTwoHanded,
                ManaCost = item.WeaponData.ManaCost
            },
            ArmorData = item.ArmorData == null ? null : new ArmorStatsDTO
            {
                Slot = item.ArmorData.Slot,
                Defense = item.ArmorData.Defense,
                MagicResistance = item.ArmorData.MagicResistance,
                Durability = item.ArmorData.Durability
            },
            ConsumableData = item.ConsumableData == null ? null : new ConsumableEffectDTO
            {
                Type = item.ConsumableData.Type,
                Amount = item.ConsumableData.Amount,
                Duration = item.ConsumableData.Duration,
                EffectName = item.ConsumableData.EffectName
            },
            Sprite = item.Sprite,
            Sound = item.Sound,
            IconPath = item.IconPath,
            CreatedAt = item.CreatedAt,
            UpdatedAt = item.UpdatedAt
        };
    }
}
