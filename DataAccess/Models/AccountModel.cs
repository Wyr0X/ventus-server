using System;

public class AccountModel
{
    public Guid AccountId { get; set; }

    public required string AccountName { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsBanned { get; set; }

    public int Credits { get; set; }

    public required string LastIpAddress { get; set; }

    public int? ActivePlayerId { get; set; }

    public Guid SessionId { get; set; }

    public DateTime TokenIssuedAt { get; set; }

    public DateTime LastLogin { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive()
    {
        return !IsDeleted && !IsBanned;
    }

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
        Console.WriteLine($"SessionId:       {SessionId}");
        Console.WriteLine($"TokenIssuedAt:   {TokenIssuedAt}");
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
