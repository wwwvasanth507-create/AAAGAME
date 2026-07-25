using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Settlement;
using HeroOfEternia.Core;

namespace HeroOfEternia.Tests
{
    /// <summary>
    /// Settlement system test suite.
    /// Tests settlement loading, NPC schedules, service availability,
    /// building activation, world event triggering, save/load, streaming, and stress.
    /// </summary>
    public static class SettlementSystemTests
    {
        private static int _passed = 0;
        private static int _failed = 0;
        private static readonly List<string> _failures = new();

        public static int RunAll()
        {
            Logger.Info("===== SETTLEMENT SYSTEM TESTS =====");
            _passed = 0;
            _failed = 0;
            _failures.Clear();

            // ============================================
            // TASK 1: Settlement Database Tests
            // ============================================
            Test("S1 Database: Load settlements", () =>
            {
                var db = new SettlementDatabase();
                db.Load();
                Assert(db.SettlementCount > 0, $"Expected settlements > 0, got {db.SettlementCount}");
            });

            Test("S2 Database: Get settlement by ID", () =>
            {
                var db = new SettlementDatabase();
                db.Load();
                var settlement = db.GetSettlement("village_harmony");
                Assert(settlement != null, "Expected settlement 'village_harmony'");
                Assert(settlement?.DisplayName == "Harmony Village", $"Expected 'Harmony Village', got '{settlement?.DisplayName}'");
            });

            Test("S3 Database: Get settlements by type", () =>
            {
                var db = new SettlementDatabase();
                db.Load();
                var towns = db.GetSettlementsByType(SettlementType.Town);
                Assert(towns.Count > 0, $"Expected towns > 0, got {towns.Count}");
                Assert(towns.Any(s => s.SettlementId == "town_haven"), "Expected 'town_haven' in towns");
            });

            Test("S4 Database: Search settlements", () =>
            {
                var db = new SettlementDatabase();
                db.Load();
                var results = db.SearchSettlements("Harmony");
                Assert(results.Count > 0, $"Expected search results > 0, got {results.Count}");
            });

            Test("S5 Database: Load type definitions", () =>
            {
                var db = new SettlementDatabase();
                db.LoadTypeDefinitions();
                var defs = db.GetAllTypeDefinitions();
                Assert(defs.Count > 0, $"Expected type definitions > 0, got {defs.Count}");
                var villageDef = db.GetTypeDefinition(SettlementType.Village);
                Assert(villageDef != null, "Expected Village type definition");
                Assert(villageDef?.MinPopulation == 50, $"Expected min population 50, got {villageDef?.MinPopulation}");
            });

            Test("S6 Database: Get settlements by region", () =>
            {
                var db = new SettlementDatabase();
                db.Load();
                var valley = db.GetSettlementsByRegion("eternia_valley");
                Assert(valley.Count > 0, $"Expected valley settlements > 0, got {valley.Count}");
            });

            Test("S7 Database: Get settlements by biome", () =>
            {
                var db = new SettlementDatabase();
                db.Load();
                var forest = db.GetSettlementsByBiome("temperate_forest");
                Assert(forest.Count > 0, $"Expected forest settlements > 0, got {forest.Count}");
            });

            // ============================================
            // TASK 2: Building Database Tests
            // ============================================
            Test("S8 Building: Load buildings", () =>
            {
                var db = new BuildingDatabase();
                db.Load();
                Assert(db.BuildingCount > 0, $"Expected buildings > 0, got {db.BuildingCount}");
            });

            Test("S9 Building: Get building by ID", () =>
            {
                var db = new BuildingDatabase();
                db.Load();
                var inn = db.GetBuilding("inn_01");
                Assert(inn != null, "Expected building 'inn_01'");
                Assert(inn?.Services.Contains(ServiceType.InnRest), "Expected inn to provide InnRest service");
            });

            Test("S10 Building: Get buildings by category", () =>
            {
                var db = new BuildingDatabase();
                db.Load();
                var residential = db.GetBuildingsByCategory(BuildingCategory.Residential);
                Assert(residential.Count > 0, $"Expected residential buildings > 0, got {residential.Count}");
            });

            Test("S11 Building: Get buildings by service", () =>
            {
                var db = new BuildingDatabase();
                db.Load();
                var healing = db.GetBuildingsByService(ServiceType.Healing);
                Assert(healing.Count > 0, $"Expected healing buildings > 0, got {healing.Count}");
            });

            Test("S12 Building: Get buildings for settlement type", () =>
            {
                var db = new BuildingDatabase();
                db.Load();
                var villageBuildings = db.GetBuildingsForSettlementType(SettlementType.Village);
                Assert(villageBuildings.Count > 0, $"Expected village buildings > 0, got {villageBuildings.Count}");
            });

            Test("S13 Building: Get default buildings", () =>
            {
                var db = new BuildingDatabase();
                db.Load();
                var defaults = db.GetDefaultBuildings();
                Assert(defaults.Count > 0, $"Expected default buildings > 0, got {defaults.Count}");
            });

            // ============================================
            // TASK 3: NPC Schedule Tests
            // ============================================
            Test("S14 Schedule: Load default schedules", () =>
            {
                var scheduler = new NpcScheduleExpanded();
                scheduler.LoadDefaultSchedules();
                Assert(scheduler.GetDefaultSchedule() != null, "Expected default schedule");
                var merchantSchedules = scheduler.GetSchedulesForProfession(NpcProfession.Merchant);
                Assert(merchantSchedules.Count > 0, $"Expected merchant schedules > 0, got {merchantSchedules.Count}");
            });

            Test("S15 Schedule: Get active block for time of day", () =>
            {
                var scheduler = new NpcScheduleExpanded();
                scheduler.LoadDefaultSchedules();
                var civilianSchedule = scheduler.GetDefaultSchedule();
                Assert(civilianSchedule != null, "Expected civilian schedule");

                var earlyBlock = scheduler.GetActiveBlock(civilianSchedule!, 0.1);
                Assert(earlyBlock != null, "Expected active block at time 0.1");
                Assert(earlyBlock?.Activity == ScheduleActivity.Sleep, $"Expected Sleep at 0.1, got {earlyBlock?.Activity}");
            });

            Test("S16 Schedule: Weather adaptation (storm)", () =>
            {
                var scheduler = new NpcScheduleExpanded();
                scheduler.LoadDefaultSchedules();
                scheduler.SetCurrentWeather(WeatherCondition.Storm);

                var civilianSchedule = scheduler.GetDefaultSchedule();
                Assert(civilianSchedule != null, "Expected civilian schedule");

                // During storm, NPCs should stay indoors
                var block = scheduler.GetActiveBlock(civilianSchedule!, 0.5);
                Assert(block != null, "Expected active block during storm");
                Assert(block?.LocationTag == "home", $"Expected 'home' during storm, got '{block?.LocationTag}'");
            });

            Test("S17 Schedule: Festival override", () =>
            {
                var scheduler = new NpcScheduleExpanded();
                scheduler.LoadDefaultSchedules();
                scheduler.SetFestivalOverride(true);

                var merchantSchedule = scheduler.GetSchedulesForProfession(NpcProfession.Merchant);
                Assert(merchantSchedule.Count > 0, "Expected merchant schedules");

                var block = scheduler.GetActiveBlock(merchantSchedule[0], 0.5);
                Assert(block != null, "Expected active block during festival");
                Assert(block?.Activity == ScheduleActivity.Festival, $"Expected Festival activity, got {block?.Activity}");
            });

            Test("S18 Schedule: Emergency override (monster alert)", () =>
            {
                var scheduler = new NpcScheduleExpanded();
                scheduler.LoadDefaultSchedules();
                scheduler.SetEmergencyOverride(true);

                var guardSchedule = scheduler.GetSchedulesForProfession(NpcProfession.Guard);
                Assert(guardSchedule.Count > 0, "Expected guard schedules");

                var block = scheduler.GetActiveBlock(guardSchedule[0], 0.5);
                Assert(block != null, "Expected active block during emergency");
                Assert(block?.Activity == ScheduleActivity.Emergency, $"Expected Emergency during monster alert, got {block?.Activity}");
            });

            Test("S19 Schedule: Location tag resolution", () =>
            {
                var scheduler = new NpcScheduleExpanded();
                var buildings = new List<string> { "farm_01", "inn_01", "blacksmith_01" };

                string farmLoc = scheduler.ResolveLocationTag("workplace", NpcProfession.Farmer, buildings);
                Assert(farmLoc == "farm", $"Expected 'farm', got '{farmLoc}'");

                string blacksmithLoc = scheduler.ResolveLocationTag("workplace", NpcProfession.Blacksmith, buildings);
                Assert(blacksmithLoc == "blacksmith", $"Expected 'blacksmith', got '{blacksmithLoc}'");

                string innLoc = scheduler.ResolveLocationTag("inn", NpcProfession.Merchant, buildings);
                Assert(innLoc == "inn_01", $"Expected 'inn_01', got '{innLoc}'");
            });

            // ============================================
            // TASK 4: World Event Tests
            // ============================================
            Test("S20 Event: Load templates", () =>
            {
                var events = new WorldEventFramework();
                events.Load();
                var templates = events.GetAllTemplates();
                Assert(templates.Count > 0, $"Expected templates > 0, got {templates.Count}");
            });

            Test("S21 Event: Trigger event", () =>
            {
                var db = new SettlementDatabase();
                db.Load();
                var settlement = db.GetSettlement("village_harmony");
                Assert(settlement != null, "Expected settlement");

                var events = new WorldEventFramework();
                events.Load();
                var instance = events.TriggerEvent(WorldEventType.MarketDay, "village_harmony", settlement!);
                Assert(instance != null, "Expected event instance");
                Assert(instance?.Type == WorldEventType.MarketDay, $"Expected MarketDay, got {instance?.Type}");
                Assert(instance?.Phase == EventPhase.Active, $"Expected Active phase, got {instance?.Phase}");
            });

            Test("S22 Event: Daily update resolves events", () =>
            {
                var db = new SettlementDatabase();
                db.Load();
                var settlement = db.GetSettlement("village_harmony");
                Assert(settlement != null, "Expected settlement");

                var events = new WorldEventFramework();
                events.Load();
                events.TriggerEvent(WorldEventType.MarketDay, "village_harmony", settlement!);
                Assert(events.ActiveEventCount > 0, $"Expected active events > 0, got {events.ActiveEventCount}");

                for (int i = 0; i < 10; i++) events.DailyUpdate();
                Assert(events.ActiveEventCount == 0, $"Expected 0 active events, got {events.ActiveEventCount}");
            });

            Test("S23 Event: Random event trigger", () =>
            {
                var db = new SettlementDatabase();
                db.Load();
                var settlement = db.GetSettlement("village_harmony");
                Assert(settlement != null, "Expected settlement");

                var events = new WorldEventFramework();
                events.Load();

                bool triggered = false;
                for (int i = 0; i < 100; i++)
                {
                    var instance = events.TryTriggerRandomEvent("village_harmony", settlement!);
                    if (instance != null) { triggered = true; break; }
                }
                Assert(triggered, "Expected at least one random event trigger in 100 attempts");
            });

            Test("S24 Event: Cooldown enforcement", () =>
            {
                var db = new SettlementDatabase();
                db.Load();
                var settlement = db.GetSettlement("village_harmony");
                Assert(settlement != null, "Expected settlement");

                var events = new WorldEventFramework();
                events.Load();

                var first = events.TriggerEvent(WorldEventType.MarketDay, "village_harmony", settlement!);
                Assert(first != null, "Expected first trigger to succeed");

                var second = events.TriggerEvent(WorldEventType.MarketDay, "village_harmony", settlement!);
                Assert(second == null, "Expected second trigger to fail due to cooldown");
            });

            Test("S25 Event: Save/restore state", () =>
            {
                var db = new SettlementDatabase();
                db.Load();
                var settlement = db.GetSettlement("village_harmony");
                Assert(settlement != null, "Expected settlement");

                var events = new WorldEventFramework();
                events.Load();
                events.TriggerEvent(WorldEventType.Festival, "village_harmony", settlement!);

                var saveState = events.GetSaveState();
                Assert(saveState.Count > 0, $"Expected save state count > 0, got {saveState.Count}");

                var events2 = new WorldEventFramework();
                events2.Load();
                events2.RestoreSaveState(saveState);
                Assert(events2.ActiveEventCount > 0, $"Expected restored events > 0, got {events2.ActiveEventCount}");
            });

            // ============================================
            // TASK 5: Settlement Manager Tests
            // ============================================
            Test("S26 Manager: Initialize", () =>
            {
                var manager = new SettlementManager();
                manager.Initialize();
                Assert(manager.IsInitialized, "Expected manager to be initialized");
                Assert(manager.TotalSettlements > 0, $"Expected settlements > 0, got {manager.TotalSettlements}");
            });

            Test("S27 Manager: Load and unload settlement", () =>
            {
                var manager = new SettlementManager();
                manager.Initialize();

                manager.LoadSettlement("village_harmony");
                Assert(manager.IsSettlementLoaded("village_harmony"), "Expected settlement to be loaded");

                manager.UnloadSettlement("village_harmony");
                Assert(!manager.IsSettlementLoaded("village_harmony"), "Expected settlement to be unloaded");
            });

            Test("S28 Manager: NPC spawning", () =>
            {
                var manager = new SettlementManager();
                manager.Initialize();
                manager.LoadSettlement("village_harmony");

                int npcCount = manager.GetActiveNpcCount("village_harmony");
                Assert(npcCount > 0, $"Expected NPCs > 0, got {npcCount}");
            });

            Test("S29 Manager: Get available services", () =>
            {
                var manager = new SettlementManager();
                manager.Initialize();

                var services = manager.GetAvailableServices("village_harmony");
                Assert(services.Count > 0, $"Expected services > 0, got {services.Count}");
                Assert(services.Contains(ServiceType.InnRest), "Expected InnRest service in village");
            });

            Test("S30 Manager: Get settlement buildings", () =>
            {
                var manager = new SettlementManager();
                manager.Initialize();

                var buildings = manager.GetSettlementBuildings("village_harmony");
                Assert(buildings.Count > 0, $"Expected buildings > 0, got {buildings.Count}");

                var (data, state) = buildings[0];
                Assert(data != null, "Expected building data");
                Assert(state != null, "Expected building state");
            });

            Test("S31 Manager: Building upgrade", () =>
            {
                var manager = new SettlementManager();
                manager.Initialize();

                bool upgraded = manager.UpgradeBuilding("village_harmony", "inn_01");
                Assert(upgraded, "Expected upgrade to succeed");

                var state = manager.GetBuildingState("village_harmony", "inn_01");
                Assert(state?.UpgradeLevel == 1, $"Expected upgrade level 1, got {state?.UpgradeLevel}");
            });

            Test("S32 Manager: Handle emergency", () =>
            {
                var manager = new SettlementManager();
                manager.Initialize();

                manager.HandleEmergency("village_harmony", WorldEventType.MonsterAlert);
                Assert(manager.ActiveEventCount > 0, "Expected active events after emergency");

                manager.ResolveEmergency("village_harmony");
            });

            Test("S33 Manager: Save/restore state", () =>
            {
                var manager = new SettlementManager();
                manager.Initialize();
                manager.LoadSettlement("village_harmony");

                var saveState = manager.GetSaveState();
                Assert(saveState != null, "Expected save state");
                Assert(saveState.Settlements.Count > 0, $"Expected settlements in save > 0, got {saveState.Settlements.Count}");

                var manager2 = new SettlementManager();
                manager2.Initialize();
                manager2.RestoreSaveState(saveState);

                var restoredPop = manager2.GetSettlementPopulation("village_harmony");
                var originalPop = manager.GetSettlementPopulation("village_harmony");
                Assert(restoredPop == originalPop, $"Expected population {originalPop}, got {restoredPop}");
            });

            // ============================================
            // TASK 6: Integration & Stress Tests
            // ============================================
            Test("S34 Integration: Database cross-reference", () =>
            {
                var settlementDb = new SettlementDatabase();
                settlementDb.Load();
                var buildingDb = new BuildingDatabase();
                buildingDb.Load();

                var settlement = settlementDb.GetSettlement("city_eternia");
                Assert(settlement != null, "Expected city_eternia");

                foreach (var buildingId in settlement!.BuildingIds)
                {
                    var building = buildingDb.GetBuilding(buildingId);
                    Assert(building != null, $"Expected building '{buildingId}' to exist in database");
                }
            });

            Test("S35 Stress: Database load performance", () =>
            {
                var db = new SettlementDatabase();
                var startTime = DateTime.UtcNow;
                db.Load();
                var loadTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
                Assert(loadTime < 500, $"Load time {loadTime:F1}ms exceeded 500ms limit");
            });

            Test("S36 Stress: Building lookup performance", () =>
            {
                var db = new BuildingDatabase();
                db.Load();
                var startTime = DateTime.UtcNow;

                for (int i = 0; i < 1000; i++)
                {
                    db.GetBuilding("inn_01");
                    db.GetBuildingsByCategory(BuildingCategory.Commercial);
                    db.GetBuildingsByService(ServiceType.Healing);
                }

                var lookupTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
                Assert(lookupTime < 500, $"1000 lookups took {lookupTime:F1}ms, exceeded 500ms limit");
            });

            Test("S37 Stress: Schedule resolution performance", () =>
            {
                var scheduler = new NpcScheduleExpanded();
                scheduler.LoadDefaultSchedules();
                var civilianSchedule = scheduler.GetDefaultSchedule();
                Assert(civilianSchedule != null, "Expected civilian schedule");

                var startTime = DateTime.UtcNow;
                for (int i = 0; i < 10000; i++)
                {
                    double time = (i % 1000) / 1000.0;
                    scheduler.GetActiveBlock(civilianSchedule!, time);
                }

                var resolveTime = (DateTime.UtcNow - startTime).TotalMilliseconds;
                Assert(resolveTime < 500, $"10000 schedule resolutions took {resolveTime:F1}ms, exceeded 500ms limit");
            });

            Test("S38 Integration: Settlement types hierarchy", () =>
            {
                var db = new SettlementDatabase();
                db.LoadTypeDefinitions();

                var camp = db.GetTypeDefinition(SettlementType.Camp);
                var village = db.GetTypeDefinition(SettlementType.Village);
                var city = db.GetTypeDefinition(SettlementType.City);
                var capital = db.GetTypeDefinition(SettlementType.Capital);

                Assert(camp?.MinPopulation < village?.MinPopulation, "Camp min pop should be less than village");
                Assert(village?.MinPopulation < city?.MinPopulation, "Village min pop should be less than city");
                Assert(city?.MinPopulation < capital?.MinPopulation, "City min pop should be less than capital");
            });

            // ============================================
            // RESULTS
            // ============================================
            int total = _passed + _failed;
            Logger.Info($"===== SETTLEMENT TESTS COMPLETE: {_passed}/{total} passed, {_failed} failed =====");
            if (_failures.Count > 0)
            {
                Logger.Warning("Failures:");
                foreach (var f in _failures) Logger.Warning($"  - {f}");
            }
            return _failed;
        }

        private static void Test(string name, Action action)
        {
            try
            {
                action();
                _passed++;
                Logger.Info($"  [PASS] {name}");
            }
            catch (Exception ex)
            {
                _failed++;
                _failures.Add($"{name}: {ex.Message}");
                Logger.Error($"  [FAIL] {name}: {ex.Message}");
            }
        }

        private static void Assert(bool condition, string message)
        {
            if (!condition) throw new Exception($"Assertion failed: {message}");
        }
    }
}