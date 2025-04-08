using System.Threading.Tasks;
using VentusServer.DataAccess.Interfaces;
using VentusServer.Domain.Enums;
using VentusServer.Services;

public class PermissionService : IPermissionService
{
    private readonly RoleService _roleService;

    public PermissionService(RoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<bool> HasPermission(AccountModel account, Permission permission)
    {
        RoleModel? accountRole = await _roleService.GetRoleByIdAsync(account.RoleId);
        if (accountRole == null) return false;
        return accountRole.Permissions.Contains(permission);
    }

  
}
