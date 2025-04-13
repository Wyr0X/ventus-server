using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VentusServer.Domain.Models;

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

                var Name = itemData["name"]?["en"]?.ToString() ?? "Unnamed";
                var Description = itemData["desc"]?["en"]?.ToString() ?? "";

                int? hpMin = null, hpMax = null;
                if (itemData["hp"] is JArray hpArray && hpArray.Count == 2)
                {
                    hpMin = (int?)hpArray[0];
                    hpMax = (int?)hpArray[1];
                }

                int? mp = null;
                if (itemData["mp"] != null)
                {
                    // Si es número (ej. 5) -> parse directo
                    // Si es string tipo "5%" -> ignoramos o parseamos como -1 o similar
                    if (int.TryParse(itemData["mp"]?.ToString(), out var mpVal))
                        mp = mpVal;
                }

                var sprite = itemData["sprite"]?.Select(t => (int)t).ToArray() ?? Array.Empty<int>();
                var sound = itemData["sound"]?.ToString();

                var model = new ItemModel
                {
                    Key = key,
                    Name = Name,
                    Description = Description,
                    HpMin = hpMin,
                    HpMax = hpMax,
                    MP = mp,
                    Sprite = sprite,
                    Sound = sound,
                    CreatedAt = DateTime.UtcNow, // Asignar la fecha actual al crear el modelo

                };
                result.Add(model);
            }

            return result;
        }
    }
}
