using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.Models;

namespace VentusServer.DataAccess.Interfaces
{
    public interface IRoleDAO
    {
        Task<RoleModel?> GetRoleByIdAsync(string roleId);
        Task<RoleModel?> GetRoleByDisplayNameAsync(string displayName);
        Task<List<RoleModel>> GetAllRolesAsync();
        Task<bool> CreateRoleAsync(RoleModel role);
        Task<bool> UpdateRoleAsync(RoleModel role);
        Task<bool> DeleteRoleAsync(string roleId);
    }
}
