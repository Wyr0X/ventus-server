using Game.Server;
using Ventus;
using Ventus.Network.Packets;

public class SpellHandler
{
    private readonly GameServer _gameServer;

    public SpellHandler(GameServer gameServer, GameEventHandler gameEventHandler)
    {
        _gameServer = gameServer;

        gameEventHandler.Subscribe(ClientPacket.TryCastSpellRequest, HandleTryCastSpell);
    }

    public Task HandleTryCastSpell(UserMessagePair userMessagePair)
    {
        var spellPacket = userMessagePair.ClientMessage as TryCastSpellRequest;
        if (spellPacket == null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SpellHandler, "Invalid packet type (expected TryCastSpell)");
            return Task.CompletedTask;
        }

        if (!_gameServer.playersByAccountId.TryGetValue(userMessagePair.AccountId, out var player))
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SpellHandler, $"No player found for accountId {userMessagePair.AccountId}");
            return Task.CompletedTask;
        }

        var spellId = spellPacket.SpellId;
        var targetX = spellPacket.TargetX;
        var targetY = spellPacket.TargetY;

        LoggerUtil.Log(LoggerUtil.LogTag.SpellHandler, $"Player {player.Id} tried to cast spell {spellId} at ({targetX}, {targetY})");

        // TODO: Validar si puede lanzar el hechizo (CD, maná, etc.)
        // TODO: Aplicar efectos (proyectil, AOE, instantáneo, etc.)
        // TODO: Enviar animación o resultado a los jugadores cercanos
        SpellAttack spellAttack = new SpellAttack(player.Id, new Vec2(targetX, targetY), spellId);
        _gameServer.attackSystem.QueueAttack(spellAttack);

        return Task.CompletedTask;
    }
}
