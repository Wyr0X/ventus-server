// VentusServer.DataAccess.DAO/IPlayerInventoryDAO.cs

using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.Domain.Models;

namespace VentusServer.DataAccess.DAO
{
    public interface IPlayerInventoryDAO
    {
        /// <summary>
        /// Devuelve el inventario del jugador o null si no existe.
        /// </summary>
        Task<PlayerInventoryModel?> GetByPlayerId(int playerId);

        /// <summary>
        /// Crea un nuevo registro de inventario para el jugador.
        /// </summary>
        Task CreateAsync(PlayerInventoryModel model);

        /// <summary>
        /// Actualiza únicamente la cantidad de oro en el inventario.
        /// </summary>
        Task UpdateGold(int playerId, int gold);

        /// <summary>
        /// Actualiza únicamente la lista de ítems (JSONB) en el inventario.
        /// </summary>
        Task UpdateItems(int playerId, List<PlayerInventoryItemModel> items);

        /// <summary>
        /// Inserta o actualiza (upsert) todo el inventario (oro + items).
        /// </summary>
        Task UpsertAsync(PlayerInventoryModel model);

        /// <summary>
        /// Elimina el inventario asociado a un jugador.
        /// </summary>
        Task DeleteByPlayerId(int playerId);

        /// <summary>
        /// Comprueba si existe inventario para un jugador dado.
        /// </summary>
        Task<bool> ExistsByPlayerId(int playerId);
    }
}
