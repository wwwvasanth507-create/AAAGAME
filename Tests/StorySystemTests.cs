using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;
using HeroOfEternia.Story;

namespace HeroOfEternia.Tests
{
    public static class StorySystemTests
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
            Logger.Info("RUNNING STORY SYSTEM TESTS (PROMPT 26)");
            Logger.Info("==================================================");

            TestStoryFrameworkInit();
            TestStoryDatabaseQueries();
            TestChapterPrerequisites();
            TestWorldStateFlagsAndReversibility();
            TestCinematicTriggers();
            TestMissionFlowLifecycle();
            TestStoryEventOverrides();
            TestLoreDiscovery();
            TestSaveV21Integration();

            Logger.Info($"STORY TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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

        private static void TestStoryFrameworkInit()
        {
            var mgr = new StoryFrameworkManager();
            mgr.Initialize();

            Assert(mgr.IsInitialized, "StoryFrameworkManager is initialized");
            Assert(mgr.Progression.ActiveChapterId == "chapter_prologue", "Active chapter is chapter_prologue");

            mgr.Shutdown();
        }

        private static void TestStoryDatabaseQueries()
        {
            var db = new StoryDatabase();
            db.RegisterDefaultEntries();

            var entry = db.GetStoryEntry("story_prologue_01");
            Assert(entry != null, "Found story_prologue_01 in database");
            Assert(entry?.RecommendedLevel == 1, "Recommended level is 1");
        }

        private static void TestChapterPrerequisites()
        {
            var chapters = new ChapterFramework();
            chapters.RegisterDefaultChapters();

            bool act1UnlockedBeforePrologue = chapters.IsChapterUnlocked("chapter_act1_ch1", new List<string>());
            Assert(!act1UnlockedBeforePrologue, "Act I locked before completing prologue");

            bool act1UnlockedAfterPrologue = chapters.IsChapterUnlocked("chapter_act1_ch1", new List<string> { "chapter_prologue" });
            Assert(act1UnlockedAfterPrologue, "Act I unlocked after completing prologue");
        }

        private static void TestWorldStateFlagsAndReversibility()
        {
            var world = new WorldStateManager();
            world.SetFlag("settlement_oakvale_destroyed", "true");
            Assert(world.GetFlag("settlement_oakvale_destroyed") == "true", "World flag settlement_oakvale_destroyed is true");

            bool reverted = world.RevertLastStateChange();
            Assert(reverted, "Reverted last state change");
            Assert(string.IsNullOrEmpty(world.GetFlag("settlement_oakvale_destroyed")), "World flag reset after revert");
        }

        private static void TestCinematicTriggers()
        {
            var trig = new CinematicTriggerFramework();
            trig.RegisterTrigger(new CinematicTriggerDefinition
            {
                TriggerId = "cutscene_boss_entrance",
                ConditionType = TriggerConditionType.EnterArea,
                TargetAreaPosition = new Vector3(0, 0, 50),
                TriggerRadius = 10.0f
            });

            bool evaluatedOut = trig.EvaluateTrigger("cutscene_boss_entrance", Vector3.Zero);
            Assert(!evaluatedOut, "Cutscene not triggered outside radius");

            bool evaluatedIn = trig.EvaluateTrigger("cutscene_boss_entrance", new Vector3(0, 0, 45));
            Assert(evaluatedIn, "Cutscene triggered inside radius");
        }

        private static void TestMissionFlowLifecycle()
        {
            var flow = new MissionFlowController();
            flow.StartMission("mission_01", Vector3.Zero);
            Assert(flow.CurrentMission.State == MissionState.InProgress, "Mission state is InProgress");

            flow.SetCheckpoint(1, new Vector3(50, 0, 50));
            Assert(flow.CurrentMission.ActiveCheckpointIndex == 1, "Checkpoint index updated to 1");

            flow.CompleteMission();
            Assert(flow.CurrentMission.State == MissionState.Completed, "Mission state is Completed");
        }

        private static void TestStoryEventOverrides()
        {
            var evts = new StoryEventManager();
            evts.RegisterOverride(new StoryEventOverride
            {
                EventId = "event_blood_moon",
                LightingProfileOverride = "profile_blood_moon"
            });

            bool triggered = evts.TriggerStoryEvent("event_blood_moon");
            Assert(triggered, "Triggered story event_blood_moon");
        }

        private static void TestLoreDiscovery()
        {
            var lore = new LoreManager();
            lore.RegisterLore(new LoreDefinition
            {
                LoreId = "lore_tablet_creation",
                Title = "Tablet of Creation",
                Category = LoreCategory.StoneTablet
            });

            bool discovered = lore.DiscoverLore("lore_tablet_creation");
            Assert(discovered, "Discovered Tablet of Creation");
            Assert(lore.GetDiscoveredLore().Count == 1, "Discovered lore count is 1");
        }

        private static void TestSaveV21Integration()
        {
            var profile = new SaveProfile
            {
                StoryProgressionData = new StoryProgressionSaveData
                {
                    ActiveChapterId = "chapter_act1_ch1",
                    CompletedChapterIds = new List<string> { "chapter_prologue" }
                }
            };

            Assert(profile.StoryProgressionData != null, "SaveProfile contains StoryProgressionData");
            Assert(profile.StoryProgressionData.SaveVersion == 21, "SaveVersion is 21");
        }
    }
}
