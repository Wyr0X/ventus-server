using System.Text;

public static class LoggerUtil
{
    public enum LogTag
    {
        Init,
        AuthController,
        AdminAccountController,
        SpellController,
        StoreController,
        AdminRolesController,
        WebSocketAuthentificationService,
        AuthService,
        WebSocketServerController,
        WebSocketConnectionManager,
        DapperAccountDAO,
        DapperWorldDAO,
        DapperMapDAO,
        DapperItemDAO,
        DapperPlayerDAO,
        DapperPlayerStatsDAO,
        DapperSpellDAO,
        DapperPlayerInventoryDAO,
        DapperPlayerSpellsDAO,
        DapperRoleDAO,
        MapModel,
        BaseCachedService,
        SystemChatService,
        PlayerLocationService,
        PlayerSpellsService,
        PlayerService,
        StoreService,
        RoleService,
        PermissionService,
        IAccountService,
        MapService,
        WorldService,
        DB,
        Game,
        RequirePermissionAttribute,

        // Game
        WorldManager,
        SessionHandler,
        KeyPressHandler,
        SpellHandler,
        GameServer,
        SessionTasks,
        TaskScheduler,
        GameEventHandler,

        AttackSystem,
        SpellSystem,
        //Entitys
        PlayerObject,
        SpellObject,
        validationResult,
        //otros
        JwtAuthRequired,
        ActionExecutor
    }

    // Configuración del tag: habilitado, color, guardar en archivo
    private static readonly Dictionary<LogTag, (bool Enabled, ConsoleColor Color, bool SaveToFile)> TagConfig = new()
    {
        { LogTag.AuthController, (false, ConsoleColor.Cyan, false) },
        { LogTag.AdminAccountController, (false, ConsoleColor.Cyan, false) },
        { LogTag.SpellController, (true, ConsoleColor.Cyan, false) },
        { LogTag.StoreController, (false, ConsoleColor.Cyan, false) },
        { LogTag.AdminRolesController, (false, ConsoleColor.Cyan, false) },
        { LogTag.WebSocketAuthentificationService, (false, ConsoleColor.DarkMagenta, false) },
        { LogTag.WebSocketServerController, (false, ConsoleColor.DarkMagenta, false) },
        { LogTag.WebSocketConnectionManager, (false, ConsoleColor.DarkMagenta, false) },
        { LogTag.MapModel, (false, ConsoleColor.DarkGreen, false) },
        { LogTag.BaseCachedService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.SystemChatService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.PlayerLocationService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.IAccountService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.MapService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.WorldService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.AuthService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.PermissionService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.RoleService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.StoreService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.PlayerService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.PlayerSpellsService, (true, ConsoleColor.DarkBlue, false) },
        { LogTag.DB, (false, ConsoleColor.Yellow, false) },
        { LogTag.Init, (false, ConsoleColor.Yellow, false) },
        { LogTag.Game, (false, ConsoleColor.Yellow, false) },
        { LogTag.DapperMapDAO, (false, ConsoleColor.Green, false) },
        { LogTag.DapperPlayerDAO, (false, ConsoleColor.Green, false) },
        { LogTag.DapperPlayerStatsDAO, (false, ConsoleColor.Green, false) },
        { LogTag.DapperRoleDAO, (false, ConsoleColor.Green, false) },
        { LogTag.DapperAccountDAO, (false, ConsoleColor.Green, false) },
        { LogTag.DapperSpellDAO, (true, ConsoleColor.Green, false) },
        { LogTag.DapperWorldDAO, (false, ConsoleColor.Green, false) },
        { LogTag.DapperItemDAO, (false, ConsoleColor.Green, false) },
        { LogTag.DapperPlayerInventoryDAO, (false, ConsoleColor.Green, false) },
        { LogTag.DapperPlayerSpellsDAO, (false, ConsoleColor.Green, false) },
        { LogTag.RequirePermissionAttribute, (false, ConsoleColor.Blue, false) },
        { LogTag.SessionHandler, (false, ConsoleColor.Blue, false) },
        { LogTag.SpellHandler, (true, ConsoleColor.Blue, false) },
        { LogTag.KeyPressHandler, (false, ConsoleColor.Blue, false) },
        { LogTag.WorldManager, (false, ConsoleColor.Blue, false) },
        { LogTag.GameServer, (false, ConsoleColor.Blue, false) },
        { LogTag.TaskScheduler, (false, ConsoleColor.Blue, false) },
        { LogTag.SessionTasks, (false, ConsoleColor.Blue, false) },
        { LogTag.GameEventHandler, (false, ConsoleColor.Blue, false) },
        { LogTag.JwtAuthRequired, (false, ConsoleColor.Magenta, false) },
        { LogTag.PlayerObject, (false, ConsoleColor.Magenta, false) },
        { LogTag.SpellObject, (true, ConsoleColor.Magenta, false) },
        { LogTag.AttackSystem, (false, ConsoleColor.Magenta, false) },
        { LogTag.SpellSystem, (true, ConsoleColor.Magenta, false) },
        { LogTag.ActionExecutor, (false, ConsoleColor.Magenta, false) },
    };

