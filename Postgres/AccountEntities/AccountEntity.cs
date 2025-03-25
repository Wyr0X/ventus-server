using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VentusServer.Models;

namespace VentusServer.Database.Entities
{
    [Table("accounts")] // Especifica el nombre de la tabla en la BD
    public class AccountEntity
    {
        [Key]
        [Column("user_id")]
        public string UserId { get; set; } = null!; // UID de Firebase, clave primaria

        [Required]
        [Column("email")]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        [Column("password")]
        [MaxLength(255)]
        public string Password { get; set; } = null!; // Hasheada

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        [Column("is_banned")]
        public bool IsBanned { get; set; } = false;

        [Column("credits")]
        public int Credits { get; set; } = 0; // Moneda premium

        [Column("last_ip")]
        [MaxLength(45)]
        public string? LastIp { get; set; }

        [Column("last_login")]
        public DateTime LastLogin { get; set; } = DateTime.UtcNow;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("name")]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        // Relación uno a muchos: una cuenta puede tener varios personajes (PlayerEntity)
        public List<PlayerEntity> Players { get; set; } = new List<PlayerEntity>();
    }
}
