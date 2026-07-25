using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;
using HeroOfEternia.Story.Campaign;

namespace HeroOfEternia.Tests
{
    public static class CampaignDesignSystemTests
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
            Logger.Info("RUNNING CAMPAIGN DESIGN SYSTEM TESTS (PROMPT 27)");
            Logger.Info("==================================================");

            TestManagerInit();
            TestRegionDatabase12Regions();
            TestCharacterDatabaseProfiles();
            TestVillainDatabaseProfiles();
            TestCampaignOutlineStructure();
            TestSaveV22Integration();

            Logger.Info($"CAMPAIGN DESIGN TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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
            var mgr = new CampaignManager();
            mgr.Initialize();

            Assert(mgr.IsInitialized, "CampaignManager initialized");
            Assert(mgr.Regions.GetAllRegions().Count >= 12, "Registered 12+ world regions");

            mgr.Shutdown();
        }

        private static void TestRegionDatabase12Regions()
        {
            var db = new RegionDatabase();
            db.RegisterDefaultRegions();

            Assert(db.GetRegion("region_starting_kingdom") != null, "Valenoria registered");
            Assert(db.GetRegion("region_forest") != null, "Sylvanwood Wilds registered");
            Assert(db.GetRegion("region_desert") != null, "Sunfire Wastes registered");
            Assert(db.GetRegion("region_frozen_north") != null, "Frostpeak Mountains registered");
            Assert(db.GetRegion("region_swamp") != null, "Mirkwood Swamps registered");
            Assert(db.GetRegion("region_highlands") != null, "Stormrage Highlands registered");
            Assert(db.GetRegion("region_volcanic") != null, "Ashen Peaks registered");
            Assert(db.GetRegion("region_ancient_ruins") != null, "Eternian Empire Ruins registered");
            Assert(db.GetRegion("region_magical_islands") != null, "Arcane Archipelago registered");
            Assert(db.GetRegion("region_dark_wastes") != null, "Abyssal Wastes registered");
            Assert(db.GetRegion("region_sky_realm") != null, "Sky Realm (Expansion) registered");
            Assert(db.GetRegion("region_underworld") != null, "Underworld (Expansion) registered");
        }

        private static void TestCharacterDatabaseProfiles()
        {
            var db = new CharacterDatabase();
            db.RegisterDefaultCharacters();

            var hero = db.GetCharacter("char_hero_of_eternia");
            Assert(hero != null, "Found char_hero_of_eternia");
            Assert(hero?.Role == CharacterRole.Protagonist, "Role is Protagonist");
        }

        private static void TestVillainDatabaseProfiles()
        {
            var db = new VillainDatabase();
            db.RegisterDefaultVillains();

            var main = db.GetVillain("villain_malakor_voidlord");
            Assert(main != null, "Found Malakor the Void Lord");
            Assert(main?.Rank == VillainRank.PrimaryVillain, "Malakor rank is PrimaryVillain");
        }

        private static void TestCampaignOutlineStructure()
        {
            var db = new CampaignDatabase();
            db.RegisterDefaultCampaign();

            var prologue = db.GetAct("act_0_prologue");
            Assert(prologue != null, "Act 0 Prologue registered");
            Assert(db.GetAllActs().Count >= 5, "Campaign consists of 5 main story acts/prologues");
        }

        private static void TestSaveV22Integration()
        {
            var profile = new SaveProfile
            {
                CampaignData = new CampaignSaveData
                {
                    ActiveActId = "act_1_shadows",
                    DiscoveredRegionIds = new List<string> { "region_starting_kingdom", "region_forest" }
                }
            };

            Assert(profile.CampaignData != null, "SaveProfile contains CampaignData");
            Assert(profile.CampaignData.SaveVersion == 22, "SaveVersion is 22");
        }
    }
}
