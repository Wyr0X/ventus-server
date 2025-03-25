using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VentusServer.Models;

namespace VentusServer.Models
{
    [Table("player_times")]
    public class PlayerTimeEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlayerId { get; set; }

        public DateTime LastLogin { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int Deaths { get; set; }

        // Relación con Player
        public virtual PlayerEntity Player { get; set; }
    }
}
