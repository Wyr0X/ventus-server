using System;
using System.ComponentModel.DataAnnotations;

public class AccountModel  : BaseModel
{
    // Identificación
    public Guid AccountId { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    public string AccountName { get; set; } = default!;

    [Required]
    public string PasswordHash { get; set; } = default!;

    // Estado
    public bool IsDeleted { get; set; }
    public bool IsBanned { get; set; }

    public bool IsActive() => !IsDeleted && !IsBanned;

    // Datos adicionales
    public int Credits { get; set; }
    public string LastIpAddress { get; set; } = default!;
    public int? ActivePlayerId { get; set; }

    // Sesión
    public Guid SessionId { get; set; }

    // Tiempos
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedAt { get; set; }

    public override string ToString()
        => $"[Account: {AccountName} | Email: {Email} | ID: {AccountId}]";

    public void PrintInfo()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"""
            \n############################
            AccountId:       {AccountId}
            AccountName:     {AccountName}
            Email:           {Email}
            PasswordHash:    {PasswordHash}
            IsDeleted:       {IsDeleted}
            IsBanned:        {IsBanned}
            Credits:         {Credits}
            LastIpAddress:   {LastIpAddress}
            ActivePlayerId:  {ActivePlayerId}
            SessionId:       {SessionId}
            LastLogin:       {LastLogin}
            CreatedAt:       {CreatedAt}
            ############################\n
        """);
        Console.ResetColor();
    }
}
