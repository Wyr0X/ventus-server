public static class LoggerUtil
{
    public static void Log(string tag, string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine($"[{tag}] {message}");
        Console.ResetColor();
    }
}
