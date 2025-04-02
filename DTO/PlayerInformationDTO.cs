namespace Game.DTOs
{
    public class PlayerLocationDTO
    {
        public int PosX { get; set; }
        public int PosY { get; set; }

        // Información del mapa donde está el jugador
        public required MapDTO Map { get; set; }

        // Información del mundo donde está el mapa
        public required WorldDTO WorldModel { get; set; }
    }

    public class MapDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int MinLevel { get; set; }
        public int MaxPlayers { get; set; }
    }

    public class WorldDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
    }

    public class PlayerDTO
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string Name { get; set; }
        public required string Gender { get; set; }
        public required string Race { get; set; }
        public int Level { get; set; }
        public required string Class { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public required string Status { get; set; }

        // Información de la ubicación del jugador
        public required PlayerLocationDTO Location { get; set; }
    }

    public class GetPlayersResponseDTO
    {
        public required List<PlayerDTO> Players { get; set; }
    }
}
