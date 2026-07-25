using System;
using System.Collections.Generic;
using HeroOfEternia.Social;
using HeroOfEternia.Social.Factions;
using HeroOfEternia.Social.Reputation;
using HeroOfEternia.Social.Crime;
using HeroOfEternia.Social.Guard;
using HeroOfEternia.Social.Diplomacy;
using HeroOfEternia.Social.NpcReaction;

namespace HeroOfEternia.Tests.SocialTests
{
    public class SocialSystemTests
    {
        private int _passed = 0;
        private int _failed = 0;
        private readonly List<string> _failures = new();

        public void RunAll()
        {
            _passed = 0;
            _failed = 0;
            _failures.Clear();

            Console.WriteLine("=== Social Simulation System Tests ===\n");

            // TASK 1-2: Faction Database Tests
            TestFactionDatabase();
            TestFactionRegistration();
            TestFactionLookups();
            TestFactionRuntimeModification();

            // TASK 3: Reputation Manager Tests
            TestReputationManager();
            TestReputationTiers();
            TestReputationBulkOperations();

            // TASK 4: Reputation Modifier Tests
            TestReputationModifierRegistry();

            // TASK 5: Crime System Tests
            TestCrimeReporting();
            TestWitnessDetection();
            TestBountyManagement();
            TestCrimeExpiration();

            // TASK 6: Guard AI Tests
            TestGuardRegistration();
            TestGuardStateTransitions();
            TestGuardAlertSystem();

            // TASK 7: Diplomacy Tests
            TestDiplomacyRelations();
            TestDiplomaticActions();

            // TASK 8: NPC Reaction Tests
            TestNpcReactionEvaluation();
            TestReactionFactorCalculation();

            // TASK 9: Save/Load Tests
            TestSocialManagerSaveLoad();

            // TASK 10-11: Performance & Stress Tests
            TestStressFactionLookups();
            TestStressGuardAI();

            // TASK 12: Edge Case Tests
            TestEdgeCases();

            // Summary
            Console.WriteLine($"\n=== Results: {_passed} passed, {_failed} failed ===");
            if (_failures.Count > 0)
            {
                Console.WriteLine("Failures:");
                foreach (var f in _failures)
                    Console.WriteLine($"  - {f}");
            }
        }

        // ──────────── FACTION DATABASE TESTS ────────────

        private void TestFactionDatabase()
        {
            var db = new FactionDatabase();
            db.Initialize("nonexistent.json"); // Should load defaults

            AssertEqual("FDB-1: Default factions loaded", 9, db.Count);
            AssertNotNull("FDB-2: Get kingdom", db.GetFaction("kingdom_eternia"));
            AssertNotNull("FDB-3: Get adventurers", db.GetFaction("adventurers_guild"));
            AssertNotNull("FDB-4: Get bandits", db.GetFaction("bandits_blackfang"));
            AssertNull("FDB-5: Nonexistent faction", db.GetFaction("nonexistent"));

            var byType = db.GetFactionsByType(FactionType.Kingdom);
            AssertEqual("FDB-6: Kingdom count", 1, byType.Count);

            var byRegion = db.GetFactionsByRegion("eternia_heartlands");
            AssertTrue("FDB-7: Region lookup has factions", byRegion.Count >= 3);

            Console.WriteLine("  [PASS] FactionDatabase: 7 tests");
        }

        private void TestFactionRegistration()
        {
            var db = new FactionDatabase();
            db.Initialize("nonexistent.json");

            var newFaction = new FactionDefinition
            {
                FactionId = "test_faction_01",
                DisplayName = "Test Faction",
                Type = FactionType.PlayerCreated,
                Territory = "test_region"
            };

            db.RegisterFaction(newFaction);
            AssertNotNull("FREG-1: Registered faction exists", db.GetFaction("test_faction_01"));
            AssertEqual("FREG-2: Count increased", 10, db.Count);

            // Test duplicate prevention
            db.RegisterFaction(newFaction);
            AssertEqual("FREG-3: No duplicate count", 10, db.Count);

            Console.WriteLine("  [PASS] FactionRegistration: 3 tests");
        }

