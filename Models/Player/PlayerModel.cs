using System;
using System.Collections.Generic;

public class PlayerModel
{
    public int Id { get; set; }
    public string UserId { get; set; }  // Relación con la cuenta
    public string Name { get; set; }
    public string Gender { get; set; }
    public string Race { get; set; }
    public int Level { get; set; }
    public string Class { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLogin { get; set; }
    public string Status { get; private set; } // Estado: "Online", "Offline", "Banned"


    public PlayerModel(int id, string userId, string name, string gender, string race, int level, string playerClass)
    {
        Id = id;
        UserId = userId;
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


}

