namespace VentusServer.Domain.Objects
{
    /// <summary>
    /// Representa las estadísticas de un jugador, incluyendo salud, maná, resistencia y efectos activos.
    /// </summary>
    public class PlayerStatsObject
    {
        /// <summary>
        /// Puntos de Salud actuales del jugador.
        /// </summary>
        public int Health { get; private set; }

        /// <summary>
        /// Puntos de Maná actuales del jugador.
        /// </summary>
        public int Mana { get; private set; }

        /// <summary>
        /// Puntos de Resistencia actuales del jugador.
        /// </summary>
        public int Stamina { get; private set; }

        /// <summary>
        /// Puntos máximos de Salud del jugador.
        /// </summary>
        public int MaxHealth { get; private set; }

        /// <summary>
        /// Puntos máximos de Maná del jugador.
        /// </summary>
        public int MaxMana { get; private set; }

        /// <summary>
        /// Puntos máximos de Resistencia del jugador.
        /// </summary>
        public int MaxStamina { get; private set; }

        /// <summary>
        /// Indica si el jugador está silenciado (no puede usar habilidades).
        /// </summary>
        public bool IsSilenced { get; private set; } = false;

        /// <summary>
        /// Indica si el jugador puede atacar (no está atónito).
        /// </summary>
        public bool CanAttack { get; private set; } = true;

        /// <summary>
        /// Indica si el jugador está atónito (no puede moverse ni usar habilidades).
        /// </summary>
        public bool IsStunned { get; private set; } = false;

        /// <summary>
        /// Indica si el jugador está muerto (su salud es 0 o menos).
        /// </summary>
        public bool IsDead => Health <= 0;

        /// <summary>
        /// Diccionario de efectos activos que el jugador está experimentando.
        /// </summary>
        public Dictionary<string, StatusEffect> ActiveEffects { get; private set; } = new();

        /// <summary>
        /// Marca de tiempo de la última vez que el jugador usó un hechizo.
        /// </summary>
        public DateTime LastSpellCastTime { get; set; }
        /// <summary>
        /// Rango de ataque del jugador, que define la distancia máxima a la que puede atacar.
        /// </summary>
        public int AttackRange { get; private set; } = 1;
        public bool IsChanneling { get; internal set; }
        public bool IsRooted { get; internal set; }

        public int VisionRange { get; internal set; } = 10;

        /// <summary>
        /// Constructor para inicializar las estadísticas del jugador.
        /// </summary>
        /// <param name="health">Salud inicial del jugador.</param>
        /// <param name="mana">Maná inicial del jugador.</param>
        /// <param name="stamina">Resistencia inicial del jugador.</param>
        /// <param name="maxHealth">Salud máxima del jugador.</param>
        /// <param name="maxMana">Maná máximo del jugador.</param>
        /// <param name="maxStamina">Resistencia máxima del jugador.</param>
        public PlayerStatsObject(int health, int mana, int stamina, int maxHealth, int maxMana, int maxStamina)
        {
            Health = health;
            Mana = mana;
            Stamina = stamina;

            MaxHealth = maxHealth;
            MaxMana = maxMana;
            MaxStamina = maxStamina;
        }

        /// <summary>
        /// Verifica si el jugador tiene suficiente maná para usar una habilidad.
        /// </summary>
        /// <param name="amount">Cantidad de maná requerida.</param>
        /// <returns>True si tiene suficiente maná, false si no.</returns>
        public bool CanUseMana(int amount)
        {
            return Mana >= amount;
        }

        /// <summary>
        /// Verifica si el jugador tiene suficiente resistencia para realizar una acción.
        /// </summary>
        /// <param name="amount">Cantidad de resistencia requerida.</param>
        /// <returns>True si tiene suficiente resistencia, false si no.</returns>
        public bool CanUseStamina(int amount)
        {
            return Stamina >= amount;
        }

        /// <summary>
        /// Aplica daño al jugador. La salud no puede ser negativa.
        /// </summary>
        /// <param name="amount">Cantidad de daño a recibir.</param>
        public void TakeDamage(int amount)
        {
            Health = Math.Max(0, Health - amount);
        }

        /// <summary>
        /// Cura al jugador. La salud no puede exceder el máximo.
        /// </summary>
        /// <param name="amount">Cantidad de curación.</param>
        public void Heal(int amount)
        {
            Health = Math.Min(MaxHealth, Health + amount);
        }

        /// <summary>
        /// Utiliza maná. El maná no puede ser negativo.
        /// </summary>
        /// <param name="amount">Cantidad de maná a gastar.</param>
        public void UseMana(int amount)
        {
            Mana = Math.Max(0, Mana - amount);
        }

        /// <summary>
        /// Recupera maná. El maná no puede exceder el máximo.
        /// </summary>
        /// <param name="amount">Cantidad de maná a recuperar.</param>
        public void RegainMana(int amount)
        {
            Mana = Math.Min(MaxMana, Mana + amount);
        }

        /// <summary>
        /// Utiliza resistencia. La resistencia no puede ser negativa.
        /// </summary>
        /// <param name="amount">Cantidad de resistencia a usar.</param>
        public void UseStamina(int amount)
        {
            Stamina = Math.Max(0, Stamina - amount);
        }

        /// <summary>
        /// Recupera resistencia. La resistencia no puede exceder el máximo.
        /// </summary>
        /// <param name="amount">Cantidad de resistencia a recuperar.</param>
        public void RegainStamina(int amount)
        {
            Stamina = Math.Min(MaxStamina, Stamina + amount);
        }

        /// <summary>
        /// Aplica un efecto de estado al jugador.
        /// </summary>
        /// <param name="effect">El efecto de estado a aplicar.</param>
        public void ApplyEffect(StatusEffect effect)
        {
            ActiveEffects[effect.Id] = effect;
            UpdateFlags();
        }

        /// <summary>
        /// Elimina un efecto de estado activo por su identificador.
        /// </summary>
        /// <param name="effectId">El identificador del efecto a eliminar.</param>
        public void RemoveEffect(string effectId)
        {
            if (ActiveEffects.Remove(effectId))
                UpdateFlags();
        }

        /// <summary>
        /// Actualiza el estado de los efectos activos y las banderas de estado.
        /// </summary>
        /// <param name="deltaTime">El tiempo transcurrido desde la última actualización.</param>
        public void Update(float deltaTime)
        {
            var expired = new List<string>();

            foreach (var effect in ActiveEffects.Values)
            {
                effect.Duration -= deltaTime;
                if (effect.Duration <= 0)
                    expired.Add(effect.Id);
            }

            foreach (var id in expired)
                ActiveEffects.Remove(id);

            UpdateFlags();
        }

        /// <summary>
        /// Actualiza las banderas de estado del jugador según los efectos activos.
        /// </summary>
        private void UpdateFlags()
        {
            IsSilenced = ActiveEffects.Values.Any(e => e.Type == EffectType.Silence);
            IsStunned = ActiveEffects.Values.Any(e => e.Type == EffectType.Stun);
        }

        /// <summary>
        /// Verifica si el jugador está silenciado o muerto, lo que le impide lanzar hechizos.
        /// </summary>
        /// <returns>True si el jugador está silenciado o muerto.</returns>
        public bool IsSilencedOrCannotCast()
        {
            return IsSilenced || IsDead;
        }
    }
}
