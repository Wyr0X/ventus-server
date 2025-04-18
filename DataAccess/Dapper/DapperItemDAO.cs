using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using VentusServer.DataAccess.Interfaces;
using VentusServer.DataAccess.Mappers;
using VentusServer.Domain.Models;
using VentusServer.DataAccess.Queries;
using VentusServer.DataAccess.Dapper;
using System.Text.Json;

namespace VentusServer.DataAccess.Postgres
{
    public class DapperItemDAO : BaseDAO, IItemDAO
    {
        public DapperItemDAO(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }

        public async Task<ItemModel?> GetItemByIdAsync(int id)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"🔍 Buscando item por ID: {id}");

            using var conn = GetConnection();

            try
            {
                var row = await conn.QueryFirstOrDefaultAsync(ItemQueries.SelectById, new { Id = id });
                if (row == null)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"❌ No se encontró el item con ID: {id}");
                    return null;
                }
                ItemMapper.PrintRow(row);
                var item = ItemMapper.Map(row);
                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"✅ Item encontrado: {item.Name.En}");
                return item;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"❌ Error al buscar item por ID: {ex.Message}");
                return null;
            }
        }

        public async Task<ItemModel?> GetItemByKeyAsync(string key)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"🔍 Buscando item por Key: {key}");

            if (string.IsNullOrWhiteSpace(key))
                return null;

            using var conn = GetConnection();

            try
            {
                var row = await conn.QueryFirstOrDefaultAsync(ItemQueries.SelectByKey, new { Key = key });
                if (row == null)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"❌ No se encontró el item con key: {key}");
                    return null;
                }
                ItemMapper.PrintRow(row);

                var item = ItemMapper.Map(row);
                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"✅ Item encontrado: {item.Name.En}");
                return item;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"❌ Error al buscar item por key: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<ItemModel>> GetAllItemsAsync()
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"📦 Obteniendo todos los items");

            using var conn = GetConnection();

            try
            {
                var rows = await conn.QueryAsync(ItemQueries.SelectAll);
                ItemMapper.PrintRow(rows.FirstOrDefault());

                var items = ItemMapper.MapRowsToItems(rows);
                var firstRow = rows.FirstOrDefault();
                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"✅ Total items cargados: {items.Count()} {firstRow?.sprite.Count()}");
                return items;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"❌ Error al obtener items: {ex.Message}");
                return Array.Empty<ItemModel>();
            }
        }

        public async Task CreateItemAsync(ItemModel item)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"➕ Creando item: {item.Name.En}");

            using var conn = GetConnection();

            try
            {
                await conn.ExecuteAsync(ItemQueries.Insert, new
                {
                    item.Id,
                    Name = JsonSerializer.Serialize(item.Name),
                    Description = JsonSerializer.Serialize(item.Description),
                    item.Type,
                    item.Rarity,
                    item.Sound,
                    item.Damage,
                    item.Defense,
                    item.ManaBonus,
                    item.StrengthBonus,
                    item.SpeedBonus,
                    item.MaxStack,
                    item.IconPath,
                    item.Sprite,
                    item.IsTradable,
                    item.IsDroppable,
                    item.IsUsable,
                    item.CreatedAt,
                    item.UpdatedAt
                });

                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"✅ Item creado: {item.Name.En}");
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"❌ Error al crear item {item.Name.En}: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateItemAsync(ItemModel item)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"✏️ Actualizando item: {item.Name.En}");

            using var conn = GetConnection();

            try
            {
                await conn.ExecuteAsync(ItemQueries.Update, new
                {
                    item.Id,
                    Name = JsonSerializer.Serialize(item.Name),
                    Description = JsonSerializer.Serialize(item.Description),
                    item.Type,
                    item.Rarity,
                    item.Sound,
                    item.Damage,
                    item.Defense,
                    item.ManaBonus,
                    item.StrengthBonus,
                    item.SpeedBonus,
                    item.MaxStack,
                    item.IconPath,
                    item.Sprite,
                    item.IsTradable,
                    item.IsDroppable,
                    item.IsUsable,
                    item.UpdatedAt
                });

                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"✅ Item actualizado: {item.Name.En}");
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"❌ Error al actualizar item {item.Name.En}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteItemAsync(Guid id)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"🗑 Eliminando item con ID: {id}");

            using var conn = GetConnection();

            try
            {
                await conn.ExecuteAsync(ItemQueries.Delete, new { Id = id });
                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"✅ Item eliminado con ID: {id}");
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"❌ Error al eliminar item con ID {id}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteItemAsync(int id)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"🗑 Eliminando item con ID: {id}");

            using var conn = GetConnection();

            try
            {
                await conn.ExecuteAsync(ItemQueries.Delete, new { Id = id });
                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"✅ Item eliminado con ID: {id}");
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"❌ Error al eliminar item con ID {id}: {ex.Message}");
                throw;
            }
        }
    }
}
