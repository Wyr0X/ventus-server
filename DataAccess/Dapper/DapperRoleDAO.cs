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
        public async Task<RoleModel?> GetRoleByIdAsync(string roleId)
        {
            if (roleId == "") return null;

            try
            {
                LoggerUtil.Log(LogTag.DapperRoleDAO, $"Buscando rol por ID: {roleId}");
                using var connection = _connectionFactory.CreateConnection();
                var result = await connection.QueryFirstOrDefaultAsync(RoleQueries.SelectRoleById, new { RoleId = roleId });

                return result == null ? null : RoleMapper.ToModel(RoleMapper.ToEntityFromRow(result));
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LogTag.DapperRoleDAO, $"Error en GetRoleByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<RoleModel?> GetRoleByDisplayNameAsync(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName)) return null;

            try
            {
                LoggerUtil.Log(LogTag.DapperRoleDAO, $"Buscando rol por nombre 2: {displayName}");
                using var connection = _connectionFactory.CreateConnection();
                var row = await connection.QueryFirstOrDefaultAsync(RoleQueries.SelectRoleByDisplayName, new { DisplayName = displayName });
                var entity = RoleMapper.ToEntityFromRow(row);
                return row == null ? null : RoleMapper.ToModel(entity);
            }
            catch (Exception ex)
            {
                LoggerUtil.Log(LogTag.DapperRoleDAO, $"Error en GetRoleByDisplayNameAsync: {ex.Message}");
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

                LoggerUtil.Log(LogTag.DapperRoleDAO, $"Creando rol con ID: {role.DisplayName}");
                using var connection = _connectionFactory.CreateConnection();

                var entity = RoleMapper.ToEntity(role);
                RoleMapper.PrintDbRoleEntity(entity);


                await connection.ExecuteAsync(RoleQueries.InsertRole, entity);
                LoggerUtil.Log(LogTag.DapperRoleDAO, $"Rol creado:: {role.DisplayName}");

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

        public async Task<bool> DeleteRoleAsync(string roleId)
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
