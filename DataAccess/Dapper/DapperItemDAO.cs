using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using VentusServer.DataAccess.Dapper;
using VentusServer.DataAccess.Interfaces;
using VentusServer.DataAccess.Mappers;
using VentusServer.DataAccess.Queries;
using VentusServer.Domain.Models;

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
            if (string.IsNullOrWhiteSpace(key)) return null;

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
                var items = ItemMapper.MapMultipleFromRows(rows);
                LoggerUtil.Log(LoggerUtil.LogTag.DapperItemDAO, $"✅ Total items cargados: {items.Count()}");
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
                Console.WriteLine(item.RequiredLevel);
                await conn.ExecuteAsync(ItemQueries.Insert, new
                {
                    item.Key,
                    Name = JsonSerializer.Serialize(item.Name),
                    Description = JsonSerializer.Serialize(item.Description),
                    item.Type,
                    item.Rarity,
                    item.RequiredLevel,
                    item.Price,
                    item.Quantity,
                    item.MaxStack,
                    item.IsTradable,
                    item.IsDroppable,
                    item.IsUsable,
                    item.IconPath,
                    item.Sprite,
                    item.Sound,
                    Data = JsonSerializer.Serialize(new
                    {
                        item.WeaponData,
                        item.ArmorData,
                        item.ConsumableData
                    }),
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
                    item.MaxStack,
                    item.RequiredLevel,
                    item.Price,
                    item.Quantity,
                    item.IsTradable,
                    item.IsDroppable,
                    item.IsUsable,
                    WeaponData = item.WeaponData == null ? null : JsonSerializer.Serialize(item.WeaponData),
                    ArmorData = item.ArmorData == null ? null : JsonSerializer.Serialize(item.ArmorData),
                    ConsumableData = item.ConsumableData == null ? null : JsonSerializer.Serialize(item.ConsumableData),
                    item.Sound,
                    item.IconPath,
                    item.Sprite,
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
