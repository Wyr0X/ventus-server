using System;
using System.Threading.Tasks;
using Game.Models;
using VentusServer.DataAccess.Postgres;

namespace VentusServer.Services
{
    public class PlayerService
    {
        private readonly PostgresPlayerDAO _playerDAO;
        private readonly PlayerLocationService _playerLocationService;

        public PlayerService(PostgresPlayerDAO playerDAO, PlayerLocationService playerLocationService)
        {
            _playerDAO = playerDAO;
            _playerLocationService = playerLocationService;
        }

        // Obtener un jugador por su ID
        public async Task<PlayerModel?> GetPlayerByIdAsync(int playerId)
        {
            try
            {
                return await _playerDAO.GetPlayerByIdAsync(playerId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al obtener el jugador: {ex.Message}");
                return null;
            }
        }

        // Guardar un jugador (crear o actualizar)
        public async Task SavePlayerAsync(PlayerModel player)
        {
            try
            {
                await _playerDAO.SavePlayerAsync(player);
                Console.WriteLine("✅ Jugador guardado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al guardar el jugador: {ex.Message}");
            }
        }

        // Eliminar un jugador por su ID
        public async Task DeletePlayerAsync(int playerId)
        {
            try
            {
                await _playerDAO.DeletePlayerAsync(playerId);
                Console.WriteLine("✅ Jugador eliminado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al eliminar el jugador: {ex.Message}");
            }
        }

        // Verificar si un jugador existe
        public async Task<bool> PlayerExistsAsync(int playerId)
        {
            try
            {
                return await _playerDAO.PlayerExistsAsync(playerId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al verificar la existencia del jugador: {ex.Message}");
                return false;
            }
        }
        public async Task<PlayerModel?> CreatePlayer(string userId, string name, string gender, string race, string playerClass)
        {
            try
            {
                PlayerModel player = await _playerDAO.CreatePlayerAsync(userId, name, gender, race, playerClass);
                await _playerLocationService.CreateDefaultPlayerLocation(player);
                return player;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al verificar la existencia del jugador: {ex.Message}");
                return null;
            }
        }

        public async Task<List<PlayerModel>> GetAllPlayers(){
            return await _playerDAO.GetAllPlayersAsync();
        }

        public async Task<List<PlayerModel>> GetPlayerWithCompleteInfo(){
            return await _playerDAO.GetAllPlayersAsync();
        }


    }
}