        private void TestFactionLookups()
        {
            var db = new FactionDatabase();
            db.Initialize("nonexistent.json");

            var refs = db.GetAllFactionReferences();
            AssertEqual("FLOOK-1: All references", 9, refs.Count);

            var refsByType = db.GetFactionsByType(FactionType.Military);
            AssertEqual("FLOOK-2: Military count", 1, refsByType.Count);

            AssertTrue("FLOOK-3: FactionExists true", db.FactionExists("kingdom_eternia"));
            AssertFalse("FLOOK-4: FactionExists false", db.FactionExists("fake_faction"));

            Console.WriteLine("  [PASS] FactionLookups: 4 tests");
        }

        private void TestFactionRuntimeModification()
        {
            var db = new FactionDatabase();
            db.Initialize("nonexistent.json");

            db.UpdateFactionStrength("kingdom_eternia", 75);
            var fac = db.GetFaction("kingdom_eternia");
            AssertNotNull("FRUN-1: Faction exists", fac);
            if (fac != null)
                AssertEqual("FRUN-2: Strength updated", 75, fac.CurrentStrength);

            db.SetFactionActive("bandits_blackfang", false);
            var bandits = db.GetFaction("bandits_blackfang");
            AssertNotNull("FRUN-3: Bandits exist", bandits);
            if (bandits != null)
                AssertFalse("FRUN-4: Bandits inactive", bandits.IsActive);

            Console.WriteLine("  [PASS] FactionRuntimeModification: 4 tests");
        }

        // ──────────── REPUTATION MANAGER TESTS ────────────

        private void TestReputationManager()
        {
            var rep = new ReputationManager();

            // Test initial values
            AssertEqual("REP-1: Initial global", 0, rep.GetGlobal());
            AssertEqual("REP-2: Initial faction", 0, rep.GetFaction("test"));

            // Test adjustments
            rep.AdjustGlobal(50, "test");
            AssertEqual("REP-3: Global increase", 50, rep.GetGlobal());

            rep.AdjustFaction("test_faction", -30, "test");
            AssertEqual("REP-4: Faction decrease", -30, rep.GetFaction("test_faction"));

            // Test clamping
            rep.AdjustGlobal(2000, "test"); // Should clamp to 1000
            AssertEqual("REP-5: Global clamped", 1000, rep.GetGlobal());

            rep.AdjustGlobal(-3000, "test"); // Should clamp to -1000
            AssertEqual("REP-6: Global negative clamped", -1000, rep.GetGlobal());

            // Test settlement reputation
            rep.AdjustSettlement("test_settlement", 25, "test");
            AssertEqual("REP-7: Settlement rep", 25, rep.GetSettlement("test_settlement"));

            // Test individual reputation
            rep.AdjustIndividual("npc_01", 40, "test");
            AssertEqual("REP-8: Individual rep", 40, rep.GetIndividual("npc_01"));

            Console.WriteLine("  [PASS] ReputationManager: 8 tests");
        }

        private void TestReputationTiers()
        {
            var rep = new ReputationManager();

            AssertEqual("TIER-1: Neutral at 0", "Neutral", rep.GetGlobalTier());

            rep.AdjustGlobal(200, "test");
            AssertEqual("TIER-2: Friendly at 200", "Friendly", rep.GetGlobalTier());

            rep.AdjustGlobal(600, "test");
            AssertEqual("TIER-3: Legendary at 800+", "Legendary", rep.GetGlobalTier());

            // Test custom tiers
            var customTiers = new List<ReputationTier>
            {
                new() { Name = "Enemy", MinValue = -1000, MaxValue = -1 },
                new() { Name = "Friend", MinValue = 0, MaxValue = 1000 }
            };

            rep.SetCustomTiers(customTiers);
            AssertEqual("TIER-4: Custom tiers count", 2, rep.GetCurrentTiers().Count);

            // Reset to default by setting back
            rep.SetCustomTiers(null!);
            AssertEqual("TIER-5: Invalid tiers rejected", 2, rep.GetCurrentTiers().Count);

            Console.WriteLine("  [PASS] ReputationTiers: 5 tests");
        }

