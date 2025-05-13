using System.ComponentModel;
using Newtonsoft.Json;
public static class TimeProvider
{
    public static Func<DateTime> UtcNow = () => DateTime.UtcNow;
}
public enum NotificationLevel
{
    [Description("INFO")]
    Info,

    [Description("WARNING")]
    Warning,

    [Description("ERROR")]
    Error
}
public static class CommonUtils
{
    public static T? SafeDeserialize<T>(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return default;

        json = json.Trim();

        // Solo intentar deserializar si parece un JSON válido
        if (!(json.StartsWith("{") || json.StartsWith("[")))
            return default;

        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (JsonException)
        {
            // Log si querés: Console.WriteLine($"Falló la deserialización: {json}");
            return default;
        }
    }
    private static T? DeserializeJsonField<T>(ExpandoObject expando, string fieldName)
    {
        if (!expando.TryGetValue(fieldName, out var rawValue) || rawValue is not string json || string.IsNullOrWhiteSpace(json))
            return default;

        return JsonConvert.DeserializeObject<T>(json);
    }

}
