namespace VentusServer.DataAccess.Entities
{
    public class DbMapEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MinLevel { get; set; }
        public int MaxPlayers { get; set; }
        public int WorldId { get; set; }
    }
}
