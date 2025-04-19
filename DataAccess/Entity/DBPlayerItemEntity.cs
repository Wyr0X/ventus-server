using System;

namespace VentusServer.DataAccess.Entities
{
    public class DbPlayerInventoryEntity
    {
        public Guid Id { get; set; }  // UUID de la tabla player_inventory
        public int PlayerId { get; set; }  // FK al jugador

        public int Gold { get; set; }  // Oro que posee

        public DateTime CreatedAt { get; set; }  // Fecha de creación
        public DateTime UpdatedAt { get; set; }  // Fecha de última modificación
    }
}
