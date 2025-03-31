namespace Game.DTOs
{
    public class PlayerLocationDTO
    {
        public int PosX { get; set; }
        public int PosY { get; set; }

        // Información del mapa donde está el jugador
        public MapDTO Map { get; set; }

        // Información del mundo donde está el mapa
        public WorldDTO World { get; set; }
    }

    public class MapDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MinLevel { get; set; }
        public int MaxPlayers { get; set; }
    }

    public class WorldDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class PlayerDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Race { get; set; }
        public int Level { get; set; }
        public string Class { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public string Status { get; set; }

        // Información de la ubicación del jugador
        public PlayerLocationDTO Location { get; set; }
    }

    public class GetPlayersResponseDTO
    {
        public List<PlayerDTO> Players { get; set; }
    }
}
