using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentusServer.Models
{
    [Table("maps")]
    public class MapEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Coordinates { get; set; }

        public int MinLevel { get; set; }
        public int MaxPlayers { get; set; }

        [MaxLength(500)]
        public string Features { get; set; }

        public int WorldId { get; set; }  // Relación con el mundo

        // Relación con World
        public virtual WorldEntity World { get; set; }

        // Relación con PlayerWorldRelation
        public virtual ICollection<PlayerWorldRelationEntity> PlayerWorldRelations { get; set; }  // Relación entre jugadores y mapas
    }
}
