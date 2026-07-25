using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Player.Abilities
{
    /// <summary>
    /// Defines the type of effect an ability can produce.
    /// </summary>
    public enum EffectType
    {
        Damage,
        Healing,
        Shield,
        Buff,
        Debuff,
        Teleport,
        Summon,
        ProjectileSpawn,
        AreaCreation,
        Movement,
        EnvironmentalInteraction,
        Custom
    }

    /// <summary>
    /// Defines a reusable ability effect that can be composed into abilities.
    /// Effects are data-driven and support multiple levels/tiers.
    /// </summary>
    public class AbilityEffect
    {
        public string EffectId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public EffectType Type { get; set; } = EffectType.Damage;
        public float BaseValue { get; set; } = 0f;
        public float ValuePerLevel { get; set; } = 0f;
        public float Duration { get; set; } = 0f;
        public float TickInterval { get; set; } = 0f;
        public int MaxStacks { get; set; } = 1;
        public string StatModifierKey { get; set; } = string.Empty;
        public float ModifierPercent { get; set; } = 0f;
        public string VfxKey { get; set; } = string.Empty;
        public string SfxKey { get; set; } = string.Empty;
        public string CustomData { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents an active instance of an ability effect on a target.
    /// </summary>
    public class ActiveEffect
    {
        public string EffectId { get; }
        public string SourceAbilityId { get; }
        public float RemainingDuration { get; set; }
        public float TickTimer { get; set; }
        public int CurrentStacks { get; set; }
        public float TotalValue { get; set; }
        public bool IsExpired => RemainingDuration <= 0f;
        public int EffectLevel { get; set; }

        public ActiveEffect(string effectId, string sourceAbilityId, float duration, float totalValue, int level = 1)
        {
            EffectId = effectId;
            SourceAbilityId = sourceAbilityId;
            RemainingDuration = duration;
            TotalValue = totalValue;
            CurrentStacks = 1;
            EffectLevel = level;
        }

        public void Tick(float delta)
        {
            if (RemainingDuration > 0)
                RemainingDuration -= delta;
        }
    }

    /// <summary>
    /// Manages active ability effects on an entity (player or enemy).
    /// Handles application, stacking, ticking, and expiration.
    /// </summary>
    public class EffectsManager
    {
        private readonly Dictionary<string, List<ActiveEffect>> _activeEffects = new();
        private readonly Dictionary<string, AbilityEffect> _effectDefinitions = new();

        public event Action<string, int>? OnEffectApplied;
        public event Action<string>? OnEffectExpired;
        public event Action<string, int>? OnEffectStacked;

        public void RegisterEffect(AbilityEffect effect)
        {
            _effectDefinitions[effect.EffectId] = effect;
        }

        public void RegisterEffects(List<AbilityEffect> effects)
        {
            foreach (var effect in effects)
                RegisterEffect(effect);
        }

        public AbilityEffect? GetDefinition(string effectId)
        {
            _effectDefinitions.TryGetValue(effectId, out var def);
            return def;
        }

        public void ApplyEffect(string targetId, string effectId, string sourceAbilityId, int level = 1)
        {
            if (!_effectDefinitions.TryGetValue(effectId, out var def))
            {
                Logger.Warning($"EffectsManager: Unknown effect '{effectId}'.");
                return;
            }

            float value = def.BaseValue + (def.ValuePerLevel * (level - 1));
            float duration = def.Duration;

            // Check for existing effect
            if (_activeEffects.TryGetValue(targetId, out var existing))
            {
                var active = existing.Find(e => e.EffectId == effectId);
                if (active != null)
                {
                    // Stack or refresh
                    if (active.CurrentStacks < def.MaxStacks)
                    {
                        active.CurrentStacks++;
                        active.TotalValue = value * active.CurrentStacks;
                        OnEffectStacked?.Invoke(effectId, active.CurrentStacks);
                    }
                    active.RemainingDuration = duration; // Refresh duration
                    return;
                }
            }

            // Apply new effect
            var newEffect = new ActiveEffect(effectId, sourceAbilityId, duration, value, level)
            {
                TickTimer = def.TickInterval
            };

            if (!_activeEffects.ContainsKey(targetId))
                _activeEffects[targetId] = new List<ActiveEffect>();

            _activeEffects[targetId].Add(newEffect);
            OnEffectApplied?.Invoke(effectId, level);

            Logger.Info($"EffectsManager: Applied '{effectId}' to '{targetId}' (Lv.{level}, {value:F1}, {duration:F1}s)");
        }

        public void RemoveEffect(string targetId, string effectId)
        {
            if (!_activeEffects.TryGetValue(targetId, out var effects)) return;
            effects.RemoveAll(e => e.EffectId == effectId);
            OnEffectExpired?.Invoke(effectId);
        }

        public void RemoveAllEffects(string targetId)
        {
            if (!_activeEffects.TryGetValue(targetId, out var effects)) return;
            foreach (var effect in effects)
                OnEffectExpired?.Invoke(effect.EffectId);
            effects.Clear();
        }

        public void Tick(string targetId, float delta)
        {
            if (!_activeEffects.TryGetValue(targetId, out var effects)) return;

            for (int i = effects.Count - 1; i >= 0; i--)
            {
                var effect = effects[i];
                effect.Tick(delta);

                if (effect.IsExpired)
                {
                    OnEffectExpired?.Invoke(effect.EffectId);
                    effects.RemoveAt(i);
                }
            }
        }

        public void TickAll(float delta)
        {
            foreach (var targetId in _activeEffects.Keys)
                Tick(targetId, delta);
        }

        public bool HasEffect(string targetId, string effectId)
        {
            return _activeEffects.TryGetValue(targetId, out var effects) &&
                   effects.Exists(e => e.EffectId == effectId);
        }

        public int GetEffectStacks(string targetId, string effectId)
        {
            if (!_activeEffects.TryGetValue(targetId, out var effects)) return 0;
            var effect = effects.Find(e => e.EffectId == effectId);
            return effect?.CurrentStacks ?? 0;
        }

        public IReadOnlyList<ActiveEffect> GetActiveEffects(string targetId)
        {
            return _activeEffects.TryGetValue(targetId, out var effects)
                ? effects.AsReadOnly()
                : new List<ActiveEffect>().AsReadOnly();
        }

        public int TotalActiveEffects => _activeEffects.Values.Sum(list => list.Count);

        public string DebugSummary()
        {
            int count = TotalActiveEffects;
            return $"EffectsManager: {_effectDefinitions.Count} definitions, {count} active effects across {_activeEffects.Count} targets.";
        }
    }
}