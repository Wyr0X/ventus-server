using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VentusServer.Models
{
    [Table("worlds")]
    public class WorldEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public int MaxMaps { get; set; }
        public int MaxPlayers { get; set; }
        public int LevelRequirements { get; set; }

        // Relaciones
        public virtual ICollection<MapEntity> Maps { get; set; }  // Relación con los mapas
        public virtual ICollection<PlayerWorldRelationEntity> PlayerRelations { get; set; }  // Relación con los jugadores

        // Métodos lógicos se pueden agregar si se necesitan en el contexto de la base de datos
    }
}