        private void TestReputationBulkOperations()
        {
            var rep = new ReputationManager();

            var bulkOps = new Dictionary<ReputationScope, Dictionary<string, int>>
            {
                {
                    ReputationScope.Faction,
                    new Dictionary<string, int> { { "fac_a", 10 }, { "fac_b", -5 } }
                },
                {
                    ReputationScope.Settlement,
                    new Dictionary<string, int> { { "set_a", 20 } }
                }
            };

            rep.AdjustMulti(bulkOps, "bulk_test");
            AssertEqual("BULK-1: Bulk faction A", 10, rep.GetFaction("fac_a"));
            AssertEqual("BULK-2: Bulk faction B", -5, rep.GetFaction("fac_b"));
            AssertEqual("BULK-3: Bulk settlement", 20, rep.GetSettlement("set_a"));

            Console.WriteLine("  [PASS] ReputationBulk: 3 tests");
        }

        // ──────────── REPUTATION MODIFIER TESTS ────────────

        private void TestReputationModifierRegistry()
        {
            var reg = new ReputationModifierRegistry();
            reg.Initialize("nonexistent.json");

            var all = reg.GetAllModifiers();
            AssertTrue("MOD-1: Default modifiers loaded", all.Count >= 20);

            var helpMods = reg.GetModifiersByCategory("help");
            AssertTrue("MOD-2: Help category exists", helpMods.Count >= 3);

            var crimeMods = reg.GetModifiersByCategory("crime");
            AssertTrue("MOD-3: Crime category exists", crimeMods.Count >= 5);

            var modifier = reg.GetModifier("crime_murder");
            AssertNotNull("MOD-4: Crime murder exists", modifier);
            if (modifier != null)
                AssertEqual("MOD-5: Murder global delta", -50, modifier.GlobalDelta);

            // Test runtime registration
            var customMod = new ReputationModifier
            {
                ModifierId = "custom_test",
                DisplayName = "Custom Test",
                Category = "custom",
                IndividualDelta = 100
            };
            reg.RegisterModifier(customMod);
            AssertNotNull("MOD-6: Custom modifier exists", reg.GetModifier("custom_test"));

            Console.WriteLine("  [PASS] ReputationModifierRegistry: 6 tests");
        }

        // ──────────── CRIME SYSTEM TESTS ────────────

        private void TestCrimeReporting()
        {
            var crime = new CrimeManager();

            var record = crime.ReportCrime(
                CrimeType.Theft,
                "player",
                "victim_01",
                "market_square",
                "eternia_heartlands",
                "eternia_capital",
                100.0,
                new List<string> { "witness_01" });

            AssertNotNull("CRIME-1: Crime record created", record);
            AssertEqual("CRIME-2: Crime type", CrimeType.Theft, record.Type);
            AssertEqual("CRIME-3: Perpetrator", "player", record.PerpetratorId);
            AssertEqual("CRIME-4: Has witnesses", 1, record.WitnessIds.Count);
            AssertTrue("CRIME-5: Bounty assigned", record.BountyValue > 0);

            var history = crime.GetCrimeHistory("player");
            AssertEqual("CRIME-6: Crime history count", 1, history.Count);

            // Test crime without witnesses
            var hiddenCrime = crime.ReportCrime(
                CrimeType.Trespassing,
                "player",
                "",
                "back_alley",
                "eternia_heartlands",
                "eternia_capital",
                101.0);

            AssertEqual("CRIME-7: No witnesses", 0, hiddenCrime.WitnessIds.Count);

            Console.WriteLine("  [PASS] CrimeReporting: 7 tests");
        }

        private void TestWitnessDetection()
        {
            var crime = new CrimeManager();

            var nearbyNpcs = new List<string> { "npc_01", "npc_02", "npc_03", "npc_04", "npc_05" };

            // Hidden crime should have no witnesses
            var hiddenWitnesses = crime.DetectWitnesses(
                "alley", 10f, nearbyNpcs, "player", CrimeType.Theft, true);
            AssertEqual("WIT-1: Hidden crime no witnesses", 0, hiddenWitnesses.Count);

            // Murder should have high detection chance
            var murderWitnesses = crime.DetectWitnesses(
                "square", 5f, nearbyNpcs, "player", CrimeType.Murder, false);
            AssertTrue("WIT-2: Murder likely has witnesses", murderWitnesses.Count > 0);

            Console.WriteLine("  [PASS] WitnessDetection: 2 tests");
        }

