using System;
using System.Collections.Generic;

namespace VentusServer.DataAccess.Entities
{
    public class DbRoleEntity
    {
        public Guid Id { get; set; } // role_id
        public string Name { get; set; } = default!; // "owner", "admin", etc.
        public string DisplayName { get; set; } = default!; // Nombre visible para UI
        public bool IsEditable { get; set; } = true;

        // Relación M:M (a través de tabla intermedia, ej: role_permissions)
        public List<string> Permissions { get; set; } = new();
    }
}
