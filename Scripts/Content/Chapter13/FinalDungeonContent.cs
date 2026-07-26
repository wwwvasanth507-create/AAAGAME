using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter13
{
    public class CitadelSectorDefinition
    {
        public string SectorId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> PreFinalEncounterIds { get; set; } = new();
        public List<string> CheckpointIds { get; set; } = new();
        public bool HasShortcutGate { get; set; } = false;
        public bool IsPreFinalAntechamber { get; set; } = false;
    }

    /// <summary>
    /// Content definition for The Citadel of Obsidian Void, the massive 8-sector final dungeon in Act IV.
    /// Manages sectors, pre-final encounter placements, checkpoint gates, and shortcuts leading to Malakor's Antechamber.
    /// </summary>
    public class FinalDungeonContent
    {
        private readonly Dictionary<string, CitadelSectorDefinition> _sectors = new(StringComparer.OrdinalIgnoreCase);

        public string DungeonId { get; } = "dungeon_citadel_obsidian_void";
        public string DisplayName { get; } = "The Citadel of Obsidian Void";
        public int TotalSectors => _sectors.Count;

        public FinalDungeonContent()
        {
            InitializeSectors();
        }

        public void InitializeSectors()
        {
            // 1. Citadel Outer Breach
            RegisterSector(new CitadelSectorDefinition
            {
                SectorId = "sector_outer_breach",
                Name = "Citadel Outer Breach",
                Description = "Shattered black stone walls breached by allied siege engines, engulfed in void embers.",
                CheckpointIds = new List<string> { "chk_outer_breach" },
                HasShortcutGate = false
            });

            // 2. Fortified Gatehouse
            RegisterSector(new CitadelSectorDefinition
            {
                SectorId = "sector_fortified_gatehouse",
                Name = "Fortified Gatehouse",
                Description = "Heavy iron portcullis defense hub guarded by elite dreadnought constructs.",
                PreFinalEncounterIds = new List<string> { "encounter_iron_dreadnought" },
                CheckpointIds = new List<string> { "chk_gatehouse" },
                HasShortcutGate = true
            });

            // 3. Machine Core & Steam Chambers
            RegisterSector(new CitadelSectorDefinition
            {
                SectorId = "sector_machine_core",
                Name = "Machine Core & Steam Chambers",
                Description = "Subterranean mechanical foundry powering the Citadel's void shields.",
                CheckpointIds = new List<string> { "chk_machine_core" }
            });

            // 4. Void Crystal Corridors
            RegisterSector(new CitadelSectorDefinition
            {
                SectorId = "sector_crystal_corridors",
                Name = "Void Crystal Corridors",
                Description = "Narrow crystalline tunnels pulsing with dark starlight energy and void traps.",
                PreFinalEncounterIds = new List<string> { "encounter_void_matriarch" },
                CheckpointIds = new List<string> { "chk_crystal_corridors" }
            });

            // 5. Archive of the Sun-Kings
            RegisterSector(new CitadelSectorDefinition
            {
                SectorId = "sector_sun_king_archive",
                Name = "Archive of the Sun-Kings",
                Description = "Desecrated ancient library containing stolen celestial relics.",
                CheckpointIds = new List<string> { "chk_sun_king_archive" },
                HasShortcutGate = true
            });

            // 6. Subterranean Portal Vault
            RegisterSector(new CitadelSectorDefinition
            {
                SectorId = "sector_portal_vault",
                Name = "Subterranean Portal Vault",
                Description = "Chamber housing ancient astral teleporters connecting deep Citadel wings.",
                PreFinalEncounterIds = new List<string> { "encounter_archon_sunless_void" },
                CheckpointIds = new List<string> { "chk_portal_vault" }
            });

            // 7. Grand Promenade of Shadows
            RegisterSector(new CitadelSectorDefinition
            {
                SectorId = "sector_grand_promenade",
                Name = "Grand Promenade of Shadows",
                Description = "Colossal obsidian hallway lined with gargantuan statues of ancient void wardens.",
                PreFinalEncounterIds = new List<string> { "encounter_vaelis_remnant" },
                CheckpointIds = new List<string> { "chk_grand_promenade" },
                HasShortcutGate = true
            });

            // 8. Pre-Final Antechamber of Malakor
            RegisterSector(new CitadelSectorDefinition
            {
                SectorId = "sector_pre_final_antechamber",
                Name = "Pre-Final Antechamber of Malakor",
                Description = "The quiet sanctuary threshold immediately preceding Arch-Sorcerer Malakor's Throne Room.",
                CheckpointIds = new List<string> { "chk_antechamber_threshold" },
                IsPreFinalAntechamber = true
            });
        }

        public void RegisterSector(CitadelSectorDefinition sector)
        {
            if (sector != null && !string.IsNullOrEmpty(sector.SectorId))
            {
                _sectors[sector.SectorId] = sector;
            }
        }

        public CitadelSectorDefinition? GetSector(string sectorId)
        {
            return _sectors.TryGetValue(sectorId, out var s) ? s : null;
        }

        public List<CitadelSectorDefinition> GetAllSectors()
        {
            return new List<CitadelSectorDefinition>(_sectors.Values);
        }
    }
}
