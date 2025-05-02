
using Game.Models;

public class PlayerLocationModel : BaseModel
{
    public int PosX { get; set; }
    public int PosY { get; set; }

    // Relaciones
    public required int PlayerId { get; set; }
    public required int WorldId { get; set; }

    public required int MapId { get; set; }


}
