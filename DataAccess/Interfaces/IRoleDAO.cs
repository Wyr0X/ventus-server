using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VentusServer.Models;

namespace VentusServer.DataAccess.Interfaces
{
    public interface IRoleDAO
    {
        Task<RoleModel?> GetRoleByIdAsync(Guid roleId);
        Task<RoleModel?> GetRoleByNameAsync(string name);
        Task<List<RoleModel>> GetAllRolesAsync();
        Task<bool> CreateRoleAsync(RoleModel role);
        Task<bool> UpdateRoleAsync(RoleModel role);
        Task<bool> DeleteRoleAsync(Guid roleId);
    }
}
