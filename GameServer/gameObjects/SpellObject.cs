
namespace VentusServer.Domain.Objects
{
    public class SpellObject : GameObject, IValidatableObject
    {

        public new string Id { get; private set; }
        public SpellModel Model { get; }

        public DateTime LastCastTime { get; private set; } = DateTime.MinValue;
        private CooldownManager CooldownManager { get; } = new CooldownManager();

        private SpellActionValidator validator = new SpellActionValidator();
        public bool IsOnCooldown => !CooldownManager.IsOffCooldown(Id);
        public SpellObject(string id, Vec2 position, SpellModel model)
      : base(id, position)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Spell ID cannot be null or empty.", nameof(id));

            if (position == null)
                throw new ArgumentNullException(nameof(position), "Position cannot be null.");

            if (model == null)
                throw new ArgumentNullException(nameof(model), "Spell model cannot be null.");

            Id = id;
            Model = model;
        }

        public bool CanTryToCast(PlayerObject player)
        {
            ValidationResult validationResult = validator.CanAttemptAction(this, player);
            if (!validationResult.IsValid)
                LoggerUtil.Log(LoggerUtil.LogTag.SpellObject, $"[CanTryToCast] Spell {Id} cannot be cast: {validationResult.Reason}");


            return validationResult.IsValid;
        }

        public void StartCooldown()
        {
            this.CooldownManager.SetCooldown(Id, TimeSpan.FromSeconds(Model.Cooldown));
        }


    }
}