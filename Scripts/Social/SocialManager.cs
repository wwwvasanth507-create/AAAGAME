using System;
using System.Collections.Generic;
using HeroOfEternia.Core;
using HeroOfEternia.Social.Factions;
using HeroOfEternia.Social.Reputation;
using HeroOfEternia.Social.Crime;
using HeroOfEternia.Social.Guard;
using HeroOfEternia.Social.Diplomacy;
using HeroOfEternia.Social.NpcReaction;

namespace HeroOfEternia.Social
{
    /// <summary>
    /// Central orchestrator for all social simulation systems.
    /// Manages initialization, update loop, and save/load integration.
    /// Registered in ServiceLocator as "SocialManager".
    /// </summary>
    public class SocialManager
    {
        public const string ServiceKey = "SocialManager";

        // Sub-systems
        public FactionDatabase FactionDb { get; private set; } = new();
        public ReputationManager Reputation { get; private set; } = new();
        public ReputationModifierRegistry ModifierRegistry { get; private set; } = new();
        public CrimeManager Crime { get; private set; } = new();
        public GuardAISystem GuardAI { get; private set; } = new();
        public DiplomacyManager Diplomacy { get; private set; } = new();
        public NpcReactionSystem NpcReactions { get; private set; } = new();

        private bool _initialized = false;
        private double _worldTime = 0;
        private double _lastCrimeExpirationCheck = 0;
        private const double CrimeExpirationInterval = 60.0; // Check every 60 seconds

        // ─────────────────────── Initialization ───────────────────────

        /// <summary>
        /// Initialize all social systems. Call once on game boot.
        /// </summary>
        public void Initialize()
        {
            if (_initialized) return;

            Logger.Info("SocialManager: Initializing social simulation systems...");

            // 1. Load faction database
            FactionDb.Initialize("res://Settings/faction_database.json");

            // 2. Load reputation modifiers
            ModifierRegistry.Initialize("res://Settings/reputation_modifiers.json");

            // 3. Initialize diplomacy with default relations
            Diplomacy.InitializeDefaults(FactionDb);

            // 4. Initialize NPC reaction system
            NpcReactions.Initialize(Reputation, Crime, Diplomacy, FactionDb);

            // 5. Wire up cross-system events
            WireEvents();

            _initialized = true;
            Logger.Info($"SocialManager: Initialized. {FactionDb.Count} factions, {ModifierRegistry.GetAllModifiers().Count} modifiers.");
        }

        private void WireEvents()
        {
            // When reputation changes, update NPC reactions
            Reputation.OnReputationChanged += (evt) =>
            {
                // Future: trigger NPC reaction updates
            };

            // When a crime is committed, alert nearby guards
            Crime.OnCrimeCommitted += (evt) =>
            {
                if (evt.HasWitnesses && !string.IsNullOrEmpty(evt.Crime.SettlementId))
                {
                    var guards = GuardAI.GetGuardsInSettlement(evt.Crime.SettlementId);
                    if (guards.Count > 0)
                    {
                        // Alert the nearest guard
                        GuardAI.ReportSuspiciousActivity(
                            guards[0],
                            evt.Crime.PerpetratorId,
                            evt.Crime.LocationId,
                            evt.Crime.Type);
                    }
                }
            };

            // When diplomatic relations change, update faction database
            Diplomacy.OnDiplomaticChange += (evt) =>
            {
                if (evt.NewRelation == DiplomaticRelation.Alliance)
                {
                    FactionDb.AddFriendlyFaction(evt.FactionIdA, evt.FactionIdB);
                    FactionDb.AddFriendlyFaction(evt.FactionIdB, evt.FactionIdA);
                }
                else if (evt.NewRelation == DiplomaticRelation.War)
                {
                    FactionDb.AddHostileFaction(evt.FactionIdA, evt.FactionIdB);
                    FactionDb.AddHostileFaction(evt.FactionIdB, evt.FactionIdA);
                }
            };
        }

        // ─────────────────────── Update Loop ───────────────────────

        /// <summary>
        /// Main update. Call from GameManager's _Process.
        /// </summary>
        public void Update(double delta, double worldTimeFraction)
        {
            if (!_initialized) return;

            _worldTime += delta;

            // Update guard AI
            GuardAI.UpdateAll(delta, worldTimeFraction);

            // Periodic crime expiration check
            if (_worldTime - _lastCrimeExpirationCheck >= CrimeExpirationInterval)
            {
                _lastCrimeExpirationCheck = _worldTime;
                Crime.ProcessExpirations(_worldTime);
            }
        }

