using System.Collections.Generic;
using System.Linq;
using Game.Models;
using VentusServer.DataAccess.Entities;

namespace VentusServer.DataAccess.Mappers
{
    public class WorldMapper : BaseMapper
    {
        public static WorldModel Map(dynamic row)
        {
            return new WorldModel
            {
                Id = row.id,
                Name = row.name,
                Description = row.description,
                MaxMaps = row.max_maps,
                MaxPlayers = row.max_players,
                LevelRequirements = row.level_requirements,
                Maps = new List<MapModel>(),
                PlayersLocation = new List<PlayerLocation>(),
                spawnedPlayers = new List<PlayerLocation>()
            };
        }

        public static List<WorldModel> MapRowsToWorlds(IEnumerable<dynamic> rows)
        {
            return rows.Select(Map).ToList();
        }

        public static DbWorldEntity ToEntity(WorldModel model)
        {
            return new DbWorldEntity
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                MaxMaps = model.MaxMaps,
                MaxPlayers = model.MaxPlayers,
                LevelRequirements = model.LevelRequirements
            };
        }

        public static WorldModel ToModel(DbWorldEntity entity)
        {
            return new WorldModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                MaxMaps = entity.MaxMaps,
                MaxPlayers = entity.MaxPlayers,
                LevelRequirements = entity.LevelRequirements,
                Maps = new List<MapModel>(),
                PlayersLocation = new List<PlayerLocation>(),
                spawnedPlayers = new List<PlayerLocation>()
            };
        }
        public static void PrintRow(dynamic row)
        {
            // Convertir el dynamic a un diccionario para acceder a las claves y valores
            var dictionary = (IDictionary<string, object>)row;

            // Imprimir todos los pares clave-valor
            Console.WriteLine("Row Data:");
            foreach (var kvp in dictionary)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }
        }


    }
}