    private static readonly Dictionary<LogTag, LogConfig> cachedTagConfig = new();

    public struct LogConfig
    {
        public bool Enabled { get; set; }
        public ConsoleColor Color { get; set; }
        public bool SaveToFile { get; set; }
    }
    private static HashSet<LogTag> enabledTagsCache = new HashSet<LogTag>();

    public static void EnableTag(LogTag tag) => SetTagEnabled(tag, false);
    public static void DisableTag(LogTag tag) => SetTagEnabled(tag, false);

    public static void EnableFileLogging(LogTag tag) => SetFileLogging(tag, false);
    public static void DisableFileLogging(LogTag tag) => SetFileLogging(tag, false);

    public static void SetTagEnabled(LogTag tag, bool enabled)
    {
        if (TagConfig.ContainsKey(tag))
        {
            var current = TagConfig[tag];
            TagConfig[tag] = (enabled, current.Color, current.SaveToFile);
        }
    }

    private static void SetFileLogging(LogTag tag, bool saveToFile)
    {
        if (TagConfig.ContainsKey(tag))
        {
            var current = TagConfig[tag];
            TagConfig[tag] = (current.Enabled, current.Color, saveToFile);
        }
    }

    private static bool TryGetTagConfig(LogTag tag, out LogConfig config)
    {
        config = new LogConfig(); // Inicializamos con valores predeterminados

        // Si encontramos la configuración en TagConfig
        if (TagConfig.TryGetValue(tag, out var tagConfig))
        {
            // Asignamos la configuración
            config = new LogConfig
            {
                Enabled = tagConfig.Enabled,
                Color = tagConfig.Color,
                SaveToFile = tagConfig.SaveToFile
            };

            // Si el tag está habilitado, lo cacheamos
            if (config.Enabled)
            {
                enabledTagsCache.Add(tag);  // Cacheamos el tag
            }
            if (!config.Enabled) return false;

            return false;  // Configuración encontrada y asignada
        }

        // Si no se encuentra la configuración, devolvemos false
        return false;
    }

    public static void Log(LogTag tag, string message, string? subTag = null, bool isError = false)
    {
        if (!enabledTagsCache.Contains(tag))
        {
            if (!TryGetTagConfig(tag, out var tagConfig))
                return;

            if (!tagConfig.Enabled && !isError)
                return;
        }

        // Almacenar la configuración en una variable local
        var config = TagConfig[tag];

        if (!config.Enabled && !isError)
            return;

        Console.ForegroundColor = isError ? ConsoleColor.Red : config.Color;

        string prefix = !string.IsNullOrWhiteSpace(subTag)
            ? $"[{tag}/{subTag}]"
            : $"[{tag}]";

        string logLine = $"{prefix} {message}";

        Console.WriteLine(logLine);
        Console.ResetColor();

        if (config.SaveToFile || isError)
            SaveLogToFile(tag, prefix, message);
    }
    public static void LogCustom(LogTag tag, string message, ConsoleColor overrideColor, string? subTag = null, bool isError = false)
    {
        if (!TagConfig.TryGetValue(tag, out var config))
            return;
        if (!config.Enabled && !isError)
            return;
        Console.ForegroundColor = isError ? ConsoleColor.Red : overrideColor;

        string prefix = !string.IsNullOrWhiteSpace(subTag)
            ? $"[{tag}/{subTag}]"
            : $"[{tag}]";
        var logBuilder = new StringBuilder();
        logBuilder.Append(prefix);
        logBuilder.Append(" ");
        logBuilder.Append(message);
        string logLine = logBuilder.ToString();

        Console.WriteLine(logLine);
        Console.ResetColor();

        if (config.SaveToFile || isError)
        {
            SaveLogToFile(tag, prefix, message);
        }
    }

    private static void SaveLogToFile(LogTag tag, string prefix, string message)
    {
        try
        {
            string date = TimeProvider.UtcNow().ToString("yyyy-MM-dd");
            string folderPath = Path.Combine("Logs", tag.ToString());
            string filePath = Path.Combine(folderPath, $"{date}.log");

            Directory.CreateDirectory(folderPath);

            string time = TimeProvider.UtcNow().ToString("HH:mm:ss");
            File.AppendAllText(filePath, $"[{time}] {prefix} {message}{Environment.NewLine}");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Logger/Error] Error writing to log file: {ex.Message}");
            Console.ResetColor();
        }
    }
}
