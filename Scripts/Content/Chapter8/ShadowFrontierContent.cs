using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter8
{
    public class ShadowFrontierZoneDefinition
    {
        public string ZoneId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> TraversalHazards { get; set; } = new();
        public List<string> NativeEnemies { get; set; } = new();
        public bool RequiresGrappleHook { get; set; } = false;
        public int DangerRatingLevel { get; set; } = 32;
    }

    /// <summary>
    /// Content definition for The Shadow Frontier, the premier high-level region of Act III.
    /// Manages 7 hazardous sub-zones, environmental hazards, and ruined fortress strongholds.
    /// </summary>
    public class ShadowFrontierContent
    {
        private readonly Dictionary<string, ShadowFrontierZoneDefinition> _zones = new(StringComparer.OrdinalIgnoreCase);

        public string RegionId { get; } = "region_shadow_frontier";
        public string DisplayName { get; } = "The Shadow Frontier";
        public int RecommendedLevel { get; } = 32;

        public ShadowFrontierContent()
        {
            InitializeZones();
        }

        public void InitializeZones()
        {
            // 1. Corrupted Whispering Woods
            RegisterZone(new ShadowFrontierZoneDefinition
            {
                ZoneId = "zone_corrupted_woods",
                Name = "Corrupted Whispering Woods",
                Description = "Dense ancient forest twisted by void miasma, filled with toxic spore clouds and shadow beasts.",
                TraversalHazards = new List<string> { "hazard_spore_fog", "hazard_corrupted_vines" },
                NativeEnemies = new List<string> { "enemy_shadow_stalker", "enemy_corrupted_beast" },
                RequiresGrappleHook = false,
                DangerRatingLevel = 31
            });

            // 2. Dread Ravine
            RegisterZone(new ShadowFrontierZoneDefinition
            {
                ZoneId = "zone_dread_ravine",
                Name = "Dread Ravine & Chasm",
                Description = "Massive cliffside chasm requiring advanced grapple hook and zipline traversal to cross.",
                TraversalHazards = new List<string> { "hazard_bottomless_pit", "hazard_falling_rocks" },
                NativeEnemies = new List<string> { "enemy_void_drake", "enemy_shadow_stalker" },
                RequiresGrappleHook = true,
                DangerRatingLevel = 32
            });

            // 3. Ruined Fort Ironwood
            RegisterZone(new ShadowFrontierZoneDefinition
            {
                ZoneId = "zone_ruined_fort_ironwood",
                Name = "Ruined Fort Ironwood",
                Description = "Desolated imperial bastion overrun by corrupted knights and shadow siege engines.",
                TraversalHazards = new List<string> { "hazard_crumbling_walls", "hazard_shadow_flame" },
                NativeEnemies = new List<string> { "enemy_corrupted_iron_knight", "enemy_shadow_brute" },
                RequiresGrappleHook = false,
                DangerRatingLevel = 33
            });

            // 4. Ashen Battlefield
            RegisterZone(new ShadowFrontierZoneDefinition
            {
                ZoneId = "zone_ashen_battlefield",
                Name = "Ashen Battlefield of the Ancients",
                Description = "Desolate plains littered with skeletal remains and ancient obelisk defense wardens.",
                TraversalHazards = new List<string> { "hazard_void_lightning", "hazard_ash_storm" },
                NativeEnemies = new List<string> { "enemy_ancient_obelisk_guardian", "enemy_void_spellweaver" },
                RequiresGrappleHook = false,
                DangerRatingLevel = 33
            });

            // 5. Gloomstone Caverns
            RegisterZone(new ShadowFrontierZoneDefinition
            {
                ZoneId = "zone_gloomstone_caverns",
                Name = "Gloomstone Subterranean Caverns",
                Description = "Subterranean crystal caves filled with glowing void crystals and subterranean shadow horrors.",
                TraversalHazards = new List<string> { "hazard_crystal_spikes", "hazard_darkness" },
                NativeEnemies = new List<string> { "enemy_subterranean_crawler", "enemy_shadow_behemoth" },
                RequiresGrappleHook = true,
                DangerRatingLevel = 34
            });

            // 6. Blighted Marshlands
            RegisterZone(new ShadowFrontierZoneDefinition
            {
                ZoneId = "zone_blighted_marshlands",
                Name = "Blighted Shadow Marshlands",
                Description = "Sunken swamplands with poisonous waters and shifting mud pits requiring rope bridge navigation.",
                TraversalHazards = new List<string> { "hazard_poison_swamp", "hazard_quicksand" },
                NativeEnemies = new List<string> { "enemy_swamp_horror", "enemy_shadow_stalker" },
                RequiresGrappleHook = false,
                DangerRatingLevel = 34
            });

            // 7. Obsidian Crag Sanctuary
            RegisterZone(new ShadowFrontierZoneDefinition
            {
                ZoneId = "zone_obsidian_crag_sanctuary",
                Name = "Obsidian Crag Secret Sanctuary",
                Description = "Ancient high mountain sanctuary perched on obsidian pillars, housing lost Eternian secrets.",
                TraversalHazards = new List<string> { "hazard_high_winds", "hazard_obsidian_shards" },
                NativeEnemies = new List<string> { "enemy_obsidian_golem", "enemy_shadow_champion" },
                RequiresGrappleHook = true,
                DangerRatingLevel = 35
            });
        }

        public void RegisterZone(ShadowFrontierZoneDefinition zone)
        {
            if (zone != null && !string.IsNullOrEmpty(zone.ZoneId))
            {
                _zones[zone.ZoneId] = zone;
            }
        }

        public ShadowFrontierZoneDefinition? GetZone(string zoneId)
        {
            return _zones.TryGetValue(zoneId, out var z) ? z : null;
        }

        public List<ShadowFrontierZoneDefinition> GetAllZones()
        {
            return new List<ShadowFrontierZoneDefinition>(_zones.Values);
        }
    }
}
