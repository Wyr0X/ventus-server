using System;
using System.Collections.Generic;
using VentusServer.Domain.Models;

public class PlayerModel : BaseModel
{
    public required int Id { get; set; }
    public required Guid AccountId { get; set; }  // Relación con la cuenta
    public required string Name { get; set; }
    public required Gender Gender { get; set; }
    public required Race Race { get; set; }
    public int Level { get; set; }
    public required CharacterClass Class { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLogin { get; set; }
    public Boolean isSpawned { get; set; } = false;
    public PlayerInventoryModel? Inventory { get; set; }
    public PlayerStatsModel? Stats { get; set; }
    public PlayerLocationModel? Location { get; set; }
    public PlayerSpellsModel? Spells { get; set; }


    public string Status { get; set; } = "Offline"; // Estado: "Online", "Offline", "Banned"




    public void UpdateStatus(string newStatus)
    {
        Status = newStatus;
    }

    public void PrintInfo()
    {
        Console.ForegroundColor = ConsoleColor.Blue;

        Console.WriteLine("\n##################################");
        Console.WriteLine("           PLAYER INFO            ");
        Console.WriteLine("##################################");
        Console.WriteLine($"ID:          {Id}");
        Console.WriteLine($"Account ID:  {AccountId}");
        Console.WriteLine($"Name:        {Name}");
        Console.WriteLine($"Gender:      {Gender}");
        Console.WriteLine($"Race:        {Race}");
        Console.WriteLine($"Level:       {Level}");
        Console.WriteLine($"Class:       {Class}");
        Console.WriteLine($"Created At:  {CreatedAt}");
        Console.WriteLine($"Last Login:  {LastLogin}");
        Console.WriteLine($"Status:      {Status}");
        Console.WriteLine($"Is Spawned:  {isSpawned}");
        Console.WriteLine("##################################\n");

        Console.ResetColor();
    }

}

