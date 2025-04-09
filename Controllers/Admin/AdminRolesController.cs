using Microsoft.AspNetCore.Mvc;
using VentusServer.Auth;
using VentusServer.Domain.Enums;
using VentusServer.DTOs.Admin;
using VentusServer.Services;
using static LoggerUtil;

[ApiController]
[Route("admin/roles")]
[JwtAuthRequired]
[RequirePermission]
public class AdminRolesController : ControllerBase
{
    private readonly RoleService _roleService;

    public AdminRolesController(RoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        Log(LogTag.AdminRolesController, "Solicitando todos los roles...");

        var roles = await _roleService.GetAllRolesAsync();
        if (roles == null || roles.Count == 0)
        {
            return NotFound("No se encontraron roles.");
        }

        var dto = roles.Select(role => new RoleSummaryDTO
        {
            RoleId = role.RoleId,
            DisplayName = role.DisplayName,
            Permissions = role.Permissions.Select(p => new PermissionDTO
            {
                Name = p.ToString(),
                Value = (int)p
            }).ToList(),
            IsEditable = role.IsEditable
        });

        return Ok(dto);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllDetailedRoles()
    {
        Log(LogTag.AdminRolesController, "Obteniendo todos los roles completos...");

        var roles = await _roleService.GetAllRolesAsync();
        return Ok(roles);
    }

    [HttpGet("{roleId}")]
    public async Task<IActionResult> GetRoleById(string roleId)
    {
        Log(LogTag.AdminRolesController, $"Buscando rol con ID: {roleId}");

        var role = await _roleService.GetRoleByIdAsync(roleId);
        if (role == null)
        {
            return NotFound("Rol no encontrado.");
        }

        var dto = new RoleSummaryDTO
        {
            RoleId = role.RoleId,
            DisplayName = role.DisplayName,
            Permissions = role.Permissions.Select(p => new PermissionDTO
            {
                Name = p.ToString(),
                Value = (int)p
            }).ToList(),
            IsEditable = role.IsEditable
        };

        return Ok(dto);
    }

    [HttpGet("available-permissions")]
    public IActionResult GetAllPermissions()
    {
        Log(LogTag.AdminRolesController, "Obteniendo todos los permisos disponibles...");

        var allPermissions = Enum.GetValues(typeof(Permission))
            .Cast<Permission>()
            .Select(p => new PermissionDTO
            {
                Name = p.ToString(),
                Value = (int)p
            }).ToList();

        return Ok(allPermissions);
    }
    [HttpPut("{roleId}")]
    public async Task<IActionResult> UpdateRole( [FromBody] UpdateRoleDTO updatedRole)
    {
        Log(LogTag.AdminRolesController, $"Actualizando rol con ID: {updatedRole.RoleId}");

        var success = await _roleService.UpdateRoleAsync(updatedRole);
        if (!success)
        {
            return BadRequest("No se pudo actualizar el rol.");
        }

        return Ok("Rol actualizado correctamente.");
    }

    [HttpDelete("{roleId}")]
    public async Task<IActionResult> DeleteRole(string roleId)
    {
        Log(LogTag.AdminRolesController, $"Eliminando rol con ID: {roleId}");

        var success = await _roleService.DeleteRoleAsync(roleId);
        if (!success)
        {
            return BadRequest("No se pudo eliminar el rol.");
        }

        return Ok("Rol eliminado correctamente.");
    }
}
