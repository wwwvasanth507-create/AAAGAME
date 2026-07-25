using System;
using System.Collections.Generic;

namespace HeroOfEternia.Story.Campaign
{
    public class RegionDefinition
    {
        public string RegionId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Climate { get; set; } = "Temperate";
        public List<string> BiomeMix { get; set; } = new();
        public List<string> MajorSettlements { get; set; } = new();
        public List<string> ImportantLandmarks { get; set; } = new();
        public List<string> NativeCreatures { get; set; } = new();
        public List<string> Resources { get; set; } = new();
        public string DominantFactionId { get; set; } = string.Empty;
        public string ArchitecturalStyle { get; set; } = "EternianWood";
        public string MusicalTheme { get; set; } = "music_plains";
        public string VisualTheme { get; set; } = "theme_plains";
        public int MinLevel { get; set; } = 1;
        public int MaxLevel { get; set; } = 10;
        public string RegionalLore { get; set; } = string.Empty;
        public string DlcModuleId { get; set; } = string.Empty;
    }

    public class RegionDatabase
    {
        private readonly Dictionary<string, RegionDefinition> _regions = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterRegion(RegionDefinition region)
        {
            if (region != null && !string.IsNullOrEmpty(region.RegionId))
            {
                _regions[region.RegionId] = region;
            }
        }

        public RegionDefinition? GetRegion(string regionId)
        {
            return _regions.TryGetValue(regionId, out var r) ? r : null;
        }

        public IReadOnlyCollection<RegionDefinition> GetAllRegions() => _regions.Values;

