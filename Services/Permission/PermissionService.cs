using System.Threading.Tasks;
using VentusServer.DataAccess.Interfaces;
using VentusServer.Domain.Enums;
using VentusServer.Services;
using static LoggerUtil;

public class PermissionService : IPermissionService
{
    private readonly RoleService _roleService;

    public PermissionService(RoleService roleService)
    {
        _roleService = roleService;
    }

    public async Task<bool> HasPermission(AccountModel account, Permission permission)
    {
        Log(LogTag.PermissionService, $"Verificando permisos para la cuenta: {account.AccountId}, RolId: {account.RoleId}");

        RoleModel? accountRole = await _roleService.GetRoleByIdAsync(account.RoleId);

        if (accountRole == null)
        {
            Log(LogTag.PermissionService, "Rol no encontrado.", isError: true);
            return false;
        }

        Log(LogTag.PermissionService, $"Rol encontrado: {accountRole.DisplayName} ({accountRole.DisplayName})");
        Log(LogTag.PermissionService, $"Permisos del rol: {string.Join(", ", accountRole.Permissions)}");
        Log(LogTag.PermissionService, $"Permiso requerido: {permission}");

        bool hasPerm = accountRole.Permissions.Contains(permission);
        Log(LogTag.PermissionService, $"Resultado: {(hasPerm ? "PERMITIDO" : "DENEGADO")}");

        return hasPerm;
    }

    public List<PermissionDTO> GetAllPermissionDTOs()
    {
        return Enum.GetValues(typeof(Permission))
                   .Cast<Permission>()
                   .Select(p => new PermissionDTO
                   {
                       Name = p.ToString(),
                       Value = (int)p
                   })
                   .ToList();
    }
}
