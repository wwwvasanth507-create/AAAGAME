using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;

namespace HeroOfEternia.Equipment.Upgrade
{
    /// <summary>
    /// Result of an upgrade attempt.
    /// </summary>
    public enum UpgradeResult
    {
        Success,
        Failure,
        CriticalFailure,
        MaxLevelReached,
        InvalidItem
    }

    /// <summary>
    /// Rules for upgrade success/failure at a given level.
    /// </summary>
    public class UpgradeLevelRule
    {
        public int Level { get; }
        public float BaseSuccessChance { get; }
        public float FailureDowngradeChance { get; }
        public float CriticalFailureDestroyChance { get; }
        public float StatMultiplierIncrease { get; }

        public UpgradeLevelRule(
            int level,
            float baseSuccessChance,
            float failureDowngradeChance = 0f,
            float criticalFailureDestroyChance = 0f,
            float statMultiplierIncrease = 0.1f)
        {
            Level = level;
            BaseSuccessChance = baseSuccessChance;
            FailureDowngradeChance = failureDowngradeChance;
            CriticalFailureDestroyChance = criticalFailureDestroyChance;
            StatMultiplierIncrease = statMultiplierIncrease;
        }
    }

    /// <summary>
    /// Runtime state of an item's upgrade level.
    /// </summary>
    public class UpgradeState
    {
        public string ItemId { get; }
        public int CurrentLevel { get; private set; }
        public int MaxLevel { get; private set; }
        public float CurrentMultiplier { get; private set; }

        /// <summary>Fired when the upgrade level changes. Parameters: itemId, oldLevel, newLevel.</summary>
        public event Action<string, int, int> OnUpgradeLevelChanged;
        /// <summary>Fired when an upgrade attempt fails.</summary>
        public event Action<string, int, UpgradeResult> OnUpgradeAttempted;

        public UpgradeState(string itemId, int maxLevel, float baseMultiplier = 1.0f)
        {
            ItemId = itemId;
            MaxLevel = maxLevel;
            CurrentLevel = 0;
            CurrentMultiplier = baseMultiplier;
        }

        /// <summary>
        /// Initializes with specific values (used for save/load).
        /// </summary>
        public void Initialize(int currentLevel, float currentMultiplier, int maxLevel)
        {
            CurrentLevel = Math.Clamp(currentLevel, 0, maxLevel);
            MaxLevel = maxLevel;
            CurrentMultiplier = currentMultiplier;
        }

        /// <summary>
        /// Attempts to upgrade the item. Returns the result.
        /// </summary>
        public UpgradeResult TryUpgrade(float successBonus, Random rng)
        {
            if (CurrentLevel >= MaxLevel)
                return UpgradeResult.MaxLevelReached;

            double roll = rng.NextDouble();
            double successChance = GetSuccessChance(successBonus);

            if (roll < successChance)
            {
                // Success
                int oldLevel = CurrentLevel;
                CurrentLevel++;
                CurrentMultiplier += GetLevelRule(CurrentLevel)?.StatMultiplierIncrease ?? 0.1f;
                OnUpgradeLevelChanged?.Invoke(ItemId, oldLevel, CurrentLevel);
                OnUpgradeAttempted?.Invoke(ItemId, CurrentLevel, UpgradeResult.Success);
                return UpgradeResult.Success;
            }

            // Check for critical failure (destroy)
            double criticalChance = GetLevelRule(CurrentLevel + 1)?.CriticalFailureDestroyChance ?? 0f;
            if (roll < successChance + criticalChance)
            {
                OnUpgradeAttempted?.Invoke(ItemId, CurrentLevel, UpgradeResult.CriticalFailure);
                return UpgradeResult.CriticalFailure;
            }

            // Check for failure (downgrade)
            double downgradeChance = GetLevelRule(CurrentLevel + 1)?.FailureDowngradeChance ?? 0f;
            if (roll < successChance + criticalChance + downgradeChance)
            {
                if (CurrentLevel > 0)
                {
                    CurrentLevel--;
                    CurrentMultiplier -= GetLevelRule(CurrentLevel + 1)?.StatMultiplierIncrease ?? 0.1f;
                }
            }

            OnUpgradeAttempted?.Invoke(ItemId, CurrentLevel, UpgradeResult.Failure);
            return UpgradeResult.Failure;
        }

        private float GetSuccessChance(float bonus)
        {
            var rule = GetLevelRule(CurrentLevel + 1);
            return Math.Clamp((rule?.BaseSuccessChance ?? 0.5f) + bonus, 0.05f, 0.99f);
        }

        private UpgradeLevelRule GetLevelRule(int level)
        {
            return DefaultUpgradeRules.FirstOrDefault(r => r.Level == level);
        }