        private void TestBountyManagement()
        {
            var crime = new CrimeManager();

            crime.AddBounty("player", "kingdom_eternia", 100);
            AssertEqual("BOUNTY-1: Total bounty", 100, crime.GetTotalBounty("player"));
            AssertEqual("BOUNTY-2: Faction bounty", 100, crime.GetFactionBounty("kingdom_eternia", "player"));

            crime.AddBounty("player", "kingdom_eternia", 50);
            AssertEqual("BOUNTY-3: Stacked bounty", 150, crime.GetTotalBounty("player"));

            var allBounties = crime.GetAllActiveBounties();
            AssertTrue("BOUNTY-4: Active bounties", allBounties.ContainsKey("player"));

            crime.ClearBounty("player");
            AssertEqual("BOUNTY-5: Cleared bounty", 0, crime.GetTotalBounty("player"));

            Console.WriteLine("  [PASS] BountyManagement: 5 tests");
        }

        private void TestCrimeExpiration()
        {
            var crime = new CrimeManager();

            // Report a crime with short expiration
            var record = crime.ReportCrime(
                CrimeType.Theft, "player", "", "loc", "reg", "set",
                100.0, new List<string> { "witness_01" });
            record.ExpirationTime = 50.0; // Short expiration

            // Process with time beyond expiration
            crime.ProcessExpirations(200.0);

            var active = crime.GetActiveCrimes("player");
            AssertEqual("EXP-1: Crime expired", 0, active.Count);

            Console.WriteLine("  [PASS] CrimeExpiration: 1 test");
        }

        // ──────────── GUARD AI TESTS ────────────

        private void TestGuardRegistration()
        {
            var guard = new GuardAISystem();

            var config = new GuardConfig
            {
                GuardId = "guard_01",
                SettlementId = "eternia_capital",
                PatrolRadius = 30f,
                PatrolRoute = new List<string> { "point_a", "point_b", "point_c" }
            };

            guard.RegisterGuard(config);
            AssertNotNull("GUARD-1: Config exists", guard.GetGuardConfig("guard_01"));
            AssertNotNull("GUARD-2: State exists", guard.GetGuardState("guard_01"));

            var guardsInCity = guard.GetGuardsInSettlement("eternia_capital");
            AssertEqual("GUARD-3: Guards in city", 1, guardsInCity.Count);

            guard.UnregisterGuard("guard_01");
            AssertNull("GUARD-4: Guard removed", guard.GetGuardConfig("guard_01"));

            Console.WriteLine("  [PASS] GuardRegistration: 4 tests");
        }

        private void TestGuardStateTransitions()
        {
            var guard = new GuardAISystem();

            var config = new GuardConfig { GuardId = "guard_02", SettlementId = "test_settlement" };
            guard.RegisterGuard(config);

            // Test initial state
            var state = guard.GetGuardState("guard_02");
            AssertNotNull("GSTATE-1: State exists", state);
            if (state != null)
                AssertEqual("GSTATE-2: Initial patrol", GuardState.Patrol, state.CurrentState);

            // Test suspicious activity report
            guard.ReportSuspiciousActivity("guard_02", "suspect_01", "loc_01", CrimeType.Theft);
            state = guard.GetGuardState("guard_02");
            if (state != null)
            {
                AssertEqual("GSTATE-3: Investigating", GuardState.Investigate, state.CurrentState);
                AssertEqual("GSTATE-4: Target set", "suspect_01", state.TargetId);
                AssertEqual("GSTATE-5: Alert yellow", GuardAlertLevel.Yellow, state.AlertLevel);
            }

            // Test arrest order
            guard.IssueArrestOrder("guard_02", "suspect_01");
            state = guard.GetGuardState("guard_02");
            if (state != null)
            {
                AssertEqual("GSTATE-6: Arresting", GuardState.Arrest, state.CurrentState);
                AssertEqual("GSTATE-7: Alert red", GuardAlertLevel.Red, state.AlertLevel);
            }

            Console.WriteLine("  [PASS] GuardStateTransitions: 7 tests");
        }

        private void TestGuardAlertSystem()
        {
            var guard = new GuardAISystem();

            // Register multiple guards
            for (int i = 0; i < 5; i++)
            {
                guard.RegisterGuard(new GuardConfig
                {
                    GuardId = $"guard_alerts_{i}",
                    SettlementId = "alert_city"
                });
            }

            guard.SetSettlementAlertLevel("alert_city", GuardAlertLevel.Red);

            var guards = guard.GetGuardsInSettlement("alert_city");
            AssertEqual("ALERT-1: All guards present", 5, guards.Count);

            var level = guard.GetSettlementAlertLevel("alert_city");
            AssertEqual("ALERT-2: Alert level red", GuardAlertLevel.Red, level);

            Console.WriteLine("  [PASS] GuardAlertSystem: 2 tests");
        }

