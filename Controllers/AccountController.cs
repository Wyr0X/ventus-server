using Microsoft.AspNetCore.Mvc;
using VentusServer.DataAccess;
using VentusServer.Models;
using System;
using System.Threading.Tasks;
using VentusServer.Auth;
using VentusServer.DataAccess.Postgres;

namespace VentusServer.Controllers
{
    [Route("account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly PostgresAccountDAO _accountDAO;

        public AccountController(PostgresAccountDAO accountDAO)
        {
            _accountDAO = accountDAO;
        }

        [HttpGet]
        [JwtAuthRequired]
        public async Task<IActionResult> GetAccountInfo()
        {
            try
            {
                var accountIdParam = HttpContext.Items["AccountId"]?.ToString();
                if (string.IsNullOrEmpty(accountIdParam) || !Guid.TryParse(accountIdParam, out Guid accountId))
                {
                    return BadRequest("Error al obtener la cuenta.");
                }

                var account = await _accountDAO.GetAccountByAccountIdAsync(accountId);
                if (account == null)
                {
                    return NotFound("Cuenta no encontrada.");
                }

                return Ok(new
                {
                    account.Email,
                    account.AccountName,
                    account.Credits,
                    account.CreatedAt,
                    account.IsBanned
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut("/change-name")]
        [JwtAuthRequired]
        public async Task<IActionResult> ChangeName([FromBody] ChangeNameRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NewName))
            {
                return BadRequest("El nombre no puede estar vacío.");
            }

            var accountIdParam = HttpContext.Items["AccountId"]?.ToString();
            if (string.IsNullOrEmpty(accountIdParam) || !Guid.TryParse(accountIdParam, out Guid accountId))
            {
                return BadRequest("Error al obtener la cuenta.");
            }

            var existingAccount = await _accountDAO.GetAccountByNameAsync(request.NewName);
            if (existingAccount != null)
            {
                return Conflict("El nombre ya está en uso.");
            }

            var success = await _accountDAO.UpdateAccountNameAsync(accountId, request.NewName);
            if (!success)
            {
                return StatusCode(500, "Error al actualizar el nombre.");
            }

            return Ok("Nombre actualizado correctamente.");
        }

        [HttpPut("/change-password")]
        [JwtAuthRequired]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword) || string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                return BadRequest("Las contraseñas no pueden estar vacías.");
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest("Las contraseñas no coinciden.");
            }

            var accountIdParam = HttpContext.Items["AccountId"]?.ToString();
            if (string.IsNullOrEmpty(accountIdParam) || !Guid.TryParse(accountIdParam, out Guid accountId))
            {
                return BadRequest("Error al obtener la cuenta.");
            }

            var success = await _accountDAO.UpdateAccountPasswordAsync(accountId, request.NewPassword);
            if (!success)
            {
                return StatusCode(500, "Error al actualizar la contraseña.");
            }

            return Ok("Contraseña actualizada correctamente.");
        }
        
    }

    public class ChangeNameRequest
    {
        public string NewName { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}