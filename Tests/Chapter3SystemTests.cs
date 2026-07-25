using System;
using System.Collections.Generic;
using HeroOfEternia.Content.Chapter3;
using HeroOfEternia.Core;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Tests
{
    public static class Chapter3SystemTests
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
            Logger.Info("RUNNING CHAPTER 3 & ACT I FINALE TESTS (PROMPT 30)");
            Logger.Info("==================================================");

            TestManagerInit();
            TestDungeonRooms();
            TestDungeonCheckpoints();
            TestBossPhases();
            TestBossAbilities();
            TestChapter3QuestChain();
            TestFactionEscalation();
            TestWorldEvolution();
            TestActICompletion();
            TestTier2Rewards();
            TestSaveV25Integration();

            Logger.Info($"CHAPTER 3 TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
            if (_failed > 0)
            {
                foreach (var fail in _failures)
                    Logger.Error($"  [FAIL] {fail}");
            }
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
            var mgr = new Chapter3Manager();
            mgr.Initialize();
            Assert(mgr.IsInitialized, "Chapter3Manager initialized");
            Assert(mgr.Dungeon.AllRooms.Count >= 9, "Dungeon has 9 registered rooms");
            mgr.Shutdown();
        }

        private static void TestDungeonRooms()
        {
            var dungeon = new FirstDungeonContent();
            dungeon.InitializeDungeon();

            Assert(dungeon.GetRoom("room_citadel_entrance") != null, "Found Citadel Entrance room");
            Assert(dungeon.GetRoom("room_boss_arena") != null, "Found Boss Arena room");
            Assert(dungeon.GetRoom("room_floor2_secret_vault") != null, "Found Secret Vault room");
        }

        private static void TestDungeonCheckpoints()
        {
            var dungeon = new FirstDungeonContent();
            dungeon.InitializeDungeon();

            int checkpoints = 0;
            foreach (var cp in dungeon.GetCheckpoints()) checkpoints++;
            Assert(checkpoints >= 4, $"Dungeon has 4+ checkpoints (found {checkpoints})");
        }

        private static void TestBossPhases()
        {
            var boss = new RegionalBossDefinition();
            boss.RegisterAbilities();

            Assert(boss.CurrentPhase == BossPhase.Intro, "Boss starts in Intro phase");
            boss.AdvancePhase(BossPhase.PhaseOne);
            Assert(boss.CurrentPhase == BossPhase.PhaseOne, "Boss advanced to PhaseOne");
            boss.AdvancePhase(BossPhase.PhaseTwo);
            Assert(boss.CurrentPhase == BossPhase.PhaseTwo, "Boss advanced to PhaseTwo");
            boss.AdvancePhase(BossPhase.PhaseThree);
            Assert(boss.CurrentPhase == BossPhase.PhaseThree, "Boss advanced to PhaseThree");
        }

        private static void TestBossAbilities()
        {
            var boss = new RegionalBossDefinition();
            boss.RegisterAbilities();
            Assert(boss.AllAbilities.Count >= 5, "Boss has 5+ abilities registered");
        }

        private static void TestChapter3QuestChain()
        {
            var chain = new Chapter3QuestChain();
            chain.RegisterChapter3Quests();

            Assert(QuestDatabase.GetQuest("q_chapter3_preparation") != null, "Found q_chapter3_preparation");
            Assert(QuestDatabase.GetQuest("q_citadel_entry") != null, "Found q_citadel_entry");
            Assert(QuestDatabase.GetQuest("q_shadow_knight_miniboss") != null, "Found q_shadow_knight_miniboss");
            Assert(QuestDatabase.GetQuest("q_void_knight_boss") != null, "Found q_void_knight_boss");
            Assert(QuestDatabase.GetQuest("q_act1_conclusion") != null, "Found q_act1_conclusion");
        }

        private static void TestFactionEscalation()
        {
            var factions = new FactionEscalationManager();
            factions.InitializeFactions();

            var crown = factions.GetFaction("faction_valen_crown");
            Assert(crown != null, "Found faction_valen_crown");
            Assert(crown?.RelationToPlayer == FactionRelation.Allied, "Valen Crown is Allied");

            var shadowCult = factions.GetFaction("faction_shadow_cult");
            Assert(shadowCult?.RelationToPlayer == FactionRelation.AtWar, "Shadow Cult is AtWar");
        }

        private static void TestWorldEvolution()
        {
            var evo = new ActIWorldEvolution();
            evo.InitializeActIEvolution();

            Assert(evo.AllEvents.Count >= 5, "Act I has 5+ world evolution events");
            evo.UnlockEvent("evt_citadel_sealed");
            Assert(evo.GetEvent("evt_citadel_sealed")?.IsUnlocked ?? false, "evt_citadel_sealed unlocked");
        }

        private static void TestActICompletion()
        {
            var mgr = new Chapter3Manager();
            mgr.Initialize();
            mgr.OnActICompleted();

            Assert(mgr.WorldEvolution.GetEvent("evt_citadel_sealed")?.IsUnlocked ?? false,
                "Citadel sealed event fires on Act I completion");
            Assert(mgr.WorldEvolution.GetEvent("evt_tier2_merchants")?.IsUnlocked ?? false,
                "Tier 2 merchants event fires on Act I completion");

            mgr.Shutdown();
        }

        private static void TestTier2Rewards()
        {
            var rewards = new Chapter3Rewards();
            rewards.RegisterTier2Rewards();
            Assert(rewards.AllTier2Items.Count >= 4, "Registered 4+ Tier 2 reward items");
        }

        private static void TestSaveV25Integration()
        {
            var profile = new SaveProfile
            {
                Chapter3Data = new Chapter3SaveData
                {
                    IsBossDefeated = true,
                    IsActIComplete = true,
                    VarethBossPhase = BossPhase.Defeated
                }
            };
            Assert(profile.Chapter3Data != null, "SaveProfile contains Chapter3Data");
            Assert(profile.Chapter3Data.SaveVersion == 25, "SaveVersion is 25");
        }
    }
}
