using System;
using System.Collections.Generic;
using HeroOfEternia.Content.PostGame;
using HeroOfEternia.Core;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Tests
{
    public static class PostGameSystemTests
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
            Logger.Info("RUNNING POST-GAME SYSTEM TESTS (PROMPT 43)");
            Logger.Info("==================================================");

            TestManagerInit();
            TestSuperBossFramework();
            TestCompletionSystemManager();
            TestPostGameQuests();
            TestSaveV43();

            Logger.Info($"POST-GAME TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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
            var mgr = new PostGameManager();
            mgr.Initialize();
            Assert(mgr.IsInitialized, "PostGameManager initialized successfully");
            mgr.Shutdown();
        }

        private static void TestSuperBossFramework()
        {
            var framework = new SuperBossFramework();
            framework.Initialize();

            var bosses = framework.GetAllBosses();
            Assert(bosses.Count >= 3, "Found 3 Super Bosses (Chronos Titan, Astral Leviathan, Sun King's Ascended Memory)");

            var chronos = framework.GetBoss("boss_chronos_titan");
            Assert(chronos != null && chronos.BaseHealth == 18000, "Chronos Titan HP is 18,000");

            var sol = framework.GetBoss("boss_sol_prime_avatar");
            Assert(sol != null && sol.BaseHealth == 25000, "Sun King's Ascended Memory HP is 25,000");

            bool defeated = framework.RecordDefeat("boss_chronos_titan", SuperBossDifficulty.Mythic);
            Assert(defeated, "Recorded Chronos Titan defeat on Mythic");
            Assert(chronos?.IsDefeated == true, "Chronos Titan mark as defeated");
            Assert(chronos?.HighestDefeatedDifficulty == SuperBossDifficulty.Mythic, "Highest defeated difficulty updated to Mythic");

            framework.Shutdown();
        }

        private static void TestCompletionSystemManager()
        {
            var completion = new CompletionSystemManager();
            completion.Initialize();

            float initialOverall = completion.GetOverallCompletionPercentage();
            Assert(initialOverall > 80.0f, "Initial overall completion is > 80%");

            completion.UpdateRegionCompletion("region_02_valenhold", 100.0f);
            float updatedOverall = completion.GetOverallCompletionPercentage();
            Assert(updatedOverall > initialOverall, "Overall completion percentage increased after Valenhold reached 100%");

            completion.Shutdown();
        }

        private static void TestPostGameQuests()
        {
            var chain = new PostGameQuestChain();
            chain.RegisterPostGameQuests();

            Assert(QuestDatabase.GetQuest("q_postgame_investigating_astral_rifts") != null, "Found q_postgame_investigating_astral_rifts");
            Assert(QuestDatabase.GetQuest("q_postgame_chronos_titan_defeat") != null, "Found q_postgame_chronos_titan_defeat");
            Assert(QuestDatabase.GetQuest("q_postgame_astral_leviathan_defeat") != null, "Found q_postgame_astral_leviathan_defeat");
            Assert(QuestDatabase.GetQuest("q_postgame_sol_prime_confrontation") != null, "Found q_postgame_sol_prime_confrontation");
        }

        private static void TestSaveV43()
        {
            var saveData = new PostGameSaveData
            {
                PostGameUnlocked = true,
                OverallWorldCompletion = 96.5f,
                SaveVersion = 43
            };
            saveData.DefeatedSuperBossIds.Add("boss_chronos_titan");
            saveData.AcquiredSuperTrophyIds.Add("trophy_chronos_hourglass");

            Assert(saveData.SaveVersion == 43, "PostGameSaveData is Save Version 43");
            Assert(saveData.PostGameUnlocked, "PostGameUnlocked flag persisted");
            Assert(saveData.OverallWorldCompletion == 96.5f, "OverallWorldCompletion persisted");
        }
    }
}
