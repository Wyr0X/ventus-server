using VentusServer.Models; // O donde tengas definidos los enums Gender y Race

public class CreatePlayerDTO
{
    public required string Name { get; set; }
    public required Gender Gender { get; set; }
    public required Race Race { get; set; }
    public required CharacterClass Class { get; set; }
}
