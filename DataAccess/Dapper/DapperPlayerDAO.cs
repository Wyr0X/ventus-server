using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using VentusServer.DataAccess.Interfaces;
using VentusServer.Models;
using VentusServer.DataAccess.Mappers;
using static LoggerUtil;
using VentusServer.DataAccess.Queries;

namespace VentusServer.DataAccess.Dapper
{
    public class DapperPlayerDAO(IDbConnectionFactory connectionFactory) : BaseDAO(connectionFactory), IPlayerDAO
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public async Task<PlayerModel?> GetPlayerByIdAsync(int playerId)
        {
            if (playerId <= 0) return null;

            try
            {
                Log("PlayerDAO", $"Buscando jugador por ID: {playerId}", ConsoleColor.Cyan);
                using var connection = _connectionFactory.CreateConnection();
                var row = await connection.QueryFirstOrDefaultAsync(PlayerQueries.SelectById, new { PlayerId = playerId });
                return row == null ? null : PlayerMapper.MapRowToPlayer(row);
            }
            catch (Exception ex)
            {
                Log("PlayerDAO", $"Error en GetPlayerByIdAsync: {ex.Message}", ConsoleColor.Red);
                return null;
            }
        }

        public async Task<PlayerModel?> GetPlayerByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            try
            {
                Log("PlayerDAO", $"Buscando jugador por nombre: {name}", ConsoleColor.Cyan);
                using var connection = _connectionFactory.CreateConnection();
                var row = await connection.QueryFirstOrDefaultAsync(PlayerQueries.SelectByName, new { Name = name });
                return row == null ? null : PlayerMapper.MapRowToPlayer(row);
            }
            catch (Exception ex)
            {
                Log("PlayerDAO", $"Error en GetPlayerByNameAsync: {ex.Message}", ConsoleColor.Red);
                return null;
            }
        }

        public async Task<List<PlayerModel>> GetAllPlayersAsync()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryAsync(PlayerQueries.SelectAll);
                return PlayerMapper.MapRowsToPlayers(result);
            }
            catch (Exception ex)
            {
                Log("PlayerDAO", $"Error en GetAllPlayersAsync: {ex.Message}", ConsoleColor.Red);
                return new List<PlayerModel>();
            }
        }

        public async Task<List<PlayerModel>> GetPlayersByAccountIdAsync(Guid accountId)
        {
            if (accountId == Guid.Empty) return new();

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryAsync(PlayerQueries.SelectByAccountId, new { AccountId = accountId });
                return PlayerMapper.MapRowsToPlayers(result);
            }
            catch (Exception ex)
            {
                Log("PlayerDAO", $"Error en GetPlayersByAccountIdAsync: {ex.Message}", ConsoleColor.Red);
                return new List<PlayerModel>();
            }
        }

        public async Task<List<PlayerModel>> GetAllPlayersByUserIdAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var accountId)) return new();
            return await GetPlayersByAccountIdAsync(accountId);
        }

        public async Task<PlayerModel> CreatePlayerAsync(Guid accountId, string name, string gender, string race, string playerClass)
        {
            var newPlayer = new PlayerModel(
                id: 0,
                accountId: accountId,
                name: name,
                gender: gender,
                race: race,
                level: 0,
                playerClass: playerClass
            )
            {
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Status = "active"
            };

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var newId = await connection.ExecuteScalarAsync<int>(PlayerQueries.Insert, newPlayer);
                newPlayer.Id = newId;
                return newPlayer;
            }
            catch (Exception ex)
            {
                Log("PlayerDAO", $"Error creando jugador: {ex.Message}", ConsoleColor.Red);
                throw;
            }
        }

        public async Task SavePlayerAsync(PlayerModel player)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(PlayerQueries.Upsert, player);
            }
            catch (Exception ex)
            {
                Log("PlayerDAO", $"Error guardando jugador: {ex.Message}", ConsoleColor.Red);
                throw;
            }
        }

        public async Task<bool> DeletePlayerAsync(int playerId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var affected = await connection.ExecuteAsync(PlayerQueries.DeleteById, new { PlayerId = playerId });
                return affected > 0;
            }
            catch (Exception ex)
            {
                Log("PlayerDAO", $"Error eliminando jugador: {ex.Message}", ConsoleColor.Red);
                return false;
            }
        }

        public async Task<bool> PlayerExistsAsync(int playerId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                return await connection.ExecuteScalarAsync<bool>(PlayerQueries.ExistsById, new { PlayerId = playerId });
            }
            catch (Exception ex)
            {
                Log("PlayerDAO", $"Error en PlayerExistsAsync: {ex.Message}", ConsoleColor.Red);
                return false;
            }
        }

        public async Task<bool> PlayerNameExistsAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                return await connection.ExecuteScalarAsync<bool>(PlayerQueries.ExistsByName, new { Name = name });
            }
            catch (Exception ex)
            {
                Log("PlayerDAO", $"Error en PlayerNameExistsAsync: {ex.Message}", ConsoleColor.Red);
                return false;
            }
        }
    }
}
