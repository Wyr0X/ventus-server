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

                var item = ItemMapper.Map(row);
                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"✅ Item encontrado: {item.Key}");
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

                var item = ItemMapper.Map(row);
                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"✅ Item encontrado: {item.Key}");
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
                var items = ItemMapper.MapRowsToItems(rows);
                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"✅ Total items cargados: {items.Count}");
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
            LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"➕ Creando item: {item.Key}");

            using var conn = GetConnection();

            try
            {
                await conn.ExecuteAsync(ItemQueries.Insert, new
                {
                    item.Key,
                    item.Name,
                    item.Description,
                    item.Sprite,
                    item.Sound,
                    item.HpMin,
                    item.HpMax,
                    item.MP,
                });

                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"✅ Item creado: {item.Key}");
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"❌ Error al crear item {item.Key}: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateItemAsync(ItemModel item)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"✏️ Actualizando item: {item.Key}");

            using var conn = GetConnection();

            try
            {
                await conn.ExecuteAsync(ItemQueries.Update, new
                {
                    item.Key,
                    item.Name,
                    item.Description,
                    item.Sprite,
                    item.Sound,
                    item.HpMin,
                    item.HpMax,
                    item.MP,
                });

                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"✅ Item actualizado: {item.Key}");
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"❌ Error al actualizar item {item.Key}: {ex.Message}");
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
