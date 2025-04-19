using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.Domain.Models;

namespace VentusServer.DataAccess.DAO
{
    public interface IPlayerInventoryDAO
    {
        Task<PlayerInventoryModel?> GetByPlayerId(int playerId);
        Task Insert(PlayerInventoryModel model);
        Task UpdateGold(int playerId, int gold, DateTime updatedAt);
        Task DeleteByPlayerId(int playerId);
        Task<bool> ExistsByPlayerId(int playerId);
    }
}
