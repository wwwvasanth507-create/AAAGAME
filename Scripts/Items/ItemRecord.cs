using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HeroOfEternia.Items
{
    /// <summary>
    /// Rarity types canonical to all items in Eternia.
    /// </summary>
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Mythic,
        Ancient,
        Divine
    }

    /// <summary>
    /// Slots where items can be equipped on the player character.
    /// </summary>
    public enum EquipmentSlotType
    {
        Helmet,
        Chest,
        Legs,
        Boots,
        Gloves,
        MainWeapon,
        OffHand,
        Ring1,
        Ring2,
        Necklace,
        Pet,
        Mount
    }

    /// <summary>
    /// Serialized container for attribute modifier properties applied to player when equipped.
    /// </summary>
    public class AttributeModifierData
    {
        public string AttributeType { get; set; } = ""; // e.g. "Health", "Speed", "Defense"
        public float Value { get; set; }
        public string ModifierType { get; set; } = "Flat"; // "Flat", "PercentAdd", "PercentMult"
    }

    /// <summary>
    /// Data-driven definition record for a single item type.
    /// Supports dynamic parameters, DLC variables, and stats modifiers.
    /// </summary>
    public class ItemRecord
    {
        public string UniqueId { get; set; } = "";
        public string InternalName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Description { get; set; } = "";
        
        // Extensible category strings allow DLC updates without code re-compiles
        public string Category { get; set; } = "";
        public string Subcategory { get; set; } = "";
        
        public int Tier { get; set; } = 1;
        public ItemRarity Rarity { get; set; } = ItemRarity.Common;
        public float Weight { get; set; } = 0.1f;
        public int StackSize { get; set; } = 99;
        
        public int SellValue { get; set; } = 0;
        public int BuyValue { get; set; } = 0;
        
        public string IconPath { get; set; } = "";
        public string ModelPath { get; set; } = "";
        public string MaterialPath { get; set; } = "";
        
        public string AnimRef { get; set; } = "";
        public string SoundRef { get; set; } = "";
        public string LocKey { get; set; } = "";
        public int Version { get; set; } = 1;

        // Attribute modifiers mapped to player stats upon equip
        public List<AttributeModifierData> StatModifiers { get; set; } = new();

        // Dynamic extension map for plugins, future DLCs, or special mechanics parameters
        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; } = new();
    }
}
