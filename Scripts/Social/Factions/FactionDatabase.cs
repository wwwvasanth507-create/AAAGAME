using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;
using Newtonsoft.Json;

namespace HeroOfEternia.Social.Factions
{
    /// <summary>
    /// Central faction registry. Loads from JSON, supports runtime registration,
    /// indexed lookups, and future DLC extension. Thread-safe read operations.
    /// </summary>
    public class FactionDatabase
    {
        public const string ServiceKey = "FactionDatabase";

        private readonly Dictionary<string, FactionDefinition> _factions = new();
        private readonly Dictionary<FactionType, List<string>> _byType = new();
        private readonly Dictionary<string, List<string>> _byRegion = new();
        private readonly object _lock = new();
        private bool _initialized = false;

        /// <summary>Fired when a faction is registered or modified.</summary>
        public event Action<FactionDefinition>? OnFactionChanged;

        // ─────────────────────── Initialization ───────────────────────

        /// <summary>
        /// Loads faction definitions from a JSON file path.
        /// </summary>
        public void Initialize(string jsonPath = "res://Settings/faction_database.json")
        {
            if (_initialized) return;

            try
            {
                var file = Godot.FileAccess.Open(jsonPath, Godot.FileAccess.ModeFlags.Read);
                if (file == null)
                {
                    Logger.Warn($"FactionDatabase: Could not open {jsonPath} — loading defaults.");
                    LoadDefaults();
                    _initialized = true;
                    return;
                }

                string json = file.GetAsText();
                file.Close();

                var definitions = JsonConvert.DeserializeObject<List<FactionDefinition>>(json);
                if (definitions == null || definitions.Count == 0)
                {
                    Logger.Warn("FactionDatabase: Empty or invalid JSON — loading defaults.");
                    LoadDefaults();
                }
                else
                {
                    lock (_lock)
                    {
                        foreach (var fac in definitions)
                        {
                            RegisterInternal(fac);
                        }
                    }
                }

                _initialized = true;
                Logger.Info($"FactionDatabase: Loaded {_factions.Count} factions.");
            }
            catch (Exception ex)
            {
                Logger.Error($"FactionDatabase: Failed to load — {ex.Message}. Loading defaults.");
                LoadDefaults();
                _initialized = true;
            }
        }

        /// <summary>
        /// Registers a faction at runtime. Allows adding new factions without code changes.
        /// </summary>
        public void RegisterFaction(FactionDefinition faction)
        {
            if (faction == null || string.IsNullOrEmpty(faction.FactionId))
            {
                Logger.Warn("FactionDatabase: Attempted to register null/empty faction.");
                return;
            }

            lock (_lock)
            {
                RegisterInternal(faction);
            }

            OnFactionChanged?.Invoke(faction);
            Logger.Info($"FactionDatabase: Registered faction '{faction.FactionId}' ({faction.DisplayName})");
        }

        // ─────────────────────── Lookups ───────────────────────

        public FactionDefinition? GetFaction(string factionId)
        {
            lock (_lock)
            {
                return _factions.TryGetValue(factionId, out var f) ? f : null;
            }
        }

        public List<FactionDefinition> GetFactionsByType(FactionType type)
        {
            lock (_lock)
            {
                if (!_byType.TryGetValue(type, out var ids))
                    return new List<FactionDefinition>();
                return ids.Select(id => _factions[id]).ToList();
            }
        }

        public List<FactionDefinition> GetFactionsByRegion(string regionId)
        {
            lock (_lock)
            {
                if (!_byRegion.TryGetValue(regionId, out var ids))
                    return new List<FactionDefinition>();
                return ids.Select(id => _factions[id]).ToList();
            }
        }

        public List<FactionDefinition> GetAllFactions()
        {
            lock (_lock)
            {
                return _factions.Values.ToList();
            }
        }

        public List<FactionReference> GetAllFactionReferences()
        {
            lock (_lock)
            {
                return _factions.Values.Select(f => new FactionReference
                {
                    FactionId = f.FactionId,
                    DisplayName = f.DisplayName,
                    Type = f.Type,
                    Alignment = f.Alignment,
                    IsActive = f.IsActive
                }).ToList();
            }
        }

        public bool FactionExists(string factionId)
        {
            lock (_lock) { return _factions.ContainsKey(factionId); }
        }

        public int Count
        {
            get { lock (_lock) { return _factions.Count; } }
        }

        // ─────────────────────── Runtime Modification ───────────────────────

        public void UpdateFactionStrength(string factionId, int newStrength)
        {
            lock (_lock)
            {
                if (_factions.TryGetValue(factionId, out var f))
                {
                    f.CurrentStrength = Math.Clamp(newStrength, 0, f.MaxStrength);
                    OnFactionChanged?.Invoke(f);
                }
            }
        }

