using System;
using System.Collections.Generic;
using HeroOfEternia.Content.Chapter14;
using HeroOfEternia.Core;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Tests
{
    public static class Chapter14SystemTests
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
            Logger.Info("RUNNING CHAPTER 14 SYSTEM TESTS (PROMPT 41)");
            Logger.Info("==================================================");

            TestManagerInit();
            TestFinalBossDefinition();
            TestFinalBossAIEngine();
            TestFinalBossArenaManager();
            TestChapter14Quests();
            TestSaveV41();

            Logger.Info($"CHAPTER 14 TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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
            var mgr = new Chapter14Manager();
            mgr.Initialize();
            Assert(mgr.IsInitialized, "Chapter14Manager initialized successfully");
            mgr.Shutdown();
        }

        private static void TestFinalBossDefinition()
        {
            var boss = new FinalBossDefinition();
            Assert(boss.TotalHealth == 12000, "Arch-Sorcerer Malakor total health is 12,000 HP");
            Assert(boss.GetAllPhases().Count == 4, "Final Boss encounter comprises 4 distinct combat phases");

            var p1 = boss.GetPhaseDefinition(BossPhaseType.Phase1_HighWarden);
            Assert(p1 != null && p1.HealthThresholdHP == 3000, "Phase 1 High Warden health is 3,000 HP");

            var p4 = boss.GetPhaseDefinition(BossPhaseType.Phase4_UnboundVoidCore);
            Assert(p4 != null && p4.PhaseName == "Unbound Void Core", "Phase 4 Unbound Void Core verified");
        }

        private static void TestFinalBossAIEngine()
        {
            var ai = new FinalBossAIEngine();
            ai.Initialize();

            Assert(ai.CurrentPhase == BossPhaseType.Phase1_HighWarden, "Initially in Phase 1");
            Assert(ai.CurrentPhaseHP == 3000, "Phase 1 initial HP is 3000");

            ai.ApplyDamage(3000); // Defeat Phase 1
            Assert(ai.CurrentPhase == BossPhaseType.Phase2_CorruptedWarden, "Shifted to Phase 2 Corrupted Warden after 3,000 dmg");
            Assert(ai.CurrentPhaseHP == 3000, "Phase 2 HP reset to 3000");

            ai.ApplyDamage(3000); // Defeat Phase 2
            Assert(ai.CurrentPhase == BossPhaseType.Phase3_VoidAvatar, "Shifted to Phase 3 Void Avatar");

            ai.ApplyDamage(3000); // Defeat Phase 3
            Assert(ai.CurrentPhase == BossPhaseType.Phase4_UnboundVoidCore, "Shifted to Phase 4 Unbound Void Core");

            ai.Shutdown();
        }

        private static void TestFinalBossArenaManager()
        {
            var arena = new FinalBossArenaManager();
            arena.Initialize();

            var hazard = arena.GetHazard("hazard_sun_flares");
            Assert(hazard != null, "Found Solar Beam Flares hazard");
            Assert(hazard?.IsActive == false, "Initially inactive");

            bool activated = arena.ActivateHazard("hazard_sun_flares");
            Assert(activated, "Activated Solar Beam Flares hazard");
            Assert(hazard?.IsActive == true, "Hazard status updated to Active");

            arena.Shutdown();
        }

        private static void TestChapter14Quests()
        {
            var chain = new Chapter14QuestChain();
            chain.RegisterChapter14Quests();

            Assert(QuestDatabase.GetQuest("q_chapter14_entering_throne_room") != null, "Found q_chapter14_entering_throne_room");
            Assert(QuestDatabase.GetQuest("q_chapter14_malakor_phase1_defeat") != null, "Found q_chapter14_malakor_phase1_defeat");
            Assert(QuestDatabase.GetQuest("q_chapter14_malakor_phase2_defeat") != null, "Found q_chapter14_malakor_phase2_defeat");
            Assert(QuestDatabase.GetQuest("q_chapter14_malakor_final_defeat") != null, "Found q_chapter14_malakor_final_defeat");
        }

        private static void TestSaveV41()
        {
            var saveData = new Chapter14SaveData
            {
                FinalBossEngaged = true,
                ArchSorcererMalakorDefeated = true,
                HighestPhaseReached = BossPhaseType.Phase4_UnboundVoidCore,
                SaveVersion = 41
            };
            saveData.DefeatedPhaseIds.Add("Phase1_HighWarden");
            saveData.AcquiredBossTrophyIds.Add("trophy_malakor_void_crown");

            Assert(saveData.SaveVersion == 41, "Chapter14SaveData is Save Version 41");
            Assert(saveData.ArchSorcererMalakorDefeated, "ArchSorcererMalakorDefeated flag persisted");
            Assert(saveData.HighestPhaseReached == BossPhaseType.Phase4_UnboundVoidCore, "HighestPhaseReached persisted");
        }
    }
}
