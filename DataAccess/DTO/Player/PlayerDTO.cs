
namespace VentusServer.Models
{
    public class PlayerDTO
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Gender { get; set; }
        public required string Race { get; set; }
        public int Level { get; set; }
        public required string Class { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string Status { get; set; }
        public required string AccountName { get; set; }
    }
}
