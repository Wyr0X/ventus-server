using System;
using System.Collections.Generic;

namespace VentusServer.Domain.Objects
{
    /// <summary>
    /// Representa un efecto de estado (como "Silencio", "Quemadura", etc.) en el juego.
    /// Esta clase define las propiedades básicas del efecto.
    /// </summary>
    public class StatusEffect
    {
        /// <summary>
        /// Identificador único del efecto (ej: "burn_basic", "stun_1s").
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Tipo del efecto (ej: Burn, Freeze, Poison, etc.).
        /// </summary>
        public EffectType Type { get; set; }

        /// <summary>
        /// Duración del efecto en segundos.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// Datos adicionales que pueden ser necesarios para efectos específicos.
        /// Por ejemplo, daño por segundo para "Poison" o "Burn", curación para "Regen".
        /// </summary>
        public Dictionary<string, object> CustomData { get; set; }

        /// <summary>
        /// Constructor de un efecto de estado.
        /// </summary>
        /// <param name="id">Identificador único del efecto.</param>
        /// <param name="type">Tipo de efecto (Silence, Stun, Burn, etc.).</param>
        /// <param name="duration">Duración del efecto en segundos.</param>
        /// <param name="customData">Datos adicionales, como magnitudes o valores específicos para el efecto.</param>
        public StatusEffect(string id, EffectType type, float duration, Dictionary<string, object> customData = null)
        {
            Id = id;
            Type = type;
            Duration = duration;
            CustomData = customData ?? new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Tipos de efectos de estado posibles.
    /// </summary>
    public enum EffectType
    {
        Silence,   // Impide el uso de habilidades
        Stun,      // Deja inmóvil al objetivo
        Burn,      // Daño periódico por fuego
        Freeze,    // Congela al objetivo
        Poison,    // Daño periódico por veneno
        Shield,    // Proporciona un escudo que absorbe daño
        Regen      // Regeneración de salud periódica
    }

    /// <summary>
    /// Representa un efecto activo aplicado a una entidad en el juego.
    /// Es la instancia en tiempo real de un efecto de estado.
    /// </summary>
    public class ActiveEffect
    {
        /// <summary>
        /// Referencia al efecto de estado original.
        /// </summary>
        public StatusEffect Source { get; set; }

        /// <summary>
        /// Tiempo restante de duración del efecto.
        /// </summary>
        public float TimeRemaining { get; set; }

        /// <summary>
        /// Indica si el efecto sigue activo.
        /// </summary>
        public bool IsActive => TimeRemaining > 0;

        /// <summary>
        /// Constructor para crear un efecto activo basado en un efecto de estado.
        /// </summary>
        /// <param name="source">El efecto de estado que esta instancia representa.</param>
        public ActiveEffect(StatusEffect source)
        {
            Source = source;
            TimeRemaining = source.Duration;
        }

        /// <summary>
        /// Actualiza el tiempo restante del efecto activo.
        /// </summary>
        /// <param name="deltaTime">El tiempo transcurrido desde la última actualización.</param>
        public void Update(float deltaTime)
        {
            TimeRemaining -= deltaTime;
        }
    }

    /// <summary>
    /// Representa una clase de utilidad que maneja efectos de estado activos en un jugador u otra entidad.
    /// </summary>
    public class PlayerStats
    {
        /// <summary>
        /// Lista de efectos activos sobre el jugador.
        /// </summary>
        public List<ActiveEffect> ActiveEffects { get; set; } = new();

        /// <summary>
        /// Aplica un efecto al jugador.
        /// </summary>
        /// <param name="statusEffect">El efecto de estado a aplicar.</param>
        public void ApplyEffect(StatusEffect statusEffect)
        {
            ActiveEffects.Add(new ActiveEffect(statusEffect));
        }

        /// <summary>
        /// Actualiza todos los efectos activos en el jugador.
        /// </summary>
        /// <param name="deltaTime">El tiempo transcurrido desde la última actualización.</param>
        public void UpdateEffects(float deltaTime)
        {
            foreach (var effect in ActiveEffects)
            {
                effect.Update(deltaTime);
            }

            // Elimina los efectos inactivos
            ActiveEffects.RemoveAll(e => !e.IsActive);
        }
    }
}
