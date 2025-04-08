namespace VentusServer.Domain.Enums
{
    public enum Permission
    {
        // 🧑‍💼 Administración General
        ManageRoles,
        AssignRoles,
        ViewLogs,
        ShutdownServer,
        ManageConfig,
        AdminPanel,

        // 👥 Cuentas y Usuarios
        BanAccount,
        UnbanAccount,
        ViewAccounts,
        EditAccountCredits,

        // 🌍 Mapas y Mundos
        CreateMap,
        EditMap,
        DeleteMap,
        MovePlayer,
        SpawnNpc,

        // 🧙‍♂️ Jugadores y Personajes
        ViewPlayers,
        KickPlayer,
        MutePlayer,
        EditPlayerStats,
        TeleportSelf,

        // 🧪 Moderación Suave
        WarnPlayer,
        MonitorChats,
        TemporaryBan
    }
}
