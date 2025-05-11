public class CooldownManager
{
    private readonly Dictionary<string, DateTime> _cooldowns = new();

    // Aplica un cooldown a una acción (por ID o nombre)
    public void SetCooldown(string actionId, TimeSpan duration)
    {
        _cooldowns[actionId] = DateTime.UtcNow.Add(duration);
    }

    // Verifica si una acción ya puede usarse
    public bool IsOffCooldown(string actionId)
    {
        if (!_cooldowns.TryGetValue(actionId, out var readyTime))
            return true; // Nunca usada, está lista

        return DateTime.UtcNow >= readyTime;
    }

    // Tiempo restante (en segundos) de una acción en cooldown
    public double GetRemainingCooldown(string actionId)
    {
        if (!_cooldowns.TryGetValue(actionId, out var readyTime))
            return 0;

        return Math.Max((readyTime - DateTime.UtcNow).TotalSeconds, 0);
    }

    // Limpia acciones que ya expiraron (opcional, mantenimiento)
    public void ClearExpiredCooldowns()
    {
        var now = DateTime.UtcNow;
        var expired = _cooldowns.Where(pair => pair.Value <= now).Select(pair => pair.Key).ToList();
        foreach (var key in expired)
            _cooldowns.Remove(key);
    }
}
