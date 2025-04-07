using System;
using System.Collections.Generic;

public class PlayerModel  : BaseModel
{
    public int Id { get; set; }
    public Guid AccountId { get; set; }  // Relación con la cuenta
    public string Name { get; set; }
    public string Gender { get; set; }
    public string Race { get; set; }
    public int Level { get; set; }
    public string Class { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLogin { get; set; }
    public Boolean isSpawned { get; set; } = false;

    public string Status { get; set; } // Estado: "Online", "Offline", "Banned"


    public PlayerModel(int id, Guid accountId, string name, string gender, string race, int level, string playerClass)
    {
        Id = id;
        AccountId = accountId;
        Name = name;
        Gender = gender;
        Race = race;
        Level = level;
        Class = playerClass;
        CreatedAt = DateTime.Now;
        LastLogin = DateTime.Now;
        Status = "Offline";
    }

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

