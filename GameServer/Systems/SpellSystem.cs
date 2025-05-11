using Game.Server;

public class SpellSystem
{
    private GameServer _game;
    private IEnumerable<SpellModel> _spells = new List<SpellModel>();
    public SpellSystem(GameServer game)
    {
        _game = game;
        _ = InitializeSpellsAsync();
    }

    private async Task InitializeSpellsAsync()
    {
        _spells = await _game._gameServiceMediator.GetSpells();
    }


    public async Task PlayerTryToCastSpell(SpellAttack spellAttack)
    {
        PlayerObject? player = _game.GetPlayerById(spellAttack.AttackerId);
        if (player == null) return;

        SpellObject? spellObject = player.GetSpellById(spellAttack.SpellId);
        if (spellObject == null)
        {
            LoggerUtil.Log(LoggerUtil.LogTag.SpellSystem, $"[PlayerTryToCastSpell] Spell with ID {spellAttack.SpellId} not found for Player {player.Id}");
        }
        SpellModel baseSpell = spellObject.Model;

        if (spellObject == null || baseSpell == null) return;

        if (!player.CanCastSpell(spellObject))
        {
            // Enviar mensaje al cliente: "no puedes castear"
            return;
        }

        player.Stats.UseMana(baseSpell.ManaCost);

        // player.ApplySpellEffect(baseSpell, spellAttack.TargetX, spellAttack.TargetY);

        // Notificar al resto de jugadores o al cliente sobre el casteo
    }

}
// public bool TryCastSpell(PlayerObject caster, int spellId, float x, float y)
// {
//     var spell = caster.GetSpellById(spellId);
//     if (spell == null || !CanCast(caster, spell)) return false;

//     ApplyCost(caster, spell);
//     ApplySpellEffect(caster, spell, x, y);
//     StartCooldown(caster, spell);

//     return true;
// }

// private void ApplySpellEffect(PlayerObject caster, Spell spell, float x, float y)
// {
//     // Área de efecto, daño, estados aplicados, etc.
// }

// private bool CanCast(PlayerObject caster, Spell spell)
// {
//     // Chequear maná, cooldowns, silenciado, etc.
// }

// private void ApplyCost(PlayerObject caster, Spell spell)
// {
//     // Restar maná, aplicar cooldown, etc.
// }
