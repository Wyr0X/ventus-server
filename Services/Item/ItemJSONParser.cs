using System;
using System.Collections.Generic;
using System.Text.Json;
using VentusServer.Domain.Models;
using VentusServer.Domain.Enums; // Asegúrate de tener tus enums aquí si están separados

namespace VentusServer.Services
{
    public static class ItemJsonParser
    {
        public static List<ItemModel> ParseItemsFromJson(string json)
        {
            var jsonItems = JsonSerializer.Deserialize<List<JsonItem>>(json, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var result = new List<ItemModel>();

            foreach (var jsonItem in jsonItems!)
            {
                var item = new ItemModel
                {
                    Key = jsonItem.Key,
                    Name = new TranslatedTextModel
                    {
                        En = jsonItem.Name.En,
                        Es = jsonItem.Name.Es
                    },
                    Description = new TranslatedTextModel
                    {
                        En = jsonItem.Description.En,
                        Es = jsonItem.Description.Es
                    },
                    Type = Enum.Parse<ItemType>(jsonItem.Type),
                    Rarity = Enum.Parse<ItemRarity>(jsonItem.Rarity),
                    MaxStack = jsonItem.MaxStack,
                    Quantity = null,
                    RequiredLevel = jsonItem.RequiredLevel,
                    IsTradeable = jsonItem.IsTradable,
                    IsDroppable = jsonItem.IsDroppable,
                    IsUsable = jsonItem.IsUsable,
                    Price = jsonItem.Value,
                    Sprite = jsonItem.Sprite,
                    Sound = jsonItem.Sound,
                    IconPath = jsonItem.IconPath,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                if (jsonItem.ConsumableData != null)
                {
                    item.ConsumableData = new ConsumableEffect
                    {
                        Type = jsonItem.ConsumableData.Type,
                        Amount = jsonItem.ConsumableData.Amount,
                        Duration = (float)jsonItem.ConsumableData.Duration,
                        EffectName = jsonItem.ConsumableData.EffectName
                    };
                }

                result.Add(item);
            }

            return result;
        }

        // Estructuras auxiliares para deserialización
        private class JsonItem
        {
            public string Key { get; set; } = null!;
            public Translations Name { get; set; } = null!;
            public Translations Description { get; set; } = null!;
            public string Type { get; set; } = null!;
            public string Rarity { get; set; } = null!;
            public int? MaxStack { get; set; }
            public int? RequiredLevel { get; set; }
            public int Value { get; set; }
            public bool IsTradable { get; set; }
            public bool IsDroppable { get; set; }
            public bool IsUsable { get; set; }
            public ConsumableData? ConsumableData { get; set; }
            public int[]? Sprite { get; set; }
            public string? Sound { get; set; }
            public string IconPath { get; set; } = null!;
        }

        private class Translations
        {
            public string En { get; set; } = null!;
            public string Es { get; set; } = null!;
        }

        private class ConsumableData
        {
            public string Type { get; set; } = null!;
            public int Amount { get; set; }
            public double Duration { get; set; }
            public string? EffectName { get; set; }
        }
    }
}
