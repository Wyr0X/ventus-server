using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentusServer.Models
{
    [Table("player_event_histories")]
    public class PlayerEventHistoryEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlayerId { get; set; }

        public int KilledNpcs { get; set; }
        public int KilledUsers { get; set; }

        // Esto puede ser un campo JSON o String, dependiendo del tipo de base de datos que uses
        [Required]
        public string EventsSerialized { get; set; }

        // Relación con Player
        public virtual PlayerEntity Player { get; set; }
    }
}
