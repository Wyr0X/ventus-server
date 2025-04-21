using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using VentusServer.DataAccess.Entities;
using VentusServer.Domain.Models;

namespace VentusServer.DataAccess.Mappers
{
    public static class PlayerInventoryMapper
    {
        // 1) Mapea desde la entidad DB ya tipada
        public static PlayerInventoryModel Map(DbPlayerInventoryEntity entity)
        {
            var model = new PlayerInventoryModel
            {
                Id = entity.Id,
                PlayerId = entity.PlayerId,
                Gold = entity.Gold,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Items = new List<PlayerInventoryItemModel>()
            };

            if (entity.Items != null)
            {
                model.Items = JsonSerializer
                    .Deserialize<List<PlayerInventoryItemModel>>(
                        entity.Items.RootElement.GetRawText()
                    ) ?? new();
            }

            return model;
        }

        // 2) Mapea desde un 'dynamic' (Dapper) -> construye un DbPlayerInventoryEntity -> reusa Map(entity)
        public static PlayerInventoryModel MapFromFrow(dynamic row)
        {
            // Convertimos el JSONB 'items' (puede venir como JsonDocument o string) a JsonDocument
            JsonDocument itemsDoc;
            if (row.items is JsonDocument jd)
            {
                itemsDoc = jd;
            }
            else
            {
                var jsonText = row.items?.ToString() ?? "[]";
                itemsDoc = JsonDocument.Parse(jsonText);
            }

            var entity = new DbPlayerInventoryEntity
            {
                Id = (int)row.id,
                PlayerId = (int)row.player_id,
                Gold = (int)row.gold,
                Items = itemsDoc,
                CreatedAt = (DateTime)row.created_at,
                UpdatedAt = (DateTime)row.updated_at
            };

            return Map(entity);
        }

        // Convierte el modelo de dominio a la entidad para BD
        public static DbPlayerInventoryEntity ToEntity(PlayerInventoryModel model)
        {
            return new DbPlayerInventoryEntity
            {
                Id = model.Id,
                PlayerId = model.PlayerId,
                Gold = model.Gold,
                Items = JsonDocument.Parse(JsonSerializer.Serialize(model.Items)),
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt
            };
        }
    }
}
