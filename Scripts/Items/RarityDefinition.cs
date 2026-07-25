using System;

namespace HeroOfEternia.Items
{
    /// <summary>
    /// Configuration record defining visual styling, drop heuristics, and effects
    /// for a single ItemRarity level. Loaded dynamically from json configurations.
    /// </summary>
    public class RarityDefinition
    {
        public ItemRarity Rarity { get; set; }
        
        // Visual Styling
        public string ColorHex { get; set; } = "#FFFFFF"; // RGB Hex representation
        public string BorderSpritePath { get; set; } = ""; // UI boundary graphic
        
        // Spawn/Drop heuristics
        public float DropWeight { get; set; } = 1.0f; // Drop allocation weight
        
        // Visual / Audio Hooks
        public string VisualEffectHook { get; set; } = ""; // e.g. "GlowEpic", "DivineSpark"
        public string AudioHook { get; set; } = ""; // sound trigger ID when item drops/looted
    }
}
