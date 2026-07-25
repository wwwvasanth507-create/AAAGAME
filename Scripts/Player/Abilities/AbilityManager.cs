using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;
using HeroOfEternia.Player.Resources;
using HeroOfEternia.Player.Progression;

namespace HeroOfEternia.Player.Abilities
{
    // ----------------------------------------------------------------
    // Events
    // ----------------------------------------------------------------
    public record AbilityActivatedEvent(string AbilityId, string DisplayName, float Cooldown, float CastTime);
    public record AbilityCompletedEvent(string AbilityId, bool Success);
    public record AbilityInterruptedEvent(string AbilityId, string Reason);
    public record AbilityCooldownCompleteEvent(string AbilityId);
    public record AbilityFailedEvent(string AbilityId, string Reason);
    public record AbilityChargesChangedEvent(string AbilityId, int CurrentCharges, int MaxCharges);
    public record AbilityCastStartedEvent(string AbilityId, float CastTime);
    public record AbilityResourceConsumedEvent(string AbilityId, string ResourceType, float Amount);

    /// <summary>
    /// Tracks the runtime state of a single ability instance.
    /// </summary>
    public class AbilityState
    {
        public string AbilityId { get; }
        public float CooldownRemaining { get; set; }
        public float CastTimeRemaining { get; set; }
        public int CurrentCharges { get; set; }
        public int MaxCharges { get; set; }
        public float ChargeRechargeTimer { get; set; }
        public bool IsCasting { get; set; }
        public bool IsOnCooldown => CooldownRemaining > 0f;
        public bool IsReady => !IsOnCooldown && !IsCasting && CurrentCharges > 0;

        public AbilityState(string abilityId, int maxCharges = 1, float chargeRechargeSec = 0f)
        {
            AbilityId = abilityId;
            MaxCharges = maxCharges;
            CurrentCharges = maxCharges;
            ChargeRechargeTimer = chargeRechargeSec;
        }

        public void TickCooldown(float delta)
        {
            if (CooldownRemaining > 0f)
                CooldownRemaining = MathF.Max(0f, CooldownRemaining - delta);
        }

        public void TickCast(float delta)
        {
            if (IsCasting && CastTimeRemaining > 0f)
            {
                CastTimeRemaining -= delta;
                if (CastTimeRemaining <= 0f)
                {
                    CastTimeRemaining = 0f;
                    IsCasting = false;
                }
            }
        }

        public void TickChargeRecharge(float delta)
        {
            if (CurrentCharges < MaxCharges && ChargeRechargeTimer > 0f)
            {
                ChargeRechargeTimer -= delta;
                if (ChargeRechargeTimer <= 0f)
                {
                    CurrentCharges = Math.Min(MaxCharges, CurrentCharges + 1);
                    ChargeRechargeTimer = ChargeRechargeTimer; // Reset to original interval
                }
            }
        }

        public void StartCooldown(float duration)
        {
            CooldownRemaining = duration;
        }

        public void StartCast(float castTime)
        {
            if (castTime > 0f)
            {
                IsCasting = true;
                CastTimeRemaining = castTime;
            }
        }

        public void Interrupt()
        {
            IsCasting = false;
            CastTimeRemaining = 0f;
        }

        public bool ConsumeCharge()
        {
            if (CurrentCharges <= 0) return false;
            CurrentCharges--;
            if (CurrentCharges < MaxCharges)
                ChargeRechargeTimer = ChargeRechargeTimer; // Will be set externally
            return true;
        }

        public override string ToString() =>
            $"[{AbilityId}] CD:{CooldownRemaining:F1}s Cast:{CastTimeRemaining:F1}s Charges:{CurrentCharges}/{MaxCharges}";
    }

    /// <summary>
    /// Result of an ability execution attempt.
    /// </summary>
    public class AbilityResult
    {
        public bool Success { get; set; }
        public string AbilityId { get; set; } = string.Empty;
        public string FailureReason { get; set; } = string.Empty;
        public float DamageDealt { get; set; }
        public float HealingDone { get; set; }
        public float ShieldApplied { get; set; }
        public List<string> EffectsApplied { get; set; } = new();
    }

