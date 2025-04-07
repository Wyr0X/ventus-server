namespace VentusServer.DataAccess.Entities
{
    public class DbLocationEntity
    {
        public int PlayerId { get; set; }
        public int WorldId { get; set; }
        public int MapId { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public string Direction { get; set; } = "down"; // Default direction if needed
    }
}
