using Newtonsoft.Json;
using VentusServer.DataAccess.Entities;

namespace VentusServer.DataAccess.Mappers
{
    public class ItemMapper : BaseMapper
    {
        // Mapea una fila de la base de datos a un modelo de dominio
        public static ItemModel Map(dynamic row)
        {
            return new ItemModel
            {
                Id = row.id,
                Key = row.key,
                Name = JsonConvert.DeserializeObject<TranslatedTextModel>(row.name.ToString()), // Deserialización del JSON
                Description = JsonConvert.DeserializeObject<TranslatedTextModel>(row.description.ToString()), // Deserialización del JSON
                Type = (ItemType)Enum.Parse(typeof(ItemType), row.type.ToString()), // Mapeo del tipo (suponiendo que se guarda como un string o entero)
                Rarity = (ItemRarity)Enum.Parse(typeof(ItemRarity), row.rarity.ToString()), // Mapeo de rareza (similar al tipo)
                Sound = row.sound,
                Damage = row.damage, // Asegúrate de que este campo esté presente en la base de datos
                Defense = row.defense, // Asegúrate de que este campo esté presente en la base de datos
                ManaBonus = row.mana_bonus, // Asegúrate de que este campo esté presente en la base de datos
                StrengthBonus = row.strength_bonus, // Asegúrate de que este campo esté presente en la base de datos
                SpeedBonus = row.speed_bonus, // Asegúrate de que este campo esté presente en la base de datos
                MaxStack = row.max_stack, // Asegúrate de que este campo esté presente en la base de datos
                IconPath = row.icon_path, // Ruta del ícono
                Sprite = row.sprite.ToObject<int[]>(), // Convierte el sprite a un array de enteros
                IsTradable = row.is_tradable, // Booleano si es comerciable
                IsDroppable = row.is_droppable, // Booleano si es descartable
                IsUsable = row.is_usable, // Booleano si es usable
                CreatedAt = row.created_at,
                UpdatedAt = row.updated_at // Asegúrate de que esta fecha esté presente en la base de datos
            };
        }

        // Mapea múltiples filas a una lista de objetos ItemModel
        public static List<ItemModel> MapRowsToItems(IEnumerable<dynamic> rows)
        {
            return rows.Select(Map).ToList();
        }

        // Convierte un modelo de dominio a la entidad que se guarda en la base de datos
        public static DbItemEntity ToEntity(ItemModel model)
        {
            return new DbItemEntity
            {
                Id = model.Id,
                Key = string.Join(",", model.Key), // Convertir el array de claves a un string separado por comas
                NameJson = JsonConvert.SerializeObject(model.Name), // Serialización del objeto TranslatedTextModel a JSON
                DescriptionJson = JsonConvert.SerializeObject(model.Description), // Serialización del objeto TranslatedTextModel a JSON
                Type = model.Type.ToString(), // Guardar como string
                Rarity = model.Rarity.ToString(), // Guardar como string
                Sound = model.Sound,
                Damage = model.Damage,
                Defense = model.Defense,
                ManaBonus = model.ManaBonus,
                StrengthBonus = model.StrengthBonus,
                SpeedBonus = model.SpeedBonus,
                MaxStack = model.MaxStack,
                IconPath = model.IconPath,
                Sprite = model.Sprite, // Convertir el array de sprite a un string
                IsTradable = model.IsTradable,
                IsDroppable = model.IsDroppable,
                IsUsable = model.IsUsable,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt
            };
        }

        // Convierte la entidad de la base de datos a un modelo de dominio
        public static ItemModel ToModel(DbItemEntity entity)
        {
            return new ItemModel
            {
                Id = entity.Id,
                Key = entity.Key, // Convierte el string a un array de claves
                Name = JsonConvert.DeserializeObject<TranslatedTextModel>(entity.NameJson ?? "{}") ?? new TranslatedTextModel(), // Deserializa el JSON con un valor predeterminado y asegura un objeto no nulo
                Description = JsonConvert.DeserializeObject<TranslatedTextModel>(entity.DescriptionJson ?? "{}") ?? new TranslatedTextModel(), // Deserializa el JSON con un valor predeterminado y asegura un objeto no nulo
                Type = (ItemType)Enum.Parse(typeof(ItemType), entity.Type), // Mapeo del tipo
                Rarity = (ItemRarity)Enum.Parse(typeof(ItemRarity), entity.Rarity), // Mapeo de rareza
                Sound = entity.Sound,
                Damage = entity.Damage,
                Defense = entity.Defense,
                ManaBonus = entity.ManaBonus,
                StrengthBonus = entity.StrengthBonus,
                SpeedBonus = entity.SpeedBonus,
                MaxStack = entity.MaxStack,
                IconPath = entity.IconPath,
                Sprite = entity.Sprite, // Asigna directamente el array de enteros
                IsTradable = entity.IsTradable,
                IsDroppable = entity.IsDroppable,
                IsUsable = entity.IsUsable,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
