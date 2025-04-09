using System;

namespace VentusServer.DTOs
{
    public class AccountDTO
    {
        public Guid AccountId { get; set; }
        public string? Email { get; set; }
        public string? AccountName { get; set; }
        public bool IsBanned { get; set; }
        public string RoleName { get; set; } = "Jugador";
        public DateTime CreatedAt { get; set; }
        public int? ActivePlayerId { get; set; }
    }
}
