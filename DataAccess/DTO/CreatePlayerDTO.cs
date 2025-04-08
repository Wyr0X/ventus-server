using VentusServer.Models; // O donde tengas definidos los enums Gender y Race

public class CreatePlayerDTO
{
    public string Name { get; set; }
    public Gender Gender { get; set; }
    public Race Race { get; set; }
    public string Class { get; set; }
}
