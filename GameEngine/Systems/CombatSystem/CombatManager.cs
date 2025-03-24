using System.Net.WebSockets;
using Protos.Game.Combat;

public class CombatManager
{
    private readonly CombatLogic _combatLogic;

    public CombatManager(CombatLogic combatLogic)
    {
        _combatLogic = combatLogic;
    }

    // Procesa una solicitud de ataque
    public void ProcessAttackRequest(AttackRequest attackRequest, WebSocket webSocket)
    {
        // Validación de los datos, si es necesario
        if (attackRequest.TargetId <= 0)
        {
            Console.WriteLine("❌ ID de objetivo no válido.");
            return;
        }

        // Llama a CombatLogic para manejar la lógica del ataque
        _combatLogic.HandleAttackLogic(attackRequest, webSocket);
    }

    // Procesa una solicitud de lanzamiento de hechizo
    public void ProcessCastSpellRequest(CastSpellRequest castSpellRequest, WebSocket webSocket)
    {
        // Validación de los datos, si es necesario
        if (castSpellRequest.SpellId <= 0 || castSpellRequest.TargetId <= 0)
        {
            Console.WriteLine("❌ Datos de hechizo o objetivo no válidos.");
            return;
        }

        // Llama a CombatLogic para manejar la lógica del hechizo
        _combatLogic.HandleCastSpellLogic(castSpellRequest, webSocket);
    }
}
