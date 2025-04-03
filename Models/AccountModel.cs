using System;

public class Account
{
    public Guid AccountId { get; set; } = Guid.NewGuid();
    public required string Name { get; set; } // Nuevo campo

    public required string Email { get; set; }

    public required string Password { get; set; } // 🔹 Hasheada y salteada

    public bool IsDeleted { get; set; } = false;

    public bool IsBanned { get; set; } = false;

    public int Credits { get; set; } = 0;

    public required string LastIp { get; set; }

    public DateTime LastLogin { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
