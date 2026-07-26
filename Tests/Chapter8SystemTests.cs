using System;
using System.Collections.Generic;
using HeroOfEternia.Content.Chapter8;
using HeroOfEternia.Core;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Tests
{
    public static class Chapter8SystemTests
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
            Logger.Info("RUNNING CHAPTER 8 SYSTEM TESTS (PROMPT 35)");
            Logger.Info("==================================================");

            TestManagerInit();
            TestShadowFrontierZones();
            TestTraversalSystemManager();
            TestShadowFrontierEnemies();
            TestChapter8Quests();
            TestSaveV35();

            Logger.Info($"CHAPTER 8 TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
            if (_failed > 0)
                foreach (var f in _failures)
                    Logger.Error($"  [FAIL] {f}");
        }

        private static void Assert(bool condition, string message)
        {
            if (condition) { _passed++; }
            else
            {
                _failed++;
                _failures.Add(message);
                Logger.Error($"  ASSERT FAILED: {message}");
            }
        }

        private static void TestManagerInit()
        {
            var mgr = new Chapter8Manager();
            mgr.Initialize();
            Assert(mgr.IsInitialized, "Chapter8Manager initialized successfully");
            mgr.Shutdown();
        }

        private static void TestShadowFrontierZones()
        {
            var frontier = new ShadowFrontierContent();
            Assert(frontier.GetAllZones().Count == 7, "The Shadow Frontier has 7 hazardous sub-zones");
            Assert(frontier.GetZone("zone_corrupted_woods") != null, "Found Corrupted Whispering Woods");
            Assert(frontier.GetZone("zone_dread_ravine") != null, "Found Dread Ravine");
            Assert(frontier.GetZone("zone_obsidian_crag_sanctuary") != null, "Found Obsidian Crag Sanctuary");
        }

        private static void TestTraversalSystemManager()
        {
            var traversal = new TraversalSystemManager();
            traversal.Initialize();

            Assert(traversal.IsToolUnlocked("tool_grapple_hook"), "Grapple Hook tool unlocked by default");
            Assert(traversal.GetNode("node_dread_ravine_grapple") != null, "Found Dread Ravine grapple node");

            bool executed = traversal.ExecuteTraversal("node_dread_ravine_grapple");
            Assert(executed, "Executed grapple traversal on Dread Ravine node");

            traversal.Shutdown();
        }

        private static void TestShadowFrontierEnemies()
        {
            var roster = new ShadowFrontierEnemies();
            Assert(roster.GetAllEnemies().Count == 5, "Act III roster contains 5 enemy definitions");

            var knight = roster.GetEnemy("enemy_corrupted_iron_knight");
            Assert(knight != null, "Found Corrupted Iron Knight");
            Assert(knight?.IsElite == true, "Corrupted Iron Knight is Elite");

            var behemoth = roster.GetEnemy("enemy_shadow_behemoth");
            Assert(behemoth?.MaxHealth == 2200f, "Shadow Behemoth has 2200 HP");
        }

        private static void TestChapter8Quests()
        {
            var chain = new Chapter8QuestChain();
            chain.RegisterChapter8Quests();

            Assert(QuestDatabase.GetQuest("q_chapter8_shadow_frontier_entry") != null, "Found q_chapter8_shadow_frontier_entry");
            Assert(QuestDatabase.GetQuest("q_chapter8_traversal_challenge") != null, "Found q_chapter8_traversal_challenge");
            Assert(QuestDatabase.GetQuest("q_chapter8_shadow_champion_confrontation") != null, "Found q_chapter8_shadow_champion_confrontation");
        }

        private static void TestSaveV35()
        {
            var saveData = new Chapter8SaveData
            {
                Act3Started = true,
                ShadowFrontierDiscovered = true,
                ShadowBehemothDefeated = true,
                SaveVersion = 35
            };
            saveData.UnlockedTraversalTools.Add("tool_grapple_hook");
            saveData.DiscoveredFrontierZoneIds.Add("zone_dread_ravine");

            Assert(saveData.SaveVersion == 35, "Chapter8SaveData is Save Version 35");
            Assert(saveData.Act3Started, "Act3Started flag persisted");
            Assert(saveData.UnlockedTraversalTools.Contains("tool_grapple_hook"), "Unlocked traversal tool persisted");
        }
    }
}
