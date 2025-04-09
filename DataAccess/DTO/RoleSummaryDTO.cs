using System;
using System.Collections.Generic;
using VentusServer.DTOs;

namespace VentusServer.DTOs.Admin
{
    public class RoleSummaryDTO
    {
        public string RoleId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public List<PermissionDTO> Permissions { get; set; } = new();
        public bool IsEditable { get; set; } = true;
    }
}
