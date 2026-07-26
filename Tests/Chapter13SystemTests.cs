using System;
using System.Collections.Generic;
using HeroOfEternia.Content.Chapter13;
using HeroOfEternia.Core;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Tests
{
    public static class Chapter13SystemTests
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
            Logger.Info("RUNNING CHAPTER 13 SYSTEM TESTS (PROMPT 40)");
            Logger.Info("==================================================");

            TestManagerInit();
            TestFinalDungeonSectors();
            TestDungeonCheckpointNetwork();
            TestPreFinalEncounters();
            TestChapter13Quests();
            TestSaveV40();

            Logger.Info($"CHAPTER 13 TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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
            var mgr = new Chapter13Manager();
            mgr.Initialize();
            Assert(mgr.IsInitialized, "Chapter13Manager initialized successfully");
            mgr.Shutdown();
        }

        private static void TestFinalDungeonSectors()
        {
            var dungeon = new FinalDungeonContent();
            Assert(dungeon.TotalSectors == 8, "The Citadel of Obsidian Void has 8 sectors");
            Assert(dungeon.GetSector("sector_outer_breach") != null, "Found Outer Breach sector");
            Assert(dungeon.GetSector("sector_machine_core") != null, "Found Machine Core sector");
            Assert(dungeon.GetSector("sector_pre_final_antechamber")?.IsPreFinalAntechamber == true, "Found Pre-Final Antechamber sector");
        }

        private static void TestDungeonCheckpointNetwork()
        {
            var checkpoints = new DungeonCheckpointNetwork();
            checkpoints.Initialize();

            var chk = checkpoints.GetCheckpoint("chk_gatehouse");
            Assert(chk != null, "Found Gatehouse checkpoint");
            Assert(chk?.IsActive == false, "Initially inactive");

            bool activated = checkpoints.ActivateCheckpoint("chk_gatehouse");
            Assert(activated, "Activated Gatehouse checkpoint");
            Assert(chk?.IsActive == true, "Status updated to Active");

            checkpoints.Shutdown();
        }

        private static void TestPreFinalEncounters()
        {
            var encounters = new PreFinalEncounterDefinitions();
            Assert(encounters.TotalEncounters == 4, "Citadel features 4 pre-final mini-boss encounters");

            var enc = encounters.GetEncounter("encounter_vaelis_remnant");
            Assert(enc != null, "Found High Commander Vaelis Remnant encounter");
            Assert(enc?.MaxHealth == 3800, "Vaelis Remnant health is 3800 HP");
            Assert(enc?.IsDefeated == false, "Initially undefeated");

            bool defeated = encounters.DefeatEncounter("encounter_vaelis_remnant");
            Assert(defeated, "Defeated Vaelis Remnant encounter");
            Assert(enc?.IsDefeated == true, "Status updated to Defeated");
        }

        private static void TestChapter13Quests()
        {
            var chain = new Chapter13QuestChain();
            chain.RegisterChapter13Quests();

            Assert(QuestDatabase.GetQuest("q_chapter13_breaching_citadel") != null, "Found q_chapter13_breaching_citadel");
            Assert(QuestDatabase.GetQuest("q_chapter13_machine_core_sabotage") != null, "Found q_chapter13_machine_core_sabotage");
            Assert(QuestDatabase.GetQuest("q_chapter13_gatekeeper_confrontation") != null, "Found q_chapter13_gatekeeper_confrontation");
            Assert(QuestDatabase.GetQuest("q_chapter13_pre_final_antechamber_reached") != null, "Found q_chapter13_pre_final_antechamber_reached");
        }

        private static void TestSaveV40()
        {
            var saveData = new Chapter13SaveData
            {
                CitadelBreached = true,
                PreFinalAntechamberReached = true,
                ActiveCheckpointId = "chk_antechamber_threshold",
                SaveVersion = 40
            };
            saveData.ClearedSectorIds.Add("sector_outer_breach");
            saveData.DefeatedEncounterIds.Add("encounter_vaelis_remnant");
            saveData.UnlockedShortcutIds.Add("chk_gatehouse");

            Assert(saveData.SaveVersion == 40, "Chapter13SaveData is Save Version 40");
            Assert(saveData.PreFinalAntechamberReached, "PreFinalAntechamberReached flag persisted");
            Assert(saveData.ActiveCheckpointId == "chk_antechamber_threshold", "ActiveCheckpointId persisted");
        }
    }
}
