using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter11
{
    public class EndgameZoneDefinition
    {
        public string ZoneId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int DangerLevel { get; set; } = 40;
        public List<string> EnvironmentalHazards { get; set; } = new();
        public List<string> EliteEnemyTypes { get; set; } = new();
        public List<string> LegendaryResourceIds { get; set; } = new();
    }

    /// <summary>
    /// Content definition for The Astral Divide, the premier endgame region initiating Act IV.
    /// Manages 7 high-level endgame sub-zones characterized by crystal formations, floating ruins, and ancient battlefields.
    /// </summary>
    public class EndgameRegionContent
    {
        private readonly Dictionary<string, EndgameZoneDefinition> _zones = new(StringComparer.OrdinalIgnoreCase);

        public string RegionId { get; } = "region_astral_divide";
        public string DisplayName { get; } = "The Astral Divide";
        public int TotalZones => _zones.Count;

        public EndgameRegionContent()
        {
            InitializeZones();
        }

        public void InitializeZones()
        {
            // 1. Crystal Wasteland
            RegisterZone(new EndgameZoneDefinition
            {
                ZoneId = "zone_crystal_wasteland",
                Name = "The Crystal Wasteland",
                Description = "Desolate plain studded with giant purple starlight crystals and razor-sharp crystal storms.",
                DangerLevel = 40,
                EnvironmentalHazards = new List<string> { "hazard_crystal_storm", "hazard_void_fracture" },
                EliteEnemyTypes = new List<string> { "enemy_crystal_behemoth", "enemy_void_stalker" },
                LegendaryResourceIds = new List<string> { "resource_astral_crystal_shard" }
            });

            // 2. Shattered Realm of Sol
            RegisterZone(new EndgameZoneDefinition
            {
                ZoneId = "zone_shattered_realm",
                Name = "Shattered Realm of Sol",
                Description = "Floating archipelagos of golden stone suspended over a glowing solar rift.",
                DangerLevel = 41,
                EnvironmentalHazards = new List<string> { "hazard_solar_flare_burst" },
                EliteEnemyTypes = new List<string> { "enemy_sun_warrior_remnant", "enemy_astral_sentinel" },
                LegendaryResourceIds = new List<string> { "resource_sol_ore" }
            });

            // 3. Floating Ruins of Caelum
            RegisterZone(new EndgameZoneDefinition
            {
                ZoneId = "zone_caelum_ruins",
                Name = "Floating Ruins of Caelum",
                Description = "Ancient floating temple spires connected by spectral light bridges.",
                DangerLevel = 42,
                EnvironmentalHazards = new List<string> { "hazard_gravity_reversal" },
                EliteEnemyTypes = new List<string> { "enemy_caelum_guardian", "enemy_arcane_spectre" },
                LegendaryResourceIds = new List<string> { "resource_caelum_dust" }
            });

            // 4. Celestial Canyon
            RegisterZone(new EndgameZoneDefinition
            {
                ZoneId = "zone_celestial_canyon",
                Name = "Celestial Rift Canyon",
                Description = "Chasm carved by cosmic lightning where intense astral winds buffet explorers.",
                DangerLevel = 42,
                EnvironmentalHazards = new List<string> { "hazard_astral_gale" },
                EliteEnemyTypes = new List<string> { "enemy_storm_drake", "enemy_void_weaver" },
                LegendaryResourceIds = new List<string> { "resource_thunder_stone" }
            });

            // 5. Ancient Astral Battlefield
            RegisterZone(new EndgameZoneDefinition
            {
                ZoneId = "zone_astral_battlefield",
                Name = "Ashen Astral Battlefield",
                Description = "Massive ancient war zone littered with giant broken swords and rusted siege constructs.",
                DangerLevel = 43,
                EnvironmentalHazards = new List<string> { "hazard_corrupted_ash" },
                EliteEnemyTypes = new List<string> { "enemy_war_dreadnought", "enemy_shadow_legion_champion" },
                LegendaryResourceIds = new List<string> { "resource_ancient_steel_ingot" }
            });

            // 6. Forgotten Sun Temple
            RegisterZone(new EndgameZoneDefinition
            {
                ZoneId = "zone_forgotten_sun_temple",
                Name = "Forgotten Sun Spire",
                Description = "Sunken golden spire harboring ancient sun rituals and high-tier crafting altars.",
                DangerLevel = 44,
                EnvironmentalHazards = new List<string> { "hazard_sun_searing_beam" },
                EliteEnemyTypes = new List<string> { "enemy_sun_high_priest_lich" },
                LegendaryResourceIds = new List<string> { "resource_pure_sun_core" }
            });

            // 7. Obsidian Citadel Threshold
            RegisterZone(new EndgameZoneDefinition
            {
                ZoneId = "zone_obsidian_threshold",
                Name = "Obsidian Citadel Gate",
                Description = "The heavily fortified perimeter wall protecting Arch-Sorcerer Malakor's ultimate stronghold.",
                DangerLevel = 45,
                EnvironmentalHazards = new List<string> { "hazard_void_cataclysm" },
                EliteEnemyTypes = new List<string> { "enemy_malakor_shadow_guard", "enemy_void_colossus" },
                LegendaryResourceIds = new List<string> { "resource_obsidian_essence" }
            });
        }

        public void RegisterZone(EndgameZoneDefinition zone)
        {
            if (zone != null && !string.IsNullOrEmpty(zone.ZoneId))
            {
                _zones[zone.ZoneId] = zone;
            }
        }

        public EndgameZoneDefinition? GetZone(string zoneId)
        {
            return _zones.TryGetValue(zoneId, out var z) ? z : null;
        }

        public List<EndgameZoneDefinition> GetAllZones()
        {
            return new List<EndgameZoneDefinition>(_zones.Values);
        }
    }
}
