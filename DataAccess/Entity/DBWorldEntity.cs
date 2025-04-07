namespace VentusServer.DataAccess.Entities
{
    public class DbWorldEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int MaxMaps { get; set; }
        public int MaxPlayers { get; set; }
        public int LevelRequirements { get; set; }
    }
}
