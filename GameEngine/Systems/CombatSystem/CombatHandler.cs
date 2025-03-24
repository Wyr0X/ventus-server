using System.Net.WebSockets;
using Protos.Game.Combat;

public class CombatHandler
{
    private readonly CombatManager _combatManager;

    public CombatHandler(CombatManager combatManager)
    {
        _combatManager = combatManager;
    }

    // Función que maneja los mensajes de combate recibidos desde el cliente
    public void HandleCombatMessage(ClientMessageCombat combatMessage, WebSocket webSocket)
    {
        switch (combatMessage.MessageTypeCase)
        {
            case ClientMessageCombat.MessageTypeOneofCase.AttackRequest:
                _combatManager.ProcessAttackRequest(combatMessage.AttackRequest, webSocket);
                break;
            case ClientMessageCombat.MessageTypeOneofCase.CastSpellRequest:
                _combatManager.ProcessCastSpellRequest(combatMessage.CastSpellRequest, webSocket);
                break;
            default:
                Console.WriteLine("❌ Tipo de mensaje de combate no reconocido.");
                break;
        }
    }
}
