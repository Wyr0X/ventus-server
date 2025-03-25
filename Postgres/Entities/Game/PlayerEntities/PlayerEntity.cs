using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VentusServer.Database.Entities;

namespace VentusServer.Models
{
    [Table("player_economies")]
    public class PlayerEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int AccountId { get; set; } // Relación con la cuenta

        [ForeignKey("AccountId")]
        public AccountEntity Account { get; set; }  // Propiedad de navegación

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public string Gender { get; set; }
        public string Race { get; set; }
        public string Class { get; set; }
        public int Level { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastLogin { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } // Puede representar rol o afiliación

    
    }
}
