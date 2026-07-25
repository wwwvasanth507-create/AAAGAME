using Godot;
using System;
using System.Collections.Generic;

namespace HeroOfEternia.Player
{
    /// <summary>
    /// Supported status effect types.
    /// </summary>
    public enum PlayerEffectType
    {
        Glow,
        Fire,
        Ice,
        Poison,
        Electric,
        Wind,
        Water,
        Dark,
        Light,
        Aura,
        Healing,
        Shield
    }

    /// <summary>
    /// Tracks an active status effect instance.
    /// </summary>
    public class ActiveEffect
    {
        public PlayerEffectType Type { get; }
        public float Duration { get; } // In seconds. <= 0 means infinite
        public float TimeElapsed { get; private set; }
        public Node3D? VisualNode { get; set; }

        public bool IsExpired => Duration > 0f && TimeElapsed >= Duration;

        public ActiveEffect(PlayerEffectType type, float duration)
        {
            Type = type;
            Duration = duration;
            TimeElapsed = 0f;
        }

        public void Update(float delta)
        {
            if (Duration > 0f)
            {
                TimeElapsed += delta;
            }
        }
    }

    /// <summary>
    /// Reusable effects framework node attached to PlayerRoot.
    /// Manages active visual / status effects, timing them out and updating visualization nodes.
    /// </summary>
    public partial class PlayerEffectsController : Node3D
    {
        private readonly Dictionary<PlayerEffectType, ActiveEffect> _activeEffects = new();
        
        public event Action<PlayerEffectType>? OnEffectApplied;
        public event Action<PlayerEffectType>? OnEffectRemoved;

        public override void _PhysicsProcess(double delta)
        {
            List<PlayerEffectType> expiredEffects = new();

            foreach (var kvp in _activeEffects)
            {
                var effect = kvp.Value;
                effect.Update((float)delta);
                if (effect.IsExpired)
                {
                    expiredEffects.Add(kvp.Key);
                }
            }

            foreach (var type in expiredEffects)
            {
                RemoveEffect(type);
            }
        }

        /// <summary>
        /// Applies an effect with optional duration.
        /// </summary>
        public void ApplyEffect(PlayerEffectType type, float duration = -1f)
        {
            if (_activeEffects.TryGetValue(type, out var existing))
            {
                // Refresh duration
                RemoveEffect(type);
            }

            var newEffect = new ActiveEffect(type, duration);
            
            // Create a mock visual representation (e.g. glowing sphere, particles node placeholder)
            var visual = new Node3D { Name = $"Vfx_{type}" };
            AddChild(visual);
            newEffect.VisualNode = visual;

            _activeEffects[type] = newEffect;
            Core.Logger.Info($"PlayerEffectsController: Effect '{type}' applied for {duration} seconds.");

            OnEffectApplied?.Invoke(type);
        }

        /// <summary>
        /// Removes an active effect.
        /// </summary>
        public void RemoveEffect(PlayerEffectType type)
        {
            if (_activeEffects.TryGetValue(type, out var effect))
            {
                if (effect.VisualNode != null && IsInstanceValid(effect.VisualNode))
                {
                    effect.VisualNode.QueueFree();
                }
                _activeEffects.Remove(type);
                Core.Logger.Info($"PlayerEffectsController: Effect '{type}' removed.");
                OnEffectRemoved?.Invoke(type);
            }
        }

        /// <summary>
        /// Returns true if the player currently has the specified effect active.
        /// </summary>
        public bool HasEffect(PlayerEffectType type)
        {
            return _activeEffects.ContainsKey(type);
        }

        /// <summary>
        /// Clears all active effects.
        /// </summary>
        public void ClearAllEffects()
        {
            var activeKeys = new List<PlayerEffectType>(_activeEffects.Keys);
            foreach (var key in activeKeys)
            {
                RemoveEffect(key);
            }
        }

        /// <summary>
        /// Returns list of all active effect types.
        /// </summary>
        public List<PlayerEffectType> GetActiveEffects()
        {
            return new List<PlayerEffectType>(_activeEffects.Keys);
        }
    }
}
