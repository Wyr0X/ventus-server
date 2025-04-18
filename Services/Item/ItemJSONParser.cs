using System;
using Newtonsoft.Json.Linq;

namespace VentusServer.Services
{
    public static class ItemJsonParser
    {
        public static List<ItemModel> ParseItemsFromJson(string json)
        {
            var result = new List<ItemModel>();
            var root = JObject.Parse(json);

            foreach (var itemProp in root.Properties())
            {
                var key = itemProp.Name;
                var itemData = itemProp.Value;

                var name = new TranslatedTextModel
                {
                    En = itemData["name"]?["en"]?.ToString() ?? "Unnamed",
                    Es = itemData["name"]?["es"]?.ToString() ?? ""
                };

                var desc = new TranslatedTextModel
                {
                    En = itemData["desc"]?["en"]?.ToString() ?? "",
                    Es = itemData["desc"]?["es"]?.ToString() ?? ""
                };

                int? hpMin = null, hpMax = null;
                if (itemData["hp"] is JArray hpArray && hpArray.Count == 2)
                {
                    hpMin = (int?)hpArray[0];
                    hpMax = (int?)hpArray[1];
                }

                int? mp = null;
                if (itemData["mp"] != null && int.TryParse(itemData["mp"]?.ToString(), out var mpVal))
                    mp = mpVal;

                var sprite = itemData["sprite"]?.Select(t => (int)t).ToArray() ?? Array.Empty<int>();
                var sound = itemData["sound"]?.ToString();

                // Aquí mapeamos los campos faltantes
                var itemType = Enum.TryParse<ItemType>(itemData["type"]?.ToString(), out var type) ? type : ItemType.Consumable;
                var itemRarity = Enum.TryParse<ItemRarity>(itemData["rarity"]?.ToString(), out var rarity) ? rarity : ItemRarity.Common;

                int? damage = itemData["damage"] != null ? (int?)itemData["damage"] : null;
                int? defense = itemData["defense"] != null ? (int?)itemData["defense"] : null;
                int? manaBonus = itemData["manaBonus"] != null ? (int?)itemData["manaBonus"] : null;
                int? strengthBonus = itemData["strengthBonus"] != null ? (int?)itemData["strengthBonus"] : null;
                int? speedBonus = itemData["speedBonus"] != null ? (int?)itemData["speedBonus"] : null;

                int maxStack = itemData["maxStack"] != null ? (int)itemData["maxStack"] : 1;
                var iconPath = itemData["iconPath"]?.ToString();
                bool isTradable = itemData["isTradable"]?.ToObject<bool>() ?? false;
                bool isDroppable = itemData["isDroppable"]?.ToObject<bool>() ?? false;
                bool isUsable = itemData["isUsable"]?.ToObject<bool>() ?? false;

                var model = new ItemModel
                {
                    Key = key, // Se espera un arreglo de keys
                    Name = name,
                    Description = desc,
                    Type = itemType,
                    Rarity = itemRarity,
                    Damage = damage,
                    Defense = defense,
                    ManaBonus = manaBonus,
                    StrengthBonus = strengthBonus,
                    SpeedBonus = speedBonus,
                    MaxStack = maxStack,
                    IconPath = iconPath,
                    Sprite = sprite,
                    Sound = sound,
                    IsTradable = isTradable,
                    IsDroppable = isDroppable,
                    IsUsable = isUsable,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow // Lo establecemos como ahora, aunque en la práctica se actualizaría después
                };

                result.Add(model);
            }

            return result;
        }
    }
}
