using System;
using System.Collections.Generic;
using System.Linq;

namespace HeroOfEternia.Equipment.Quality
{
    /// <summary>
    /// Quality grades for equipment items.
    /// Influences stat calculations, visual appearance, and upgrade success rates.
    /// </summary>
    public enum ItemQuality
    {
        Broken = 0,
        Poor = 1,
        Normal = 2,
        Fine = 3,
        Superior = 4,
        Masterwork = 5,
        Legendary = 6,
        Divine = 7
    }

    /// <summary>
    /// Configuration for a quality grade, including stat multipliers and visual hooks.
    /// </summary>
    public class QualityDefinition
    {
        public ItemQuality Quality { get; }
        public string DisplayName { get; }
        public string ColorHex { get; }
        public float StatMultiplier { get; }
        public float UpgradeSuccessBonus { get; }
        public int MaxUpgradeLevel { get; }
        public string VfxHook { get; }

        public QualityDefinition(
            ItemQuality quality,
            string displayName,
            string colorHex,
            float statMultiplier,
            float upgradeSuccessBonus = 0f,
            int maxUpgradeLevel = 0,
            string vfxHook = "")
        {
            Quality = quality;
            DisplayName = displayName;
            ColorHex = colorHex;
            StatMultiplier = statMultiplier;
            UpgradeSuccessBonus = upgradeSuccessBonus;
            MaxUpgradeLevel = maxUpgradeLevel;
            VfxHook = vfxHook;
        }
    }

    /// <summary>
    /// Central system for managing item quality grades and their effects.
    /// </summary>
    public class ItemQualitySystem
    {
        private readonly Dictionary<ItemQuality, QualityDefinition> _qualityDefinitions = new();

        // ---------------------------------------------------------------
        // REGISTRATION
        // ---------------------------------------------------------------

        /// <summary>
        /// Registers a quality definition.
        /// </summary>
        public void RegisterQuality(QualityDefinition definition)
        {
            _qualityDefinitions[definition.Quality] = definition;
        }

        /// <summary>
        /// Gets the quality definition for a given quality grade.
        /// </summary>
        public QualityDefinition GetQualityDefinition(ItemQuality quality)
        {
            return _qualityDefinitions.TryGetValue(quality, out var def) ? def : _qualityDefinitions[ItemQuality.Normal];
        }

        /// <summary>
        /// Applies the quality stat multiplier to a base value.
        /// </summary>
        public float ApplyQualityMultiplier(ItemQuality quality, float baseValue)
        {
            var def = GetQualityDefinition(quality);
            return baseValue * def.StatMultiplier;
        }

        /// <summary>
        /// Gets the upgrade success bonus for an item's quality.
        /// </summary>
        public float GetUpgradeSuccessBonus(ItemQuality quality)
        {
            return GetQualityDefinition(quality).UpgradeSuccessBonus;
        }

        /// <summary>
        /// Gets the maximum upgrade level allowed for an item's quality.
        /// </summary>
        public int GetMaxUpgradeLevel(ItemQuality quality)
        {
            return GetQualityDefinition(quality).MaxUpgradeLevel;
        }

        // ---------------------------------------------------------------
        // QUALITY PROGRESSION
        // ---------------------------------------------------------------

        /// <summary>
        /// Upgrades an item's quality to the next tier. Returns the new quality, or the same if already max.
        /// </summary>
        public ItemQuality UpgradeQuality(ItemQuality current)
        {
            if (current >= ItemQuality.Divine)
                return current;
            return current + 1;
        }

        /// <summary>
        /// Downgrades an item's quality. Returns the new quality, or Broken if already at minimum.
        /// </summary>
        public ItemQuality DowngradeQuality(ItemQuality current)
        {
            if (current <= ItemQuality.Broken)
                return current;
            return current - 1;
        }

        /// <summary>
        /// Gets all registered quality definitions.
        /// </summary>
        public List<QualityDefinition> GetAllDefinitions()
        {
            return _qualityDefinitions.Values.ToList();
        }

        // ---------------------------------------------------------------
        // DEFAULT QUALITY DEFINITIONS
        // ---------------------------------------------------------------

        /// <summary>
        /// Creates and registers the default quality definitions.
        /// </summary>
        public static List<QualityDefinition> CreateDefaultDefinitions()
        {
            return new List<QualityDefinition>
            {
                new(ItemQuality.Broken, "Broken", "#8B0000", 0.25f, -0.20f, 0, "Vfx_Broken"),
                new(ItemQuality.Poor, "Poor", "#696969", 0.50f, -0.10f, 1, "Vfx_Poor"),
                new(ItemQuality.Normal, "Normal", "#9D9D9D", 1.00f, 0.00f, 3, "Vfx_Normal"),
                new(ItemQuality.Fine, "Fine", "#1EFF00", 1.25f, 0.05f, 5, "Vfx_Fine"),
                new(ItemQuality.Superior, "Superior", "#0070DD", 1.50f, 0.10f, 8, "Vfx_Superior"),
                new(ItemQuality.Masterwork, "Masterwork", "#A335EE", 2.00f, 0.15f, 10, "Vfx_Masterwork"),
                new(ItemQuality.Legendary, "Legendary", "#FF8000", 3.00f, 0.20f, 15, "Vfx_Legendary"),
                new(ItemQuality.Divine, "Divine", "#00FFFF", 5.00f, 0.30f, 20, "Vfx_Divine"),
            };
        }
    }
}