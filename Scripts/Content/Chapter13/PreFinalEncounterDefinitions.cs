using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter13
{
    public class PreFinalEncounterRecord
    {
        public string EncounterId { get; set; } = "";
        public string Name { get; set; } = "";
        public string SectorId { get; set; } = "";
        public int MaxHealth { get; set; } = 4000;
        public int RecommendedLevel { get; set; } = 45;
        public List<string> SpecialAbilities { get; set; } = new();
        public bool IsDefeated { get; set; } = false;
        public string KeyDropId { get; set; } = "";
    }

    /// <summary>
    /// Definitions for the 4 elite mini-boss encounters guarding the Citadel sectors leading to Malakor's Antechamber.
    /// </summary>
    public class PreFinalEncounterDefinitions
    {
        private readonly Dictionary<string, PreFinalEncounterRecord> _encounters = new(StringComparer.OrdinalIgnoreCase);

        public int TotalEncounters => _encounters.Count;

        public PreFinalEncounterDefinitions()
        {
            InitializeEncounters();
        }

        public void InitializeEncounters()
        {
            // 1. Iron Construct Dreadnought
            RegisterEncounter(new PreFinalEncounterRecord
            {
                EncounterId = "encounter_iron_dreadnought",
                Name = "Iron Construct Dreadnought",
                SectorId = "sector_fortified_gatehouse",
                MaxHealth = 5000,
                RecommendedLevel = 45,
                SpecialAbilities = new List<string> { "Steam Slam", "Iron Shield Fortress", "Molten Rocket Burst" },
                KeyDropId = "key_gatehouse_iron"
            });

            // 2. Void Weaving Matriarch
            RegisterEncounter(new PreFinalEncounterRecord
            {
                EncounterId = "encounter_void_matriarch",
                Name = "Void Weaving Matriarch",
                SectorId = "sector_crystal_corridors",
                MaxHealth = 4200,
                RecommendedLevel = 45,
                SpecialAbilities = new List<string> { "Starlight Trap", "Void Web Teleport", "Singularity Nova" },
                KeyDropId = "key_crystal_vault"
            });

            // 3. Archon of the Sunless Void
            RegisterEncounter(new PreFinalEncounterRecord
            {
                EncounterId = "encounter_archon_sunless_void",
                Name = "Archon of the Sunless Void",
                SectorId = "sector_portal_vault",
                MaxHealth = 4500,
                RecommendedLevel = 45,
                SpecialAbilities = new List<string> { "Eclipse Beam", "Mirror Image Clones", "Void Rupture" },
                KeyDropId = "key_portal_sanctum"
            });

            // 4. High Commander Vaelis Remnant
            RegisterEncounter(new PreFinalEncounterRecord
            {
                EncounterId = "encounter_vaelis_remnant",
                Name = "High Commander Vaelis (Void Remnant)",
                SectorId = "sector_grand_promenade",
                MaxHealth = 3800,
                RecommendedLevel = 45,
                SpecialAbilities = new List<string> { "Shadow Execution Cleave", "Undying Void Frenzy", "Legion Banner Call" },
                KeyDropId = "key_antechamber_throne"
            });
        }

        public void RegisterEncounter(PreFinalEncounterRecord encounter)
        {
            if (encounter != null && !string.IsNullOrEmpty(encounter.EncounterId))
            {
                _encounters[encounter.EncounterId] = encounter;
            }
        }

        public bool DefeatEncounter(string encounterId)
        {
            if (!_encounters.TryGetValue(encounterId, out var enc)) return false;
            if (enc.IsDefeated) return true;

            enc.IsDefeated = true;
            Core.Logger.Info($"PreFinalEncounterDefinitions: Defeated pre-final mini-boss '{enc.Name}' ({encounterId})! Obtained key: {enc.KeyDropId}.");
            return true;
        }

        public PreFinalEncounterRecord? GetEncounter(string encounterId)
        {
            return _encounters.TryGetValue(encounterId, out var e) ? e : null;
        }

        public List<PreFinalEncounterRecord> GetAllEncounters()
        {
            return new List<PreFinalEncounterRecord>(_encounters.Values);
        }
    }
}
