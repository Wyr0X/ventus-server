using System;
using System.Threading.Tasks;
using Npgsql;
using Game.Models;

namespace VentusServer.DataAccess.Postgres
{
    public interface IPlayerLocationDAO
    {
        Task<PlayerLocation?> GetPlayerLocationAsync(int playerId);
        Task SavePlayerLocationAsync(PlayerLocation location);
        Task DeletePlayerLocationAsync(int playerId);
    }

}
