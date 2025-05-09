using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.DataAccess.Interfaces;
using VentusServer.Domain.Enums;
using VentusServer.Models;

namespace VentusServer.Services
{
    public class RoleService : BaseCachedService<RoleModel, string>
    {
        private readonly IRoleDAO _roleDao;
        private readonly Dictionary<string, string> _nameToIdCache = new(StringComparer.OrdinalIgnoreCase);

        public RoleService(IRoleDAO roleDao)
        {
            _roleDao = roleDao;
            LoggerUtil.Log(LoggerUtil.LogTag.RoleService, "RoleService inicializado.");
        }

        protected override async Task<RoleModel?> LoadModelAsync(string roleId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Cargando rol con ID {roleId} desde la base de datos...");
            var role = await _roleDao.GetRoleByIdAsync(roleId).ConfigureAwait(false);

            if (role != null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Rol encontrado: {role.DisplayName}");
                if (!string.IsNullOrWhiteSpace(role.DisplayName))
                    _nameToIdCache[role.DisplayName] = role.RoleId;
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"No se encontró rol con ID {roleId}");
            }

            return role;
        }

        public async Task<RoleModel?> GetOrCreateRoleInCacheAsync(string roleId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Obteniendo o creando en caché el rol con ID {roleId}");
            return await GetOrLoadAsync(roleId).ConfigureAwait(false);
        }

        public async Task<RoleModel?> GetRoleByDisplayNameAsync(string name)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Buscando rol por nombre: {name}");

            if (_nameToIdCache.TryGetValue(name, out var cachedId))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Nombre encontrado en caché : {cachedId}");
                return await GetOrLoadAsync(cachedId).ConfigureAwait(false);
            }

            var role = await _roleDao.GetRoleByDisplayNameAsync(name).ConfigureAwait(false);
            if (role != null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Rol encontrado en DB para nombre: {name}, ID: {role.RoleId}");
                _nameToIdCache[name] = role.DisplayName;
                Set(role.RoleId, role);
            }
            else
            {
                LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"No se encontró rol para el nombre: {name}");
            }

            return role;
        }

        public async Task<RoleModel?> GetRoleByIdAsync(string roleId)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Buscando rol por Id: {roleId}");


            return await GetOrLoadAsync(roleId).ConfigureAwait(false);
        }
        public async Task<List<RoleModel>> GetAllRolesAsync()
        {
            LoggerUtil.Log(LoggerUtil.LogTag.RoleService, "Obteniendo todos los roles...");
            var roles = await _roleDao.GetAllRolesAsync().ConfigureAwait(false);

            foreach (var role in roles)
            {
                Set(role.RoleId, role);
                if (!string.IsNullOrWhiteSpace(role.DisplayName))
                    _nameToIdCache[role.DisplayName] = role.RoleId;
            }

            return roles;
        }

        public async Task CreateRoleAsync(RoleModel role)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Creando nuevo rol: {role.DisplayName} (ID: {role.RoleId})");

            var existing = await GetRoleByDisplayNameAsync(role.DisplayName).ConfigureAwait(false);
            if (existing != null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Ya existe un rol con el nombre: {role.DisplayName}", isError: true);
                throw new Exception($"El rol '{role.DisplayName}' ya existe.");
            }

            var created = await _roleDao.CreateRoleAsync(role).ConfigureAwait(false);
            if (!created)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Error al crear el rol {role.DisplayName}", isError: true);
                throw new Exception("Error al crear el rol.");
            }

            Set(role.RoleId, role);
            _nameToIdCache[role.DisplayName] = role.RoleId;
        }

        public async Task<bool> UpdateRoleAsync(UpdateRoleDTO updateRoleDTO)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"Actualizando rol: {updateRoleDTO.RoleId}");

            RoleModel roleModel = new RoleModel
            {
                RoleId = updateRoleDTO.RoleId,
                DisplayName = updateRoleDTO.DisplayName,
                Permissions = updateRoleDTO.Permissions?
                    .Select(p => Enum.TryParse<Permission>(p, out var perm) ? perm : default)
                    .Where(p => p != default) // Filtra valores inválidos
                    .ToList() ?? new()

            };
            var updated = await _roleDao.UpdateRoleAsync(roleModel);
            if (!updated)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.RoleService, $"No se pudo actualizar el rol con ID: {roleModel.RoleId}", isError: true);
                throw new Exception("Error al actualizar el rol.");
            }

            Set(roleModel.RoleId, roleModel);
            if (!string.IsNullOrWhiteSpace(roleModel.DisplayName))
                _nameToIdCache[roleModel.DisplayName] = roleModel.RoleId;
            return updated;
        }

        public async Task<bool> DeleteRoleAsync(string roleId)
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