        public void SetFactionActive(string factionId, bool active)
        {
            lock (_lock)
            {
                if (_factions.TryGetValue(factionId, out var f))
                {
                    f.IsActive = active;
                    OnFactionChanged?.Invoke(f);
                }
            }
        }

        public void AddFriendlyFaction(string factionId, string otherFactionId)
        {
            lock (_lock)
            {
                if (_factions.TryGetValue(factionId, out var f) && !f.FriendlyFactions.Contains(otherFactionId))
                {
                    f.FriendlyFactions.Add(otherFactionId);
                    OnFactionChanged?.Invoke(f);
                }
            }
        }

        public void AddHostileFaction(string factionId, string otherFactionId)
        {
            lock (_lock)
            {
                if (_factions.TryGetValue(factionId, out var f) && !f.HostileFactions.Contains(otherFactionId))
                {
                    f.HostileFactions.Add(otherFactionId);
                    OnFactionChanged?.Invoke(f);
                }
            }
        }

        public void RemoveDiplomaticRelation(string factionId, string otherFactionId)
        {
            lock (_lock)
            {
                if (_factions.TryGetValue(factionId, out var f))
                {
                    f.FriendlyFactions.Remove(otherFactionId);
                    f.HostileFactions.Remove(otherFactionId);
                    f.NeutralFactions.Remove(otherFactionId);
                    OnFactionChanged?.Invoke(f);
                }
            }
        }

        // ─────────────────────── Private Helpers ───────────────────────

        private void RegisterInternal(FactionDefinition faction)
        {
            _factions[faction.FactionId] = faction;

            if (!_byType.ContainsKey(faction.Type))
                _byType[faction.Type] = new List<string>();
            if (!_byType[faction.Type].Contains(faction.FactionId))
                _byType[faction.Type].Add(faction.FactionId);

            if (!string.IsNullOrEmpty(faction.Territory))
            {
                var regions = faction.Territory.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var region in regions)
                {
                    string trimmed = region.Trim();
                    if (!_byRegion.ContainsKey(trimmed))
                        _byRegion[trimmed] = new List<string>();
                    if (!_byRegion[trimmed].Contains(faction.FactionId))
                        _byRegion[trimmed].Add(faction.FactionId);
                }
            }
        }