        public void RegisterDefaultRegions()
        {
            RegisterRegion(new RegionDefinition
            {
                RegionId = "region_starting_kingdom",
                DisplayName = "Valenoria (Starting Kingdom)",
                Climate = "Temperate Plains",
                BiomeMix = new List<string> { "Plains", "Hills", "Forest" },
                MajorSettlements = new List<string> { "Oakvale", "Valenhold" },
                DominantFactionId = "faction_valen_crown",
                MinLevel = 1,
                MaxLevel = 10,
                RegionalLore = "The peaceful cradle of Eternia, protected by ancient guardians."
            });

            RegisterRegion(new RegionDefinition
            {
                RegionId = "region_forest",
                DisplayName = "Sylvanwood Wilds",
                Climate = "Dense Humid Forest",
                BiomeMix = new List<string> { "Forest", "DeepJungle" },
                MajorSettlements = new List<string> { "Elderwood Grove" },
                DominantFactionId = "faction_sylvan_guardians",
                MinLevel = 8,
                MaxLevel = 18,
                RegionalLore = "Ancient primeval woods home to nature spirits and elusive hunters."
            });

            RegisterRegion(new RegionDefinition
            {
                RegionId = "region_desert",
                DisplayName = "Sunfire Wastes",
                Climate = "Arid Desert",
                BiomeMix = new List<string> { "Desert", "Canyon" },
                MajorSettlements = new List<string> { "Sunspire Oasis" },
                DominantFactionId = "faction_sunfire_nomads",
                MinLevel = 15,
                MaxLevel = 25,
                RegionalLore = "Scorching dunes hiding forgotten subterranean tombs."
            });

            RegisterRegion(new RegionDefinition
            {
                RegionId = "region_frozen_north",
                DisplayName = "Frostpeak Mountains",
                Climate = "Sub-Zero Arctic",
                BiomeMix = new List<string> { "Tundra", "Glacier" },
                MajorSettlements = new List<string> { "Frostfang Hold" },
                DominantFactionId = "faction_frost_clans",
                MinLevel = 22,
                MaxLevel = 32,
                RegionalLore = "Glacial peaks where frozen titans slumber."
            });

            RegisterRegion(new RegionDefinition
            {
                RegionId = "region_swamp",
                DisplayName = "Mirkwood Swamps",
                Climate = "Stagnant Marsh",
                BiomeMix = new List<string> { "Swamp", "Bog" },
                MajorSettlements = new List<string> { "Mudport" },
                DominantFactionId = "faction_bog_coven",
                MinLevel = 12,
                MaxLevel = 20,
                RegionalLore = "Treacherous wetlands shrouded in poisonous mist."
            });

            RegisterRegion(new RegionDefinition
            {
                RegionId = "region_highlands",
                DisplayName = "Stormrage Highlands",
                Climate = "Windy Steppe",
                BiomeMix = new List<string> { "Highlands", "Cliffs" },
                MajorSettlements = new List<string> { "Skyreach Citadel" },
                DominantFactionId = "faction_highland_riders",
                MinLevel = 25,
                MaxLevel = 35,
                RegionalLore = "High elevation plateaus battered by thunderous storms."
            });

            RegisterRegion(new RegionDefinition
            {
                RegionId = "region_volcanic",
                DisplayName = "Ashen Peaks",
                Climate = "Volcanic Geothermal",
                BiomeMix = new List<string> { "Volcanic", "LavaFields" },
                MajorSettlements = new List<string> { "Ironforge Bastion" },
                DominantFactionId = "faction_ashes_order",
                MinLevel = 30,
                MaxLevel = 40,
                RegionalLore = "Active volcanic ridges forged in primordial fires."
            });

            RegisterRegion(new RegionDefinition
            {
                RegionId = "region_ancient_ruins",
                DisplayName = "Eternian Empire Ruins",
                Climate = "Desolate Arid",
                BiomeMix = new List<string> { "Ruins", "Badlands" },
                MajorSettlements = new List<string> { "Ruined Imperial City" },
                DominantFactionId = "faction_ancient_remnants",
                MinLevel = 35,
                MaxLevel = 45,
                RegionalLore = "The fallen capital of the First Age of Eternia."
            });

            RegisterRegion(new RegionDefinition
            {
                RegionId = "region_magical_islands",
                DisplayName = "Arcane Archipelago",
                Climate = "Tropical Arcane",
                BiomeMix = new List<string> { "Island", "ArcaneReef" },
                MajorSettlements = new List<string> { "Starfall Haven" },
                DominantFactionId = "faction_arcane_council",
                MinLevel = 40,
                MaxLevel = 50,
                RegionalLore = "Floating island chains suspended by leyline magic."
            });

            RegisterRegion(new RegionDefinition
            {
                RegionId = "region_dark_wastes",
                DisplayName = "Abyssal Wastes",
                Climate = "Shadowed Void",
                BiomeMix = new List<string> { "CorruptedLand", "VoidPlains" },
                MajorSettlements = new List<string> { "Outpost of Hope" },
                DominantFactionId = "faction_shadow_cult",
                MinLevel = 45,
                MaxLevel = 55,
                RegionalLore = "Corrupted wasteland bordering the Void Realm."
            });

            RegisterRegion(new RegionDefinition
            {
                RegionId = "region_sky_realm",
                DisplayName = "Aetheria (Sky Realm - Expansion)",
                Climate = "Celestial Ether",
                BiomeMix = new List<string> { "CloudPlains", "AetherSpires" },
                DominantFactionId = "faction_celestials",
                MinLevel = 50,
                MaxLevel = 60,
                RegionalLore = "Expansion realm soaring above the clouds.",
                DlcModuleId = "dlc_sky_realm"
            });

            RegisterRegion(new RegionDefinition
            {
                RegionId = "region_underworld",
                DisplayName = "Netherdeep (Underworld - Expansion)",
                Climate = "Subterranean Cavern",
                BiomeMix = new List<string> { "Underdark", "CrystalCaverns" },
                DominantFactionId = "faction_nether_lords",
                MinLevel = 55,
                MaxLevel = 65,
                RegionalLore = "Deep subterranean realm beneath Eternia's crust.",
                DlcModuleId = "dlc_underworld"
            });
        }
    }
}
