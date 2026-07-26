using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter10
{
    public class TempleChamberDefinition
    {
        public string ChamberId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> EnvironmentalLoreIds { get; set; } = new();
        public List<string> PuzzleMechanismIds { get; set; } = new();
        public List<string> GuardianTypes { get; set; } = new();
        public bool HasSanctuaryAltar { get; set; } = false;
    }

    /// <summary>
    /// Content definition for the Temple of the Eternal Sun, the ancient temple complex concluding Act III.
    /// Manages 7 handcrafted chambers centered on exploration, environmental storytelling, light/water puzzles, and lore.
    /// </summary>
    public class AncientTempleContent
    {
        private readonly Dictionary<string, TempleChamberDefinition> _chambers = new(StringComparer.OrdinalIgnoreCase);

        public string TempleId { get; } = "dungeon_temple_eternal_sun";
        public string DisplayName { get; } = "Temple of the Eternal Sun";
        public int TotalChambers => _chambers.Count;

        public AncientTempleContent()
        {
            InitializeChambers();
        }

        public void InitializeChambers()
        {
            // 1. Grand Entrance
            RegisterChamber(new TempleChamberDefinition
            {
                ChamberId = "chamber_temple_entrance",
                Name = "Portal of Astral Light",
                Description = "Towering white granite archway flanked by stone guardians and ancient sun carvings.",
                EnvironmentalLoreIds = new List<string> { "lore_sun_carving_mural" },
                PuzzleMechanismIds = new List<string> { "puzzle_entrance_sun_dial" },
                GuardianTypes = new List<string> { "enemy_ancient_sentinel" }
            });

            // 2. Collapsed Sun Court
            RegisterChamber(new TempleChamberDefinition
            {
                ChamberId = "chamber_sun_court",
                Name = "Collapsed Sun Court",
                Description = "Sunken courtyard open to the sky, featuring fallen marble pillars and sun rune mosaics.",
                EnvironmentalLoreIds = new List<string> { "lore_astral_war_inscription" },
                PuzzleMechanismIds = new List<string> { "puzzle_rune_mosaic" },
                GuardianTypes = new List<string> { "enemy_construct_guardian" }
            });

            // 3. Sacred Botanical Gardens
            RegisterChamber(new TempleChamberDefinition
            {
                ChamberId = "chamber_sacred_gardens",
                Name = "Sacred Botanical Sanctuary",
                Description = "Overgrown conservatory housing rare luminescent flora and ancient herbalist tablets.",
                EnvironmentalLoreIds = new List<string> { "lore_herbalist_tablet" },
                PuzzleMechanismIds = new List<string> { "puzzle_water_valve_east" },
                GuardianTypes = new List<string> { "enemy_arcane_spirit" }
            });

            // 4. Observatory of the Ancients
            RegisterChamber(new TempleChamberDefinition
            {
                ChamberId = "chamber_observatory",
                Name = "Observatory of the Ancients",
                Description = "High domed chamber containing a massive brass orrery mapping the celestial spheres.",
                EnvironmentalLoreIds = new List<string> { "lore_celestial_orrery_chart" },
                PuzzleMechanismIds = new List<string> { "puzzle_orrery_alignment" },
                GuardianTypes = new List<string> { "enemy_temple_protector" }
            });

            // 5. Water Prism Chamber
            RegisterChamber(new TempleChamberDefinition
            {
                ChamberId = "chamber_water_prism",
                Name = "Water & Prism Hall",
                Description = "Flooded marble hall featuring moveable crystal prisms that reflect sunlight beams into water gates.",
                EnvironmentalLoreIds = new List<string> { "lore_prism_crystal_scroll" },
                PuzzleMechanismIds = new List<string> { "puzzle_light_reflection_array" },
                GuardianTypes = new List<string> { "enemy_elite_warden" }
            });

            // 6. Subterranean Sanctum
            RegisterChamber(new TempleChamberDefinition
            {
                ChamberId = "chamber_subterranean_sanctum",
                Name = "Subterranean Archive Sanctum",
                Description = "Quiet archive beneath the temple floor holding golden Eternian codex plates.",
                EnvironmentalLoreIds = new List<string> { "lore_codex_malakor_truth" },
                PuzzleMechanismIds = new List<string> { "puzzle_weight_plate_sequence" },
                GuardianTypes = new List<string> { "enemy_ancient_sentinel" }
            });

            // 7. Core Astral Vault
            RegisterChamber(new TempleChamberDefinition
            {
                ChamberId = "chamber_astral_vault",
                Name = "Core Astral Vault",
                Description = "The innermost sanctuary where the ancient Astral Tear is suspended above a sun Altar.",
                EnvironmentalLoreIds = new List<string> { "lore_astral_tear_revelation" },
                PuzzleMechanismIds = new List<string> { "puzzle_astral_seal_core" },
                GuardianTypes = new List<string> { "enemy_boss_astral_guardian" },
                HasSanctuaryAltar = true
            });
        }

        public void RegisterChamber(TempleChamberDefinition chamber)
        {
            if (chamber != null && !string.IsNullOrEmpty(chamber.ChamberId))
            {
                _chambers[chamber.ChamberId] = chamber;
            }
        }

        public TempleChamberDefinition? GetChamber(string chamberId)
        {
            return _chambers.TryGetValue(chamberId, out var c) ? c : null;
        }

        public List<TempleChamberDefinition> GetAllChambers()
        {
            return new List<TempleChamberDefinition>(_chambers.Values);
        }
    }
}
