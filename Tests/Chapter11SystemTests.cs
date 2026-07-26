using System;
using System.Collections.Generic;
using HeroOfEternia.Content.Chapter11;
using HeroOfEternia.Core;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Tests
{
    public static class Chapter11SystemTests
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
            Logger.Info("RUNNING CHAPTER 11 SYSTEM TESTS (PROMPT 38)");
            Logger.Info("==================================================");

            TestManagerInit();
            TestEndgameRegionZones();
            TestLegendaryProgressionManager();
            TestEliteWorldContentManager();
            TestChapter11Quests();
            TestSaveV38();

            Logger.Info($"CHAPTER 11 TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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
            var mgr = new Chapter11Manager();
            mgr.Initialize();
            Assert(mgr.IsInitialized, "Chapter11Manager initialized successfully");
            mgr.Shutdown();
        }

        private static void TestEndgameRegionZones()
        {
            var region = new EndgameRegionContent();
            Assert(region.TotalZones == 7, "The Astral Divide has 7 sub-zones");
            Assert(region.GetZone("zone_crystal_wasteland") != null, "Found The Crystal Wasteland zone");
            Assert(region.GetZone("zone_shattered_realm") != null, "Found Shattered Realm of Sol zone");
            Assert(region.GetZone("zone_obsidian_threshold") != null, "Found Obsidian Citadel Gate zone");
        }

        private static void TestLegendaryProgressionManager()
        {
            var legendary = new LegendaryProgressionManager();
            legendary.Initialize();

            var recipe = legendary.GetRecipe("recipe_legendary_sol_blade");
            Assert(recipe != null, "Found Tier 5 Sol Blade legendary recipe");
            Assert(recipe?.IsUnlocked == false, "Initially locked");

            bool unlocked = legendary.UnlockRecipe("recipe_legendary_sol_blade");
            Assert(unlocked, "Unlocked Sol Blade legendary recipe");
            Assert(recipe?.IsUnlocked == true, "Recipe status updated to Unlocked");

            legendary.AddMaterial("material_astral_essence", 5);
            Assert(legendary.GetMaterialQuantity("material_astral_essence") == 5, "Added 5x material_astral_essence");

            legendary.Shutdown();
        }

        private static void TestEliteWorldContentManager()
        {
            var elite = new EliteWorldContentManager();
            elite.Initialize();

            var enc = elite.GetEncounter("elite_crystal_behemoth");
            Assert(enc != null, "Found Crystal Behemoth elite encounter");
            Assert(enc?.IsCleared == false, "Initially uncleared");

            bool cleared = elite.ClearEncounter("elite_crystal_behemoth");
            Assert(cleared, "Cleared Crystal Behemoth elite encounter");
            Assert(enc?.IsCleared == true, "Encounter status updated to Cleared");

            elite.Shutdown();
        }

        private static void TestChapter11Quests()
        {
            var chain = new Chapter11QuestChain();
            chain.RegisterChapter11Quests();

            Assert(QuestDatabase.GetQuest("q_chapter11_astral_divide_entry") != null, "Found q_chapter11_astral_divide_entry");
            Assert(QuestDatabase.GetQuest("q_chapter11_legendary_research") != null, "Found q_chapter11_legendary_research");
            Assert(QuestDatabase.GetQuest("q_chapter11_elite_trial") != null, "Found q_chapter11_elite_trial");
            Assert(QuestDatabase.GetQuest("q_chapter11_astral_champion_confrontation") != null, "Found q_chapter11_astral_champion_confrontation");
        }

        private static void TestSaveV38()
        {
            var saveData = new Chapter11SaveData
            {
                Act4Started = true,
                ObsidianThresholdBreached = true,
                SaveVersion = 38
            };
            saveData.DiscoveredZoneIds.Add("zone_crystal_wasteland");
            saveData.UnlockedLegendaryRecipeIds.Add("recipe_legendary_sol_blade");
            saveData.ClearedEliteEncounterIds.Add("elite_crystal_behemoth");
            saveData.SavedLegendaryMaterials["material_astral_essence"] = 5;

            Assert(saveData.SaveVersion == 38, "Chapter11SaveData is Save Version 38");
            Assert(saveData.Act4Started, "Act4Started flag persisted");
            Assert(saveData.ObsidianThresholdBreached, "ObsidianThresholdBreached flag persisted");
        }
    }
}
