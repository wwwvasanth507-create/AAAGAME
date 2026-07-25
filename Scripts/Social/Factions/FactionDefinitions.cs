using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HeroOfEternia.Social.Factions
{
    /// <summary>
    /// Reusable faction type definitions for the social simulation framework.
    /// Supports all required types and future player-created factions.
    /// </summary>
    public enum FactionType
    {
        Kingdom,
        Empire,
        VillageCouncil,
        MerchantGuild,
        AdventurersGuild,
        MagesGuild,
        ReligiousOrder,
        Military,
        Bandits,
        Mercenaries,
        Pirates,
        Scholars,
        Nomads,
        MonsterTribe,
        SecretSociety,
        PlayerCreated
    }

    /// <summary>
    /// Alignment axis for faction moral/ethical standing.
    /// </summary>
    public enum FactionAlignment
    {
        LawfulGood,
        NeutralGood,
        ChaoticGood,
        LawfulNeutral,
        TrueNeutral,
        ChaoticNeutral,
        LawfulEvil,
        NeutralEvil,
        ChaoticEvil
    }

    /// <summary>
    /// Color theme for faction UI representation.
    /// </summary>
    public class FactionColorTheme
    {
        public string PrimaryColor { get; set; } = "#FFFFFF";
        public string SecondaryColor { get; set; } = "#000000";
        public string AccentColor { get; set; } = "#FFD700";
    }

    /// <summary>
    /// Complete faction definition with all fields required by the specification.
    /// Fully data-driven, JSON-serializable, and extensible for DLC.
    /// </summary>
    public class FactionDefinition
    {
        // Core identity
        public string FactionId { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Description { get; set; } = "";
        public FactionType Type { get; set; } = FactionType.Kingdom;
        
        // Territory & location
        public string Headquarters { get; set; } = "";
        public string Territory { get; set; } = "";
        public string LeadershipHook { get; set; } = "";
        
        // Alignment & goals
        public FactionAlignment Alignment { get; set; } = FactionAlignment.TrueNeutral;
        public List<string> PrimaryGoals { get; set; } = new();
        
        // Diplomatic relations (faction IDs)
        public List<string> FriendlyFactions { get; set; } = new();
        public List<string> HostileFactions { get; set; } = new();
        public List<string> NeutralFactions { get; set; } = new();
        
        // Visual identity
        public string UniformProfile { get; set; } = "";
        public string Symbol { get; set; } = "";
        public FactionColorTheme ColorTheme { get; set; } = new();
        public string MusicHook { get; set; } = "";
        
        // Localization
        public string LocalizationKey { get; set; } = "";
        
        // Future DLC extension fields
        public Dictionary<string, object> DlcFields { get; set; } = new();
        
        // Runtime state
        public int CurrentStrength { get; set; } = 100;
        public int MaxStrength { get; set; } = 100;
        public int MemberCount { get; set; } = 0;
        public float Treasury { get; set; } = 0f;
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Lightweight faction reference for save/load and quick lookups.
    /// </summary>
    public class FactionReference
    {
        public string FactionId { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public FactionType Type { get; set; }
        public FactionAlignment Alignment { get; set; }
        public bool IsActive { get; set; } = true;
    }
}