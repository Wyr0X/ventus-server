using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentusServer.Models
{
    [Table("player_progressions")]
    public class PlayerProgressionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlayerId { get; set; }  // Relación con Player

        [Required]
        public int Level { get; set; }

        [Required]
        public int Xp { get; set; }

        [Required]
        public int FreeSkillPoints { get; set; }

        [Required]
        public int KilledNpcs { get; set; }

        [Required]
        public int KilledUsers { get; set; }

        [Required]
        public int Deaths { get; set; }

        [Required]
        public int Hp { get; set; }

        [Required]
        public int Mp { get; set; }

        [Required]
        public int Sp { get; set; }

        // Relación con Player
        [ForeignKey("PlayerId")]
        public virtual PlayerEntity Player { get; set; }

        // Métodos (Opcionales, si los quieres mantener en la entidad de EF Core)
        // Nota: Las lógicas como GainExperience, DistributeSkillPoints, etc. no se suelen colocar en las entidades de EF, ya que EF se centra solo en el mapeo de datos.
    }
}
