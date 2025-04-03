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
        [JwtAuthRequired] // 🔒 Protegemos esta ruta con JWT
        public async Task<IActionResult> GetAccountInfo()
        {
            try
            {
                var accountIdParam = HttpContext.Items["AccountId"]?.ToString(); // Recuperamos el UserId del middleware
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
                    account.Name,
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
    }
}
