using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.NPC
{
    public enum NpcTypeEnum
    {
        Villager,
        Guard,
        Farmer,
        Merchant,
        Blacksmith,
        Wizard,
        Hunter,
        Scholar,
        Priest,
        King,
        Queen,
        Child,
        Traveler,
        Bandit,
        Companion
    }

    public enum GenderType
    {
        Male,
        Female,
        NonBinary
    }

    public enum EmotionState
    {
        Neutral,
        Happy,
        Sad,
        Angry,
        Fearful,
        Surprised,
        Disgusted,
        Celebratory
    }

    /// <summary>
    /// Complete NPC data record. Every field is data-driven and JSON-serializable.
    /// Supports unlimited future NPC types through the NpcTypeEnum extension pattern.
    /// </summary>
    public class NpcData
    {
        public string UniqueId { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public int Age { get; set; } = 25;
        public GenderType Gender { get; set; } = GenderType.Male;
        public string Species { get; set; } = "Human";
        public NpcTypeEnum Occupation { get; set; } = NpcTypeEnum.Villager;
        public string FactionId { get; set; } = "none";
        public string HomeLocationId { get; set; } = "";
        public string CurrentRegionId { get; set; } = "";

        // Profiles — localization/asset path references
        public string VoiceProfileKey { get; set; } = "voice_default";
        public string AnimationProfileKey { get; set; } = "anim_humanoid";
        public string AppearanceProfileKey { get; set; } = "appearance_default";
        public string DialogueReferenceKey { get; set; } = "";
        public string InventoryReferenceId { get; set; } = "";

        // Emotional & Health state
        public EmotionState CurrentEmotion { get; set; } = EmotionState.Neutral;
        public float CurrentHealth { get; set; } = 100f;
        public float MaxHealth { get; set; } = 100f;

        // Relationship keys list (e.g. "npc_001_npc_002")
        public List<string> RelationshipKeys { get; set; } = new();

        // Placeholder hooks for future systems
        public string CombatProfileKey { get; set; } = "";
        public List<string> QuestHookIds { get; set; } = new();

        // Global coordinates
        public float WorldX { get; set; } = 0f;
        public float WorldY { get; set; } = 0f;
        public float WorldZ { get; set; } = 0f;
    }

    /// <summary>
    /// Lightweight save-state snapshot for persisting active NPC runtime data.
    /// </summary>
    public class NpcSaveState
    {
        public string UniqueId { get; set; } = "";
        public float WorldX { get; set; }
        public float WorldY { get; set; }
        public float WorldZ { get; set; }
        public EmotionState Emotion { get; set; }
        public float CurrentHealth { get; set; }
        public string ActiveScheduleOverride { get; set; } = "";
    }
}
