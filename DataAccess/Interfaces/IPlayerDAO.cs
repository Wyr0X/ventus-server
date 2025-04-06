using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.Models;

namespace VentusServer.DataAccess.Interfaces
{
    public interface IPlayerDAO
    {
        Task<PlayerModel?> GetPlayerByIdAsync(int playerId);
        Task<PlayerModel?> GetPlayerByNameAsync(string name);
        Task<List<PlayerModel>> GetAllPlayersAsync();
        Task<List<PlayerModel>> GetPlayersByAccountIdAsync(Guid accountId);
        Task<List<PlayerModel>> GetAllPlayersByUserIdAsync(string userId);
        Task<PlayerModel> CreatePlayerAsync(Guid accountId, string name, string gender, string race, string playerClass);
        Task SavePlayerAsync(PlayerModel player);
        Task<bool> DeletePlayerAsync(int playerId);
        Task<bool> PlayerExistsAsync(int playerId);
        Task<bool> PlayerNameExistsAsync(string name);
    }
}
