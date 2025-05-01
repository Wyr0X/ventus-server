using Newtonsoft.Json;
using VentusServer.DataAccess.Entities;

namespace VentusServer.DataAccess.Mappers
{
    public class ItemMapper : BaseMapper
    {
        public static ItemModel ToModel(DbItemEntity entity)
        {
            // Deserializa datos generales
            var name = JsonConvert.DeserializeObject<TranslatedTextModel>(entity.NameJson ?? "{}") ?? new();
            var description = JsonConvert.DeserializeObject<TranslatedTextModel>(entity.DescriptionJson ?? "{}") ?? new();

            // Mapeo de tipo y rareza
            var type = Enum.Parse<ItemType>(entity.Type);
            var rarity = Enum.Parse<ItemRarity>(entity.Rarity);

            // Submodelo desde el campo "data"
            object? data = string.IsNullOrWhiteSpace(entity.DataJson) ? null : JsonConvert.DeserializeObject(entity.DataJson);

            // Asignar submodelo según el tipo
            WeaponStats? weapon = null;
            ArmorStats? armor = null;
            ConsumableEffect? consumable = null;

            switch (type)
            {
                case ItemType.Weapon:
                    weapon = JsonConvert.DeserializeObject<WeaponStats>(entity.DataJson ?? "{}");
                    break;
                case ItemType.Armor:
                    armor = JsonConvert.DeserializeObject<ArmorStats>(entity.DataJson ?? "{}");
                    break;
                case ItemType.Consumable:
                    consumable = JsonConvert.DeserializeObject<ConsumableEffect>(entity.DataJson ?? "{}");
                    break;
            }

            return new ItemModel
            {
                Id = entity.Id,
                Key = entity.Key,
                Name = name,
                Description = description,
                Type = type,
                Rarity = rarity,
                RequiredLevel = entity.RequiredLevel,
                Price = entity.Price ?? 0,
                Quantity = entity.Quantity,
                MaxStack = entity.MaxStack,
                IsTradeable = entity.IsTradable,
                IsDroppable = entity.IsDroppable,
                IsUsable = entity.IsUsable,
                IconPath = entity.IconPath,
                Sprite = entity.Sprite,
                Sound = entity.Sound,
                WeaponData = weapon,
                ArmorData = armor,
                ConsumableData = consumable
            };
        }

