using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;
using HeroOfEternia.World.Content;

namespace HeroOfEternia.Tests
{
    public static class WorldContentSystemTests
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
            Logger.Info("RUNNING WORLD CONTENT SYSTEM TESTS (PROMPT 24)");
            Logger.Info("==================================================");

            TestWorldContentManagerInit();
            TestPOIDatabaseQueries();
            TestWorldGenerationRulesValidation();
            TestLandmarkDatabase();
            TestDungeonFrameworkRoomGraph();
            TestExplorationTracking();
            TestSeededDecorationGeneration();
            TestRegionalVariation();
            TestSeedReproducibility();
            TestSaveV19Integration();

            Logger.Info($"WORLD CONTENT TESTS COMPLETED: {_passed} Passed, {_failed} Failed.");
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

        private static void TestWorldContentManagerInit()
        {
            var mgr = new WorldContentManager();
            mgr.Initialize();

            var context = new PlacementValidationContext
            {
                TargetPosition = new Vector3(500, 10, 500),
                BiomeName = "Forest",
                SlopeAngleDegrees = 5f,
                DistanceToNearestSettlement = 300f
            };

            bool spawned = mgr.TrySpawnPOI("ruins_ancient_watchtower", context, out var spawn);
            Assert(spawned && spawn != null, "Spawned ruins_ancient_watchtower successfully");

            mgr.Shutdown();
        }

        private static void TestPOIDatabaseQueries()
        {
            var db = new PointOfInterestDatabase();
            db.RegisterDefaultPOIs();

            var forestPois = db.GetPOIsByBiome("Forest");
            Assert(forestPois.Count >= 2, "Forest biome contains registered POIs");

            var watchtowers = db.GetPOIsByType(POIType.Watchtower);
            Assert(watchtowers.Count >= 1, "Found Watchtower POI type");
        }

        private static void TestWorldGenerationRulesValidation()
        {
            var rules = new WorldGenerationRules { MaxAllowedSlopeDegrees = 20f };
            var poi = new POIDefinition { PoiId = "test_poi", MinDistanceToSettlement = 100f };

            var invalidContext = new PlacementValidationContext
            {
                TargetPosition = Vector3.Zero,
                SlopeAngleDegrees = 30f, // Steep slope
                DistanceToNearestSettlement = 200f
            };

            bool isValid = rules.ValidatePOIPosition(poi, invalidContext, new List<POISpawnInstance>());
            Assert(!isValid, "POI placement rejected on steep slope (>20 deg)");
        }

        private static void TestLandmarkDatabase()
        {
            var db = new LandmarkDatabase();
            db.RegisterDefaultLandmarks();

            var lm = db.GetLandmark("lm_titan_spire");
            Assert(lm != null, "Landmark lm_titan_spire registered");
            Assert(lm?.Category == LandmarkCategory.Major, "Landmark category is Major");
        }

        private static void TestDungeonFrameworkRoomGraph()
        {
            var dung = new DungeonFramework();
            var def = new DungeonDefinition
            {
                DungeonId = "dung_crypt_01",
                DisplayName = "Forgotten Crypt",
                Type = DungeonType.Crypt,
                DifficultyRating = 2
            };

            def.RoomGraph.Add(new DungeonRoomNode { RoomType = "Entrance" });
            def.RoomGraph.Add(new DungeonRoomNode { RoomType = "BossChamber", IsBossRoom = true });
            dung.RegisterDungeon(def);

            Assert(dung.GetDungeon("dung_crypt_01") != null, "Registered dung_crypt_01");
            dung.MarkDungeonCleared("dung_crypt_01");
            Assert(dung.GetDungeon("dung_crypt_01")?.IsCleared ?? false, "Marked dungeon cleared");
        }

        private static void TestExplorationTracking()
        {
            var exp = new ExplorationManager();
            bool eventTriggered = false;

            exp.OnLocationDiscovered += (evt) =>
            {
                eventTriggered = true;
                Assert(evt.LocationId == "loc_whispering_falls", "LocationId matches");
            };

            bool discovered = exp.DiscoverLocation("loc_whispering_falls", "Whispering Falls", Vector3.Zero);
            Assert(discovered, "Discovered loc_whispering_falls");
            Assert(eventTriggered, "Discovered event dispatched");

            bool repeat = exp.DiscoverLocation("loc_whispering_falls", "Whispering Falls", Vector3.Zero);
            Assert(!repeat, "Repeat discovery returned false");
        }

        private static void TestSeededDecorationGeneration()
        {
            var deco = new WorldDecorationSystem();
            var spawns1 = deco.GenerateChunkDecorations(5, 10, 42, 1.0f);
            var spawns2 = deco.GenerateChunkDecorations(5, 10, 42, 1.0f);

            Assert(spawns1.Count == spawns2.Count, "Decoration counts match across identical seed & chunk coords");
            Assert(spawns1[0].Type == spawns2[0].Type, "First decoration type matches");
        }

        private static void TestRegionalVariation()
        {
            var reg = new RegionalVariationManager();
            reg.RegisterRegion(new RegionalVariationProfile
            {
                RegionId = "dark_forest",
                VegetationDensity = 2.0f,
                FogDensityMultiplier = 1.5f
            });

            var prof = reg.GetRegionProfile("dark_forest");
            Assert(Math.Abs(prof.VegetationDensity - 2.0f) < 0.001f, "Dark forest vegetation density multiplier is 2.0");
        }

        private static void TestSeedReproducibility()
        {
            var rules = new WorldGenerationRules { WorldSeed = 12345 };
            var rnd1 = rules.GetSeededRandom(10, 20);
            var rnd2 = rules.GetSeededRandom(10, 20);

            Assert(rnd1.Next() == rnd2.Next(), "Seeded random produces identical numbers");
        }

        private static void TestSaveV19Integration()
        {
            var profile = new SaveProfile
            {
                WorldContentData = new WorldContentSaveData
                {
                    WorldSeed = 9999,
                    DiscoveredLocations = new List<string> { "loc_titan_spire" }
                }
            };

            Assert(profile.WorldContentData != null, "SaveProfile contains WorldContentData");
            Assert(profile.WorldContentData.SaveVersion == 19, "SaveVersion is 19");
        }
    }
}
