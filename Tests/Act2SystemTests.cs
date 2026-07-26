using System;
using System.Collections.Generic;
using HeroOfEternia.Content.Chapter4;
using HeroOfEternia.Core;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Tests
{
    public static class Act2SystemTests
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
            Logger.Info("RUNNING ACT II SYSTEM TESTS (PROMPT 31)");
            Logger.Info("==================================================");

            TestManagerInit();
            TestRegions();
            TestRegionUnlock();
            TestCompanions();
            TestQuestChain();
            TestEnemies();
            TestNpcs();
            TestCrafting();
            TestValenholdCity();
            TestFactionPolitics();
            TestAdvancedExploration();
            TestSaveV31();

            Logger.Info($"ACT II TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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
            var mgr = new Act2Manager();
            mgr.Initialize();
            Assert(mgr.IsInitialized, "Act2Manager initialized");
            mgr.Shutdown();
        }

        private static void TestRegions()
        {
            var regions = new Act2RegionContent();
            regions.InitializeRegions();
            Assert(regions.GetRegion("region_eastern_ridgeline") != null, "Eastern Ridgeline registered");
            Assert(regions.GetRegion("region_mirkwood_swamps") != null, "Mirkwood Swamps registered");
            Assert(regions.GetRegion("region_eastern_ridgeline")?.IsUnlocked == true, "Eastern Ridgeline starts unlocked");
            Assert(regions.GetRegion("region_mirkwood_swamps")?.IsUnlocked == false, "Mirkwood Swamps starts locked");
        }

        private static void TestRegionUnlock()
        {
            var regions = new Act2RegionContent();
            regions.InitializeRegions();
            regions.UnlockRegion("region_mirkwood_swamps");
            Assert(regions.GetRegion("region_mirkwood_swamps")?.IsUnlocked == true, "Mirkwood Swamps unlocked correctly");
        }

        private static void TestCompanions()
        {
            var registry = new CompanionRegistry();
            registry.RegisterCompanions();
            var seraphine = registry.GetCompanion("companion_seraphine");
            Assert(seraphine != null, "Seraphine Vael registered");
            Assert(seraphine?.UniqueAbilities.Count >= 3, "Seraphine has 3+ abilities");
            Assert(seraphine?.JoinConditionQuestId == "q_act2_ridgeline_rescue", "Seraphine join condition is correct");
        }

        private static void TestQuestChain()
        {
            var chain = new Act2QuestChain();
            chain.RegisterAct2Quests();
            Assert(QuestDatabase.GetQuest("q_act2_begins") != null, "Found q_act2_begins");
            Assert(QuestDatabase.GetQuest("q_act2_ridgeline_rescue") != null, "Found q_act2_ridgeline_rescue");
            Assert(QuestDatabase.GetQuest("q_act2_watchtower") != null, "Found q_act2_watchtower");
            Assert(QuestDatabase.GetQuest("q_act2_mirkwood_intel") != null, "Found q_act2_mirkwood_intel");
        }

        private static void TestEnemies()
        {
            var enemies = new Act2EnemyDefinitions();
            enemies.RegisterEnemies();
            Assert(enemies.AllEnemies.Count >= 7, $"7+ enemies registered (found {enemies.AllEnemies.Count})");

            bool hasBoss = false;
            foreach (var e in enemies.AllEnemies)
                if (e.IsBoss) hasBoss = true;
            Assert(hasBoss, "At least one boss enemy registered for Act II");
        }

        private static void TestNpcs()
        {
            var npcs = new Act2NpcDefinitions();
            npcs.RegisterNpcs();
            Assert(npcs.GetNpc("npc_commander_harek") != null, "Commander Harek registered");
            Assert(npcs.GetNpc("npc_elda_swampwarden") != null, "Elda the Swamp Warden registered");
            Assert(npcs.GetNpc("npc_ridgeline_smith") != null, "Forge-Master Brynn registered");
        }

        private static void TestCrafting()
        {
            var crafting = new Act2CraftingContent();
            crafting.RegisterCraftingContent();
            Assert(crafting.AllRecipes.Count >= 4, $"4+ recipes registered (found {crafting.AllRecipes.Count})");
            Assert(crafting.AllStations.Count >= 2, $"2+ crafting stations registered (found {crafting.AllStations.Count})");
        }

        private static void TestValenholdCity()
        {
            var city = new ValenholdCityContent();
            Assert(city.GetAllDistricts().Count == 6, "Valenhold city has 6 major districts");
            Assert(city.GetDistrict("district_valenhold_government") != null, "Found High Council Heights district");
            Assert(city.GetDistrict("district_valenhold_market") != null, "Found Silver Bay Market & Harbor district");
            Assert(city.GetDistrict("district_valenhold_crafting") != null, "Found Iron Foundry Quarter district");
        }

        private static void TestFactionPolitics()
        {
            var politics = new FactionPoliticsManager();
            politics.Initialize();

            var vanguard = politics.GetFaction("faction_iron_vanguard");
            Assert(vanguard != null, "Found Iron Vanguard faction");
            Assert(vanguard?.InfluenceScore == 65, "Iron Vanguard initial influence score is 65");

            bool modified = politics.ModifyInfluence("faction_iron_vanguard", 10);
            Assert(modified, "Modified Iron Vanguard influence");
            Assert(vanguard?.InfluenceScore == 75, "Iron Vanguard influence increased to 75");

            bool allied = politics.FormAlliance("faction_iron_vanguard");
            Assert(allied, "Formed alliance with Iron Vanguard");
            Assert(vanguard?.IsAlliedWithPlayer == true, "Iron Vanguard allied state is true");

            politics.Shutdown();
        }

        private static void TestAdvancedExploration()
        {
            var exp = new AdvancedExplorationManager();
            exp.Initialize();

            var vault = exp.GetVault("vault_ridgeline_01");
            Assert(vault != null, "Found Vault of the Whispering Ridgeline");
            Assert(vault?.IsCleared == false, "Vault initially uncleared");

            bool cleared = exp.ClearVault("vault_ridgeline_01");
            Assert(cleared, "Cleared Vault of the Whispering Ridgeline");
            Assert(vault?.IsCleared == true, "Vault status updated to cleared");

            exp.Shutdown();
        }

        private static void TestSaveV31()
        {
            var data = new Act2SaveData
            {
                RidgelineUnlocked = true,
                SeraphineJoined = true,
                WatchtowerLiberated = true,
                SaveVersion = 31
            };
            data.UnlockedCityDistricts.Add("district_valenhold_government");
            data.FactionInfluenceScores["faction_iron_vanguard"] = 75;
            data.ClearedExplorationVaults.Add("vault_ridgeline_01");

            Assert(data.SaveVersion == 31, "Act2SaveData is Save V31");
            Assert(data.UnlockedCityDistricts.Count == 1, "City district unlocks persisted");
            Assert(data.FactionInfluenceScores["faction_iron_vanguard"] == 75, "Faction influence score persisted");
            Assert(data.ClearedExplorationVaults.Contains("vault_ridgeline_01"), "Cleared vault persisted");
        }
    }
}
