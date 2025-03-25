using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentusServer.Models
{
    [Table("player_locations")]
    public class PlayerLocationEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlayerId { get; set; }

        public int PlayerWorldRelationId { get; set; }
        public string PosMap { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public string Direction { get; set; }

        // Relaciones con Player y PlayerWorldRelation
        public virtual PlayerEntity Player { get; set; }
        public virtual PlayerWorldRelationEntity PlayerWorldRelation { get; set; }
    }
}
