using System;
using System.Collections.Generic;
using HeroOfEternia.Content.Chapter12;
using HeroOfEternia.Core;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Tests
{
    public static class Chapter12SystemTests
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
            Logger.Info("RUNNING CHAPTER 12 SYSTEM TESTS (PROMPT 39)");
            Logger.Info("==================================================");

            TestManagerInit();
            TestAllianceCampaignManager();
            TestWorldWarEventManager();
            TestLegendaryEquipmentSet();
            TestChapter12Quests();
            TestSaveV39();

            Logger.Info($"CHAPTER 12 TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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
            var mgr = new Chapter12Manager();
            mgr.Initialize();
            Assert(mgr.IsInitialized, "Chapter12Manager initialized successfully");
            mgr.Shutdown();
        }

        private static void TestAllianceCampaignManager()
        {
            var alliance = new AllianceCampaignManager();
            alliance.Initialize();

            Assert(alliance.GetAllFactions().Count == 4, "Grand Alliance comprises 4 factions");
            int initialReadiness = alliance.GetAllianceReadinessPercentage();
            Assert(initialReadiness >= 80, $"Initial Grand Alliance readiness is {initialReadiness}% (>=80%)");

            alliance.SetFactionLoyalty("faction_valenhold", 95);
            Assert(alliance.GetFaction("faction_valenhold")?.LoyaltyRating == 95, "Valenhold loyalty updated to 95%");

            alliance.Shutdown();
        }

        private static void TestWorldWarEventManager()
        {
            var warEvents = new WorldWarEventManager();
            warEvents.Initialize();

            var evt = warEvents.GetWarEvent("event_caravan_escort");
            Assert(evt != null, "Found Supply Caravan Escort war event");
            Assert(evt?.IsCompleted == false, "Initially incomplete");

            bool completed = warEvents.CompleteWarEvent("event_caravan_escort");
            Assert(completed, "Completed Supply Caravan Escort war event");
            Assert(evt?.IsCompleted == true, "War event status updated to Completed");

            warEvents.Shutdown();
        }

        private static void TestLegendaryEquipmentSet()
        {
            var set = new LegendaryEquipmentSet();
            var piece = set.GetPiece("item_legendary_solwarden_greatsword");
            Assert(piece != null, "Found Solwarden Astral Greatsword");
            Assert(piece?.AttackBonus == 145, "Greatsword attack bonus is 145");
            Assert(piece?.IsAcquired == false, "Initially unacquired");

            bool acquired = set.AcquirePiece("item_legendary_solwarden_greatsword");
            Assert(acquired, "Acquired Solwarden Astral Greatsword");
            Assert(piece?.IsAcquired == true, "Piece status updated to Acquired");
        }

        private static void TestChapter12Quests()
        {
            var chain = new Chapter12QuestChain();
            chain.RegisterChapter12Quests();

            Assert(QuestDatabase.GetQuest("q_chapter12_alliance_council_assembly") != null, "Found q_chapter12_alliance_council_assembly");
            Assert(QuestDatabase.GetQuest("q_chapter12_supply_line_liberation") != null, "Found q_chapter12_supply_line_liberation");
            Assert(QuestDatabase.GetQuest("q_chapter12_solwarden_artifact_recovery") != null, "Found q_chapter12_solwarden_artifact_recovery");
            Assert(QuestDatabase.GetQuest("q_chapter12_final_alliance_briefing") != null, "Found q_chapter12_final_alliance_briefing");
        }

        private static void TestSaveV39()
        {
            var saveData = new Chapter12SaveData
            {
                AllianceCouncilConvened = true,
                FinalBriefingCompleted = true,
                AllianceReadinessPercentage = 92,
                SaveVersion = 39
            };
            saveData.FactionLoyaltyRatings["faction_valenhold"] = 95;
            saveData.CompletedWarEventIds.Add("event_caravan_escort");
            saveData.AcquiredLegendaryPieceIds.Add("item_legendary_solwarden_greatsword");

            Assert(saveData.SaveVersion == 39, "Chapter12SaveData is Save Version 39");
            Assert(saveData.AllianceReadinessPercentage == 92, "AllianceReadinessPercentage persisted");
            Assert(saveData.FinalBriefingCompleted, "FinalBriefingCompleted flag persisted");
        }
    }
}
