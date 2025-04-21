using System;
using System.Collections.Generic;
using System.IO;

public static class LoggerUtil
{
    public enum LogTag
    {
        Init,
        AuthController,
        AdminAccountController,
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
        DapperPlayerInventoryDAO,

        DapperRoleDAO,
        MapModel,
        BaseCachedService,
        SystemChatService,
        PlayerLocationService,
        StoreService,
        RoleService,
        AccountService,
        MapService,
        WorldService,
        DB,
        Game,
        RequirePermissionAttribute
    }

    // Configuración del tag: habilitado, color, guardar en archivo
    private static readonly Dictionary<LogTag, (bool Enabled, ConsoleColor Color, bool SaveToFile)> TagConfig = new()
    {
        { LogTag.AuthController, (false, ConsoleColor.Cyan, false) },
        { LogTag.AdminAccountController, (false, ConsoleColor.Cyan, false) },
        { LogTag.AdminRolesController, (false, ConsoleColor.Cyan, false) },
        { LogTag.WebSocketAuthentificationService, (false, ConsoleColor.DarkMagenta, false) },
        { LogTag.WebSocketServerController, (false, ConsoleColor.DarkMagenta, false) },
        { LogTag.WebSocketConnectionManager, (false, ConsoleColor.DarkMagenta, false) },
        { LogTag.MapModel, (false, ConsoleColor.DarkGreen, false) },
        { LogTag.BaseCachedService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.SystemChatService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.PlayerLocationService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.AccountService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.MapService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.WorldService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.AuthService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.RoleService, (false, ConsoleColor.DarkBlue, false) },
        { LogTag.StoreService, (true, ConsoleColor.DarkBlue, false) },
        { LogTag.DB, (false, ConsoleColor.Yellow, false) },
        { LogTag.Init, (false, ConsoleColor.Yellow, false) },
        { LogTag.Game, (false, ConsoleColor.Yellow, false) },
        { LogTag.DapperMapDAO, (false, ConsoleColor.Green, false) },
        { LogTag.DapperPlayerDAO, (false, ConsoleColor.Green, false) },
        { LogTag.DapperPlayerStatsDAO, (true, ConsoleColor.Green, false) },
        { LogTag.DapperRoleDAO, (false, ConsoleColor.Green, false) },
        { LogTag.DapperAccountDAO, (false, ConsoleColor.Green, false) },
        { LogTag.DapperWorldDAO, (false, ConsoleColor.Green, false) },
        { LogTag.DapperItemDAO, (true, ConsoleColor.Green, false) },
        { LogTag.DapperPlayerInventoryDAO, (true, ConsoleColor.Green, false) },
        { LogTag.RequirePermissionAttribute, (false, ConsoleColor.Blue, false) },


    };

    public static void EnableTag(LogTag tag) => SetTagEnabled(tag, true);
    public static void DisableTag(LogTag tag) => SetTagEnabled(tag, false);

    public static void SetTagEnabled(LogTag tag, bool enabled)
    {
        if (TagConfig.ContainsKey(tag))
        {
            var current = TagConfig[tag];
            TagConfig[tag] = (enabled, current.Color, current.SaveToFile);
        }
    }

    public static void EnableFileLogging(LogTag tag) => SetFileLogging(tag, true);
    public static void DisableFileLogging(LogTag tag) => SetFileLogging(tag, false);

    private static void SetFileLogging(LogTag tag, bool saveToFile)
    {
        if (TagConfig.ContainsKey(tag))
        {
            var current = TagConfig[tag];
            TagConfig[tag] = (current.Enabled, current.Color, saveToFile);
        }
    }

    public static void Log(LogTag tag, string message, string? subTag = null, bool isError = false)
    {
        if (TagConfig.TryGetValue(tag, out var config) && config.Enabled)
        {
            Console.ForegroundColor = isError ? ConsoleColor.Red : config.Color;

            string prefix = !string.IsNullOrWhiteSpace(subTag)
                ? $"[{tag}/{subTag}]"
                : $"[{tag}]";

            Console.WriteLine($"{prefix} {message}");
            Console.ResetColor();

            if (config.SaveToFile || isError)
            {
                SaveLogToFile(tag, prefix, message);
            }
        }
    }

    public static void LogCustom(LogTag tag, string message, ConsoleColor overrideColor, string? subTag = null, bool isError = false)
    {
        if (TagConfig.TryGetValue(tag, out var config) && config.Enabled)
        {
            Console.ForegroundColor = isError ? ConsoleColor.Red : overrideColor;

            string prefix = !string.IsNullOrWhiteSpace(subTag)
                ? $"[{tag}/{subTag}]"
                : $"[{tag}]";

            Console.WriteLine($"{prefix} {message}");
            Console.ResetColor();

            if (config.SaveToFile || isError)
            {
                SaveLogToFile(tag, prefix, message);
            }
        }
    }

    private static void SaveLogToFile(LogTag tag, string prefix, string message)
    {
        try
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string folderPath = Path.Combine("Logs", tag.ToString());
            string filePath = Path.Combine(folderPath, $"{date}.log");

            Directory.CreateDirectory(folderPath);

            string time = DateTime.Now.ToString("HH:mm:ss");
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
