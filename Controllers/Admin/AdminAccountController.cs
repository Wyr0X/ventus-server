using Microsoft.AspNetCore.Mvc;
using VentusServer.Auth;
using VentusServer.DTOs;
using VentusServer.DTOs.Admin;
using VentusServer.Models;
using VentusServer.Services;
using static LoggerUtil;

namespace VentusServer.Controllers.Admin
{
    [ApiController]
    [Route("admin/accounts")]
    [JwtAuthRequired]
    [RequirePermission]
    public class AdminAccountController : ControllerBase
    {
        private readonly IAccountService _IAccountService;
        private readonly RoleService _roleService;

        public AdminAccountController(IAccountService IAccountService, RoleService roleService)
        {
            _IAccountService = IAccountService;
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            Log(LogTag.AdminAccountController, "Solicitando lista de todas las cuentas...");

            var accounts = await _IAccountService.GetAllAccountsAsync();

            if (accounts == null || accounts.Count == 0)
            {
                Log(LogTag.AdminAccountController, "No se encontraron cuentas.");
                return NotFound("No se encontraron cuentas.");
            }


            Log(LogTag.AdminAccountController, $"Cuentas obtenidas: {accounts.Count}");
            return Ok(accounts);
        }


        [HttpPost("{id}/ban")]
        public async Task<IActionResult> BanAccount(Guid id)
        {
            Log(LogTag.AdminAccountController, $"Intentando banear cuenta: {id}");

            var account = await _IAccountService.GetOrCreateAccountInCacheAsync(id);
            if (account == null)
            {
                Log(LogTag.AdminAccountController, $"Cuenta no encontrada: {id}", isError: true);
                return NotFound("Cuenta no encontrada.");
            }

            account.IsBanned = true;
            await _IAccountService.SaveAccountAsync(account);

            Log(LogTag.AdminAccountController, $"Cuenta baneada: {id}");
            return Ok("Cuenta baneada.");
        }

        [HttpPost("{id}/unban")]
        public async Task<IActionResult> UnbanAccount(Guid id)
        {
            Log(LogTag.AdminAccountController, $"Intentando desbanear cuenta: {id}");

            var account = await _IAccountService.GetOrCreateAccountInCacheAsync(id);
            if (account == null)
            {
                Log(LogTag.AdminAccountController, $"Cuenta no encontrada: {id}", isError: true);
                return NotFound("Cuenta no encontrada.");
            }

            account.IsBanned = false;
            await _IAccountService.SaveAccountAsync(account);

            Log(LogTag.AdminAccountController, $"Cuenta desbaneada: {id}");
            return Ok("Cuenta desbaneada.");
        }

        public class ChangeRoleRequest
        {
            public string RoleName { get; set; } = string.Empty;
        }

        [HttpPost("{id}/role")]
        public async Task<IActionResult> ChangeRole(Guid id, [FromBody] ChangeRoleRequest request)
        {
            Log(LogTag.AdminAccountController, $"Intentando cambiar rol de cuenta {id} a '{request.RoleName}'");

            var account = await _IAccountService.GetOrCreateAccountInCacheAsync(id);
            if (account == null)
            {
                Log(LogTag.AdminAccountController, $"Cuenta no encontrada: {id}", isError: true);
                return NotFound("Cuenta no encontrada.");
            }

            var role = await _roleService.GetRoleByDisplayNameAsync(request.RoleName);
            if (role == null)
            {
                Log(LogTag.AdminAccountController, $"Rol no válido: {request.RoleName}", isError: true);
                return BadRequest("Rol no válido.");
            }

            account.RoleId = role.RoleId;
            await _IAccountService.SaveAccountAsync(account);

            Log(LogTag.AdminAccountController, $"Rol actualizado a '{role.DisplayName}' para cuenta {id}");
            return Ok($"Rol actualizado a '{role.DisplayName}'.");
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] UpdateAccountRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .Select(e => new
                    {
                        Field = e.Key,
                        Errors = e.Value?.Errors?.Select(err => err.ErrorMessage).ToList() ?? new List<string>()
                    });

                Log(LogTag.AdminAccountController, $"ModelState inválido: {System.Text.Json.JsonSerializer.Serialize(errors)}", isError: true);

                return BadRequest(ModelState);
            }
            Log(LogTag.AdminAccountController, $"Intentando actualizar cuenta {request.ToString()}...");

            var account = await _IAccountService.GetOrCreateAccountInCacheAsync(id);
            if (account == null)
            {
                Log(LogTag.AdminAccountController, $"Cuenta no encontrada: {id}", isError: true);
                return NotFound("Cuenta no encontrada.");
            }

            account.Email = request.Email;
            account.AccountName = request.AccountName;
            account.IsBanned = request.IsBanned;
            Log(LogTag.AdminAccountController, $"Rol a cambiar ${request.RoleName}");

            RoleModel? role = await _roleService.GetRoleByDisplayNameAsync(request.RoleName);
            if (role == null)
            {
                Log(LogTag.AdminAccountController, $"Rol invalido ${request.RoleName}", isError: true);
                return NotFound("Rol invalido.");
            }
            Log(LogTag.AdminAccountController, $"Rol a cambiar ${role.RoleId}");

            account.RoleId = role.RoleId;
            await _IAccountService.SaveAccountAsync(account);

            Log(LogTag.AdminAccountController, $"Cuenta actualizada: {id}");
            return Ok("Cuenta actualizada correctamente.");
        }

    }
}
