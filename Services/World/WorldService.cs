using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Game.Models;
using VentusServer.DataAccess.Postgres;

namespace VentusServer.Services
{
    public class WorldService
    {
        private MapService _mapService;

        private PostgresWorldDAO _worldDAO;


        public WorldService(MapService mapService, PostgresWorldDAO worldDAO)
        {
            Console.WriteLine("createDefaultWorld");

            _mapService = mapService;
            _worldDAO = worldDAO;
            createDefaultWorld();

        }

        private async void createDefaultWorld()
        {

            WorldModel? existDefaultWorld = await this.GetWorldByIdAsync(1);
            MapModel? existDefaultMap = await _mapService.GetMapByIdAsync(1);
            Console.WriteLine("createDefaultWorld");
            WorldModel? defaultWorld;
            if (existDefaultWorld == null)
            {

                defaultWorld = await CreateWorldAsync("Nuevo Mundo", "Este es un mundo de ejemplo con parámetros predeterminados.", 10, 100, 1);
                if (existDefaultMap == null && defaultWorld != null)
                {
                    MapModel? defaultMap = await _mapService.CreateMapAsync("Mapa Predeterminado", 1, 10, defaultWorld.Id);
                }


            }

        }
        public async Task<WorldModel?> CreateWorldAsync(string name, string description, int maxMaps, int MaxPlayers, int levelRequirements)
        {
            return await _worldDAO.CreateWorldAsync(name, description, maxMaps, MaxPlayers, levelRequirements);
        }
        public async Task<WorldModel?> GetWorldByIdAsync(int worldId)
        {
            return await _worldDAO.GetWorldByIdAsync(worldId);
        }

        public async Task<List<WorldModel>> GetAllWorldsAsync()
        {
            return await _worldDAO.GetAllWorldsAsync();

        }

        public async Task SaveWorldAsync(WorldModel world)
        {
            await _worldDAO.SaveWorldAsync(world);
        }

        public async Task DeleteWorldAsync(int worldId)
        {
            await _worldDAO.DeleteWorldAsync(worldId);

        }

        public async Task RemovePlayerFromWorld(int playerId, int worldId)
        {
          
                WorldModel? world = await GetWorldByIdAsync(worldId);

                if (world != null)
                {
                    world.RemovePlayer(playerId);
                    await _worldDAO.SaveWorldAsync(world);
                }

        }
    }
}
