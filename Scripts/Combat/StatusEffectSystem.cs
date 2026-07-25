using System;
using System.Collections.Generic;
using System.Text.Json;
using HeroOfEternia.Core;

namespace HeroOfEternia.Combat
{
    /// <summary>
    /// Data definition for a status effect type.
    /// Loaded from status_effects_config.json via ConfigManager.
    /// </summary>
    public class StatusEffectData
    {
        public StatusEffectType Type         { get; set; }
        public float            Duration     { get; set; } = 5f;      // seconds
        public float            TickInterval { get; set; } = 1f;      // seconds between ticks
        public float            TickDamage   { get; set; } = 0f;      // damage per tick
        public DamageType       TickDamageType { get; set; } = DamageType.Physical;
        public float            StatModifier { get; set; } = 0f;      // e.g. speed multiplier
        public int              StackLimit   { get; set; } = 1;
        public string           VfxHookKey   { get; set; } = "";
        public string           AudioHookKey { get; set; } = "";
    }

    /// <summary>
    /// Runtime instance of an active status effect on an entity.
    /// </summary>
    public class ActiveStatusEffect
    {
        public StatusEffectData Data        { get; set; } = null!;
        public string           SourceId    { get; set; } = "";
        public float            TimeRemaining { get; set; }
        public float            TickAccumulator { get; set; } = 0f;
        public int              StackCount  { get; set; } = 1;
    }

    /// <summary>
    /// Central status effect service. Manages per-entity active effects.
    /// Data-driven: effect definitions loaded from config.
    /// </summary>
    public class StatusEffectSystem
    {
        // entityId → list of active effects
        private readonly Dictionary<string, List<ActiveStatusEffect>> _active = new();

        // Effect template registry
        private readonly Dictionary<StatusEffectType, StatusEffectData> _definitions = new();

        public StatusEffectSystem() { RegisterDefaults(); }

        // ─────────────────────── Registration ───────────────────────

        public void RegisterDefinition(StatusEffectData data) =>
            _definitions[data.Type] = data;

        public void LoadFromJson(string json)
        {
            try
            {
                var list = JsonSerializer.Deserialize<List<StatusEffectData>>(json);
                if (list == null) return;
                foreach (var d in list) RegisterDefinition(d);
                Logger.Info($"StatusEffectSystem: loaded {list.Count} definitions.");
            }
            catch (Exception ex)
            {
                Logger.Error($"StatusEffectSystem: JSON load failed: {ex.Message}");
            }
        }

        // ─────────────────────── Apply ───────────────────────

        /// <summary>
        /// Applies a status effect to a target entity.
        /// Respects stack limits — refreshes duration if already at limit.
        /// </summary>
        public bool Apply(StatusEffectType type, string targetId, string sourceId)
        {
            if (!_definitions.TryGetValue(type, out var def))
            {
                Logger.Warning($"StatusEffectSystem: Unknown effect type '{type}'.");
                return false;
            }

            if (!_active.TryGetValue(targetId, out var effects))
            {
                effects = new List<ActiveStatusEffect>();
                _active[targetId] = effects;
            }

            // Count existing stacks
            int stacks = 0;
            ActiveStatusEffect? existing = null;
            foreach (var e in effects)
            {
                if (e.Data.Type == type) { stacks++; existing = e; }
            }

            if (stacks >= def.StackLimit && existing != null)
            {
                // Refresh duration instead of adding new stack
                existing.TimeRemaining = def.Duration;
                return true;
            }

            effects.Add(new ActiveStatusEffect
            {
                Data          = def,
                SourceId      = sourceId,
                TimeRemaining = def.Duration,
                StackCount    = 1
            });

            Logger.Info($"StatusEffectSystem: {type} applied to '{targetId}' by '{sourceId}'");
            return true;
        }

        // ─────────────────────── Tick ───────────────────────

