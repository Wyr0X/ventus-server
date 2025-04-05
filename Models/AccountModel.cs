using System;

public class AccountModel
{
    public Guid AccountId { get; set; } = Guid.NewGuid();

    public required string AccountName { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; } // Hasheada y salteada

    public bool IsDeleted { get; set; } = false;

    public bool IsBanned { get; set; } = false;

    public int Credits { get; set; } = 0;

    public required string LastIpAddress { get; set; }

    public int? ActivePlayerId { get; set; } = null;

    public string ValidToken { get; set; }
    public DateTime TokenIssuedAt { get; set; } 
    public DateTime LastLogin { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Devuelve si la cuenta está activa y no bloqueada.
    /// </summary>
    public bool IsActive()
    {
        return !IsDeleted && !IsBanned;
    }

    /// <summary>
    /// Debug friendly info con colores en consola.
    /// </summary>
    public void PrintInfo()
    {
        Console.ForegroundColor = ConsoleColor.Blue;

        Console.WriteLine("\n############################");
        Console.WriteLine($"AccountId:       {AccountId}");
        Console.WriteLine($"AccountName:     {AccountName}");
        Console.WriteLine($"Email:           {Email}");
        Console.WriteLine($"PasswordHash:    {PasswordHash}");
        Console.WriteLine($"IsDeleted:       {IsDeleted}");
        Console.WriteLine($"IsBanned:        {IsBanned}");
        Console.WriteLine($"Credits:         {Credits}");
        Console.WriteLine($"LastIpAddress:   {LastIpAddress}");
        Console.WriteLine($"ActivePlayerId:  {ActivePlayerId}");
        Console.WriteLine($"LastLogin:       {LastLogin}");
        Console.WriteLine($"CreatedAt:       {CreatedAt}");
        Console.WriteLine("############################\n");

        Console.ResetColor();
    }

    public override string ToString()
    {
        return $"[Account: {AccountName} | Email: {Email} | ID: {AccountId}]";
    }
}
