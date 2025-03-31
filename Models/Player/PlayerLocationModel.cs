namespace Game.Models
{
    public class PlayerLocation
    {
        public int PosX { get; set; }
        public int PosY { get; set; }

        // Relaciones
        public PlayerModel Player { get; set; }
        public World World { get; set; }
        
        public MapModel Map { get; set; }


    }
}