        public static DbItemEntity ToEntity(ItemModel model)
        {
            object? data = model.Type switch
            {
                ItemType.Weapon => model.WeaponData,
                ItemType.Armor => model.ArmorData,
                ItemType.Consumable => model.ConsumableData,
                _ => null
            };

            return new DbItemEntity
            {
                Id = model.Id,
                Key = model.Key,
                NameJson = JsonConvert.SerializeObject(model.Name),
                DescriptionJson = JsonConvert.SerializeObject(model.Description),
                Type = model.Type.ToString(),
                Rarity = model.Rarity.ToString(),
                RequiredLevel = model.RequiredLevel,
                Price = model.Price,
                Quantity = model.Quantity,
                MaxStack = model.MaxStack,
                IsTradable = model.IsTradeable,
                IsDroppable = model.IsDroppable,
                IsUsable = model.IsUsable,
                IconPath = model.IconPath,
                Sprite = model.Sprite,
                Sound = model.Sound,
                DataJson = data != null ? JsonConvert.SerializeObject(data) : null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static List<ItemModel> MapRowsToItems(IEnumerable<DbItemEntity> entities)
        {
            return entities.Select(ToModel).ToList();
        }
        public static List<ItemModel> MapMultipleFromRows(IEnumerable<dynamic> rows)
        {
            var items = new List<ItemModel>();

            foreach (var row in rows)
            {
                var name = JsonConvert.DeserializeObject<TranslatedTextModel>(row.name ?? "{}") ?? new TranslatedTextModel();
                var description = JsonConvert.DeserializeObject<TranslatedTextModel>(row.description ?? "{}") ?? new TranslatedTextModel();
                var type = Enum.Parse<ItemType>((string)row.type);
                var rarity = Enum.Parse<ItemRarity>((string)row.rarity);

                WeaponStats? weapon = null;
                ArmorStats? armor = null;
                ConsumableEffect? consumable = null;

                string? dataJson = row.data_json;

                switch (type)
                {
                    case ItemType.Weapon:
                        weapon = JsonConvert.DeserializeObject<WeaponStats>(dataJson ?? "{}");
                        break;
                    case ItemType.Armor:
                        armor = JsonConvert.DeserializeObject<ArmorStats>(dataJson ?? "{}");
                        break;
                    case ItemType.Consumable:
                        consumable = JsonConvert.DeserializeObject<ConsumableEffect>(dataJson ?? "{}");
                        break;
                }

                var item = new ItemModel
                {
                    Id = row.id,
                    Key = row.item_key,
                    Name = name,
                    Description = description,
                    Type = type,
                    Rarity = rarity,
                    RequiredLevel = row.required_level,
                    Price = row.price,
                    Quantity = row.quantity,
                    MaxStack = row.max_stack,
                    IsTradeable = row.is_tradable,
                    IsDroppable = row.is_droppable,
                    IsUsable = row.is_usable,
                    IconPath = row.icon_path,
                    Sprite = row.sprite,
                    Sound = row.sound,
                    WeaponData = weapon,
                    ArmorData = armor,
                    ConsumableData = consumable
                };

                items.Add(item);
            }

            return items;
        }

        public static ItemModel Map(dynamic row)
        {
            var name = JsonConvert.DeserializeObject<TranslatedTextModel>(row.name_json ?? "{}") ?? new TranslatedTextModel();
            var description = JsonConvert.DeserializeObject<TranslatedTextModel>(row.description_json ?? "{}") ?? new TranslatedTextModel();
            var type = Enum.Parse<ItemType>((string)row.type);
            var rarity = Enum.Parse<ItemRarity>((string)row.rarity);

            WeaponStats? weapon = null;
            ArmorStats? armor = null;
            ConsumableEffect? consumable = null;

            string? dataJson = row.data_json;

            switch (type)
            {
                case ItemType.Weapon:
                    weapon = JsonConvert.DeserializeObject<WeaponStats>(dataJson ?? "{}");
                    break;
                case ItemType.Armor:
                    armor = JsonConvert.DeserializeObject<ArmorStats>(dataJson ?? "{}");
                    break;
                case ItemType.Consumable:
                    consumable = JsonConvert.DeserializeObject<ConsumableEffect>(dataJson ?? "{}");
                    break;
            }

            return new ItemModel
            {
                Id = row.id,
                Key = row.key,
                Name = name,
                Description = description,
                Type = type,
                Rarity = rarity,
                RequiredLevel = row.required_level,
                Price = row.price,
                Quantity = row.quantity,
                MaxStack = row.max_stack,
                IsTradeable = row.is_tradable,
                IsDroppable = row.is_droppable,
                IsUsable = row.is_usable,
                IconPath = row.icon_path,
                Sprite = row.sprite,
                Sound = row.sound,
                WeaponData = weapon,
                ArmorData = armor,
                ConsumableData = consumable
            };
        }
        public static DbItemEntity MapEntity(dynamic row)
        {
            return new DbItemEntity
            {
                Id = row.id,
                Key = row.key,
                NameJson = row.name_json,
                DescriptionJson = row.description_json,
                Type = row.type,
                Rarity = row.rarity,
                RequiredLevel = row.required_level,
                Price = row.price,
                Quantity = row.quantity,
                MaxStack = row.max_stack,
                IsTradable = row.is_tradable,
                IsDroppable = row.is_droppable,
                IsUsable = row.is_usable,
                IconPath = row.icon_path,
                Sprite = row.sprite,
                Sound = row.sound,
                DataJson = row.data_json,
                CreatedAt = row.created_at,
                UpdatedAt = row.updated_at
            };
        }

    }

}
