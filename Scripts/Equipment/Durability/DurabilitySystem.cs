using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Equipment.Durability
{
    /// <summary>
    /// Tracks equipment wear, break state, and provides hooks for repair and visual damage.
    /// </summary>
    public class DurabilityComponent
    {
        public string ItemId { get; }
        public float CurrentDurability { get; private set; }
        public float MaxDurability { get; private set; }
        public bool IsBroken => CurrentDurability <= 0f;
        public float DurabilityPercent => MaxDurability > 0f ? CurrentDurability / MaxDurability : 0f;

        /// <summary>Fired when durability changes. Parameters: itemId, oldValue, newValue.</summary>
        public event Action<string, float, float> OnDurabilityChanged;
        /// <summary>Fired when the item breaks.</summary>
        public event Action<string> OnItemBroken;
        /// <summary>Fired when the item is repaired.</summary>
        public event Action<string> OnItemRepaired;

        public DurabilityComponent(string itemId, float maxDurability)
        {
            ItemId = itemId;
            MaxDurability = Math.Max(1f, maxDurability);
            CurrentDurability = MaxDurability;
        }

        /// <summary>
        /// Initializes with specific current and max durability values (used for save/load).
        /// </summary>
        public void Initialize(float currentDurability, float maxDurability)
        {
            MaxDurability = Math.Max(1f, maxDurability);
            CurrentDurability = Math.Clamp(currentDurability, 0f, MaxDurability);
        }

        /// <summary>
        /// Applies durability damage. Returns the actual amount of damage applied.
        /// </summary>
        public float ApplyDamage(float amount)
        {
            if (IsBroken || amount <= 0f) return 0f;

            float oldValue = CurrentDurability;
            CurrentDurability = Math.Max(0f, CurrentDurability - amount);
            float actualDamage = oldValue - CurrentDurability;

            OnDurabilityChanged?.Invoke(ItemId, oldValue, CurrentDurability);

            if (CurrentDurability <= 0f)
            {
                OnItemBroken?.Invoke(ItemId);
                Logger.Warning($"DurabilitySystem: Item '{ItemId}' has broken.");
            }

            return actualDamage;
        }

        /// <summary>
        /// Repairs durability by the given amount. Returns the actual amount repaired.
        /// </summary>
        public float Repair(float amount)
        {
            if (amount <= 0f) return 0f;

            bool wasBroken = IsBroken;
            float oldValue = CurrentDurability;
            CurrentDurability = Math.Min(MaxDurability, CurrentDurability + amount);
            float actualRepair = CurrentDurability - oldValue;

            OnDurabilityChanged?.Invoke(ItemId, oldValue, CurrentDurability);

            if (wasBroken && !IsBroken)
            {
                OnItemRepaired?.Invoke(ItemId);
                Logger.Info($"DurabilitySystem: Item '{ItemId}' has been repaired.");
            }

            return actualRepair;
        }

        /// <summary>
        /// Fully repairs the item to maximum durability.
        /// </summary>
        public void RepairFully()
        {
            Repair(MaxDurability - CurrentDurability);
        }

        /// <summary>
        /// Sets the maximum durability (e.g., after an upgrade).
        /// Current durability is adjusted proportionally if it exceeds the new maximum.
        /// </summary>
        public void SetMaxDurability(float newMax)
        {
            newMax = Math.Max(1f, newMax);
            float oldMax = MaxDurability;
            MaxDurability = newMax;
            CurrentDurability = Math.Min(CurrentDurability, MaxDurability);
            OnDurabilityChanged?.Invoke(ItemId, oldMax, newMax);
        }
    }

    /// <summary>
    /// Defines durability damage sources and their damage amounts.
    /// </summary>
    public class DurabilityDamageSource
    {
        public string SourceId { get; }
        public string DisplayName { get; }
        public float DamagePerUse { get; }
        public bool CanCriticallyDamage { get; }

        public DurabilityDamageSource(string sourceId, string displayName, float damagePerUse, bool canCriticallyDamage = false)
        {
            SourceId = sourceId;
            DisplayName = displayName;
            DamagePerUse = damagePerUse;
            CanCriticallyDamage = canCriticallyDamage;
        }
    }

    /// <summary>
    /// Central system managing durability for all equipment items.
    /// </summary>
    public class DurabilityManager
    {
        private readonly Dictionary<string, DurabilityComponent> _durabilityComponents = new();
        private readonly Dictionary<string, DurabilityDamageSource> _damageSources = new();
        private readonly Random _rng = new();

        // Default damage sources
        public static readonly DurabilityDamageSource DefaultAttack = new("dmg_attack", "Attack", 0.5f);
        public static readonly DurabilityDamageSource DefaultBlock = new("dmg_block", "Block", 0.3f);
        public static readonly DurabilityDamageSource DefaultHit = new("dmg_hit", "Getting Hit", 1.0f, true);
        public static readonly DurabilityDamageSource DefaultAbility = new("dmg_ability", "Ability Use", 0.8f);

        // ---------------------------------------------------------------
        // REGISTRATION
        // ---------------------------------------------------------------

        /// <summary>
        /// Registers a durability damage source definition.
        /// </summary>
        public void RegisterDamageSource(DurabilityDamageSource source)
        {
            _damageSources[source.SourceId] = source;
        }

        /// <summary>
        /// Registers a durability component for an item.
        /// </summary>
        public DurabilityComponent RegisterItem(string itemId, float maxDurability)
        {
            if (_durabilityComponents.TryGetValue(itemId, out var existing))
                return existing;

            var component = new DurabilityComponent(itemId, maxDurability);
            _durabilityComponents[itemId] = component;
            return component;
        }

        /// <summary>
        /// Gets the durability component for an item.
        /// </summary>
        public DurabilityComponent GetDurability(string itemId)
        {
            return _durabilityComponents.TryGetValue(itemId, out var comp) ? comp : null;
        }

        /// <summary>
        /// Removes a durability component (e.g., item destroyed or consumed).
        /// </summary>
        public void RemoveItem(string itemId)
        {
            _durabilityComponents.Remove(itemId);
        }

        // ---------------------------------------------------------------
        // DAMAGE APPLICATION
        // ---------------------------------------------------------------

        /// <summary>
        /// Applies durability damage from a registered source.
        /// </summary>
        public float ApplyDamageFromSource(string itemId, string sourceId)
        {
            var comp = GetDurability(itemId);
            if (comp == null) return 0f;

            if (!_damageSources.TryGetValue(sourceId, out var source))
            {
                Logger.Warning($"DurabilityManager: Unknown damage source '{sourceId}'.");
                return 0f;
            }

            float damage = source.DamagePerUse;
            if (source.CanCriticallyDamage && _rng.NextDouble() < 0.1f) // 10% crit damage
            {
                damage *= 2f;
            }

            return comp.ApplyDamage(damage);
        }

        /// <summary>
        /// Applies durability damage with a custom amount.
        /// </summary>
        public float ApplyCustomDamage(string itemId, float amount)
        {
            var comp = GetDurability(itemId);
            return comp?.ApplyDamage(amount) ?? 0f;
        }

        // ---------------------------------------------------------------
        // REPAIR
        // ---------------------------------------------------------------

        /// <summary>
        /// Repairs an item by the given amount.
        /// </summary>
        public float RepairItem(string itemId, float amount)
        {
            var comp = GetDurability(itemId);
            return comp?.Repair(amount) ?? 0f;
        }

        /// <summary>
        /// Fully repairs an item.
        /// </summary>
        public void RepairItemFully(string itemId)
        {
            var comp = GetDurability(itemId);
            comp?.RepairFully();
        }

        // ---------------------------------------------------------------
        // SAVE/LOAD
        // ---------------------------------------------------------------

        /// <summary>
        /// Gets a snapshot of all durability states for save.
        /// </summary>
        public Dictionary<string, DurabilitySaveData> GetSaveData()
        {
            var data = new Dictionary<string, DurabilitySaveData>();
            foreach (var kvp in _durabilityComponents)
            {
                data[kvp.Key] = new DurabilitySaveData
                {
                    CurrentDurability = kvp.Value.CurrentDurability,
                    MaxDurability = kvp.Value.MaxDurability
                };
            }
            return data;
        }

        /// <summary>
        /// Restores durability state from save data.
        /// </summary>
        public void LoadSaveData(Dictionary<string, DurabilitySaveData> data)
        {
            _durabilityComponents.Clear();
            if (data == null) return;

            foreach (var kvp in data)
            {
                var comp = new DurabilityComponent(kvp.Key, kvp.Value.MaxDurability);
                comp.Initialize(kvp.Value.CurrentDurability, kvp.Value.MaxDurability);
                _durabilityComponents[kvp.Key] = comp;

                // Re-fire events on load
                comp.OnDurabilityChanged?.Invoke(kvp.Key, 0f, comp.CurrentDurability);
                if (comp.IsBroken)
                    comp.OnItemBroken?.Invoke(kvp.Key);
            }
        }

        /// <summary>
        /// Registers default damage sources.
        /// </summary>
        public void RegisterDefaultSources()
        {
            RegisterDamageSource(DefaultAttack);
            RegisterDamageSource(DefaultBlock);
            RegisterDamageSource(DefaultHit);
            RegisterDamageSource(DefaultAbility);
        }
    }

    /// <summary>
    /// Save data for a single item's durability state.
    /// </summary>
    public class DurabilitySaveData
    {
        public float CurrentDurability { get; set; }
        public float MaxDurability { get; set; }
    }
}