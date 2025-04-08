using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.DataAccess.Interfaces;
using VentusServer.Models;

namespace VentusServer.Services
{
    public class RoleService : BaseCachedService<RoleModel, Guid>
    {
        private readonly IRoleDAO _roleDao;
        private readonly Dictionary<string, Guid> _nameToIdCache = new(StringComparer.OrdinalIgnoreCase);

        public RoleService(IRoleDAO roleDao)
        {
            _roleDao = roleDao;
            LoggerUtil.Log(LoggerUtil.LogTag.RoleService, "RoleService inicializado.");
        }

        protected override async Task<RoleModel?> LoadModelAsync(Guid roleId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Cargando rol con ID {roleId} desde la base de datos...");
            var role = await _roleDao.GetRoleByIdAsync(roleId);

            if (role != null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Rol encontrado: {role.Name}");
                if (!string.IsNullOrWhiteSpace(role.Name))
                    _nameToIdCache[role.Name] = role.RoleId;
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"No se encontró rol con ID {roleId}");
            }

            return role;
        }

        public Task<RoleModel?> GetOrCreateRoleInCacheAsync(Guid roleId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Obteniendo o creando en caché el rol con ID {roleId}");
            return GetOrLoadAsync(roleId);
        }

        public async Task<RoleModel?> GetRoleByNameAsync(string name)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Buscando rol por nombre: {name}");

            if (_nameToIdCache.TryGetValue(name, out var cachedId))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Nombre encontrado en caché con ID: {cachedId}");
                return await GetOrLoadAsync(cachedId);
            }

            var role = await _roleDao.GetRoleByNameAsync(name);
            if (role != null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Rol encontrado en DB para nombre: {name}, ID: {role.RoleId}");
                _nameToIdCache[name] = role.RoleId;
                Set(role.RoleId, role);
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"No se encontró rol para el nombre: {name}");
            }

            return role;
        }

        public async Task<RoleModel?> GetRoleByIdAsync(Guid roleId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Buscando rol por Id: {roleId}");

         
            return await GetOrLoadAsync(roleId);
        }
        public async Task<List<RoleModel>> GetAllRolesAsync()
        {
            LoggerUtil.Log(LoggerUtil.LogTag.RoleService, "Obteniendo todos los roles...");
            var roles = await _roleDao.GetAllRolesAsync();

            foreach (var role in roles)
            {
                Set(role.RoleId, role);
                if (!string.IsNullOrWhiteSpace(role.Name))
                    _nameToIdCache[role.Name] = role.RoleId;
            }

            return roles;
        }

        public async Task CreateRoleAsync(RoleModel role)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Creando nuevo rol: {role.Name} (ID: {role.RoleId})");

            var existing = await GetRoleByNameAsync(role.Name);
            if (existing != null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Ya existe un rol con el nombre: {role.Name}", isError: true);
                throw new Exception($"El rol '{role.Name}' ya existe.");
            }

            var created = await _roleDao.CreateRoleAsync(role);
            if (!created)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Error al crear el rol {role.Name}", isError: true);
                throw new Exception("Error al crear el rol.");
            }

            Set(role.RoleId, role);
            _nameToIdCache[role.Name] = role.RoleId;
        }

        public async Task UpdateRoleAsync(RoleModel role)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Actualizando rol: {role.RoleId}");

            var updated = await _roleDao.UpdateRoleAsync(role);
            if (!updated)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"No se pudo actualizar el rol con ID: {role.RoleId}", isError: true);
                throw new Exception("Error al actualizar el rol.");
            }

            Set(role.RoleId, role);
            if (!string.IsNullOrWhiteSpace(role.Name))
                _nameToIdCache[role.Name] = role.RoleId;
        }

        public async Task<bool> DeleteRoleAsync(Guid roleId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Eliminando rol con ID: {roleId}");

            var success = await _roleDao.DeleteRoleAsync(roleId);
            if (success)
            {
                Invalidate(roleId);
                // También quitamos de la caché por nombre si existe
                foreach (var pair in _nameToIdCache)
                {
                    if (pair.Value == roleId)
                    {
                        _nameToIdCache.Remove(pair.Key);
                        break;
                    }
                }
            }

            return success;
        }
    }
}