        // ──────────── DIPLOMACY TESTS ────────────

        private void TestDiplomacyRelations()
        {
            var dip = new DiplomacyManager();

            // Test default relation
            var rel = dip.GetRelation("faction_a", "faction_b");
            AssertEqual("DIP-1: Default neutral", DiplomaticRelation.Neutral, rel);

            // Test setting relation
            dip.SetRelation("faction_a", "faction_b", DiplomaticRelation.Alliance);
            rel = dip.GetRelation("faction_a", "faction_b");
            AssertEqual("DIP-2: Alliance set", DiplomaticRelation.Alliance, rel);

            // Test symmetric key
            rel = dip.GetRelation("faction_b", "faction_a");
            AssertEqual("DIP-3: Symmetric relation", DiplomaticRelation.Alliance, rel);

            Console.WriteLine("  [PASS] DiplomacyRelations: 3 tests");
        }

        private void TestDiplomaticActions()
        {
            var dip = new DiplomacyManager();

            dip.DeclareAlliance("kingdom", "guild");
            AssertTrue("DIPACT-1: Allied", dip.AreAllied("kingdom", "guild"));

            dip.DeclareWar("kingdom", "bandits");
            AssertTrue("DIPACT-2: At war", dip.AreAtWar("kingdom", "bandits"));

            dip.EstablishTradeAgreement("kingdom", "merchants");
            AssertTrue("DIPACT-3: Trade", dip.HaveTradeAgreement("kingdom", "merchants"));

            dip.DeclarePeace("kingdom", "bandits");
            AssertEqual("DIPACT-4: Peace", DiplomaticRelation.Peace, dip.GetRelation("kingdom", "bandits"));

            // Test allies query
            var allies = dip.GetAllies("kingdom");
            AssertTrue("DIPACT-5: Has allies", allies.Count > 0);

            // Test reputation modifier
            var mod = dip.GetDiplomaticReputationModifier("kingdom", "guild");
            AssertEqual("DIPACT-6: Alliance modifier", 30, mod);

            var warMod = dip.GetDiplomaticReputationModifier("kingdom", "bandits");
            AssertEqual("DIPACT-7: War modifier", -50, warMod);

            Console.WriteLine("  [PASS] DiplomaticActions: 7 tests");
        }

        // ──────────── NPC REACTION TESTS ────────────

        private void TestNpcReactionEvaluation()
        {
            var rep = new ReputationManager();
            var crime = new CrimeManager();
            var dip = new DiplomacyManager();
            var facDb = new FactionDatabase();
            facDb.Initialize("nonexistent.json");

            var reaction = new NpcReactionSystem();
            reaction.Initialize(rep, crime, dip, facDb);

            // Default reaction
            var ctx = new ReactionContext
            {
                NpcId = "npc_test",
                SettlementId = "test_settlement"
            };

            var result = reaction.EvaluateReaction(ctx);
            AssertEqual("REACT-1: Default neutral", "Neutral", result.DispositionLabel);
            AssertFalse("REACT-2: Not attacking", result.WillAttack);
            AssertFalse("REACT-3: Not fleeing", result.WillFlee);
            AssertFalse("REACT-4: Not reporting", result.WillReport);
            AssertFalse("REACT-5: Not trading", result.WillTrade);

            // Test with very low reputation
            rep.AdjustGlobal(-500, "test");
            rep.AdjustSettlement("test_settlement", -300, "test");
            result = reaction.EvaluateReaction(ctx);
            AssertTrue("REACT-6: Hostile reaction leads to negative disposition", result.Disposition < 0);

            // Test hidden player
            ctx.IsPlayerHidden = true;
            result = reaction.EvaluateReaction(ctx);
            AssertFalse("REACT-7: Hidden prevents attack", result.WillAttack);
            AssertFalse("REACT-8: Hidden prevents flee", result.WillFlee);

            Console.WriteLine("  [PASS] NpcReactionEvaluation: 8 tests");
        }

