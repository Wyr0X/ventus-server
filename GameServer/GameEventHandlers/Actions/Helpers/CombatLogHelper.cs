using VentusServer.Domain.Objects;

public static class CombatLogHelper
{
    // Genera un log de combate para la consola del servidor
    public static void LogCombatInteraction(string message, PlayerObject player)
    {
        Console.WriteLine($"[Combat] {message}");
        // player.SendMessageToClient($"[Combat] {message}");
    }

    // Genera un paquete de información de combate para los jugadores involucrados
    public static CombatInfoPacket GenerateCombatInfoPacket(PlayerObject attacker, PlayerObject target, int damage, string actionType)
    {
        return new CombatInfoPacket
        {
            AttackerId = attacker.Id,
            TargetId = target.Id,
            Damage = damage,
            ActionType = actionType
        };
    }
}
public class CombatInfoPacket
{
    public int AttackerId { get; set; } // ID del jugador o NPC que realiza la acción
    public int TargetId { get; set; }   // ID del jugador o NPC objetivo
    public int Damage { get; set; }        // Daño infligido
    public string ActionType { get; set; } // Tipo de acción (ejemplo: "Ataque", "Hechizo", "Curación")
    public DateTime Timestamp { get; set; } // Momento en que ocurrió el evento

    public override string ToString()
    {
        return $"[{Timestamp}] {ActionType}: {AttackerId} -> {TargetId}, Daño: {Damage}";
    }
}