        /// <summary>
        /// Default upgrade level rules for the game.
        /// </summary>
        public static readonly List<UpgradeLevelRule> DefaultUpgradeRules = new()
        {
            new(1,  0.95f, 0.00f, 0.00f, 0.10f),
            new(2,  0.90f, 0.00f, 0.00f, 0.10f),
            new(3,  0.85f, 0.00f, 0.00f, 0.10f),
            new(4,  0.80f, 0.05f, 0.00f, 0.10f),
            new(5,  0.75f, 0.10f, 0.00f, 0.10f),
            new(6,  0.65f, 0.15f, 0.05f, 0.10f),
            new(7,  0.55f, 0.20f, 0.05f, 0.10f),
            new(8,  0.45f, 0.25f, 0.10f, 0.10f),
            new(9,  0.35f, 0.25f, 0.15f, 0.10f),
            new(10, 0.25f, 0.25f, 0.20f, 0.10f),
        };
    }

    /// <summary>
    /// Central framework for managing equipment upgrades.
    /// </summary>
    public class UpgradeFramework
    {
        private readonly Dictionary<string, UpgradeState> _upgradeStates = new();
        private readonly Random _rng = new();

        // ---------------------------------------------------------------
        // REGISTRATION
        // ---------------------------------------------------------------

        /// <summary>
        /// Registers an upgrade state for an item.
        /// </summary>
        public UpgradeState RegisterItem(string itemId, int maxLevel, float baseMultiplier = 1.0f)
        {
            if (_upgradeStates.TryGetValue(itemId, out var existing))
                return existing;

            var state = new UpgradeState(itemId, maxLevel, baseMultiplier);
            _upgradeStates[itemId] = state;
            return state;
        }

        /// <summary>
        /// Gets the upgrade state for an item.
        /// </summary>
        public UpgradeState GetUpgradeState(string itemId)
        {
            return _upgradeStates.TryGetValue(itemId, out var state) ? state : null;
        }

        /// <summary>
        /// Removes an upgrade state (e.g., item destroyed).
        /// </summary>
        public void RemoveItem(string itemId)
        {
            _upgradeStates.Remove(itemId);
        }

        // ---------------------------------------------------------------
        // UPGRADE OPERATIONS
        // ---------------------------------------------------------------

        /// <summary>
        /// Attempts to upgrade an item. Returns the result.
        /// </summary>
        public UpgradeResult TryUpgrade(string itemId, float successBonus = 0f)
        {
            var state = GetUpgradeState(itemId);
            if (state == null)
                return UpgradeResult.InvalidItem;

            var result = state.TryUpgrade(successBonus, _rng);

            if (result == UpgradeResult.CriticalFailure)
            {
                Logger.Warning($"UpgradeFramework: Item '{itemId}' was destroyed by a critical upgrade failure!");
                RemoveItem(itemId);
            }

            return result;
        }

        /// <summary>
        /// Gets the current stat multiplier for an upgraded item.
        /// </summary>
        public float GetUpgradeMultiplier(string itemId)
        {
            var state = GetUpgradeState(itemId);
            return state?.CurrentMultiplier ?? 1.0f;
        }

        // ---------------------------------------------------------------
        // SAVE/LOAD
        // ---------------------------------------------------------------

        /// <summary>
        /// Gets a snapshot of all upgrade states for save.
        /// </summary>
        public Dictionary<string, UpgradeSaveData> GetSaveData()
        {
            var data = new Dictionary<string, UpgradeSaveData>();
            foreach (var kvp in _upgradeStates)
            {
                data[kvp.Key] = new UpgradeSaveData
                {
                    CurrentLevel = kvp.Value.CurrentLevel,
                    CurrentMultiplier = kvp.Value.CurrentMultiplier,
                    MaxLevel = kvp.Value.MaxLevel
                };
            }
            return data;
        }

        /// <summary>
        /// Restores upgrade state from save data.
        /// </summary>
        public void LoadSaveData(Dictionary<string, UpgradeSaveData> data)
        {
            _upgradeStates.Clear();
            if (data == null) return;

            foreach (var kvp in data)
            {
                var state = new UpgradeState(kvp.Key, kvp.Value.MaxLevel, kvp.Value.CurrentMultiplier);
                state.Initialize(kvp.Value.CurrentLevel, kvp.Value.CurrentMultiplier, kvp.Value.MaxLevel);
                _upgradeStates[kvp.Key] = state;
            }
        }
    }

    /// <summary>
    /// Save data for an item's upgrade state.
    /// </summary>
    public class UpgradeSaveData
    {
        public int CurrentLevel { get; set; }
        public float CurrentMultiplier { get; set; }
        public int MaxLevel { get; set; }
    }
}