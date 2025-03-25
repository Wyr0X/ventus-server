namespace VentusServer.Models
{
    public class RegisterRequest
    {
        public required string Name { get; set; }  // Nombre del usuario
        public required string Email { get; set; }  // Email del usuario
        public required string Password { get; set; }  // Contraseña (si quieres agregarla más adelante)
        public required string Token  { get; set; }
        
    }
}
