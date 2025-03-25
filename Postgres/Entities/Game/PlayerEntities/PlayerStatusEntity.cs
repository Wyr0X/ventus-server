using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VentusServer.Models;

namespace VentusServer.Models
{
    [Table("player_status")]
    public class PlayerStatusEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlayerId { get; set; }  // Relación con Player

        [Required]
        public bool IsPoisoned { get; set; } // Indica si el jugador está envenenado

        [Required]
        public bool IsOnFire { get; set; } // Indica si el jugador está en fuego

        [Required]
        public bool IsFrozen { get; set; } // Indica si el jugador está congelado

        [Required]
        public bool IsBanned { get; set; } // Indica si el jugador está baneado

        // Relación con Player (clave foránea)
        [ForeignKey("PlayerId")]
        public virtual PlayerEntity Player { get; set; } 
    }
}
