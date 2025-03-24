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
    public class AccountController : ControllerBase
    {
        private readonly IAccountDAO _accountDAO;
        private readonly FirebaseService _firebaseService;


        public AccountController(IAccountDAO accountDAO, FirebaseService firebaseService)
        {
            _accountDAO = accountDAO;
            _firebaseService = firebaseService;
        }

        [HttpGet]
        [FirebaseAuthRequired]
        public async Task<IActionResult> GetAccountInfo()
        {
            try
            {
              
                // Obtener el token de Bearer del encabezado Authorization
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized("Token de autenticación no encontrado.");
                }

                // Verificar el token de Firebase
                var decodedToken = await _firebaseService.VerifyTokenAsync(token);
                var userId = decodedToken.Uid; // Obtener el userId del token
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
