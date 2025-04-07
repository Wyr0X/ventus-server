using System;
using System.Collections.Generic;
using System.IO;

public static class LoggerUtil
{
    public enum LogTag
    {
        Init,
        AuthController,
        WebSocketAuthentificationService,
        AuthService,
        WebSocketServerController,
        WebSocketConnectionManager,
        DapperAccountDAO,
        DapperWorldDAO,
        DapperMapDAO,
        DapperPlayerDAO,
        DapperPlayerStatsDAO,
        MapModel,
        BaseCachedService,
        SystemChatService,
        PlayerLocationService,
        MapService,
        WorldService,
        DB,
        Game
    }

    // Configuración del tag: habilitado, color, guardar en archivo
    private static readonly Dictionary<LogTag, (bool Enabled, ConsoleColor Color, bool SaveToFile)> TagConfig = new()
    {
        { LogTag.Init, (true, ConsoleColor.White, true) },
        { LogTag.AuthController, (true, ConsoleColor.Cyan, true) },
        { LogTag.WebSocketAuthentificationService, (true, ConsoleColor.DarkCyan, true) },
        { LogTag.AuthService, (true, ConsoleColor.Blue, true) },
        { LogTag.WebSocketServerController, (true, ConsoleColor.Magenta, true) },
        { LogTag.WebSocketConnectionManager, (true, ConsoleColor.DarkMagenta, true) },
        { LogTag.DapperAccountDAO, (true, ConsoleColor.Yellow, false) },
        { LogTag.DapperWorldDAO, (true, ConsoleColor.DarkYellow, false) },
        { LogTag.MapModel, (true, ConsoleColor.DarkGreen, false) },
        { LogTag.BaseCachedService, (true, ConsoleColor.Gray, false) },
        { LogTag.SystemChatService, (true, ConsoleColor.DarkGray, false) },
        { LogTag.PlayerLocationService, (true, ConsoleColor.Green, false) },
        { LogTag.MapService, (true, ConsoleColor.DarkBlue, false) },
        { LogTag.WorldService, (true, ConsoleColor.Blue, false) },
        { LogTag.DB, (true, ConsoleColor.Red, true) },
        { LogTag.Game, (true, ConsoleColor.DarkRed, true) },
        { LogTag.DapperMapDAO, (true, ConsoleColor.DarkRed, true) },
        { LogTag.DapperPlayerDAO, (true, ConsoleColor.DarkRed, true) },
         { LogTag.DapperPlayerStatsDAO, (true, ConsoleColor.DarkRed, true) }
        

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