    /// <summary>
    /// Configuration for ability execution behavior.
    /// </summary>
    public class AbilityExecutionConfig
    {
        public bool AllowMovementWhileCasting { get; set; } = false;
        public bool AllowInterruption { get; set; } = true;
        public bool RequireLineOfSight { get; set; } = false;
        public bool RequireTarget { get; set; } = true;
        public float MaxCastRange { get; set; } = 25f;
        public float GlobalCooldownSec { get; set; } = 0.5f;
    }

    /// <summary>
    /// Manages ability activation, cancellation, interruptions, cooldowns, charges,
    /// resource consumption, target validation, animation triggers, VFX/SFX hooks.
    /// Network-ready architecture with event-driven design.
    /// </summary>
    public class AbilityManager
    {
        // ----------------------------------------------------------------
        // Dependencies
        // ----------------------------------------------------------------
        private readonly AbilityDatabase _abilityDatabase;
        private readonly ResourceManager _resourceManager;
        private readonly PlayerProgression _progression;
        private readonly EffectsManager _effectsManager;
        private readonly LoadoutManager _loadoutManager;

        // ----------------------------------------------------------------
        // State
        // ----------------------------------------------------------------
        private readonly Dictionary<string, AbilityState> _abilityStates = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<int, string> _slotToAbilityId = new();
        private float _globalCooldownRemaining;

        // ----------------------------------------------------------------
        // Configuration
        // ----------------------------------------------------------------
        public AbilityExecutionConfig Config { get; set; } = new();

        // ----------------------------------------------------------------
        // Events
        // ----------------------------------------------------------------
        public event Action<AbilityActivatedEvent>? OnAbilityActivated;
        public event Action<AbilityCompletedEvent>? OnAbilityCompleted;
        public event Action<AbilityInterruptedEvent>? OnAbilityInterrupted;
        public event Action<AbilityCooldownCompleteEvent>? OnCooldownComplete;
        public event Action<AbilityFailedEvent>? OnAbilityFailed;
        public event Action<AbilityChargesChangedEvent>? OnChargesChanged;
        public event Action<AbilityCastStartedEvent>? OnCastStarted;
        public event Action<AbilityResourceConsumedEvent>? OnResourceConsumed;

        // ----------------------------------------------------------------
        // Constructor
        // ----------------------------------------------------------------
        public AbilityManager(
            AbilityDatabase abilityDatabase,
            ResourceManager resourceManager,
            PlayerProgression progression,
            EffectsManager effectsManager,
            LoadoutManager loadoutManager)
        {
            _abilityDatabase = abilityDatabase;
            _resourceManager = resourceManager;
            _progression = progression;
            _effectsManager = effectsManager;
            _loadoutManager = loadoutManager;

            Logger.Info("AbilityManager: Initialized.");
        }

        // ----------------------------------------------------------------
        // Registration
        // ----------------------------------------------------------------
        public void RegisterAbility(string abilityId)
        {
            var def = _abilityDatabase.Get(abilityId);
            if (def == null)
            {
                Logger.Warning($"AbilityManager: Cannot register unknown ability '{abilityId}'.");
                return;
            }

            if (_abilityStates.ContainsKey(abilityId))
            {
                Logger.Warning($"AbilityManager: Ability '{abilityId}' already registered.");
                return;
            }

            var state = new AbilityState(
                abilityId,
                def.Data.MaxCharges,
                def.Data.ChargeRechargeSec
            );
            _abilityStates[abilityId] = state;
            Logger.Info($"AbilityManager: Registered ability '{abilityId}'.");
        }

        public void RegisterAbilities(IEnumerable<string> abilityIds)
        {
            foreach (var id in abilityIds)
                RegisterAbility(id);
        }

        public void RegisterAllFromDatabase()
        {
            foreach (var def in _abilityDatabase.GetAll())
                RegisterAbility(def.Data.AbilityId);
        }

        public void UnregisterAbility(string abilityId)
        {
            _abilityStates.Remove(abilityId);
        }

        // ----------------------------------------------------------------
        // Slot Binding
        // ----------------------------------------------------------------
        public void BindSlot(int slotIndex, string abilityId)
        {
            if (slotIndex < 0 || slotIndex >= AbilityExecutor.SlotCount)
            {
                Logger.Error($"AbilityManager: Invalid slot index {slotIndex}.");
                return;
            }

            if (!string.IsNullOrEmpty(abilityId) && !_abilityStates.ContainsKey(abilityId))
            {
                Logger.Warning($"AbilityManager: Cannot bind slot {slotIndex} to unregistered ability '{abilityId}'. Registering now.");
                RegisterAbility(abilityId);
            }

            _slotToAbilityId[slotIndex] = abilityId ?? string.Empty;
            Logger.Info($"AbilityManager: Slot {slotIndex} bound to '{abilityId}'.");
        }

