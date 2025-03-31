using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Game.Models;

namespace VentusServer.DataAccess.Postgres
{
    public interface WorldDAO
    {
        Task<WorldModel?> GetWorldByIdAsync(int worldId);
        Task<List<WorldModel>> GetAllWorldsAsync();
        Task SaveWorldAsync(WorldModel world);
        Task DeleteWorldAsync(int worldId);
    }

}
