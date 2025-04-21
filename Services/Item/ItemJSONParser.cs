using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using VentusServer.Domain.Models;

namespace VentusServer.Services
{
    public static class ItemJsonParser
    {
        public static List<ItemModel> ParseItemsFromJson(string json)
        {
            var result = new List<ItemModel>();

            // Soporta tanto un array en la raíz como un objeto con propiedad "items"
            JToken root = JToken.Parse(json);
            JArray itemsArray = root.Type switch
            {
                JTokenType.Array => (JArray)root,
                JTokenType.Object when root["items"] is JArray arr => arr,
                _ => throw new ArgumentException("JSON inválido: se esperaba un array o un objeto con 'items'")
            };

            foreach (JToken token in itemsArray)
            {
                // Key obligatorio
                var key = token["key"]?.ToString()
                    ?? throw new ArgumentException("Falta 'key' en el ítem JSON.");

                // Traducciones
                var name = new TranslatedTextModel
                {
                    En = token["name"]?["en"]?.ToString() ?? "",
                    Es = token["name"]?["es"]?.ToString() ?? ""
                };
                var description = new TranslatedTextModel
                {
                    En = token["description"]?["en"]?.ToString() ?? "",
                    Es = token["description"]?["es"]?.ToString() ?? ""
                };

                // Tipos y rarezas
                var type = Enum.TryParse<ItemType>(token["type"]?.ToString(), out var tVal)
                    ? tVal
                    : throw new ArgumentException($"Tipo inválido: {token["type"]}");
                var rarity = Enum.TryParse<ItemRarity>(token["rarity"]?.ToString(), out var rVal)
                    ? rVal
                    : ItemRarity.Common;

                // Flags y valores opcionales
                var maxStack = token["maxStack"]?.ToObject<int?>();
                var requiredLevel = token["requiredLevel"]?.ToObject<int?>();
                var price = token["price"]?.ToObject<int?>();
                var isTradable = token["isTradable"]?.ToObject<bool>() ?? false;
                var isDroppable = token["isDroppable"]?.ToObject<bool>() ?? false;
                var isUsable = token["isUsable"]?.ToObject<bool>() ?? false;

                // Sprite como int[]
                var sprite = token["sprite"] is JArray spArr
                    ? spArr.Select(j => j.ToObject<int>()).ToArray()
                    : Array.Empty<int>();

                var sound = token["sound"]?.ToString();
                var iconPath = token["iconPath"]?.ToString() ?? "";

                // Sub-estructuras según el tipo
                WeaponStats? weaponData = null;
                ArmorStats? armorData = null;
                ConsumableEffect? consumableData = null;

                if (type == ItemType.Weapon && token["weaponData"] is JObject w)
                {
                    weaponData = new WeaponStats
                    {
                        WeaponType = Enum.Parse<WeaponType>(w["weaponType"]?.ToString() ?? "Sword"),
                        MinDamage = w["minDamage"]?.ToObject<int>() ?? 0,
                        MaxDamage = w["maxDamage"]?.ToObject<int>() ?? 0,
                        AttackSpeed = w["attackSpeed"]?.ToObject<float>() ?? 1f,
                        Range = w["range"]?.ToObject<int>() ?? 1,
                        IsTwoHanded = w["isTwoHanded"]?.ToObject<bool>() ?? false,
                        ManaCost = w["manaCost"]?.ToObject<int?>()
                    };
                }
                else if (type == ItemType.Armor && token["armorData"] is JObject a)
                {
                    armorData = new ArmorStats
                    {
                        Slot = Enum.Parse<ArmorSlot>(a["slot"]?.ToString() ?? "Chest"),
                        Defense = a["defense"]?.ToObject<int>() ?? 0,
                        MagicResistance = a["magicResistance"]?.ToObject<int>() ?? 0,
                        Durability = a["durability"]?.ToObject<int>() ?? 100
                    };
                }
                else if (type == ItemType.Consumable && token["consumableData"] is JObject c)
                {
                    consumableData = new ConsumableEffect
                    {
                        Type = c["type"]?.ToString() ?? "",
                        Amount = c["amount"]?.ToObject<int>() ?? 0,
                        Duration = c["duration"]?.ToObject<float>() ?? 0f,
                        EffectName = c["effectName"]?.ToString()
                    };
                }

                // Construye el ItemModel final
                var item = new ItemModel
                {
                    Key = key,
                    Name = name,
                    Description = description,
                    Type = type,
                    Rarity = rarity,
                    MaxStack = maxStack,
                    RequiredLevel = requiredLevel,
                    Price = price ?? 0,
                    IsTradable = isTradable,
                    IsDroppable = isDroppable,
                    IsUsable = isUsable,
                    Sprite = sprite,
                    Sound = sound,
                    IconPath = iconPath,
                    WeaponData = weaponData,
                    ArmorData = armorData,
                    ConsumableData = consumableData,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                result.Add(item);
            }

            return result;
        }
    }
}
