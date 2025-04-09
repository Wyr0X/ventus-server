using System;
using System.Collections.Generic;

namespace VentusServer.DTOs.Admin
{
    public class AccountAdminInfoDTO
    {
        public List<AccountDTO> Accounts { get; set; } = new();
        public List<RoleSummaryDTO> Roles { get; set; } = new();
    }

   
}
