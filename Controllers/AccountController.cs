using Microsoft.AspNetCore.Mvc;
using VentusServer.DataAccess;
using VentusServer.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;

namespace VentusServer.Controllers
{
    [Route("account")]
    [ApiController]
    [Authorize] // Asegura que se requiere un token para acceder a esta ruta
    public class AccountController : ControllerBase
    {
        private readonly IAccountDAO _accountDAO;

        public AccountController(IAccountDAO accountDAO)
        {
            _accountDAO = accountDAO;
        }

        [HttpGet]
        public async Task<IActionResult> GetAccountInfo()
        {
            try
            {
                // Obtener el token desde el encabezado Authorization
                Console.WriteLine("Entra"); // Log del token recibido

                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                Console.WriteLine("Token recibido: 2 " + token); // Log del token recibido

                // Decodificar el token y obtener el userId
                string userId = TokenUtils.DecodeTokenAndGetUserId(token);
                Console.WriteLine("userId decodificado: 2 " + userId); // Log del userId decodificado

                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("Token inválido o no proporcionado."); // Log de token inválido
                    return Unauthorized("Token inválido o no proporcionado.");
                }

                // Buscar la cuenta en la base de datos usando el userId
                var account = await _accountDAO.GetAccountByUserIdAsync(userId);
                Console.WriteLine("Cuenta encontrada: " + (account != null ? "Sí" : "No")); // Log de si la cuenta fue encontrada

                if (account == null)
                {
                    Console.WriteLine("Cuenta no encontrada para el userId: " + userId); // Log si la cuenta no existe
                    return NotFound("Cuenta no encontrada.");
                }

                // Devolver la información de la cuenta
                Console.WriteLine("Devolviendo información de la cuenta: " + account.Email); // Log de la cuenta devuelta
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
                // Manejar errores generales
                Console.WriteLine("Error al obtener la información de la cuenta: " + ex.Message); // Log del error
                return BadRequest($"Error al obtener la información de la cuenta: {ex.Message}");
            }
        }
    }
}
