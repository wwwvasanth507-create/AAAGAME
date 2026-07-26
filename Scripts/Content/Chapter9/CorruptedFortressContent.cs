using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter9
{
    public class FortressSectorDefinition
    {
        public string SectorId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> SecurityMechanics { get; set; } = new();
        public List<string> StationedUnits { get; set; } = new();
        public bool HasAlarmBell { get; set; } = false;
        public int RecommendedLevel { get; set; } = 35;
    }

    /// <summary>
    /// Content definition for the Fortress of Obsidian Shadows, the primary antagonist military stronghold in Act III.
    /// Manages 7 sectors including outer battlements, armory, prison catacombs, command hall, and commander's arena.
    /// </summary>
    public class CorruptedFortressContent
    {
        private readonly Dictionary<string, FortressSectorDefinition> _sectors = new(StringComparer.OrdinalIgnoreCase);

        public string FortressId { get; } = "dungeon_fortress_obsidian_shadows";
        public string DisplayName { get; } = "Fortress of Obsidian Shadows";
        public int TotalSectors => _sectors.Count;

        public CorruptedFortressContent()
        {
            InitializeSectors();
        }

        public void InitializeSectors()
        {
            // 1. Outer Battlements
            RegisterSector(new FortressSectorDefinition
            {
                SectorId = "sector_fortress_battlements",
                Name = "Outer Battlements & Watchtowers",
                Description = "High obsidian stone ramparts guarded by archers, ballista stations, and alarm gongs.",
                SecurityMechanics = new List<string> { "alarm_gong_primary", "searchlight_towers" },
                StationedUnits = new List<string> { "enemy_shadow_scout", "enemy_legion_archer" },
                HasAlarmBell = true,
                RecommendedLevel = 34
            });

            // 2. Collapsed Courtyard
            RegisterSector(new FortressSectorDefinition
            {
                SectorId = "sector_fortress_courtyard",
                Name = "Collapsed Courtyard & Parade Grounds",
                Description = "Massive open courtyard filled with patrol squads, heavy brutes, and barricade checkpoints.",
                SecurityMechanics = new List<string> { "patrol_coordination", "barricade_gates" },
                StationedUnits = new List<string> { "enemy_corrupted_iron_knight", "enemy_shadow_brute" },
                HasAlarmBell = true,
                RecommendedLevel = 34
            });

            // 3. Void Armory
            RegisterSector(new FortressSectorDefinition
            {
                SectorId = "sector_fortress_armory",
                Name = "Void Armory & Siege Engine Depot",
                Description = "Subterranean smithy forging dark steel weapons and storing siege ballistas.",
                SecurityMechanics = new List<string> { "explosive_barrel_traps", "forge_fire_hazards" },
                StationedUnits = new List<string> { "enemy_legion_engineer", "enemy_corrupted_iron_knight" },
                HasAlarmBell = false,
                RecommendedLevel = 35
            });

            // 4. Prison Catacombs
            RegisterSector(new FortressSectorDefinition
            {
                SectorId = "sector_fortress_prison",
                Name = "Prison Catacombs & Torture Cells",
                Description = "Damp iron cage cells holding allied scouts, crown diplomats, and resistance fighters.",
                SecurityMechanics = new List<string> { "cell_key_locks", "guard_rotations" },
                StationedUnits = new List<string> { "enemy_legion_jailer", "enemy_shadow_stalker" },
                HasAlarmBell = true,
                RecommendedLevel = 35
            });

            // 5. High Command Hall
            RegisterSector(new FortressSectorDefinition
            {
                SectorId = "sector_fortress_command_hall",
                Name = "High Command Hall & Tactical Archives",
                Description = "Ornate dark marble hall housing Malakor's invasion maps, dispatch seals, and spellweavers.",
                SecurityMechanics = new List<string> { "arcane_barrier_door", "reinforcement_summons" },
                StationedUnits = new List<string> { "enemy_void_spellweaver", "enemy_legion_officer" },
                HasAlarmBell = true,
                RecommendedLevel = 36
            });

            // 6. Dark Ritual Chamber
            RegisterSector(new FortressSectorDefinition
            {
                SectorId = "sector_fortress_ritual_chamber",
                Name = "Dark Ritual Chamber & Void Altar",
                Description = "Sanctum containing a pulsing void crystal rift channeling power directly from Malakor.",
                SecurityMechanics = new List<string> { "void_crystal_pulses", "curse_traps" },
                StationedUnits = new List<string> { "enemy_high_cultist", "enemy_shadow_behemoth" },
                HasAlarmBell = false,
                RecommendedLevel = 36
            });

            // 7. Commander's Arena
            RegisterSector(new FortressSectorDefinition
            {
                SectorId = "sector_fortress_arena",
                Name = "Grand Marshal's War Arena",
                Description = "Elevated circular platform overlooking the fortress where General Vaelis commands the legion.",
                SecurityMechanics = new List<string> { "arena_wall_fire", "reinforcement_drop_gates" },
                StationedUnits = new List<string> { "enemy_boss_general_vaelis" },
                HasAlarmBell = false,
                RecommendedLevel = 36
            });
        }

        public void RegisterSector(FortressSectorDefinition sector)
        {
            if (sector != null && !string.IsNullOrEmpty(sector.SectorId))
            {
                _sectors[sector.SectorId] = sector;
            }
        }

        public FortressSectorDefinition? GetSector(string sectorId)
        {
            return _sectors.TryGetValue(sectorId, out var s) ? s : null;
        }

        public List<FortressSectorDefinition> GetAllSectors()
        {
            return new List<FortressSectorDefinition>(_sectors.Values);
        }
    }
}
