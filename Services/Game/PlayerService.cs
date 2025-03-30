using Game.Models;

public class PlayerService
{
    public PlayerBasicModel GetPlayerById(string playerId)
    {
        // Lógica para obtener el jugador por su ID
        return new PlayerBasicModel
        {
            Id = 1,
            AccountId = 1001,
            Name = "Arthas",
            Gender = "Male",
            Race = "Human",
            Level = 10,
            Class = "Paladin",
            CreatedAt = DateTime.Now.AddMonths(-6),
            LastLogin = DateTime.Now,
            Status = "Online",
        };
        ;
    }
}
