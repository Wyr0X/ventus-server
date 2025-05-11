using Ventus.GameModel.Spells;

namespace VentusServer.Domain.Objects
{
    public class SpellObject : GameObject, IValidatableObject
    {

        public new string Id { get; private set; }
        public SpellModel Model { get; }

        public float CurrentCooldown { get; private set; } = 0f;
        public bool IsOnCooldown => CurrentCooldown > 0f;
        public DateTime LastCastTime { get; private set; } = DateTime.MinValue;


        public SpellObject(string id, Vec2 position, SpellModel model)
         : base(id, position)               // ← Aquí inicializa `Id` y `Position`
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }
        public bool CanCast(float currentMana)
        {
            return currentMana >= Model.ManaCost && !IsOnCooldown;
        }

        public void StartCooldown()
        {
            CurrentCooldown = Model.Cooldown;
            LastCastTime = DateTime.UtcNow;
        }
        public bool IsInCooldown()
        {
            return CurrentCooldown > 0f;
        }

        public void UpdateCooldown(float deltaTime)
        {
            if (CurrentCooldown > 0)
                CurrentCooldown = Math.Max(0, CurrentCooldown - deltaTime);
        }
        public IActionValidator GetValidator()
        {
            return new SpellActionValidator(this);
        }
    }
}