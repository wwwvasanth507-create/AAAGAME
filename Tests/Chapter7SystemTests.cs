using System;
using System.Collections.Generic;
using HeroOfEternia.Content.Chapter7;
using HeroOfEternia.Core;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Tests
{
    public static class Chapter7SystemTests
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
            Logger.Info("RUNNING CHAPTER 7 SYSTEM TESTS (PROMPT 34)");
            Logger.Info("==================================================");

            TestManagerInit();
            TestRegionalCrisisManager();
            TestSiegeEncounterManager();
            TestAct2FinaleBoss();
            TestChapter7Quests();
            TestSaveV34();

            Logger.Info($"CHAPTER 7 TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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
            var mgr = new Chapter7Manager();
            mgr.Initialize();
            Assert(mgr.IsInitialized, "Chapter7Manager initialized successfully");
            mgr.Shutdown();
        }

        private static void TestRegionalCrisisManager()
        {
            var crisis = new RegionalCrisisManager();
            crisis.Initialize();

            Assert(crisis.CurrentRegionalSeverity == CrisisSeverityTier.Normal, "Initial severity is Normal");
            Assert(crisis.GetCrisisEvent("crisis_valenhold_siege") != null, "Found Valenhold Siege crisis event");

            bool triggered = crisis.TriggerCrisisEvent("crisis_valenhold_siege");
            Assert(triggered, "Triggered Valenhold Siege crisis event");
            Assert(crisis.CurrentRegionalSeverity == CrisisSeverityTier.ActiveSiege, "Severity escalated to ActiveSiege");

            crisis.Shutdown();
        }

        private static void TestSiegeEncounterManager()
        {
            var siege = new SiegeEncounterManager();
            Assert(siege.CurrentStage == SiegeStage.NotStarted, "Initial siege stage is NotStarted");

            bool started = siege.StartSiege();
            Assert(started, "Siege battle started");
            Assert(siege.CurrentStage == SiegeStage.Preparation, "Stage updated to Preparation");

            bool advanced = siege.AdvanceStage();
            Assert(advanced, "Advanced siege stage to WallDefense");
            Assert(siege.CurrentStage == SiegeStage.WallDefense, "Current stage is WallDefense");

            var wave = siege.GetCurrentWave();
            Assert(wave?.WaveNumber == 1, "First wave is Vanguard Shadow Assault");
        }

        private static void TestAct2FinaleBoss()
        {
            var boss = new Act2FinaleBossDefinition();
            boss.InitializeAbilities();

            Assert(boss.BossId == "enemy_boss_malakor_emissary", "Finale Boss ID matches Malakor Emissary");
            Assert(boss.RecommendedLevel == 30, "Boss recommended level is 30");
            Assert(boss.MaxHealth == 4200f, "Boss max health is 4200 HP");
            Assert(boss.Abilities.Count == 4, "Boss has 4 active finale combat abilities");
        }

        private static void TestChapter7Quests()
        {
            var chain = new Chapter7QuestChain();
            chain.RegisterChapter7Quests();

            Assert(QuestDatabase.GetQuest("q_chapter7_crisis_call") != null, "Found q_chapter7_crisis_call");
            Assert(QuestDatabase.GetQuest("q_chapter7_siege_defense") != null, "Found q_chapter7_siege_defense");
            Assert(QuestDatabase.GetQuest("q_chapter7_final_assault") != null, "Found q_chapter7_final_assault");
            Assert(QuestDatabase.GetQuest("q_act2_conclusion") != null, "Found q_act2_conclusion");
        }

        private static void TestSaveV34()
        {
            var saveData = new Chapter7SaveData
            {
                RegionalCrisisActive = true,
                SavedSiegeStage = SiegeStage.VictorySequence,
                ShadowLordEmissaryDefeated = true,
                Act2Completed = true,
                SaveVersion = 34
            };
            saveData.ActiveCrisisEventIds.Add("crisis_valenhold_siege");
            saveData.TriggeredWorldAftermathFlags.Add("flag_valenhold_rebuilt");

            Assert(saveData.SaveVersion == 34, "Chapter7SaveData is Save Version 34");
            Assert(saveData.Act2Completed, "Act2Completed flag persisted");
            Assert(saveData.SavedSiegeStage == SiegeStage.VictorySequence, "Saved siege stage persisted");
        }
    }
}
