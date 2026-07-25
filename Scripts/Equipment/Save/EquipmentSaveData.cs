using System;
using System.Collections.Generic;
using HeroOfEternia.Equipment.Durability;
using HeroOfEternia.Equipment.Enchantments;
using HeroOfEternia.Equipment.Quality;
using HeroOfEternia.Equipment.Upgrade;

namespace HeroOfEternia.Equipment.Save
{
    /// <summary>
    /// Represents the complete save state for all equipment-related systems.
    /// This is embedded in SaveProfile for Save V11.
    /// </summary>
    public class EquipmentSaveData
    {
        /// <summary>Version tracking for future migration.</summary>
        public int Version { get; set; } = 1;

        /// <summary>Durability states for all equipment items.</summary>
        public Dictionary<string, DurabilitySaveData> DurabilityData { get; set; } = new();

        /// <summary>Upgrade states for all equipment items.</summary>
        public Dictionary<string, UpgradeSaveData> UpgradeData { get; set; } = new();

        /// <summary>Enchantment instances applied to items.</summary>
        public Dictionary<string, List<EnchantmentSaveData>> EnchantmentData { get; set; } = new();

        /// <summary>Quality levels for items.</summary>
        public Dictionary<string, ItemQuality> QualityData { get; set; } = new();

        /// <summary>Modifier sets applied to items.</summary>
        public Dictionary<string, List<string>> ModifierData { get; set; } = new();

        /// <summary>Active gear set tracking.</summary>
        public Dictionary<string, int> ActiveSetCounts { get; set; } = new();
    }

    /// <summary>
    /// Save data for a single enchantment instance.
    /// </summary>
    public class EnchantmentSaveData
    {
        public string EnchantmentId { get; set; }
        public int Level { get; set; }
        public bool IsActive { get; set; }

        public EnchantmentSaveData() { }

        public EnchantmentSaveData(string enchantmentId, int level, bool isActive = true)
        {
            EnchantmentId = enchantmentId;
            Level = level;
            IsActive = isActive;
        }
    }

    /// <summary>
    /// Manages serialization and version migration for equipment save data.
    /// </summary>
    public static class EquipmentSaveManager
    {
        /// <summary>
        /// Creates the initial equipment save data with defaults.
        /// </summary>
        public static EquipmentSaveData CreateDefault()
        {
            return new EquipmentSaveData
            {
                Version = 1,
                DurabilityData = new Dictionary<string, DurabilitySaveData>(),
                UpgradeData = new Dictionary<string, UpgradeSaveData>(),
                EnchantmentData = new Dictionary<string, List<EnchantmentSaveData>>(),
                QualityData = new Dictionary<string, ItemQuality>(),
                ModifierData = new Dictionary<string, List<string>>(),
                ActiveSetCounts = new Dictionary<string, int>()
            };
        }

        /// <summary>
        /// Migrates equipment save data from a previous version to the current version.
        /// </summary>
        public static EquipmentSaveData Migrate(EquipmentSaveData data, int fromVersion)
        {
            if (data == null)
                return CreateDefault();

            switch (fromVersion)
            {
                case 0:
                    // Version 0 → 1: Initial equipment migration
                    data.Version = 1;
                    data.DurabilityData ??= new Dictionary<string, DurabilitySaveData>();
                    data.UpgradeData ??= new Dictionary<string, UpgradeSaveData>();
                    data.EnchantmentData ??= new Dictionary<string, List<EnchantmentSaveData>>();
                    data.QualityData ??= new Dictionary<string, ItemQuality>();
                    data.ModifierData ??= new Dictionary<string, List<string>>();
                    data.ActiveSetCounts ??= new Dictionary<string, int>();
                    break;
            }

            data.Version = 1;
            return data;
        }
    }
}