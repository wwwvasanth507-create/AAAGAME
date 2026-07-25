using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Player
{
    // ----------------------------------------------------------------
    // Events
    // ----------------------------------------------------------------
    public record AbilityUsedEvent(string AbilityId, string DisplayName, float Cooldown);
    public record AbilityCooldownCompleteEvent(string AbilityId);
    public record AbilityFailedEvent(string AbilityId, string Reason);

    /// <summary>
    /// AbilityExecutor manages the 4 equipped ability slots for the player.
    /// Responsibilities:
    ///   - Cooldown tracking per slot
    ///   - Mana / stamina resource validation before execution
    ///   - Damage delivery via CombatManager (future hook)
    ///   - EventBus events: AbilityUsed / AbilityCooldownComplete / AbilityFailed
    /// </summary>
    public class AbilityExecutor
    {
        // ----------------------------------------------------------------
        // Constants
        // ----------------------------------------------------------------
        public const int SlotCount = 4;

        // ----------------------------------------------------------------
        // Slot state
        // ----------------------------------------------------------------
        private readonly AbilityDefinition?[] _slots     = new AbilityDefinition?[SlotCount];
        private readonly float[]              _cooldowns = new float[SlotCount];

        // ----------------------------------------------------------------
        // Player resource references (callbacks for deferred stat access)
        // ----------------------------------------------------------------
        private readonly Func<float> _getMana;
        private readonly Func<float> _getStamina;
        private readonly Action<float> _spendMana;
        private readonly Action<float> _spendStamina;

        // ----------------------------------------------------------------
        // Constructor
        // ----------------------------------------------------------------
        public AbilityExecutor(
            Func<float>    getMana,
            Func<float>    getStamina,
            Action<float>  spendMana,
            Action<float>  spendStamina)
        {
            _getMana      = getMana;
            _getStamina   = getStamina;
            _spendMana    = spendMana;
            _spendStamina = spendStamina;
        }

        // ----------------------------------------------------------------
        // Slot management
        // ----------------------------------------------------------------
        public bool EquipAbility(int slot, AbilityDefinition ability)
        {
            if (slot < 0 || slot >= SlotCount)
            {
                Logger.Error($"AbilityExecutor: Invalid slot {slot}. Must be 0-{SlotCount - 1}.");
                return false;
            }
            _slots[slot] = ability;
            Logger.Info($"AbilityExecutor: Slot {slot} ← '{ability.Data.AbilityId}'");
            return true;
        }

        public void UnequipSlot(int slot)
        {
            if (slot >= 0 && slot < SlotCount) _slots[slot] = null;
        }

        public AbilityDefinition? GetSlot(int slot) =>
            (slot >= 0 && slot < SlotCount) ? _slots[slot] : null;

        // ----------------------------------------------------------------
        // Tick — must be called every frame with delta time (seconds)
        // ----------------------------------------------------------------
        public void Tick(float delta)
        {
            for (int i = 0; i < SlotCount; i++)
            {
                if (_cooldowns[i] <= 0f) continue;
                _cooldowns[i] -= delta;
                if (_cooldowns[i] <= 0f)
                {
                    _cooldowns[i] = 0f;
                    string? id = _slots[i]?.Data.AbilityId;
                    if (id != null) EventBus.Publish(new AbilityCooldownCompleteEvent(id));
                }
            }
        }

        // ----------------------------------------------------------------
        // Execute ability in a given slot
        // Returns true if execution was successful.
        // ----------------------------------------------------------------
        public bool Execute(int slot, object? targetContext = null)
        {
            if (slot < 0 || slot >= SlotCount)
            {
                Logger.Warning($"AbilityExecutor: Execute called with out-of-range slot {slot}.");
                return false;
            }

            var ability = _slots[slot];
            if (ability == null)
            {
                Logger.Info($"AbilityExecutor: Slot {slot} is empty.");
                return false;
            }

            string id = ability.Data.AbilityId;

            // Check cooldown
            if (_cooldowns[slot] > 0f)
            {
                string reason = $"On cooldown ({_cooldowns[slot]:F1}s remaining)";
                EventBus.Publish(new AbilityFailedEvent(id, reason));
                return false;
            }

            // Check mana
            if (ability.Data.ManaCost > 0f && _getMana() < ability.Data.ManaCost)
            {
                EventBus.Publish(new AbilityFailedEvent(id, "Not enough mana"));
                return false;
            }

            // Check stamina
            if (ability.Data.StaminaCost > 0f && _getStamina() < ability.Data.StaminaCost)
            {
                EventBus.Publish(new AbilityFailedEvent(id, "Not enough stamina"));
                return false;
            }

            // Deduct resources
            if (ability.Data.ManaCost    > 0f) _spendMana(ability.Data.ManaCost);
            if (ability.Data.StaminaCost > 0f) _spendStamina(ability.Data.StaminaCost);

            // Apply cooldown
            _cooldowns[slot] = ability.Data.CooldownSec;

            // Dispatch execution to the appropriate handler
            DispatchEffect(ability, targetContext);

            Logger.Info($"AbilityExecutor: Executed '{id}' from slot {slot}. CD={ability.Data.CooldownSec}s");
            EventBus.Publish(new AbilityUsedEvent(id, ability.Data.DisplayName, ability.Data.CooldownSec));
            return true;
        }

        // ----------------------------------------------------------------
        // Effect dispatch — routes to CombatManager or self-effect
        // ----------------------------------------------------------------
        private void DispatchEffect(AbilityDefinition ability, object? context)
        {
            switch (ability.Data.TargetType)
            {
                case AbilityTargetType.Self:
                    ApplySelfEffect(ability);
                    break;

                case AbilityTargetType.SingleEnemy:
                case AbilityTargetType.Projectile:
                case AbilityTargetType.AoE:
                case AbilityTargetType.Directional:
                    // Damage delivery delegated to CombatManager in GameLoop.
                    // ExecutionContext carries the target info (resolved upstream).
                    Logger.Info($"AbilityExecutor: Dispatched '{ability.Data.AbilityId}' " +
                                $"({ability.Data.TargetType}) for {ability.Data.BaseDamage} dmg.");
                    break;
            }
        }

        private static void ApplySelfEffect(AbilityDefinition ability)
        {
            // Self-targeting abilities (buffs, barriers) publish to the stats system.
            Logger.Info($"AbilityExecutor: Self-effect '{ability.Data.AbilityId}' " +
                        $"applied for {ability.Data.Duration}s.");
        }

        // ----------------------------------------------------------------
        // State queries
        // ----------------------------------------------------------------
        public bool IsOnCooldown(int slot) => slot >= 0 && slot < SlotCount && _cooldowns[slot] > 0f;
        public float GetCooldownRemaining(int slot) =>
            (slot >= 0 && slot < SlotCount) ? MathF.Max(0f, _cooldowns[slot]) : 0f;

        public override string ToString()
        {
            var parts = new List<string>();
            for (int i = 0; i < SlotCount; i++)
            {
                string id = _slots[i]?.Data.AbilityId ?? "empty";
                float  cd = _cooldowns[i];
                parts.Add($"Slot{i}={id}(CD:{cd:F1}s)");
            }
            return $"AbilityExecutor[{string.Join(", ", parts)}]";
        }
    }
}