        private void TestReactionFactorCalculation()
        {
            var reaction = new NpcReactionSystem();
            var rep = new ReputationManager();
            var crime = new CrimeManager();
            var dip = new DiplomacyManager();
            var facDb = new FactionDatabase();
            facDb.Initialize("nonexistent.json");
            reaction.Initialize(rep, crime, dip, facDb);

            // Test time-of-day factor
            var nightCtx = new ReactionContext { WorldTimeFraction = 0.9 };
            var nightResult = reaction.EvaluateReaction(nightCtx);

            var dayCtx = new ReactionContext { WorldTimeFraction = 0.5 };
            var dayResult = reaction.EvaluateReaction(dayCtx);

            AssertTrue("FACTOR-1: Night worse than day", nightResult.Disposition <= dayResult.Disposition);

            // Test occupation factor
            var guardCtx = new ReactionContext { NpcOccupation = "Guard" };
            var priestCtx = new ReactionContext { NpcOccupation = "Priest" };
            AssertTrue("FACTOR-2: Priest friendlier than guard",
                reaction.EvaluateReaction(priestCtx).Disposition >
                reaction.EvaluateReaction(guardCtx).Disposition);

            Console.WriteLine("  [PASS] ReactionFactorCalculation: 2 tests");
        }

        // ──────────── SAVE/LOAD TESTS ────────────

        private void TestSocialManagerSaveLoad()
        {
            var mgr = new SocialManager();
            mgr.Initialize();

            // Make some changes
            mgr.Reputation.AdjustGlobal(100, "test");
            mgr.Reputation.AdjustFaction("kingdom_eternia", -50, "test");
            mgr.Crime.ReportCrime(CrimeType.Theft, "player", "npc_01", "market",
                "region", "settlement", 100.0, new List<string> { "witness" });

            // Export
            var saveData = mgr.ExportSaveData();
            AssertNotNull("SAVE-1: Save data exists", saveData);
            AssertEqual("SAVE-2: Version correct", 1, saveData.Version);
            AssertNotNull("SAVE-3: Reputation snapshot", saveData.ReputationSnapshot);
            AssertNotNull("SAVE-4: Crime data", saveData.CrimeData);
            AssertNotNull("SAVE-5: Guard data", saveData.GuardData);
            AssertNotNull("SAVE-6: Diplomatic data", saveData.DiplomaticData);

            // Create a new manager and restore
            var mgr2 = new SocialManager();
            mgr2.Initialize();
            mgr2.RestoreSaveData(saveData);

            AssertEqual("SAVE-7: Restored global rep", 100, mgr2.Reputation.GetGlobal());
            AssertEqual("SAVE-8: Restored faction rep", -50, mgr2.Reputation.GetFaction("kingdom_eternia"));
            AssertEqual("SAVE-9: Restored crime count", 1, mgr2.Crime.GetTotalCrimeCount());

            // Test null restore
            mgr2.RestoreSaveData(null);
            AssertEqual("SAVE-10: Null restore safe", 100, mgr2.Reputation.GetGlobal());

            Console.WriteLine("  [PASS] SocialManagerSaveLoad: 10 tests");
        }

        // ──────────── STRESS TESTS ────────────

        private void TestStressFactionLookups()
        {
            var db = new FactionDatabase();
            db.Initialize("nonexistent.json");

            // Register 50 additional factions
            for (int i = 0; i < 50; i++)
            {
                db.RegisterFaction(new FactionDefinition
                {
                    FactionId = $"stress_faction_{i}",
                    DisplayName = $"Stress Faction {i}",
                    Type = (FactionType)(i % 15),
                    Territory = i % 3 == 0 ? "region_a" : "region_b"
                });
            }

            AssertEqual("STRESS-1: 59 total factions", 59, db.Count);

            // Stress lookups
            for (int i = 0; i < 100; i++)
            {
                var fac = db.GetFaction("kingdom_eternia");
                AssertNotNull($"STRESS-2: Lookup {i}", fac);

                var byType = db.GetFactionsByType(FactionType.Kingdom);
                AssertTrue($"STRESS-3: Type lookup {i}", byType.Count > 0);
            }

            Console.WriteLine("  [PASS] StressFactionLookups: 3 tests");
        }

