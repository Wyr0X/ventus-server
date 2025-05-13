using Game.World;
using Google.Type;
using VentusServer.Domain.Models;
public enum PlayerActionState
{
    Idle,            // Jugador no está realizando ninguna acción
    CastingSpell,    // Jugador está lanzando un hechizo
    UsingItem,       // Jugador está usando un ítem
    Attacking,       // Jugador está atacando
    EquippingItem    // Jugador está equipando un ítem
}

namespace VentusServer.Domain.Objects
{
    public class MovementPlayerInput
    {
        public Direction Direction { get; set; }
        public long Timestamp { get; set; }
        public int SequenceNumber { get; set; }
    }
    public class PlayerObject : Character
    {
        private const float TicksPerSecond = 1000f; // Si Timestamp está en milisegundos
        public float X { get; set; }
        public float Y { get; set; }
        public Direction CurrentDirection { get; set; }
        public bool IsMoving { get; set; }
        public long LastProcessedInputTimestamp { get; set; } = 0;
        public PlayerModel PlayerModel { get; set; }
        public bool IsActiviyConfirmed { get; set; } = false;

        public bool isReady { get; set; } = false; //Indica si el cliente termino de cargar el juego

        public int LastSequenceNumberProcessed = 0;
        private Queue<MovementPlayerInput> inputsToProcess = new Queue<MovementPlayerInput>();
        private readonly object _inputLock = new object();

        public PlayerStatsObject Stats;
        // public PlayerInventory Inventory { get; set; }  // Inventario del jugador

        public CooldownManager CooldownManager { get; set; } = new CooldownManager();
        public GameMapObject CurrentMap { get; set; }
        private IEnumerable<SpellObject> spells = Enumerable.Empty<SpellObject>();
        private PlayerActionState PlayerActionState = PlayerActionState.Idle;
        public SpellObject? currentCastingSpell { get; set; }
        public PlayerObject(int id, Vec2 position, string name, PlayerModel playerModel
       )
            : base(id, position, name)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position), "Position cannot be null.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));

            if (playerModel == null)
                throw new ArgumentNullException(nameof(playerModel), "PlayerModel cannot be null.");
            PlayerModel = playerModel;
            IsActiviyConfirmed = true;
            // Inventory = new PlayerInventory(); // Inicialización del inventario

            // if (playerModel.PlayerSpells?.Spells != null)
            // {
            //     playerModel.PlayerSpells.Spells.ForEach(spell => AddSpell(spell, spells));
            // }
            // Stats = new PlayerStatsObject(
            //     playerStatsModel.Hp,
            //     playerStatsModel.Mp,
            //     playerStatsModel.Sp,
            //     playerStatsModel.MaxHp,
            //     playerStatsModel.MaxMp,
            //     playerStatsModel.MaxSp
            // );

        }
        public void AddSpell(PlayerSpellModel playerSpell, SpellModel[] spells)
        {
            if (playerSpell == null || spells == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerObject, $"[AddSpell] Null playerSpell or spells array provided for Player {Id}");
                return;
            }

            // Buscar el SpellModel que coincide con el id de playerSpell
            var spellModel = spells.FirstOrDefault(s => s.Id == playerSpell.SpellId);

            if (spellModel == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerObject, $"[AddSpell] Spell with ID {playerSpell.SpellId} not found for Player {Id}");
                return;
            }

            // Crear un nuevo SpellObject y asociarlo a este jugador como caster
            var spellObject = new SpellObject(spellModel.Id, this.Position, spellModel);


            // Agregar el hechizo al jugador
            this.spells = this.spells.Append(spellObject);

            LoggerUtil.Log(LoggerUtil.LogTag.PlayerObject, $"[AddSpell] Spell '{spellModel.Name}' added to Player {Id}");
        }


        private bool IsPositionBlocked(float x, float y)
        {
            return (x < 0 || x >= 100 || y < 0 || y >= 100);
        }

        public void ProcessInputs()
        {
            lock (_inputLock)
            {
                while (inputsToProcess.Count > 0)
                {
                    var input = inputsToProcess.Dequeue();

                    if (input.SequenceNumber <= LastSequenceNumberProcessed)
                        continue;

                    LoggerUtil.Log(LoggerUtil.LogTag.PlayerObject,
                        $"[ProcessInputs] Processing input for Player {Id} with direction {input.Direction}, timestamp {input.Timestamp}, sequence #{input.SequenceNumber}");

                    ApplyInputToPlayer(input);
                    LastSequenceNumberProcessed = input.SequenceNumber;
                    LastProcessedInputTimestamp = input.Timestamp;
                }
            }
        }

        private void ApplyInputToPlayer(MovementPlayerInput input)
        {
            if (input.Direction == Direction.None)
                return;

            // Calcular deltaTime entre inputs (en segundos)
            float deltaTime = 0.016f; // fallback por si es el primer input

            if (LastProcessedInputTimestamp > 0 && input.Timestamp > LastProcessedInputTimestamp)
            {
                deltaTime = (input.Timestamp - LastProcessedInputTimestamp) / TicksPerSecond;
            }

            // Calcular desplazamiento
            Vec2 directionVec = Vec2.DirectionToVector(input.Direction);
            Vec2 move = Vec2.Scale(directionVec, this.Speed);

            this.Position.Add(move);

            LoggerUtil.Log(LoggerUtil.LogTag.PlayerObject,
                $"[ApplyInputToPlayer] Player {Id} moved to ({Position.X:F3}, {Position.Y:F3}) using deltaTime {deltaTime:F4}s, speed {Speed}, direction {input.Direction}");
        }

        public Task EnqueueInput(PlayerInput input)
        {
            lock (_inputLock)
            {
                inputsToProcess.Enqueue(new MovementPlayerInput
                {
                    Direction = input.Direction,
                    Timestamp = input.Timestamp,
                    SequenceNumber = input.SequenceNumber
                });
            }

            return Task.CompletedTask;
        }
        public SpellObject? GetSpellById(string spellId)
        {
            return spells.FirstOrDefault(s => s.Model.Id == spellId);
        }
        public bool TryToCastSpell(string spellId)
        {
            // Ejemplo básico de condiciones
            if (PlayerActionState != PlayerActionState.Idle)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerObject, $"[TryToCastSpell] Player {Id} is not idle.");
                return false;
            }
            var spell = GetSpellById(spellId);
            if (spell == null)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerObject, $"[TryToCastSpell] Spell with ID {spellId} not found for Player {Id}.");
                return false;
            }
            if (!spell.CanTryToCast(this))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerObject, $"[TryToCastSpell] Spell {spell.Id} cannot be cast.");
                PlayerActionState = PlayerActionState.CastingSpell;
                currentCastingSpell = spell;
                return true;
            }

            return true;
        }

    }
}