        private void LoadDefaults()
        {
            var defaults = new List<FactionDefinition>
            {
                new()
                {
                    FactionId = "kingdom_eternia",
                    DisplayName = "Kingdom of Eternia",
                    Description = "The ancient and noble kingdom that has ruled these lands for centuries. Seeks to maintain order and prosperity.",
                    Type = FactionType.Kingdom,
                    Headquarters = "eternia_capital",
                    Territory = "eternia_heartlands, eternia_coast",
                    LeadershipHook = "King Aldric the Steadfast",
                    Alignment = FactionAlignment.LawfulGood,
                    PrimaryGoals = { "MaintainOrder", "ProtectCitizens", "ExpandInfluence" },
                    FriendlyFactions = { "adventurers_guild", "merchant_guild_eternia" },
                    HostileFactions = { "bandits_blackfang", "mercenaries_iron_company" },
                    ColorTheme = new FactionColorTheme { PrimaryColor = "#1B3A5C", SecondaryColor = "#C9A84C", AccentColor = "#FFFFFF" },
                    Symbol = "Crowned_Sword",
                    MusicHook = "royal_eternia_theme",
                    LocalizationKey = "faction_kingdom_eternia",
                    MemberCount = 5000,
                    Treasury = 100000f
                },
                new()
                {
                    FactionId = "adventurers_guild",
                    DisplayName = "Adventurers Guild",
                    Description = "A fellowship of explorers, monster slayers, and treasure hunters.中立偏善.",
                    Type = FactionType.AdventurersGuild,
                    Headquarters = "guild_hall_eternia",
                    Territory = "eternia_heartlands",
                    LeadershipHook = "Guildmaster Serra Windwalker",
                    Alignment = FactionAlignment.NeutralGood,
                    PrimaryGoals = { "ExploreDungeons", "SlayMonsters", "CollectArtifacts" },
                    FriendlyFactions = { "kingdom_eternia", "merchant_guild_eternia" },
                    HostileFactions = { "bandits_blackfang", "monster_tribe_fang" },
                    ColorTheme = new FactionColorTheme { PrimaryColor = "#8B4513", SecondaryColor = "#DAA520", AccentColor = "#FF4500" },
                    Symbol = "Crossed_Swords_Star",
                    MusicHook = "adventurers_guild_theme",
                    LocalizationKey = "faction_adventurers_guild",
                    MemberCount = 350,
                    Treasury = 15000f
                },
                new()
                {
                    FactionId = "merchant_guild_eternia",
                    DisplayName = "Eternian Merchant Guild",
                    Description = "Powerful trade syndicate controlling commerce across the kingdom. Prioritizes profit and economic stability.",
                    Type = FactionType.MerchantGuild,
                    Headquarters = "trade_hall_eternia",
                    Territory = "eternia_heartlands, eternia_coast, trade_routes",
                    LeadershipHook = "High Trader Veridian Goldvein",
                    Alignment = FactionAlignment.LawfulNeutral,
                    PrimaryGoals = { "MaximizeProfit", "ControlTrade", "ExpandRoutes" },
                    FriendlyFactions = { "kingdom_eternia", "adventurers_guild" },
                    NeutralFactions = { "mages_guild_arcane" },
                    ColorTheme = new FactionColorTheme { PrimaryColor = "#2F4F2F", SecondaryColor = "#FFD700", AccentColor = "#8B0000" },
                    Symbol = "Golden_Scale",
                    MusicHook = "trade_guild_theme",
                    LocalizationKey = "faction_merchant_guild",
                    MemberCount = 1200,
                    Treasury = 500000f
                },
                new()
                {
                    FactionId = "mages_guild_arcane",
                    DisplayName = "Arcane Mages Guild",
                    Description = "Secretive order of wizards and scholars dedicated to the study and preservation of magical knowledge.",
                    Type = FactionType.MagesGuild,
                    Headquarters = "arcane_spire",
                    Territory = "eternia_heartlands, magical_nexus",
                    LeadershipHook = "Archmage Elara Spellweaver",
                    Alignment = FactionAlignment.Neutral,
                    PrimaryGoals = { "PreserveKnowledge", "StudyMagic", "RegulateArcane" },
                    FriendlyFactions = { "scholars_archive" },
                    NeutralFactions = { "kingdom_eternia", "adventurers_guild", "merchant_guild_eternia" },
                    ColorTheme = new FactionColorTheme { PrimaryColor = "#4A0080", SecondaryColor = "#C0C0C0", AccentColor = "#00FFFF" },
                    Symbol = "Arcane_Eye",
                    MusicHook = "mages_guild_theme",
                    LocalizationKey = "faction_mages_guild",
                    MemberCount = 180,
                    Treasury = 80000f
                },
                new()
                {
                    FactionId = "bandits_blackfang",
                    DisplayName = "Blackfang Bandits",
                    Description = "Notorious bandit clan operating from hidden camps in the wilderness. Known for ambushing caravans and raiding villages.",
                    Type = FactionType.Bandits,
                    Headquarters = "blackfang_hideout",
                    Territory = "dark_forest, mountain_pass",
                    LeadershipHook = "Baron Roderick Blackfang",
                    Alignment = FactionAlignment.ChaoticEvil,
                    PrimaryGoals = { "LootCaravans", "ExtortVillages", "ExpandTerritory" },
                    FriendlyFactions = { "mercenaries_iron_company" },
                    HostileFactions = { "kingdom_eternia", "adventurers_guild", "merchant_guild_eternia" },
                    ColorTheme = new FactionColorTheme { PrimaryColor = "#1A1A1A", SecondaryColor = "#8B0000", AccentColor = "#FFD700" },
                    Symbol = "Cracked_Skull",
                    MusicHook = "bandit_theme",
                    LocalizationKey = "faction_bandits_blackfang",
                    MemberCount = 150,
                    Treasury = 25000f
                },
                new()
                {
                    FactionId = "mercenaries_iron_company",
                    DisplayName = "Iron Company Mercenaries",
                    Description = "Professional soldiers for hire. Known for discipline and efficiency, but their loyalty depends entirely on payment.",
                    Type = FactionType.Mercenaries,
                    Headquarters = "iron_compound",
                    Territory = "mercenary_plains",
                    LeadershipHook = "Captain Marcus Ironheart",
                    Alignment = FactionAlignment.TrueNeutral,
                    PrimaryGoals = { "SecureContracts", "BuildReputation", "AccumulateWealth" },
                    FriendlyFactions = { "bandits_blackfang" },
                    NeutralFactions = { "kingdom_eternia", "merchant_guild_eternia" },
                    ColorTheme = new FactionColorTheme { PrimaryColor = "#696969", SecondaryColor = "#FF4500", AccentColor = "#000000" },
                    Symbol = "Iron_Shield",
                    MusicHook = "mercenary_theme",
                    LocalizationKey = "faction_mercenaries_iron",
                    MemberCount = 400,
                    Treasury = 60000f
                },
                new()
                {
                    FactionId = "religious_order_light",
                    DisplayName = "Order of the Eternal Light",
                    Description = "Devoted followers of the divine Light, providing healing, guidance, and moral authority throughout the region.",
                    Type = FactionType.ReligiousOrder,
                    Headquarters = "grand_cathedral",
                    Territory = "eternia_heartlands, holy_sites",
                    LeadershipHook = "High Priestess Cassandra Lightbringer",
                    Alignment = FactionAlignment.LawfulGood,
                    PrimaryGoals = { "SpreadFaith", "HealSick", "ProtectHoly" },
                    FriendlyFactions = { "kingdom_eternia", "adventurers_guild" },
                    HostileFactions = { "bandits_blackfang", "monster_tribe_fang" },
                    ColorTheme = new FactionColorTheme { PrimaryColor = "#FFF8DC", SecondaryColor = "#FFD700", AccentColor = "#FFFFFF" },
                    Symbol = "Radiant_Sun",
                    MusicHook = "light_order_theme",
                    LocalizationKey = "faction_religious_light",
                    MemberCount = 600,
                    Treasury = 45000f
                },
                new()
                {
                    FactionId = "scholars_archive",
                    DisplayName = "Archive of Scholars",
                    Description = "A conclave of historians, philosophers, and researchers dedicated to documenting the world's history and discoveries.",
                    Type = FactionType.Scholars,
                    Headquarters = "great_library",
                    Territory = "eternia_heartlands",
                    LeadershipHook = "Chancellor Thaddeus Quill",
                    Alignment = FactionAlignment.TrueNeutral,
                    PrimaryGoals = { "DocumentHistory", "ResearchArtifacts", "EducatePublic" },
                    FriendlyFactions = { "mages_guild_arcane", "kingdom_eternia" },
                    ColorTheme = new FactionColorTheme { PrimaryColor = "#F5F5DC", SecondaryColor = "#8B4513", AccentColor = "#00008B" },
                    Symbol = "Open_Tome",
                    MusicHook = "scholars_theme",
                    LocalizationKey = "faction_scholars_archive",
                    MemberCount = 120,
                    Treasury = 20000f
                },
                new()
                {
                    FactionId = "military_eternian_guard",
                    DisplayName = "Eternian Royal Guard",
                    Description = "The official military force of the Kingdom of Eternia. Protects the realm from external and internal threats.",
                    Type = FactionType.Military,
                    Headquarters = "royal_barracks",
                    Territory = "eternia_heartlands, border_forts",
                    LeadershipHook = "General Valerius Ironwall",
                    Alignment = FactionAlignment.LawfulGood,
                    PrimaryGoals = { "DefendRealm", "PatrolBorders", "SupportKing" },
                    FriendlyFactions = { "kingdom_eternia", "religious_order_light" },
                    HostileFactions = { "bandits_blackfang", "mercenaries_iron_company", "monster_tribe_fang" },
                    ColorTheme = new FactionColorTheme { PrimaryColor = "#000080", SecondaryColor = "#C0C0C0", AccentColor = "#FF0000" },
                    Symbol = "Royal_Crest",
                    MusicHook = "royal_guard_theme",
                    LocalizationKey = "faction_military_guard",
                    MemberCount = 2000,
                    Treasury = 150000f
                }
            };

            lock (_lock)
            {
                foreach (var fac in defaults)
                {
                    RegisterInternal(fac);
                }
            }

            Logger.Info($"FactionDatabase: Loaded {defaults.Count} default factions.");
        }

