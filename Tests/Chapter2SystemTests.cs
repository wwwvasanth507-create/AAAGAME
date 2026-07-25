using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Content.Chapter2;
using HeroOfEternia.Core;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Tests
{
    public static class Chapter2SystemTests
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
            Logger.Info("RUNNING CHAPTER 2 TESTS (PROMPT 29)");
            Logger.Info("==================================================");

            TestManagerInit();
            TestSecondRegionLocations();
            TestSecondSettlementDistricts();
            TestChapter2Npcs();
            TestChapter2QuestChain();
            TestChapter2Enemies();
            TestAdvancedRecipes();
            TestWorldEvolution();
            TestSaveV24Integration();

            Logger.Info($"CHAPTER 2 TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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

        private static void TestManagerInit()
        {
            var mgr = new Chapter2Manager();
            mgr.Initialize();

            Assert(mgr.IsInitialized, "Chapter2Manager initialized");
            Assert(mgr.RegionContent.AllLocations.Count >= 5, "Sylvanwood region location nodes created");
            Assert(mgr.SettlementContent.AllDistricts.Count >= 6, "Elderwood Grove districts created");

            mgr.Shutdown();
        }

        private static void TestSecondRegionLocations()
        {
            var region = new SecondRegionContent();
            region.InitializeSecondRegion();

            var canopy = region.GetLocation("loc_sylvanwood_canopy");
            Assert(canopy != null, "Found Sylvanwood Main Canopy location");
            Assert(region.GetLocation("loc_ancient_elven_ruins") != null, "Found Ruins of Aethelgard location");
        }

        private static void TestSecondSettlementDistricts()
        {
            var settlement = new SecondSettlementContent();
            settlement.InitializeSecondSettlement();

            var hall = settlement.GetDistrict("dist_warden_hall");
            Assert(hall != null, "Found Warden's Great Hall district");
            Assert(settlement.GetDistrict("dist_alchemy_workshop") != null, "Found Corin's Alchemy Workshop district");
        }

        private static void TestChapter2Npcs()
        {
            var npcs = new Chapter2NpcDefinitions();
            npcs.RegisterDefaultChapter2NPCs();

            var kaelen = npcs.GetNpc("npc_warden_kaelen");
            Assert(kaelen != null, "Found Warden Kaelen NPC");
            Assert(kaelen?.Profession == "Town Leader", "Kaelen profession is Town Leader");
        }

        private static void TestChapter2QuestChain()
        {
            var chain = new Chapter2QuestChain();
            chain.RegisterChapter2Quests();

            var q1 = QuestDatabase.GetQuest("q_sylvanwood_investigation");
            Assert(q1 != null, "Found quest q_sylvanwood_investigation");
            Assert(q1?.Category == QuestCategory.Main, "Category is Main");
            Assert(QuestDatabase.GetQuest("q_ruin_guardian_boss") != null, "Found quest q_ruin_guardian_boss");
        }

        private static void TestChapter2Enemies()
        {
            var enemies = new Chapter2EnemyDefinitions();
            enemies.RegisterDefaultChapter2Enemies();

            var wolf = enemies.GetEnemy("enemy_elite_wolf");
            Assert(wolf != null, "Found Sylvan Elite Wolf");

            var guardian = enemies.GetEnemy("enemy_boss_ruin_guardian");
            Assert(guardian != null, "Found Ancient Ruin Guardian boss");
            Assert(guardian?.IsBoss ?? false, "Guardian marked as boss");
        }

        private static void TestAdvancedRecipes()
        {
            var content = new Chapter2ContentAdditions();
            content.RegisterChapter2Recipes();

            Assert(content.AllRecipes.Count >= 2, "Registered 2 advanced crafting recipes");
        }

        private static void TestWorldEvolution()
        {
            var evo = new WorldEvolutionManager();
            Assert(evo.CurrentWorldPhase == WorldPhase.OakvalePeace, "Initial phase is OakvalePeace");

            evo.AdvanceWorldPhase(WorldPhase.BlightSpreading);
            Assert(evo.CurrentWorldPhase == WorldPhase.BlightSpreading, "Advanced phase to BlightSpreading");
        }

        private static void TestSaveV24Integration()
        {
            var profile = new SaveProfile
            {
                Chapter2Data = new Chapter2SaveData
                {
                    ElderwoodReputation = 150,
                    RelicEntrustedTo = "WardenKaelen",
                    ActiveWorldPhase = WorldPhase.BlightSpreading
                }
            };

            Assert(profile.Chapter2Data != null, "SaveProfile contains Chapter2Data");
            Assert(profile.Chapter2Data.SaveVersion == 24, "SaveVersion is 24");
        }
    }
}
