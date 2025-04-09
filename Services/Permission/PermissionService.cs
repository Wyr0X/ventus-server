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
        Console.WriteLine($"[PermissionService] Verificando permisos para la cuenta: {account.AccountId}, RolId: {account.RoleId}");

        RoleModel? accountRole = await _roleService.GetRoleByIdAsync(account.RoleId);

        if (accountRole == null)
        {
            Console.WriteLine("[PermissionService] Rol no encontrado.");
            return false;
        }

        Console.WriteLine($"[PermissionService] Rol encontrado: {accountRole.DisplayName} ({accountRole.DisplayName})");
        Console.WriteLine($"[PermissionService] Permisos del rol: {string.Join(", ", accountRole.Permissions)}");
        Console.WriteLine($"[PermissionService] Permiso requerido: {permission}");

        bool hasPerm = accountRole.Permissions.Contains(permission);
        Console.WriteLine($"[PermissionService] Resultado: {(hasPerm ? "PERMITIDO" : "DENEGADO")}");

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