        // ─────────────────────── Export for Save ───────────────────────

        public List<FactionDefinition> ExportAllDefinitions()
        {
            lock (_lock)
            {
                return _factions.Values.Select(f => new FactionDefinition
                {
                    FactionId = f.FactionId,
                    DisplayName = f.DisplayName,
                    Description = f.Description,
                    Type = f.Type,
                    Headquarters = f.Headquarters,
                    Territory = f.Territory,
                    LeadershipHook = f.LeadershipHook,
                    Alignment = f.Alignment,
                    PrimaryGoals = new List<string>(f.PrimaryGoals),
                    FriendlyFactions = new List<string>(f.FriendlyFactions),
                    HostileFactions = new List<string>(f.HostileFactions),
                    NeutralFactions = new List<string>(f.NeutralFactions),
                    UniformProfile = f.UniformProfile,
                    Symbol = f.Symbol,
                    ColorTheme = new FactionColorTheme
                    {
                        PrimaryColor = f.ColorTheme.PrimaryColor,
                        SecondaryColor = f.ColorTheme.SecondaryColor,
                        AccentColor = f.ColorTheme.AccentColor
                    },
                    MusicHook = f.MusicHook,
                    LocalizationKey = f.LocalizationKey,
                    CurrentStrength = f.CurrentStrength,
                    MaxStrength = f.MaxStrength,
                    MemberCount = f.MemberCount,
                    Treasury = f.Treasury,
                    IsActive = f.IsActive
                }).ToList();
            }
        }
    }
}