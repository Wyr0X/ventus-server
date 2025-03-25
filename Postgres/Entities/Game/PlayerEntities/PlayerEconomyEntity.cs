using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VentusServer.Models;

namespace VentusServer.Models

{
    [Table("player_economy")]
    public class PlayerEconomyEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlayerId { get; set; }  // Relación con Player

        [Required]
        public decimal Gold { get; set; } // Oro disponible para el jugador

        [Required]
        public decimal BankGold { get; set; } // Oro almacenado en el banco

        [Required]
        public int Hunger { get; set; } // Nivel de hambre

        [Required]
        public int Thirst { get; set; } // Nivel de sed

        // Relación con Player (clave foránea)
        [ForeignKey("PlayerId")]
        public virtual PlayerEntity Player { get; set; } 
    }
}
