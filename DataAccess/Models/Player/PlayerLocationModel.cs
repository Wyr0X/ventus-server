
using Game.Models;

public class PlayerLocation  : BaseModel
{
    public int PosX { get; set; }
    public int PosY { get; set; }

    // Relaciones
    public required PlayerModel Player { get; set; }
    public required WorldModel World { get; set; }

    public required MapModel Map { get; set; }


}