        public void UnbindSlot(int slotIndex)
        {
            _slotToAbilityId.Remove(slotIndex);
        }

        public string GetSlotAbility(int slotIndex)
        {
            return _slotToAbilityId.TryGetValue(slotIndex, out var id) ? id : string.Empty;
        }

        // ----------------------------------------------------------------
        // Tick — must be called every frame
        // ----------------------------------------------------------------
        public void Tick(float delta)
        {
            // Global cooldown
            if (_globalCooldownRemaining > 0f)
                _globalCooldownRemaining = MathF.Max(0f, _globalCooldownRemaining - delta);

            // Per-ability state ticks
            foreach (var state in _abilityStates.Values)
            {
                bool wasOnCooldown = state.IsOnCooldown;
                state.TickCooldown(delta);
                state.TickCast(delta);
                state.TickChargeRecharge(delta);

                // Fire cooldown complete event
                if (wasOnCooldown && !state.IsOnCooldown)
                {
                    EventBus.Publish(new AbilityCooldownCompleteEvent(state.AbilityId));
                    OnCooldownComplete?.Invoke(new AbilityCooldownCompleteEvent(state.AbilityId));
                }
            }
        }

        // ----------------------------------------------------------------
        // Activation
        // ----------------------------------------------------------------
        public AbilityResult ActivateAbility(string abilityId, object? targetContext = null)
        {
            var result = new AbilityResult { AbilityId = abilityId };

            // 1. Validate ability exists
            var def = _abilityDatabase.Get(abilityId);
            if (def == null)
            {
                result.FailureReason = $"Unknown ability '{abilityId}'.";
                Fail(result);
                return result;
            }

            // 2. Validate state exists
            if (!_abilityStates.TryGetValue(abilityId, out var state))
            {
                result.FailureReason = $"Ability '{abilityId}' not registered.";
                Fail(result);
                return result;
            }

            // 3. Check global cooldown
            if (_globalCooldownRemaining > 0f)
            {
                result.FailureReason = $"Global cooldown active ({_globalCooldownRemaining:F1}s).";
                Fail(result);
                return result;
            }

            // 4. Check cooldown
            if (state.IsOnCooldown)
            {
                result.FailureReason = $"On cooldown ({state.CooldownRemaining:F1}s remaining).";
                Fail(result);
                return result;
            }

            // 5. Check charges
            if (state.CurrentCharges <= 0)
            {
                result.FailureReason = "No charges remaining.";
                Fail(result);
                return result;
            }

            // 6. Check if already casting
            if (state.IsCasting)
            {
                result.FailureReason = "Already casting.";
                Fail(result);
                return result;
            }

            // 7. Check level requirement
            if (!def.IsUnlocked(_progression.Level))
            {
                result.FailureReason = $"Requires level {def.Data.LevelRequired}.";
                Fail(result);
                return result;
            }

            // 8. Validate resources
            var resourceCheck = ValidateResources(def);
            if (!resourceCheck.IsValid)
            {
                result.FailureReason = resourceCheck.FailureReason;
                Fail(result);
                return result;
            }

            // 9. Validate target
            var targetCheck = ValidateTarget(def, targetContext);
            if (!targetCheck.IsValid)
            {
                result.FailureReason = targetCheck.FailureReason;
                Fail(result);
                return result;
            }

            // 10. Consume resources
            ConsumeResources(def);

            // 11. Consume charge
            state.ConsumeCharge();

            // 12. Start cast or execute immediately
            if (def.HasCastTime)
            {
                state.StartCast(def.Data.CastTime);
                var castEvent = new AbilityCastStartedEvent(abilityId, def.Data.CastTime);
                EventBus.Publish(castEvent);
                OnCastStarted?.Invoke(castEvent);
                Logger.Info($"AbilityManager: Started casting '{abilityId}' ({def.Data.CastTime:F1}s).");
            }

            // 13. Start cooldown
            state.StartCooldown(def.Data.CooldownSec);

            // 14. Apply global cooldown
            _globalCooldownRemaining = Config.GlobalCooldownSec;

            // 15. Fire activation event
            var activatedEvent = new AbilityActivatedEvent(
                abilityId, def.Data.DisplayName, def.Data.CooldownSec, def.Data.CastTime);
            EventBus.Publish(activatedEvent);
            OnAbilityActivated?.Invoke(activatedEvent);

            // 16. Trigger animation
            TriggerAnimation(def);

            // 17. Trigger VFX/SFX
            TriggerVisualEffects(def);
            TriggerAudio(def);

            // 18. If instant, complete immediately
            if (!def.HasCastTime)
            {
                CompleteAbility(abilityId, def, targetContext, result);
            }

            result.Success = true;
            Logger.Info($"AbilityManager: Activated '{abilityId}'.");
            return result;
        }

