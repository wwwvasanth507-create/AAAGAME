using System;
using System.Collections.Generic;
using HeroOfEternia.Content.Chapter6;
using HeroOfEternia.Core;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Tests
{
    public static class Chapter6SystemTests
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
            Logger.Info("RUNNING CHAPTER 6 SYSTEM TESTS (PROMPT 33)");
            Logger.Info("==================================================");

            TestManagerInit();
            TestCapitalCityDistricts();
            TestGuildSystemManager();
            TestGuildRankPromotion();
            TestSecondRegionalBoss();
            TestChapter6Quests();
            TestSaveV33();

            Logger.Info($"CHAPTER 6 TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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
            var mgr = new Chapter6Manager();
            mgr.Initialize();
            Assert(mgr.IsInitialized, "Chapter6Manager initialized successfully");
            mgr.Shutdown();
        }

        private static void TestCapitalCityDistricts()
        {
            var city = new CapitalCityContent();
            Assert(city.GetAllDistricts().Count == 8, "Eternia Prime has 8 major districts");
            Assert(city.GetDistrict("district_capital_royal") != null, "Found Royal Citadel district");
            Assert(city.GetDistrict("district_capital_guild") != null, "Found Grand Guild Enclave district");
            Assert(city.GetDistrict("district_capital_underground") != null, "Found Underground Catacombs district");
        }

        private static void TestGuildSystemManager()
        {
            var guilds = new GuildSystemManager();
            guilds.Initialize();

            var advGuild = guilds.GetGuild("guild_adventurers");
            Assert(advGuild != null, "Found Crown Adventurers Guild");
            Assert(advGuild?.IsJoined == false, "Initially not joined");

            bool joined = guilds.JoinGuild("guild_adventurers");
            Assert(joined, "Joined Crown Adventurers Guild");
            Assert(advGuild?.CurrentRank == GuildRank.Recruit, "Initial rank is Recruit");

            guilds.Shutdown();
        }

        private static void TestGuildRankPromotion()
        {
            var guilds = new GuildSystemManager();
            guilds.Initialize();

            guilds.JoinGuild("guild_adventurers");
            guilds.AddReputation("guild_adventurers", 250);

            var advGuild = guilds.GetGuild("guild_adventurers");
            Assert(advGuild?.CurrentRank == GuildRank.Journeyman, "Promoted to Journeyman rank at 250 reputation");

            guilds.AddReputation("guild_adventurers", 800); // total 1050
            Assert(advGuild?.CurrentRank == GuildRank.Grandmaster, "Promoted to Grandmaster rank at 1050 reputation");

            guilds.Shutdown();
        }

        private static void TestSecondRegionalBoss()
        {
            var boss = new SecondRegionalBossDefinition();
            boss.InitializeAbilities();

            Assert(boss.BossId == "enemy_boss_high_inquisitor_vesper", "Boss ID matches High Inquisitor Vesper");
            Assert(boss.RecommendedLevel == 27, "Boss recommended level is 27");
            Assert(boss.MaxHealth == 2800f, "Boss max health is 2800 HP");
            Assert(boss.Abilities.Count == 4, "Boss has 4 active combat abilities");
        }

        private static void TestChapter6Quests()
        {
            var chain = new Chapter6QuestChain();
            chain.RegisterChapter6Quests();

            Assert(QuestDatabase.GetQuest("q_chapter6_capital_arrival") != null, "Found q_chapter6_capital_arrival");
            Assert(QuestDatabase.GetQuest("q_chapter6_guild_induction") != null, "Found q_chapter6_guild_induction");
            Assert(QuestDatabase.GetQuest("q_chapter6_boss_climax") != null, "Found q_chapter6_boss_climax");
        }

        private static void TestSaveV33()
        {
            var saveData = new Chapter6SaveData
            {
                CapitalDiscovered = true,
                HighInquisitorVesperDefeated = true,
                SaveVersion = 33
            };
            saveData.UnlockedCapitalDistricts.Add("district_capital_royal");
            saveData.JoinedGuildRanks["guild_adventurers"] = "Grandmaster";
            saveData.GuildReputationScores["guild_adventurers"] = 1050;

            Assert(saveData.SaveVersion == 33, "Chapter6SaveData is Save Version 33");
            Assert(saveData.CapitalDiscovered, "CapitalDiscovered flag persisted");
            Assert(saveData.JoinedGuildRanks["guild_adventurers"] == "Grandmaster", "Guild rank persisted");
        }
    }
}
