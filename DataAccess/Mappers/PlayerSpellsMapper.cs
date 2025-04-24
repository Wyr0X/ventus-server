using System;
using System.Collections.Generic;
using System.Text.Json;
using VentusServer.Domain.Models;

namespace VentusServer.DataAccess.Mappers
{
    public static class PlayerSpellsMapper
    {
        // Mapea desde un 'dynamic' (Dapper)
        public static PlayerSpellsModel MapFromFrow(dynamic row)
        {
            // Convertimos el JSONB 'spells' a List<PlayerSpellModel>
            List<PlayerSpellModel> spells;
            if (row.spells != null)
            {
                try
                {
                    spells = JsonSerializer.Deserialize<List<PlayerSpellModel>>(row.spells.ToString()) ?? new List<PlayerSpellModel>();
                }
                catch (JsonException ex)
                {
                    // Manejar el error de deserialización
                    Console.WriteLine($"Error deserializando spells: {ex.Message}");
                    spells = new List<PlayerSpellModel>(); // o lanza una excepción, dependiendo de tu estrategia de error
                }
            }
            else
            {
                spells = new List<PlayerSpellModel>();
            }

            return new PlayerSpellsModel
            {
                Id = row.id,
                PlayerId = row.player_id,
                MaxSlots = row.max_slots,
                CreatedAt = row.created_at,
                UpdatedAt = row.updated_at,
                Spells = spells
            };
        }
    }
}