        // ─────────────────────── Reputation Helpers ───────────────────────

        /// <summary>
        /// Apply a reputation modifier by ID. Looks up the modifier definition
        /// and applies all scope changes.
        /// </summary>
        public void ApplyReputationModifier(string modifierId, string npcId = "", string factionId = "", string settlementId = "", string regionId = "")
        {
            var modifier = ModifierRegistry.GetModifier(modifierId);
            if (modifier == null)
            {
                Logger.Warning($"SocialManager: Unknown reputation modifier '{modifierId}'");
                return;
            }

            // Apply global
            if (modifier.GlobalDelta != 0)
                Reputation.AdjustGlobal(modifier.GlobalDelta, modifierId);

            // Apply faction
            if (modifier.FactionDelta != 0 && !string.IsNullOrEmpty(factionId))
                Reputation.AdjustFaction(factionId, modifier.FactionDelta, modifierId);

            // Apply per-faction overrides
            foreach (var kv in modifier.PerFactionDeltas)
                Reputation.AdjustFaction(kv.Key, kv.Value, modifierId);

            // Apply settlement
            if (modifier.SettlementDelta != 0 && !string.IsNullOrEmpty(settlementId))
                Reputation.AdjustSettlement(settlementId, modifier.SettlementDelta, modifierId);

            // Apply per-settlement overrides
            foreach (var kv in modifier.PerSettlementDeltas)
                Reputation.AdjustSettlement(kv.Key, kv.Value, modifierId);

            // Apply region
            if (modifier.RegionDelta != 0 && !string.IsNullOrEmpty(regionId))
                Reputation.AdjustRegion(regionId, modifier.RegionDelta, modifierId);

            // Apply individual
            if (modifier.IndividualDelta != 0 && !string.IsNullOrEmpty(npcId))
                Reputation.AdjustIndividual(npcId, modifier.IndividualDelta, modifierId);
        }

        // ─────────────────────── Save / Load ───────────────────────

        /// <summary>
        /// Exports all social system state for save serialization.
        /// </summary>
        public SocialSaveData ExportSaveData()
        {
            return new SocialSaveData
            {
                Version = 1,
                ReputationSnapshot = Reputation.ExportSnapshot(),
                CrimeData = Crime.ExportSaveData(),
                GuardData = GuardAI.ExportSaveData(),
                DiplomaticData = Diplomacy.ExportSaveData(),
                FactionStates = FactionDb.ExportAllDefinitions()
            };
        }

        /// <summary>
        /// Restores all social system state from save data.
        /// </summary>
        public void RestoreSaveData(SocialSaveData? data)
        {
            if (data == null) return;

            Logger.Info("SocialManager: Restoring social simulation state...");

            Reputation.RestoreSnapshot(data.ReputationSnapshot);
            Crime.RestoreSaveData(data.CrimeData);
            GuardAI.RestoreSaveData(data.GuardData);
            Diplomacy.RestoreSaveData(data.DiplomaticData);

            // Restore faction runtime states
            if (data.FactionStates != null)
            {
                foreach (var faction in data.FactionStates)
                {
                    var existing = FactionDb.GetFaction(faction.FactionId);
                    if (existing != null)
                    {
                        existing.CurrentStrength = faction.CurrentStrength;
                        existing.MemberCount = faction.MemberCount;
                        existing.Treasury = faction.Treasury;
                        existing.IsActive = faction.IsActive;
                    }
                }
            }

            Logger.Info("SocialManager: Social state restored successfully.");
        }
    }

    /// <summary>
    /// Save data container for all social simulation systems.
    /// Versioned for forward compatibility.
    /// </summary>
    public class SocialSaveData
    {
        public int Version { get; set; } = 1;
        public Dictionary<string, int>? ReputationSnapshot { get; set; }
        public CrimeSaveData? CrimeData { get; set; }
        public GuardSaveData? GuardData { get; set; }
        public DiplomaticSaveData? DiplomaticData { get; set; }
        public List<FactionDefinition>? FactionStates { get; set; }
    }
}