using System;
using Game.Models;
using System.Collections.Generic;

namespace Game.World
{
    public class GameMapObject
    {
        public MapModel Model { get; }
        public int Width { get; }
        public int Height { get; }
        public int VisionRange { get; }

        private readonly TerrainType[,] _terrain;

        public GameMapObject(MapModel model, TerrainType[,] terrain, int visionRange = 10)
        {
            Model = model;
            Width = terrain.GetLength(0);
            Height = terrain.GetLength(1);
            _terrain = terrain;
            VisionRange = visionRange;
        }

        public TerrainType GetTerrainAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return TerrainType.Impassable;

            return _terrain[x, y];
        }

        public bool IsWalkable(int x, int y)
        {
            return GetTerrainAt(x, y) != TerrainType.Impassable;
        }

        public bool HasLineOfSight(Vec2 from, Vec2 to)
        {
            int x0 = (int)Math.Floor(from.X);
            int y0 = (int)Math.Floor(from.Y);
            int x1 = (int)Math.Floor(to.X);
            int y1 = (int)Math.Floor(to.Y);

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                if (!IsWalkable(x0, y0))
                    return false;

                if (x0 == x1 && y0 == y1)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }

            return true;
        }

        public bool IsWater(Vec2 position) => GetTerrainAt((int)position.X, (int)position.Y) == TerrainType.Water;
        public bool IsLava(Vec2 position) => GetTerrainAt((int)position.X, (int)position.Y) == TerrainType.Lava;
        public bool IsImpassable(Vec2 position) => GetTerrainAt((int)position.X, (int)position.Y) == TerrainType.Impassable;

        public bool IsInSafeZone(Vec2 position) => false;

        public override string ToString()
            => $"Map(Name={Model.Name}, Size={Width}x{Height}, Players={Model.PlayerCount}/{Model.MaxPlayers})";
    }
}
namespace Game.World
{
    public enum TerrainType
    {
        Normal = 0,         // Se puede caminar normalmente
        Grass = 1,          // Caminable, tal vez ralentiza ligeramente
        Sand = 2,           // Caminable, ralentiza el movimiento
        Water = 3,          // No caminable por personajes sin habilidad especial
        Lava = 4,           // No caminable, daña al contacto
        Ice = 5,            // Caminable, reduce fricción (más deslizamiento)
        Impassable = 6,     // No se puede caminar bajo ninguna circunstancia
        Bridge = 7,         // Permitido solo sobre agua/lava
        Teleport = 8,       // Zona de teletransporte automático
        DungeonEntrance = 9, // Entrada especial a dungeon
        Cliff = 10,         // Solo bajable si se permite saltar
        Void = 99           // Zona inválida o fuera del mundo
    }
}
