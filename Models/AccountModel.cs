using System;

public class AccountModel
{
    public Guid AccountId { get; set; } = Guid.NewGuid();
    public required string Name { get; set; } // Nuevo campo

    public required string Email { get; set; }

    public required string Password { get; set; } // 🔹 Hasheada y salteada

    public bool IsDeleted { get; set; } = false;

    public bool IsBanned { get; set; } = false;

    public int Credits { get; set; } = 0;

    public required string LastIp { get; set; }
    public int? ActivePlayer { get; set; } = null;

    public DateTime LastLogin { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public void PrintInfo()
    {
        Console.ForegroundColor = ConsoleColor.Blue;

        Console.WriteLine("\n############################");
        Console.WriteLine($"AccountId:    {this.AccountId}");
        Console.WriteLine($"Name:         {this.Name}");
        Console.WriteLine($"Email:        {this.Email}");
        Console.WriteLine($"Password:     {this.Password}");
        Console.WriteLine($"IsDeleted:    {this.IsDeleted}");
        Console.WriteLine($"IsBanned:     {this.IsBanned}");
        Console.WriteLine($"Credits:      {this.Credits}");
        Console.WriteLine($"LastIp:       {this.LastIp}");
        Console.WriteLine($"ActivePlayer: {this.ActivePlayer}");
        Console.WriteLine($"LastLogin:    {this.LastLogin}");
        Console.WriteLine($"CreatedAt:    {this.CreatedAt}");
        Console.WriteLine("############################\n");

        Console.ResetColor(); // Restablece el color original
    }

}
