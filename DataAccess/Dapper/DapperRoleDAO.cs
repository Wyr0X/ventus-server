using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using VentusServer.DataAccess.Interfaces;
using VentusServer.DataAccess.Mappers;
using VentusServer.Models;
using VentusServer.DataAccess.Queries;
using static LoggerUtil;

namespace VentusServer.DataAccess.Dapper
{
    public class DapperRoleDAO(IDbConnectionFactory connectionFactory) : BaseDAO(connectionFactory), IRoleDAO
    {
        public async Task<RoleModel?> GetRoleByIdAsync(Guid roleId)
        {
            if (roleId == Guid.Empty) return null;

            try
            {
                LoggerUtil.Log(LogTag.DapperRoleDAO, $"Buscando rol por ID: {roleId}");
                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync(RoleQueries.SelectRoleById, new { RoleId = roleId });

                return result == null ? null : RoleMapper.ToModel(result);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LogTag.DapperRoleDAO, $"Error en GetRoleByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<RoleModel?> GetRoleByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            try
            {
                LoggerUtil.Log(LogTag.DapperRoleDAO, $"Buscando rol por nombre: {name}");
                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync(RoleQueries.SelectRoleByName, new { Name = name });

                return result == null ? null : RoleMapper.ToModel(result);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LogTag.DapperRoleDAO, $"Error en GetRoleByNameAsync: {ex.Message}");
                return null;
            }
        }
        public async Task<List<RoleModel>> GetAllRolesAsync()
        {
            try
            {
                LoggerUtil.Log(LogTag.DapperRoleDAO, $"Obteniendo todos los roles");
                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryAsync(RoleQueries.SelectAllRoles);

                var entities = RoleMapper.ToEntitiesFromRows(result);
                return RoleMapper.ToModels(entities);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LogTag.DapperRoleDAO, $"Error en GetAllRolesAsync: {ex.Message}");
                return new();
            }
        }

        public async Task<bool> CreateRoleAsync(RoleModel role)
        {
            try
            {
                LoggerUtil.Log(LogTag.DapperRoleDAO, $"Creando rol con ID: {role.RoleId}");
                using var connection = _connectionFactory.CreateConnection();
                var entity = RoleMapper.ToEntity(role);

                await connection.ExecuteAsync(RoleQueries.InsertRole, entity);
                return true;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LogTag.DapperRoleDAO, $"Error en CreateRoleAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateRoleAsync(RoleModel role)
        {
            try
            {
                LoggerUtil.Log(LogTag.DapperRoleDAO, $"Actualizando rol con ID: {role.RoleId}");
                using var connection = _connectionFactory.CreateConnection();
                var entity = RoleMapper.ToEntity(role);

                var affected = await connection.ExecuteAsync(RoleQueries.UpdateRole, entity);
                return affected > 0;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LogTag.DapperRoleDAO, $"Error en UpdateRoleAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteRoleAsync(Guid roleId)
        {
            try
            {
                LoggerUtil.Log(LogTag.DapperRoleDAO, $"Eliminando rol con ID: {roleId}");
                using var connection = _connectionFactory.CreateConnection();

                var affected = await connection.ExecuteAsync(RoleQueries.DeleteRole, new { RoleId = roleId });
                return affected > 0;
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LogTag.DapperRoleDAO, $"Error en DeleteRoleAsync: {ex.Message}");
                return false;
            }
        }
    }
}
