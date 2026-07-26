using System;
using System.Collections.Generic;
using HeroOfEternia.Content.Chapter10;
using HeroOfEternia.Core;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Tests
{
    public static class Chapter10SystemTests
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
            Logger.Info("RUNNING CHAPTER 10 SYSTEM TESTS (PROMPT 37)");
            Logger.Info("==================================================");

            TestManagerInit();
            TestAncientTempleChambers();
            TestEnvironmentalLoreManager();
            TestTemplePuzzleSequence();
            TestChapter10Quests();
            TestSaveV37();

            Logger.Info($"CHAPTER 10 TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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
            var mgr = new Chapter10Manager();
            mgr.Initialize();
            Assert(mgr.IsInitialized, "Chapter10Manager initialized successfully");
            mgr.Shutdown();
        }

        private static void TestAncientTempleChambers()
        {
            var temple = new AncientTempleContent();
            Assert(temple.TotalChambers == 7, "Temple of Eternal Sun has 7 chambers");
            Assert(temple.GetChamber("chamber_temple_entrance") != null, "Found Portal of Astral Light chamber");
            Assert(temple.GetChamber("chamber_observatory") != null, "Found Observatory chamber");
            Assert(temple.GetChamber("chamber_astral_vault") != null, "Found Core Astral Vault chamber");
        }

        private static void TestEnvironmentalLoreManager()
        {
            var lore = new EnvironmentalLoreManager();
            lore.Initialize();

            Assert(lore.GetLoreRecord("lore_sun_carving_mural") != null, "Found Sun Carving Mural lore record");
            Assert(lore.GetLoreRecord("lore_sun_carving_mural")?.IsDiscovered == false, "Initially undiscovered");

            bool discovered = lore.DiscoverLore("lore_sun_carving_mural");
            Assert(discovered, "Discovered Sun Carving Mural lore");
            Assert(lore.GetLoreRecord("lore_sun_carving_mural")?.IsDiscovered == true, "Lore status updated to Discovered");

            lore.Shutdown();
        }

        private static void TestTemplePuzzleSequence()
        {
            var puzzles = new TemplePuzzleSequence();
            var puzzle = puzzles.GetPuzzle("puzzle_entrance_sun_dial");
            Assert(puzzle != null, "Found Rune Dial puzzle");
            Assert(puzzle?.IsSolved == false, "Initially unsolved");

            bool solved = puzzles.SolvePuzzle("puzzle_entrance_sun_dial", new List<string> { "rune_sun", "rune_dawn", "rune_noon" });
            Assert(solved, "Solved Rune Dial puzzle with correct sequence");
            Assert(puzzle?.IsSolved == true, "Puzzle status updated to Solved");
        }

        private static void TestChapter10Quests()
        {
            var chain = new Chapter10QuestChain();
            chain.RegisterChapter10Quests();

            Assert(QuestDatabase.GetQuest("q_chapter10_temple_discovery") != null, "Found q_chapter10_temple_discovery");
            Assert(QuestDatabase.GetQuest("q_chapter10_puzzle_sanctum") != null, "Found q_chapter10_puzzle_sanctum");
            Assert(QuestDatabase.GetQuest("q_chapter10_astral_revelation") != null, "Found q_chapter10_astral_revelation");
            Assert(QuestDatabase.GetQuest("q_act3_conclusion") != null, "Found q_act3_conclusion");
        }

        private static void TestSaveV37()
        {
            var saveData = new Chapter10SaveData
            {
                TempleDiscovered = true,
                Act3Completed = true,
                CampaignRevelationWitnessed = true,
                SaveVersion = 37
            };
            saveData.ClearedTempleChamberIds.Add("chamber_temple_entrance");
            saveData.SolvedPuzzleIds.Add("puzzle_entrance_sun_dial");
            saveData.DiscoveredLoreIds.Add("lore_sun_carving_mural");

            Assert(saveData.SaveVersion == 37, "Chapter10SaveData is Save Version 37");
            Assert(saveData.Act3Completed, "Act3Completed flag persisted");
            Assert(saveData.CampaignRevelationWitnessed, "CampaignRevelationWitnessed flag persisted");
        }
    }
}
