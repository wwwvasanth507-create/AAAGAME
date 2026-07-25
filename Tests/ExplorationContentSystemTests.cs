using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;
using HeroOfEternia.Exploration;

namespace HeroOfEternia.Tests
{
    public static class ExplorationContentSystemTests
    {
        private static int _passed = 0;
        private static int _failed = 0;
        private static readonly List<string> _failures = new();

        public static void RunAll()
        {
            _passed = 0;
            _failed = 0;
            _failures.Clear();

            Logger.Info("==================================================");
            Logger.Info("RUNNING EXPLORATION CONTENT SYSTEM TESTS (PROMPT 25)");
            Logger.Info("==================================================");

            TestManagerInit();
            TestActivityDatabase();
            TestPuzzleManagerStageAdvancement();
            TestPuzzleAutoSolve();
            TestSecretDiscovery();
            TestCollectibleTracker();
            TestEnvironmentalInteractions();
            TestDynamicEventScheduling();
            TestRewardDistribution();
            TestSaveV20Integration();

            Logger.Info($"EXPLORATION TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
            if (_failed > 0)
            {
                foreach (var fail in _failures)
                {
                    Logger.Error($"  [FAIL] {fail}");
                }
            }
        }

        private static void Assert(bool condition, string message)
        {
            if (condition)
            {
                _passed++;
            }
            else
            {
                _failed++;
                _failures.Add(message);
                Logger.Error($"  ASSERT FAILED: {message}");
            }
        }

        private static void TestManagerInit()
        {
            var mgr = new ExplorationContentManager();
            mgr.Initialize();

            bool completed = mgr.CompleteActivity("act_hidden_chest_forest");
            Assert(completed, "Completed activity act_hidden_chest_forest");
            Assert(mgr.IsActivityCompleted("act_hidden_chest_forest"), "Activity recorded as completed");

            mgr.Shutdown();
        }

        private static void TestActivityDatabase()
        {
            var db = new ActivityDatabase();
            db.RegisterDefaultActivities();

            var act = db.GetActivity("act_rune_puzzle_shrine");
            Assert(act != null, "Found activity act_rune_puzzle_shrine");
            Assert(act?.Category == ActivityCategory.Puzzle, "Category is Puzzle");
        }

        private static void TestPuzzleManagerStageAdvancement()
        {
            var puz = new PuzzleManager();
            puz.RegisterPuzzle("puz_multi_lever", PuzzleMechanismType.MultiStage, 3);

            puz.AdvanceStage("puz_multi_lever");
            Assert(!puz.IsSolved("puz_multi_lever"), "Stage 1/3: Puzzle not yet solved");

            puz.AdvanceStage("puz_multi_lever");
            puz.AdvanceStage("puz_multi_lever");
            Assert(puz.IsSolved("puz_multi_lever"), "Stage 3/3: Multi-stage puzzle solved");
        }

        private static void TestPuzzleAutoSolve()
        {
            var puz = new PuzzleManager();
            puz.RegisterPuzzle("puz_rune_trio", PuzzleMechanismType.RuneActivation);

            puz.ToggleComponent("puz_rune_trio", "rune_1");
            puz.ToggleComponent("puz_rune_trio", "rune_2");
            Assert(puz.IsSolved("puz_rune_trio"), "Rune activation puzzle auto-solved when all components toggled");
        }

        private static void TestSecretDiscovery()
        {
            var sec = new SecretManager();
            sec.RegisterSecret(new SecretDefinition
            {
                SecretId = "sec_illusion_wall_01",
                Type = SecretType.IllusionaryWall,
                Position = Vector3.Zero
            });

            bool discovered = sec.DiscoverSecret("sec_illusion_wall_01");
            Assert(discovered, "Discovered illusionary wall secret");
            Assert(sec.IsDiscovered("sec_illusion_wall_01"), "Secret marked as discovered");
        }

        private static void TestCollectibleTracker()
        {
            var db = new CollectibleDatabase();
            db.RegisterCollectible(new CollectibleDefinition
            {
                CollectibleId = "col_relic_eternia",
                DisplayName = "Eternian Relic",
                Category = CollectibleCategory.AncientRelic
            });

            bool collected = db.CollectItem("col_relic_eternia");
            Assert(collected, "Collected Eternian Relic");
            Assert(db.CollectedCount == 1, "CollectedCount is 1");
        }

        private static void TestEnvironmentalInteractions()
        {
            var env = new EnvironmentalInteractionEngine();
            bool eventTriggered = false;

            env.OnInteractionTriggered += (data) =>
            {
                eventTriggered = true;
                Assert(data.InteractionType == EnvironmentalInteractionType.Burn, "InteractionType is Burn");
            };

            env.TriggerInteraction(EnvironmentalInteractionType.Burn, "bramble_gate", Vector3.Zero);
            Assert(eventTriggered, "Environmental interaction triggered event");
        }

        private static void TestDynamicEventScheduling()
        {
            var mgr = new ExplorationEventManager();
            var evt = mgr.TriggerEvent(ExplorationEventType.FallingMeteor, new Vector3(100, 0, 100), 5.0f);
            Assert(mgr.ActiveEvents.Count == 1, "Active exploration events count is 1");

            mgr.Update(6.0f); // Fast-forward past 5.0s duration
            Assert(mgr.ActiveEvents.Count == 0, "Expired meteor event removed from active events list");
        }

        private static void TestRewardDistribution()
        {
            var rew = new ExplorationRewardFramework();
            bool eventFired = false;

            rew.OnRewardDistributed += (pkg) =>
            {
                eventFired = true;
                Assert(pkg.Experience == 250, "Experience reward matches 250");
            };

            rew.DistributeReward(new ExplorationRewardPackage { Experience = 250, Gold = 100 });
            Assert(eventFired, "Reward distribution event dispatched");
        }

        private static void TestSaveV20Integration()
        {
            var profile = new SaveProfile
            {
                ExplorationContentData = new ExplorationContentSaveData
                {
                    CompletedActivityIds = new List<string> { "act_hidden_chest_forest" },
                    SolvedPuzzleIds = new List<string> { "puz_rune_trio" }
                }
            };

            Assert(profile.ExplorationContentData != null, "SaveProfile contains ExplorationContentData");
            Assert(profile.ExplorationContentData.SaveVersion == 20, "SaveVersion is 20");
        }
    }
}
