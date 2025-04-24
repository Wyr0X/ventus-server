using System;

namespace VentusServer.DataAccess.Entities
{
    public class DbAccountEntity
    {
        public Guid AccountId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public bool IsBanned { get; set; }
        public int Credits { get; set; }
        public string? LastIpAddress { get; set; }
        public DateTime? LastLogin { get; set; }
        public Guid SessionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string RoleId { get; set; } = "user";
    }
}