        public AbilityResult ActivateSlot(int slotIndex, object? targetContext = null)
        {
            if (!_slotToAbilityId.TryGetValue(slotIndex, out var abilityId) || string.IsNullOrEmpty(abilityId))
            {
                return new AbilityResult
                {
                    Success = false,
                    FailureReason = $"Slot {slotIndex} is empty."
                };
            }

            return ActivateAbility(abilityId, targetContext);
        }

        // ----------------------------------------------------------------
        // Cancellation & Interruption
        // ----------------------------------------------------------------
        public bool CancelAbility(string abilityId)
        {
            if (!_abilityStates.TryGetValue(abilityId, out var state)) return false;
            if (!state.IsCasting) return false;

            state.Interrupt();
            var evt = new AbilityInterruptedEvent(abilityId, "Cancelled by player.");
            EventBus.Publish(evt);
            OnAbilityInterrupted?.Invoke(evt);
            Logger.Info($"AbilityManager: Cancelled casting '{abilityId}'.");
            return true;
        }

        public bool InterruptAbility(string abilityId, string reason = "Interrupted")
        {
            if (!_abilityStates.TryGetValue(abilityId, out var state)) return false;
            if (!state.IsCasting) return false;

            state.Interrupt();
            var evt = new AbilityInterruptedEvent(abilityId, reason);
            EventBus.Publish(evt);
            OnAbilityInterrupted?.Invoke(evt);
            Logger.Info($"AbilityManager: Interrupted '{abilityId}': {reason}.");
            return true;
        }

        public void InterruptAll(string reason = "Interrupted")
        {
            foreach (var state in _abilityStates.Values)
            {
                if (state.IsCasting)
                {
                    state.Interrupt();
                    var evt = new AbilityInterruptedEvent(state.AbilityId, reason);
                    EventBus.Publish(evt);
                    OnAbilityInterrupted?.Invoke(evt);
                }
            }
        }

        // ----------------------------------------------------------------
        // Completion
        // ----------------------------------------------------------------
        private void CompleteAbility(string abilityId, AbilityDefinition def, object? targetContext, AbilityResult result)
        {
            // Apply effects
            ApplyEffects(def, targetContext, result);

            // Fire completion event
            var completedEvent = new AbilityCompletedEvent(abilityId, true);
            EventBus.Publish(completedEvent);
            OnAbilityCompleted?.Invoke(completedEvent);
        }

        // ----------------------------------------------------------------
        // Resource Validation
        // ----------------------------------------------------------------
        private (bool IsValid, string FailureReason) ValidateResources(AbilityDefinition def)
        {
            var data = def.Data;

            if (data.ManaCost > 0f && !_resourceManager.HasEnough(ResourceType.Mana, data.ManaCost))
                return (false, "Not enough mana.");

            if (data.StaminaCost > 0f && !_resourceManager.HasEnough(ResourceType.Stamina, data.StaminaCost))
                return (false, "Not enough stamina.");

            if (data.EnergyCost > 0f && !_resourceManager.HasEnough(ResourceType.Energy, data.EnergyCost))
                return (false, "Not enough energy.");

            if (data.FocusCost > 0f && !_resourceManager.HasEnough(ResourceType.Focus, data.FocusCost))
                return (false, "Not enough focus.");

            if (data.RageCost > 0f && !_resourceManager.HasEnough(ResourceType.Rage, data.RageCost))
                return (false, "Not enough rage.");

            if (data.SpiritCost > 0f && !_resourceManager.HasEnough(ResourceType.Spirit, data.SpiritCost))
                return (false, "Not enough spirit.");

            if (data.HealthCost > 0f && !_resourceManager.HasEnough(ResourceType.Health, data.HealthCost))
                return (false, "Not enough health.");

            return (true, string.Empty);
        }

