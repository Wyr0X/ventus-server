namespace VentusServer.DTOs.Admin
{
    public class UpdateAccountRequestDTO
    {
        public required string Email { get; set; }
        public required string AccountName { get; set; }
        public required bool IsBanned { get; set; }
        public required string RoleName { get; set; } // opcional, si querés hacerlo desde este endpoint
    }
}
