using VentusServer.DataAccess.Interfaces;
using VentusServer.Domain.Enums;

namespace VentusServer.DataAccess.Seeders
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(IRoleDAO roleDAO)
        {
            var roles = new List<RoleModel>
            {
                new RoleModel
                {
                    RoleId ="owner",
                    DisplayName = "Dueño",
                    IsEditable = false,
                    Permissions = Enum.GetValues<Permission>().ToList()
                },
                new RoleModel
                {
                    RoleId = "admin",
                    DisplayName = "Administrador",
                    IsEditable = false,
                    Permissions = new List<Permission>
                    {
                        Permission.ManageRoles,
                        Permission.AssignRoles,
                        Permission.ViewLogs,
                        Permission.BanAccount,
                        Permission.UnbanAccount,
                        Permission.ViewAccounts,
                        Permission.EditAccountCredits,
                        Permission.CreateMap,
                        Permission.EditMap,
                        Permission.DeleteMap,
                        Permission.ViewPlayers,
                        Permission.KickPlayer,
                        Permission.MutePlayer,
                        Permission.EditPlayerStats,
                        Permission.MonitorChats
                    }
                },
                new RoleModel
                {
                    RoleId = "moderator",
                    DisplayName = "Moderador",
                    IsEditable = false,
                    Permissions = new List<Permission>
                    {
                        Permission.BanAccount,
                        Permission.ViewAccounts,
                        Permission.WarnPlayer,
                        Permission.MonitorChats,
                        Permission.TemporaryBan,
                        Permission.KickPlayer,
                        Permission.MutePlayer,
                        Permission.ViewPlayers
                    }
                },
                new RoleModel
                {
                    RoleId = "apprentice",
                    DisplayName = "Aprendiz",
                    IsEditable = true,
                    Permissions = new List<Permission>
                    {
                        Permission.MonitorChats,
                        Permission.WarnPlayer
                    }
                },
                   new RoleModel
                {
                    RoleId ="user",
                    DisplayName = "Usuario",
                    IsEditable = false,
                    Permissions = new List<Permission>
                    {
                      
                    }
                },
            };

            // TODO: Insertar roles si no existen en la base de datos
            // Ejemplo (requiere IRoleDAO o similar):
            foreach (var role in roles)
            {

                var exists = await roleDAO.GetRoleByIdAsync(role.RoleId);

                if (exists is null)
                {

                    await roleDAO.CreateRoleAsync(role);
                }
            }
        }
    }
}
