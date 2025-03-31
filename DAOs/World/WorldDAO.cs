using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Game.Models;

namespace VentusServer.DataAccess.Postgres
{
    public interface WorldDAO
    {
        Task<World?> GetWorldByIdAsync(int worldId);
        Task<List<World>> GetAllWorldsAsync();
        Task SaveWorldAsync(World world);
        Task DeleteWorldAsync(int worldId);
    }

}
