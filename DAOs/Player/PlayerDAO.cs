using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Models;
using Npgsql;
using VentusServer.DataAccess;
using VentusServer.Models;

namespace VentusServer.DataAccess.Postgres
{
    public interface IPlayerDAO
    {
        Task<PlayerModel?> GetPlayerByIdAsync(int playerId);
        Task SavePlayerAsync(PlayerModel player);
        Task DeletePlayerAsync(int playerId);
        Task<bool> PlayerExistsAsync(int playerId);
    }
}
