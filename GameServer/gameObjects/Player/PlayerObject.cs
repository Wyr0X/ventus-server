using Google.Type;
using Ventus.GameModel.Spells;
using VentusServer.Domain.Models;

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

        public required PlayerStatsObject Stats;
        public required PlayerInventory Inventory { get; set; }  // Inventario del jugador

        public required ZoneContext CurrentZoneContext { get; set; }
        public required CooldownManager CooldownManager { get; set; }
        private IEnumerable<SpellObject> spells = Enumerable.Empty<SpellObject>();

        public PlayerObject(int id, Vec2 position, string name, PlayerModel playerModel, ZoneContext currentZoneContext, CooldownManager cooldownManager)
            : base(id, position, name)
        {
            PlayerModel = playerModel;
            IsActiviyConfirmed = true;
            Inventory = new PlayerInventory(); // Inicialización del inventario
            CurrentZoneContext = currentZoneContext;
            CooldownManager = cooldownManager;

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
        private bool CheckCollision()
        {
            bool collision = IsPositionBlocked(X, Y);
            if (collision)
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerObject,
                    $"[CheckCollision] Collision detected for Player {Id} at position ({X}, {Y})");
            }
            return collision;
        }

        private void RevertMovement(Direction direction)
        {
            float moveDistance = Speed;
            float prevX = X;
            float prevY = Y;

            switch (direction)
            {
                case Direction.Up: Y += moveDistance; break;
                case Direction.Down: Y -= moveDistance; break;
                case Direction.Left: X += moveDistance; break;
                case Direction.Right: X -= moveDistance; break;
            }

            LoggerUtil.Log(LoggerUtil.LogTag.PlayerObject,
                $"[RevertMovement] Player {Id} reverted movement from ({prevX}, {prevY}) back to ({X}, {Y}) due to collision.");
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
        public bool CanCastSpell(SpellObject spell)
        {
            // Ejemplo básico de condiciones
            if (spell == null)
                return false;

            if (!this.Stats.CanUseMana(spell.Model.ManaCost))
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerObject, $"[CanCastSpell] Player {Id} does not have enough mana.");
                return false;
            }

            if (spell.IsInCooldown())
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerObject, $"[CanCastSpell] Spell '{spell.Model.Name}' is on cooldown for Player {Id}.");
                return false;
            }

            if (this.Stats.IsSilencedOrCannotCast())
            {
                LoggerUtil.Log(LoggerUtil.LogTag.PlayerObject, $"[CanCastSpell] Player {Id} is silenced.");
                return false;
            }

            return true;
        }

        public bool CanPerformActions()
        {
            // Verificar si el jugador está aturdido o incapacitado
            if (this.Stats.IsStunned)
            {
                return false;
            }

            // Verificar si el jugador está muerto
            if (this.Stats.IsDead)
            {
                return false;
            }

            if (this.Stats.IsSilencedOrCannotCast())
            {
                return false;
            }

            // Si ninguna de las condiciones anteriores se cumple, el jugador puede realizar acciones
            return true;
        }
    }
}