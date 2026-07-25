using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Content.Prologue;
using HeroOfEternia.Core;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Tests
{
    public static class PrologueSystemTests
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
            Logger.Info("RUNNING PROLOGUE & CHAPTER 1 TESTS (PROMPT 28)");
            Logger.Info("==================================================");

            TestManagerInit();
            TestStartingRegionLocations();
            TestTutorialFlow();
            TestStarterNpcs();
            TestChapter1QuestChain();
            TestStarterEnemies();
            TestStarterEquipment();
            TestStarterExplorationNodes();
            TestSaveV23Integration();

            Logger.Info($"PROLOGUE TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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
            var mgr = new PrologueManager();
            mgr.Initialize();

            Assert(mgr.IsInitialized, "PrologueManager initialized");
            Assert(mgr.RegionContent.AllLocations.Count >= 6, "Oakvale region location nodes created");

            mgr.Shutdown();
        }

        private static void TestStartingRegionLocations()
        {
            var region = new StartingRegionContent();
            region.InitializeStartingRegion();

            var square = region.GetLocation("loc_oakvale_square");
            Assert(square != null, "Found Oakvale Village Square location");
            Assert(region.GetLocation("loc_hidden_cave") != null, "Found Whispering Cavern location");
        }

        private static void TestTutorialFlow()
        {
            var flow = new IntroductionFlowManager();
            Assert(flow.CurrentStep == TutorialStep.Movement, "Initial tutorial step is Movement");

            bool completed = flow.CompleteStep(TutorialStep.Movement);
            Assert(completed, "Completed Movement tutorial step");
            Assert(flow.CurrentStep == TutorialStep.CameraControl, "Advanced to CameraControl tutorial step");
        }

        private static void TestStarterNpcs()
        {
            var npcs = new StarterNpcDefinitions();
            npcs.RegisterDefaultStarterNPCs();

            var alden = npcs.GetNpc("npc_elder_alden");
            Assert(alden != null, "Found Elder Alden NPC");
            Assert(alden?.Profession == "Village Elder", "Alden profession is Village Elder");
        }

        private static void TestChapter1QuestChain()
        {
            var chain = new Chapter1QuestChain();
            chain.RegisterChapter1Quests();

            var q1 = QuestDatabase.GetQuest("q_oakvale_awakening");
            Assert(q1 != null, "Found quest q_oakvale_awakening");
            Assert(q1?.Category == QuestCategory.Main, "Category is Main");
            Assert(QuestDatabase.GetQuest("q_boss_skarr_encounter") != null, "Found quest q_boss_skarr_encounter");
        }

        private static void TestStarterEnemies()
        {
            var enemies = new StarterEnemyDefinitions();
            enemies.RegisterDefaultStarterEnemies();

            var slime = enemies.GetEnemy("enemy_green_slime");
            Assert(slime != null, "Found Green Slime");

            var skarr = enemies.GetEnemy("enemy_boss_skarr");
            Assert(skarr != null, "Found Baron Skarr mini-boss");
            Assert(skarr?.IsBoss ?? false, "Skarr marked as boss");
        }

        private static void TestStarterEquipment()
        {
            var eq = new StarterEquipmentDefinitions();
            eq.RegisterDefaultItems();

            var sword = eq.GetItem("item_weapon_rusty_sword");
            Assert(sword != null, "Found Rusty Iron Sword");
            Assert(sword?.Type == ItemType.Weapon, "Type is Weapon");
        }

        private static void TestStarterExplorationNodes()
        {
            var exp = new StarterExplorationContent();
            exp.RegisterDefaultExplorationContent();

            Assert(exp.AllNodes.Count >= 4, "Registered 4 starter exploration nodes");
        }

        private static void TestSaveV23Integration()
        {
            var profile = new SaveProfile
            {
                PrologueData = new PrologueSaveData
                {
                    CompletedTutorialSteps = new List<TutorialStep> { TutorialStep.Movement, TutorialStep.CameraControl },
                    IsPrologueCompleted = true
                }
            };

            Assert(profile.PrologueData != null, "SaveProfile contains PrologueData");
            Assert(profile.PrologueData.SaveVersion == 23, "SaveVersion is 23");
        }
    }
}
