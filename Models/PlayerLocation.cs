using System;

namespace Game.Models
{
    public class PlayerLocation
    {
        public Player Player { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public World World { get; set; }
        public Map Map { get; set; }

        // Relaciones
        public Player Player { get; set; }
        public PlayerWorldRelation PlayerWorldRelation { get; set; }

        // Lógica de negocio
        public void Move(int newPosX, int newPosY, string newDirection)
        {
            PosX = newPosX;
            PosY = newPosY;
            Direction = newDirection;
        }

        public void UpdateLocation(string newMap, int newPosX, int newPosY, string newDirection)
        {
            PosMap = newMap;
            Move(newPosX, newPosY, newDirection);
        }

        public bool IsInSameLocation(int checkPosX, int checkPosY)
        {
            return PosX == checkPosX && PosY == checkPosY;
        }
    }
}