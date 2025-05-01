public class ModerationService
{
    private readonly List<string> _bannedWords = new() { "maldición", "insulto" }; // Se puede mejorar con una base de datos

    public bool IsMessageBlocked(string message)
    {
        return _bannedWords.Any(badWord => message.Contains(badWord, StringComparison.OrdinalIgnoreCase));
    }
}