        /// <summary>
        /// Updates all active effects for all entities. Call every frame with delta.
        /// Returns a list of (entityId, tickDamage, tickDamageType) for this frame's damage ticks.
        /// </summary>
        public List<(string entityId, float damage, DamageType dmgType)> Tick(double delta)
        {
            var tickDamages = new List<(string, float, DamageType)>();
            var expired     = new List<(string entityId, ActiveStatusEffect effect)>();

            foreach (var kv in _active)
            {
                string entityId = kv.Key;
                foreach (var effect in kv.Value)
                {
                    effect.TimeRemaining    -= (float)delta;
                    effect.TickAccumulator  += (float)delta;

                    // Tick damage
                    if (effect.Data.TickDamage > 0f &&
                        effect.TickAccumulator >= effect.Data.TickInterval)
                    {
                        tickDamages.Add((entityId, effect.Data.TickDamage, effect.Data.TickDamageType));
                        effect.TickAccumulator -= effect.Data.TickInterval;
                    }

                    if (effect.TimeRemaining <= 0f)
                        expired.Add((entityId, effect));
                }
            }

            // Remove expired
            foreach (var (entityId, effect) in expired)
            {
                if (_active.TryGetValue(entityId, out var list))
                {
                    list.Remove(effect);
                    Logger.Info($"StatusEffectSystem: {effect.Data.Type} expired on '{entityId}'");
                }
            }

            return tickDamages;
        }

        // ─────────────────────── Queries ───────────────────────

        public bool HasEffect(string entityId, StatusEffectType type)
        {
            if (!_active.TryGetValue(entityId, out var effects)) return false;
            foreach (var e in effects) if (e.Data.Type == type) return true;
            return false;
        }

        public void RemoveAll(string entityId) => _active.Remove(entityId);

        public IReadOnlyList<ActiveStatusEffect> GetEffects(string entityId) =>
            _active.TryGetValue(entityId, out var list)
                ? list.AsReadOnly()
                : Array.Empty<ActiveStatusEffect>();

        // ─────────────────────── Defaults ───────────────────────

        private void RegisterDefaults()
        {
            _definitions[StatusEffectType.Burn]         = new StatusEffectData { Type = StatusEffectType.Burn,         Duration = 5f,  TickInterval = 1f, TickDamage = 4f,  TickDamageType = DamageType.Fire,     VfxHookKey = "vfx_burn"  };
            _definitions[StatusEffectType.Freeze]       = new StatusEffectData { Type = StatusEffectType.Freeze,       Duration = 3f,  TickInterval = 99f,TickDamage = 0f,  StatModifier = 0f,  StackLimit = 1, VfxHookKey = "vfx_freeze" };
            _definitions[StatusEffectType.Shock]        = new StatusEffectData { Type = StatusEffectType.Shock,        Duration = 2f,  TickInterval = 0.5f,TickDamage = 2f, TickDamageType = DamageType.Lightning, VfxHookKey = "vfx_shock" };
            _definitions[StatusEffectType.Poison]       = new StatusEffectData { Type = StatusEffectType.Poison,       Duration = 8f,  TickInterval = 1f, TickDamage = 3f,  TickDamageType = DamageType.Poison,   StackLimit = 3, VfxHookKey = "vfx_poison" };
            _definitions[StatusEffectType.Bleed]        = new StatusEffectData { Type = StatusEffectType.Bleed,        Duration = 6f,  TickInterval = 1f, TickDamage = 5f,  TickDamageType = DamageType.Physical,  StackLimit = 5, VfxHookKey = "vfx_bleed" };
            _definitions[StatusEffectType.Slow]         = new StatusEffectData { Type = StatusEffectType.Slow,         Duration = 4f,  TickInterval = 99f,TickDamage = 0f,  StatModifier = -0.5f, VfxHookKey = "vfx_slow" };
            _definitions[StatusEffectType.Stun]         = new StatusEffectData { Type = StatusEffectType.Stun,         Duration = 2f,  TickInterval = 99f,TickDamage = 0f,  StackLimit = 1, VfxHookKey = "vfx_stun" };
            _definitions[StatusEffectType.Silence]      = new StatusEffectData { Type = StatusEffectType.Silence,      Duration = 3f,  TickInterval = 99f,TickDamage = 0f,  StackLimit = 1, VfxHookKey = "vfx_silence" };
            _definitions[StatusEffectType.Knockback]    = new StatusEffectData { Type = StatusEffectType.Knockback,    Duration = 0.5f,TickInterval = 99f,TickDamage = 0f,  StackLimit = 1, VfxHookKey = "vfx_knockback" };
            _definitions[StatusEffectType.Regeneration] = new StatusEffectData { Type = StatusEffectType.Regeneration, Duration = 10f, TickInterval = 1f, TickDamage = -5f, TickDamageType = DamageType.Healing, VfxHookKey = "vfx_regen" };
        }
    }
}
