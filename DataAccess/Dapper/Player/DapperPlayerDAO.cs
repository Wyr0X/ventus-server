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

        public async Task<PlayerModel?> GetPlayerByIdAsync(int playerId)
        {
            if (playerId <= 0) return null;

            try
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, $"Buscando jugador por ID: {playerId}");
                using var connection = _connectionFactory.CreateConnection();
                var row = await connection.QueryFirstOrDefaultAsync(PlayerQueries.SelectById, new { PlayerId = playerId });
                return row == null ? null : PlayerMapper.FromRow(row);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, $"Error en GetPlayerByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<PlayerModel?> GetPlayerByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            try
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, $"Buscando jugador por nombre: {name}");
                using var connection = _connectionFactory.CreateConnection();
                var row = await connection.QueryFirstOrDefaultAsync(PlayerQueries.SelectByName, new { Name = name });
                return row == null ? null : PlayerMapper.FromRow(row);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, $"Error en GetPlayerByNameAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<List<PlayerModel>> GetAllPlayersAsync()
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryAsync(PlayerQueries.SelectAll);
                return PlayerMapper.FromRows(result);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, $"Error en GetAllPlayersAsync: {ex.Message}");
                return new List<PlayerModel>();
            }
        }

        public async Task<List<PlayerModel>> GetPlayersByAccountIdAsync(Guid accountId)
        {
            if (accountId == Guid.Empty)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, "Se ha recibido un GUID vacío, retornando lista vacía.");
                return new List<PlayerModel>();
            }

            try
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, $"Consultando jugadores para el AccountId: {accountId}");

                using var connection = _connectionFactory.CreateConnection();

                // LoggerUtil.Log de la consulta antes de ejecutarla
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, $"Ejecutando consulta con AccountId = {accountId}");

                var result = await connection.QueryAsync(PlayerQueries.SelectByAccountId, new { AccountId = accountId });

                // LoggerUtil.Log del resultado crudo (antes de mapear)
                if (result != null && result.Any())
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, $"Se encontraron {result.Count()} jugadores para el AccountId {accountId}.");
                }
                else
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, $"No se encontraron jugadores para el AccountId {accountId}.");
                }

                // LoggerUtil.Log del contenido de los resultados para ver los datos crudos
                foreach (var row in result)
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, $"Resultado obtenido: {row}");
                }

                // Mapeo de los resultados
                var players = PlayerMapper.FromRows(result);

                // LoggerUtil.Log del mapeo
                if (players != null && players.Any())
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, $"Se mapeó correctamente a {players.Count} jugadores.");
                }
                else
                {
                    LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, "El mapeo no produjo jugadores.");
                }

                return players ?? new List<PlayerModel>();
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, $"Error en GetPlayersByAccountIdAsync: {ex.Message}");
                return new List<PlayerModel>();
            }
        }

        public async Task<List<PlayerModel>> GetAllPlayersByUserIdAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var accountId)) return new();
            return await GetPlayersByAccountIdAsync(accountId);
        }

        public async Task<PlayerModel> CreatePlayerAsync(Guid accountId, CreatePlayerDTO createPlayerDTO)
        {
            // Creamos el jugador usando el constructor que pasa todas las propiedades requeridas
            Gender genderEnum = (Gender)createPlayerDTO.Gender;
            Race raceEnum = (Race)createPlayerDTO.Race;
            CharacterClass classEnum = (CharacterClass)createPlayerDTO.Class;

            var newPlayer = new PlayerModel
            {
                Id = 0,
                Gender = genderEnum,
                Race = raceEnum,
                Name = createPlayerDTO.Name,
                Level = 0,
                Class = classEnum,
                AccountId = accountId,
                // Aquí solo inicializas las propiedades no requeridas
                CreatedAt = DateTime.UtcNow,          // Asignación adicional
                LastLogin = DateTime.UtcNow,          // Asignación adicional
                Status = "active"                     // Asignación adicional
            };

            try
            {
                var entity = PlayerMapper.ToEntity(newPlayer); // Conversión a entidad

                using var connection = _connectionFactory.CreateConnection();
                var newId = await connection.ExecuteScalarAsync<int>(PlayerQueries.Insert, entity);
                newPlayer.Id = newId;  // Asigna el Id generado
                return newPlayer;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, $"Error creando jugador: {ex.Message}");
                throw;
            }
        }


        public async Task SavePlayerAsync(PlayerModel player)
        {
            try
            {
                var entity = PlayerMapper.ToEntity(player); // <-- conversión a entidad

                using var connection = _connectionFactory.CreateConnection();
                await connection.ExecuteAsync(PlayerQueries.Upsert, entity);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, $"Error guardando jugador: {ex.Message}");
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
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, $"Error eliminando jugador: {ex.Message}");
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
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, $"Error en PlayerExistsAsync: {ex.Message}");
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
                LoggerUtil.Log(LoggerUtil.LogTag.DapperPlayerDAO, $"Error en PlayerNameExistsAsync: {ex.Message}");
                return false;
            }
        }
    }
}
