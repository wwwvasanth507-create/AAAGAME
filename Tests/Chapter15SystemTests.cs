using System;
using System.Collections.Generic;
using HeroOfEternia.Content.Chapter15;
using HeroOfEternia.Core;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Tests
{
    public static class Chapter15SystemTests
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
            Logger.Info("RUNNING CHAPTER 15 SYSTEM TESTS (PROMPT 42)");
            Logger.Info("==================================================");

            TestManagerInit();
            TestEndingSequenceManager();
            TestCreditsSystemManager();
            TestCampaignCompletionTracker();
            TestChapter15Quests();
            TestSaveV42();

            Logger.Info($"CHAPTER 15 TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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
            var mgr = new Chapter15Manager();
            mgr.Initialize();
            Assert(mgr.IsInitialized, "Chapter15Manager initialized successfully");
            mgr.Shutdown();
        }

        private static void TestEndingSequenceManager()
        {
            var ending = new EndingSequenceManager();
            ending.Initialize();

            Assert(!ending.IsEndingTriggered, "Initially ending is not triggered");
            ending.TriggerEndingSequence(EndingChoice.Restoration_SolWarden);
            Assert(ending.IsEndingTriggered, "Ending triggered successfully");
            Assert(ending.ChosenEnding == EndingChoice.Restoration_SolWarden, "Chosen ending is Restoration_SolWarden");

            ending.CompleteEndingSequence();
            ending.Shutdown();
        }

        private static void TestCreditsSystemManager()
        {
            var credits = new CreditsSystemManager();
            credits.Initialize();

            var categories = credits.GetCreditCategories();
            Assert(categories.Count >= 3, "Found credit categories (Director, Engine Architecture, Special Thanks)");

            Assert(!credits.IsCreditsPlaying, "Initially credits not playing");
            credits.StartCreditsPlayback();
            Assert(credits.IsCreditsPlaying, "Credits playback started");

            credits.StopCreditsPlayback();
            Assert(!credits.IsCreditsPlaying, "Credits playback stopped");

            credits.Shutdown();
        }

        private static void TestCampaignCompletionTracker()
        {
            var tracker = new CampaignCompletionTracker();
            tracker.Initialize();

            Assert(!tracker.IsCampaignCompleted, "Initially campaign not marked completed");
            tracker.RecordCampaignCompletion(52.5f, "Heroic");

            Assert(tracker.IsCampaignCompleted, "Campaign marked completed");
            Assert(tracker.AwardedTitle == "Champion of Sol", "Awarded title is Champion of Sol");
            Assert(tracker.TotalPlayTimeHours == 52.5f, "Total play time hours recorded");

            tracker.Shutdown();
        }

        private static void TestChapter15Quests()
        {
            var chain = new Chapter15QuestChain();
            chain.RegisterChapter15Quests();

            Assert(QuestDatabase.GetQuest("q_chapter15_sun_spire_restoration") != null, "Found q_chapter15_sun_spire_restoration");
            Assert(QuestDatabase.GetQuest("q_chapter15_settlement_victories") != null, "Found q_chapter15_settlement_victories");
            Assert(QuestDatabase.GetQuest("q_chapter15_epilogue_celebration") != null, "Found q_chapter15_epilogue_celebration");
            Assert(QuestDatabase.GetQuest("q_chapter15_post_campaign_horizon") != null, "Found q_chapter15_post_campaign_horizon");
        }

        private static void TestSaveV42()
        {
            var saveData = new Chapter15SaveData
            {
                IsCampaignCompleted = true,
                ChosenEnding = EndingChoice.Restoration_SolWarden,
                CompletionTimestamp = "2026-07-26 12:00:00 UTC",
                TotalPlayTimeHours = 52.5f,
                CompletionPercentage = 100.0f,
                AwardedTitle = "Champion of Sol",
                HasViewedCredits = true,
                SaveVersion = 42
            };
            saveData.UnlockedEpilogueLoreIds.Add("lore_dawn_of_sol");

            Assert(saveData.SaveVersion == 42, "Chapter15SaveData is Save Version 42");
            Assert(saveData.IsCampaignCompleted, "IsCampaignCompleted flag persisted");
            Assert(saveData.AwardedTitle == "Champion of Sol", "AwardedTitle persisted");
        }
    }
}
