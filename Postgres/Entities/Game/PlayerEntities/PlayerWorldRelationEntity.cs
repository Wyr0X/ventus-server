using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentusServer.Models
{
    [Table("player_world_relations")]
    public class PlayerWorldRelationEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlayerId { get; set; }

        public int WorldId { get; set; }
        public int MapId { get; set; }

        // Relaciones con Player, World y Map
        public virtual PlayerEntity Player { get; set; }
        public virtual WorldEntity World { get; set; }
        public virtual MapEntity Map { get; set; }
    }
}
