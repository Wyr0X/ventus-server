using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentusServer.Models
{
    [Table("player_healths")]
    public class PlayerHealthEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlayerId { get; set; }

        public int Hp { get; set; }
        public int Mp { get; set; }
        public int Sp { get; set; }
        public int MaxHp { get; set; }
        public int MaxMp { get; set; }
        public int MaxSp { get; set; }

        // Relación con Player
        public virtual PlayerEntity Player { get; set; }
    }
}
