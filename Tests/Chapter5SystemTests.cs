using System;
using System.Collections.Generic;
using HeroOfEternia.Content.Chapter5;
using HeroOfEternia.Core;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Tests
{
    public static class Chapter5SystemTests
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
            Logger.Info("RUNNING CHAPTER 5 SYSTEM TESTS (PROMPT 32)");
            Logger.Info("==================================================");

            TestManagerInit();
            TestBranchingStoryFramework();
            TestChoiceRecording();
            TestFactionDungeonContent();
            TestChapter5Quests();
            TestWorldConsequenceManager();
            TestSaveV32();

            Logger.Info($"CHAPTER 5 TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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
            var mgr = new Chapter5Manager();
            mgr.Initialize();
            Assert(mgr.IsInitialized, "Chapter5Manager initialized successfully");
            mgr.Shutdown();
        }

        private static void TestBranchingStoryFramework()
        {
            var framework = new BranchingStoryFramework();
            Assert(framework.ActiveBranch == StoryBranchId.Undecided, "Initial branch is Undecided");

            bool selected = framework.SelectBranch(StoryBranchId.IronVanguardAlliance, "faction_iron_vanguard");
            Assert(selected, "Selected Iron Vanguard alliance branch");
            Assert(framework.ActiveBranch == StoryBranchId.IronVanguardAlliance, "Active branch updated to IronVanguardAlliance");
            Assert(framework.ChosenFactionId == "faction_iron_vanguard", "Chosen faction ID is 'faction_iron_vanguard'");
        }

        private static void TestChoiceRecording()
        {
            var framework = new BranchingStoryFramework();
            var record = new ChoiceDecisionRecord
            {
                ChoiceId = "choice_vanguard_gate_assault",
                Title = "Gate Assault Strategy",
                ChosenOption = "Frontal Battering Ram",
                FavoredFactionId = "faction_iron_vanguard",
                FactionInfluenceImpact = 15
            };

            framework.RecordChoice(record);
            Assert(framework.GetDecisionHistory().Count == 1, "Recorded choice in decision history");
            Assert(framework.GetDecisionHistory()[0].ChoiceId == "choice_vanguard_gate_assault", "ChoiceId matches");
        }

        private static void TestFactionDungeonContent()
        {
            var dungeon = new FactionDungeonContent();
            Assert(dungeon.TotalFloors == 6, "Faction Dungeon has 6 floors");
            Assert(dungeon.GetRoom("room_stronghold_vanguard_gate") != null, "Found Vanguard Assault Gate room");
            Assert(dungeon.GetRoom("room_stronghold_syndicate_tunnels") != null, "Found Syndicate Tunnels room");
            Assert(dungeon.GetRoom("room_stronghold_sylvan_sewer") != null, "Found Sylvan Sewer room");

            var floor1Rooms = dungeon.GetRoomsForFloor(1);
            Assert(floor1Rooms.Count == 3, "Floor 1 has 3 alternative entrance routes");

            var bossRoom = dungeon.GetRoom("room_stronghold_boss_arena");
            Assert(bossRoom?.FloorNumber == 6, "Boss arena is on Floor 6");
        }

        private static void TestChapter5Quests()
        {
            var chain = new Chapter5QuestChain();
            chain.RegisterChapter5Quests();

            Assert(QuestDatabase.GetQuest("q_chapter5_infiltration") != null, "Found q_chapter5_infiltration");
            Assert(QuestDatabase.GetQuest("q_chapter5_alliance_choice") != null, "Found q_chapter5_alliance_choice");
            Assert(QuestDatabase.GetQuest("q_chapter5_dungeon_climax") != null, "Found q_chapter5_dungeon_climax");
        }

        private static void TestWorldConsequenceManager()
        {
            var mgr = new WorldConsequenceManager();
            mgr.Initialize();

            var consequence = mgr.GetConsequence("consequence_vanguard_patrols");
            Assert(consequence != null, "Found consequence_vanguard_patrols");
            Assert(consequence?.IsActive == false, "Consequence initially inactive");

            bool triggered = mgr.TriggerConsequence("consequence_vanguard_patrols");
            Assert(triggered, "Triggered consequence_vanguard_patrols");
            Assert(consequence?.IsActive == true, "Consequence state updated to active");

            mgr.Shutdown();
        }

        private static void TestSaveV32()
        {
            var saveData = new Chapter5SaveData
            {
                SelectedBranch = StoryBranchId.IronVanguardAlliance,
                ChosenFactionId = "faction_iron_vanguard",
                InfiltrationCompleted = true,
                AllianceChoiceCompleted = true,
                StrongholdBossDefeated = true,
                MaxClearedFloor = 6,
                SaveVersion = 32
            };

            saveData.RecordedChoiceIds.Add("choice_vanguard_gate_assault");
            saveData.ActiveConsequenceIds.Add("consequence_vanguard_patrols");

            Assert(saveData.SaveVersion == 32, "Chapter5SaveData is Save Version 32");
            Assert(saveData.SelectedBranch == StoryBranchId.IronVanguardAlliance, "Selected branch persisted");
            Assert(saveData.MaxClearedFloor == 6, "Max cleared floor persisted");
            Assert(saveData.RecordedChoiceIds.Contains("choice_vanguard_gate_assault"), "Choice ID persisted");
        }
    }
}