        // ----------------------------------------------------------------
        // Resource Consumption
        // ----------------------------------------------------------------
        private void ConsumeResources(AbilityDefinition def)
        {
            var data = def.Data;

            if (data.ManaCost > 0f)
            {
                _resourceManager.Spend(ResourceType.Mana, data.ManaCost);
                EventBus.Publish(new AbilityResourceConsumedEvent(data.AbilityId, "Mana", data.ManaCost));
                OnResourceConsumed?.Invoke(new AbilityResourceConsumedEvent(data.AbilityId, "Mana", data.ManaCost));
            }

            if (data.StaminaCost > 0f)
            {
                _resourceManager.Spend(ResourceType.Stamina, data.StaminaCost);
                EventBus.Publish(new AbilityResourceConsumedEvent(data.AbilityId, "Stamina", data.StaminaCost));
                OnResourceConsumed?.Invoke(new AbilityResourceConsumedEvent(data.AbilityId, "Stamina", data.StaminaCost));
            }

            if (data.EnergyCost > 0f)
            {
                _resourceManager.Spend(ResourceType.Energy, data.EnergyCost);
                EventBus.Publish(new AbilityResourceConsumedEvent(data.AbilityId, "Energy", data.EnergyCost));
                OnResourceConsumed?.Invoke(new AbilityResourceConsumedEvent(data.AbilityId, "Energy", data.EnergyCost));
            }

            if (data.FocusCost > 0f)
            {
                _resourceManager.Spend(ResourceType.Focus, data.FocusCost);
                EventBus.Publish(new AbilityResourceConsumedEvent(data.AbilityId, "Focus", data.FocusCost));
                OnResourceConsumed?.Invoke(new AbilityResourceConsumedEvent(data.AbilityId, "Focus", data.FocusCost));
            }

            if (data.RageCost > 0f)
            {
                _resourceManager.Spend(ResourceType.Rage, data.RageCost);
                EventBus.Publish(new AbilityResourceConsumedEvent(data.AbilityId, "Rage", data.RageCost));
                OnResourceConsumed?.Invoke(new AbilityResourceConsumedEvent(data.AbilityId, "Rage", data.RageCost));
            }

            if (data.SpiritCost > 0f)
            {
                _resourceManager.Spend(ResourceType.Spirit, data.SpiritCost);
                EventBus.Publish(new AbilityResourceConsumedEvent(data.AbilityId, "Spirit", data.SpiritCost));
                OnResourceConsumed?.Invoke(new AbilityResourceConsumedEvent(data.AbilityId, "Spirit", data.SpiritCost));
            }

            if (data.HealthCost > 0f)
            {
                _resourceManager.Spend(ResourceType.Health, data.HealthCost);
                EventBus.Publish(new AbilityResourceConsumedEvent(data.AbilityId, "Health", data.HealthCost));
                OnResourceConsumed?.Invoke(new AbilityResourceConsumedEvent(data.AbilityId, "Health", data.HealthCost));
            }
        }

        // ----------------------------------------------------------------
        // Target Validation
        // ----------------------------------------------------------------
        private (bool IsValid, string FailureReason) ValidateTarget(AbilityDefinition def, object? targetContext)
        {
            switch (def.Data.TargetType)
            {
                case AbilityTargetType.Self:
                    // Self-targeting always valid
                    return (true, string.Empty);

                case AbilityTargetType.SingleEnemy:
                    if (Config.RequireTarget && targetContext == null)
                        return (false, "No target selected.");
                    return (true, string.Empty);

                case AbilityTargetType.AoE:
                case AbilityTargetType.Projectile:
                case AbilityTargetType.Directional:
                    return (true, string.Empty);

                default:
                    return (true, string.Empty);
            }
        }

