using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter14
{
    public enum BossPhaseType
    {
        Phase1_HighWarden,      // 100% -> 75% HP (3,000 HP): Sunfire Blade & Solar Shield
        Phase2_CorruptedWarden,  // 75% -> 50% HP (3,000 HP): Void Cleaves & Shadow Teleport
        Phase3_VoidAvatar,      // 50% -> 25% HP (3,000 HP): Gravity Distortion & Singularity Nova
        Phase4_UnboundVoidCore  // 25% -> 0% HP (3,000 HP): Enrage Frenzy & Void Beam Barrage
    }

    public class BossPhaseDefinition
    {
        public BossPhaseType Phase { get; set; } = BossPhaseType.Phase1_HighWarden;
        public string PhaseName { get; set; } = "";
        public int HealthThresholdHP { get; set; } = 3000;
        public List<string> PhaseAbilities { get; set; } = new();
        public string ArenaHazardType { get; set; } = "";
    }

    /// <summary>
    /// Content definition for Arch-Sorcerer Malakor, Void Avatar — the 4-phase final boss encounter in Hero of Eternia.
    /// Manages 12,000 Total HP across 4 distinct combat phases, phase thresholds, abilities, and victory triggers.
    /// </summary>
    public class FinalBossDefinition
    {
        private readonly List<BossPhaseDefinition> _phases = new();

        public string BossId { get; } = "boss_malakor_void_avatar";
        public string DisplayName { get; } = "Arch-Sorcerer Malakor, Void Avatar";
        public int TotalHealth { get; } = 12000; // 3,000 HP per phase
        public int RecommendedLevel { get; } = 45;
        public bool IsDefeated { get; set; } = false;

        public FinalBossDefinition()
        {
            InitializePhases();
        }

        public void InitializePhases()
        {
            _phases.Clear();

            // Phase 1: High Warden Malakor
            _phases.Add(new BossPhaseDefinition
            {
                Phase = BossPhaseType.Phase1_HighWarden,
                PhaseName = "High Warden Malakor",
                HealthThresholdHP = 3000,
                PhaseAbilities = new List<string> { "Sunfire Cleave", "Solar Shield Aegis", "Golden Radiant Beam" },
                ArenaHazardType = "hazard_sun_flares"
            });

            // Phase 2: Corrupted Warden
            _phases.Add(new BossPhaseDefinition
            {
                Phase = BossPhaseType.Phase2_CorruptedWarden,
                PhaseName = "Corrupted Warden of Sol",
                HealthThresholdHP = 3000,
                PhaseAbilities = new List<string> { "Shadow Step Strike", "Void Rift Slash", "Corrupted Pulse Wave" },
                ArenaHazardType = "hazard_shadow_eruption"
            });

            // Phase 3: Void Avatar Malakor
            _phases.Add(new BossPhaseDefinition
            {
                Phase = BossPhaseType.Phase3_VoidAvatar,
                PhaseName = "Void Avatar Malakor",
                HealthThresholdHP = 3000,
                PhaseAbilities = new List<string> { "Singularity Nova", "Gravity Reversal Crush", "Void Crystal Barrage" },
                ArenaHazardType = "hazard_gravity_distortion"
            });

            // Phase 4: Unbound Void Core
            _phases.Add(new BossPhaseDefinition
            {
                Phase = BossPhaseType.Phase4_UnboundVoidCore,
                PhaseName = "Unbound Void Core",
                HealthThresholdHP = 3000,
                PhaseAbilities = new List<string> { "Hyper Void Beam Storm", "Desperate Frenzy Cleave", "Cataclysm Core Pulse" },
                ArenaHazardType = "hazard_unbound_cataclysm"
            });
        }

        public BossPhaseDefinition? GetPhaseDefinition(BossPhaseType phase)
        {
            return _phases.Find(p => p.Phase == phase);
        }

        public List<BossPhaseDefinition> GetAllPhases()
        {
            return new List<BossPhaseDefinition>(_phases);
        }
    }
}
