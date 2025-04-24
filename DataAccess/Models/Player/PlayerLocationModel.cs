
using Game.Models;

public class PlayerLocationModel : BaseModel
{
    public int PosX { get; set; }
    public int PosY { get; set; }

    // Relaciones
    public required int PlayerId { get; set; }
    public required WorldModel World { get; set; }

    public required MapModel Map { get; set; }


}