        // ----------------------------------------------------------------
        // Effect Application
        // ----------------------------------------------------------------
        private void ApplyEffects(AbilityDefinition def, object? targetContext, AbilityResult result)
        {
            var data = def.Data;

            // Damage
            if (data.BaseDamage > 0f)
            {
                float damage = data.BaseDamage;
                result.DamageDealt = damage;
                Logger.Info($"AbilityManager: '{data.AbilityId}' dealt {damage} damage.");
            }

            // Healing
            if (data.BaseHealing > 0f)
            {
                float healing = data.BaseHealing;
                result.HealingDone = healing;
                Logger.Info($"AbilityManager: '{data.AbilityId}' healed for {healing}.");
            }

            // Shield
            if (data.ShieldAmount > 0f)
            {
                result.ShieldApplied = data.ShieldAmount;
                Logger.Info($"AbilityManager: '{data.AbilityId}' applied {data.ShieldAmount} shield.");
            }

            // Apply effects from EffectsManager
            var abilityEffects = _effectsManager.GetActiveEffects(data.AbilityId);
            foreach (var effect in abilityEffects)
            {
                result.EffectsApplied.Add(effect.EffectId);
            }
        }

        // ----------------------------------------------------------------
        // Animation / VFX / SFX Triggers
        // ----------------------------------------------------------------
        private void TriggerAnimation(AbilityDefinition def)
        {
            if (!string.IsNullOrEmpty(def.Data.AnimationReference))
            {
                Logger.Info($"AbilityManager: Triggering animation '{def.Data.AnimationReference}' for '{def.Data.AbilityId}'.");
                // Hook: AnimationController.Play(def.Data.AnimationReference)
            }
        }

        private void TriggerVisualEffects(AbilityDefinition def)
        {
            if (!string.IsNullOrEmpty(def.Data.VisualEffectReference))
            {
                Logger.Info($"AbilityManager: Triggering VFX '{def.Data.VisualEffectReference}' for '{def.Data.AbilityId}'.");
                // Hook: VFXManager.Play(def.Data.VisualEffectReference)
            }

            if (!string.IsNullOrEmpty(def.Data.VfxCastKey))
            {
                Logger.Info($"AbilityManager: Triggering cast VFX '{def.Data.VfxCastKey}' for '{def.Data.AbilityId}'.");
            }

            if (!string.IsNullOrEmpty(def.Data.VfxHitKey))
            {
                Logger.Info($"AbilityManager: Triggering hit VFX '{def.Data.VfxHitKey}' for '{def.Data.AbilityId}'.");
            }
        }

        private void TriggerAudio(AbilityDefinition def)
        {
            if (!string.IsNullOrEmpty(def.Data.AudioReference))
            {
                Logger.Info($"AbilityManager: Playing audio '{def.Data.AudioReference}' for '{def.Data.AbilityId}'.");
                // Hook: AudioManager.Play(def.Data.AudioReference)
            }

            if (!string.IsNullOrEmpty(def.Data.SfxCastKey))
            {
                Logger.Info($"AbilityManager: Playing cast SFX '{def.Data.SfxCastKey}' for '{def.Data.AbilityId}'.");
            }

            if (!string.IsNullOrEmpty(def.Data.SfxHitKey))
            {
                Logger.Info($"AbilityManager: Playing hit SFX '{def.Data.SfxHitKey}' for '{def.Data.AbilityId}'.");
            }
        }

        // ----------------------------------------------------------------
        // State Queries
        // ----------------------------------------------------------------
        public AbilityState? GetAbilityState(string abilityId)
        {
            _abilityStates.TryGetValue(abilityId, out var state);
            return state;
        }

        public bool IsAbilityReady(string abilityId)
        {
            return _abilityStates.TryGetValue(abilityId, out var state) && state.IsReady;
        }

        public bool IsSlotReady(int slotIndex)
        {
            if (!_slotToAbilityId.TryGetValue(slotIndex, out var abilityId) || string.IsNullOrEmpty(abilityId))
                return false;
            return IsAbilityReady(abilityId);
        }

        public float GetCooldownRemaining(string abilityId)
        {
            return _abilityStates.TryGetValue(abilityId, out var state) ? state.CooldownRemaining : 0f;
        }

        public float GetSlotCooldownRemaining(int slotIndex)
        {
            if (!_slotToAbilityId.TryGetValue(slotIndex, out var abilityId) || string.IsNullOrEmpty(abilityId))
                return 0f;
            return GetCooldownRemaining(abilityId);
        }

