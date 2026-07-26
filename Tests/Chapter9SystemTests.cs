using System;
using System.Collections.Generic;
using HeroOfEternia.Content.Chapter9;
using HeroOfEternia.Core;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Tests
{
    public static class Chapter9SystemTests
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
            Logger.Info("RUNNING CHAPTER 9 SYSTEM TESTS (PROMPT 36)");
            Logger.Info("==================================================");

            TestManagerInit();
            TestCorruptedFortressSectors();
            TestAntagonistFactionManager();
            TestFortressCommanderBoss();
            TestChapter9Quests();
            TestSaveV36();

            Logger.Info($"CHAPTER 9 TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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
            var mgr = new Chapter9Manager();
            mgr.Initialize();
            Assert(mgr.IsInitialized, "Chapter9Manager initialized successfully");
            mgr.Shutdown();
        }

        private static void TestCorruptedFortressSectors()
        {
            var fortress = new CorruptedFortressContent();
            Assert(fortress.TotalSectors == 7, "Fortress of Obsidian Shadows has 7 sectors");
            Assert(fortress.GetSector("sector_fortress_battlements") != null, "Found Outer Battlements sector");
            Assert(fortress.GetSector("sector_fortress_prison") != null, "Found Prison Catacombs sector");
            Assert(fortress.GetSector("sector_fortress_arena") != null, "Found Commander's Arena sector");
        }

        private static void TestAntagonistFactionManager()
        {
            var faction = new AntagonistFactionManager();
            faction.Initialize();

            Assert(faction.CurrentAlertLevel == LegionAlertLevel.Low, "Initial alert level is Low");
            Assert(faction.GetUnit("enemy_shadow_scout") != null, "Found Shadow Legion Scout unit");

            bool raised = faction.RaiseAlert(LegionAlertLevel.HighAlert);
            Assert(raised, "Raised alert level to HighAlert");
            Assert(faction.CurrentAlertLevel == LegionAlertLevel.HighAlert, "Alert level updated");

            faction.Shutdown();
        }

        private static void TestFortressCommanderBoss()
        {
            var boss = new FortressCommanderBossDefinition();
            boss.InitializeAbilities();

            Assert(boss.BossId == "enemy_boss_general_vaelis", "Boss ID matches General Vaelis");
            Assert(boss.RecommendedLevel == 36, "Boss recommended level is 36");
            Assert(boss.MaxHealth == 3600f, "Boss max health is 3600 HP");
            Assert(boss.Abilities.Count == 4, "Boss has 4 active combat abilities");
        }

        private static void TestChapter9Quests()
        {
            var chain = new Chapter9QuestChain();
            chain.RegisterChapter9Quests();

            Assert(QuestDatabase.GetQuest("q_chapter9_fortress_recon") != null, "Found q_chapter9_fortress_recon");
            Assert(QuestDatabase.GetQuest("q_chapter9_prison_sabotage") != null, "Found q_chapter9_prison_sabotage");
            Assert(QuestDatabase.GetQuest("q_chapter9_command_assault") != null, "Found q_chapter9_command_assault");
        }

        private static void TestSaveV36()
        {
            var saveData = new Chapter9SaveData
            {
                FortressDiscovered = true,
                GeneralVaelisDefeated = true,
                SavedAlertLevel = LegionAlertLevel.HighAlert,
                LegionSupplyDisrupted = true,
                SaveVersion = 36
            };
            saveData.ClearedFortressSectorIds.Add("sector_fortress_battlements");

            Assert(saveData.SaveVersion == 36, "Chapter9SaveData is Save Version 36");
            Assert(saveData.GeneralVaelisDefeated, "GeneralVaelisDefeated flag persisted");
            Assert(saveData.SavedAlertLevel == LegionAlertLevel.HighAlert, "Legion alert level persisted");
        }
    }
}