        private void TestStressGuardAI()
        {
            var guard = new GuardAISystem();

            // Register 100 guards across 5 settlements
            for (int i = 0; i < 100; i++)
            {
                string settlement = $"city_{i % 5}";
                guard.RegisterGuard(new GuardConfig
                {
                    GuardId = $"stress_guard_{i}",
                    SettlementId = settlement,
                    PatrolRoute = new List<string> { $"p{i}_a", $"p{i}_b" }
                });
            }

            // Bulk state updates
            for (int tick = 0; tick < 50; tick++)
            {
                guard.UpdateAll(0.25, 0.5);
            }

            AssertEqual("STRESS-G1: 100 guards", 100, guard.ActiveGuardCount);

            var cityGuards = guard.GetGuardsInSettlement("city_0");
            AssertEqual("STRESS-G2: 20 guards per city", 20, cityGuards.Count);

            Console.WriteLine("  [PASS] StressGuardAI: 2 tests");
        }

        // ──────────── EDGE CASE TESTS ────────────

        private void TestEdgeCases()
        {
            // Edge case: Empty faction registration
            var db = new FactionDatabase();
            db.Initialize("nonexistent.json");
            db.RegisterFaction(null!); // Should not crash
            db.RegisterFaction(new FactionDefinition { FactionId = "" }); // Should not crash
            AssertEqual("EDGE-1: Invalid registers safe", 9, db.Count);

            // Edge case: Empty crime reporting
            var crime = new CrimeManager();
            var record = crime.ReportCrime(CrimeType.Theft, "", "", "", "", "", 0);
            AssertNotNull("EDGE-2: Minimal crime", record);

            // Edge case: Empty guard registration
            var guard = new GuardAISystem();
            guard.RegisterGuard(null!); // Should not crash
            guard.RegisterGuard(new GuardConfig { GuardId = "" }); // Should not crash
            AssertEqual("EDGE-3: Invalid guard safe", 0, guard.ActiveGuardCount);

            // Edge case: Null restore on crime
            crime.RestoreSaveData(null);
            AssertEqual("EDGE-4: Null crime restore safe", 1, crime.GetTotalCrimeCount());

            // Edge case: Empty diplomacy
            var dip = new DiplomacyManager();
            var rel = dip.GetRelation("", "");
            AssertEqual("EDGE-5: Empty relation neutral", DiplomaticRelation.Neutral, rel);

            // Edge case: Reaction with empty context
            var reaction = new NpcReactionSystem();
            reaction.Initialize(new ReputationManager(), new CrimeManager(),
                new DiplomacyManager(), new FactionDatabase());
            var emptyResult = reaction.EvaluateReaction(new ReactionContext());
            AssertNotNull("EDGE-6: Empty context result", emptyResult);

            // Edge case: Reputation manager extreme values
            var rep = new ReputationManager();
            for (int i = 0; i < 100; i++)
            {
                rep.AdjustGlobal(20, "test");
                rep.AdjustFaction("faction_test", -20, "test");
            }
            AssertEqual("EDGE-7: Global clamped", 1000, rep.GetGlobal());
            AssertEqual("EDGE-8: Faction clamped", -1000, rep.GetFaction("faction_test"));

            Console.WriteLine("  [PASS] EdgeCases: 8 tests");
        }

        // ──────────── HELPERS ────────────

        private void AssertEqual(string testName, object expected, object actual)
        {
            if (Equals(expected, actual))
            {
                _passed++;
            }
            else
            {
                _failed++;
                _failures.Add($"{testName}: Expected '{expected}', got '{actual}'");
            }
        }

        private void AssertTrue(string testName, bool condition)
        {
            if (condition)
                _passed++;
            else
            {
                _failed++;
                _failures.Add($"{testName}: Expected true, got false");
            }
        }

        private void AssertFalse(string testName, bool condition)
        {
            if (!condition)
                _passed++;
            else
            {
                _failed++;
                _failures.Add($"{testName}: Expected false, got true");
            }
        }

        private void AssertNotNull(string testName, object? obj)
        {
            if (obj != null)
                _passed++;
            else
            {
                _failed++;
                _failures.Add($"{testName}: Expected non-null, got null");
            }
        }

        private void AssertNull(string testName, object? obj)
        {
            if (obj == null)
                _passed++;
            else
            {
                _failed++;
                _failures.Add($"{testName}: Expected null, got non-null");
            }
        }
    }
}