        public int GetCharges(string abilityId)
        {
            return _abilityStates.TryGetValue(abilityId, out var state) ? state.CurrentCharges : 0;
        }

        public bool IsCasting(string abilityId)
        {
            return _abilityStates.TryGetValue(abilityId, out var state) && state.IsCasting;
        }

        public float GetCastProgress(string abilityId)
        {
            if (!_abilityStates.TryGetValue(abilityId, out var state) || !state.IsCasting)
                return 0f;
            var def = _abilityDatabase.Get(abilityId);
            if (def == null || def.Data.CastTime <= 0f) return 0f;
            return 1f - (state.CastTimeRemaining / def.Data.CastTime);
        }

        public float GlobalCooldownRemaining => _globalCooldownRemaining;
        public int RegisteredAbilityCount => _abilityStates.Count;
        public int ActiveSlotCount => _slotToAbilityId.Count;

        // ----------------------------------------------------------------
        // Save/Load
        // ----------------------------------------------------------------
        public AbilityManagerSaveData CreateSaveData()
        {
            var data = new AbilityManagerSaveData
            {
                Version = 1
            };

            foreach (var kvp in _abilityStates)
            {
                data.AbilityStates[kvp.Key] = new AbilityStateSaveData
                {
                    CooldownRemaining = kvp.Value.CooldownRemaining,
                    CurrentCharges = kvp.Value.CurrentCharges,
                    IsCasting = kvp.Value.IsCasting,
                    CastTimeRemaining = kvp.Value.CastTimeRemaining
                };
            }

            foreach (var kvp in _slotToAbilityId)
            {
                data.SlotBindings[kvp.Key] = kvp.Value;
            }

            data.GlobalCooldownRemaining = _globalCooldownRemaining;
            return data;
        }

        public void LoadFromSaveData(AbilityManagerSaveData data)
        {
            if (data == null) return;

            // Restore ability states
            foreach (var kvp in data.AbilityStates)
            {
                if (_abilityStates.TryGetValue(kvp.Key, out var state))
                {
                    state.CooldownRemaining = kvp.Value.CooldownRemaining;
                    state.CurrentCharges = kvp.Value.CurrentCharges;
                    state.IsCasting = kvp.Value.IsCasting;
                    state.CastTimeRemaining = kvp.Value.CastTimeRemaining;
                }
            }

            // Restore slot bindings
            _slotToAbilityId.Clear();
            foreach (var kvp in data.SlotBindings)
            {
                _slotToAbilityId[kvp.Key] = kvp.Value;
            }

            _globalCooldownRemaining = data.GlobalCooldownRemaining;
            Logger.Info("AbilityManager: Loaded state from save data.");
        }

        // ----------------------------------------------------------------
        // Helpers
        // ----------------------------------------------------------------
        private void Fail(AbilityResult result)
        {
            result.Success = false;
            var evt = new AbilityFailedEvent(result.AbilityId, result.FailureReason);
            EventBus.Publish(evt);
            OnAbilityFailed?.Invoke(evt);
            Logger.Warning($"AbilityManager: Failed '{result.AbilityId}': {result.FailureReason}");
        }

        public string DebugSummary()
        {
            int ready = _abilityStates.Values.Count(s => s.IsReady);
            int onCooldown = _abilityStates.Values.Count(s => s.IsOnCooldown);
            int casting = _abilityStates.Values.Count(s => s.IsCasting);
            return $"AbilityManager: {_abilityStates.Count} registered, {ready} ready, {onCooldown} on CD, {casting} casting. GCD: {_globalCooldownRemaining:F2}s";
        }
    }

    // ----------------------------------------------------------------
    // Save Data Classes
    // ----------------------------------------------------------------
    public class AbilityStateSaveData
    {
        public float CooldownRemaining { get; set; }
        public int CurrentCharges { get; set; }
        public bool IsCasting { get; set; }
        public float CastTimeRemaining { get; set; }
    }

    public class AbilityManagerSaveData
    {
        public int Version { get; set; } = 1;
        public Dictionary<string, AbilityStateSaveData> AbilityStates { get; set; } = new();
        public Dictionary<int, string> SlotBindings { get; set; } = new();
        public float GlobalCooldownRemaining { get; set; }
    }
}