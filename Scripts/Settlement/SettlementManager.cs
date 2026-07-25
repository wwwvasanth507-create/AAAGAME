using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;
using HeroOfEternia.Economy;
using HeroOfEternia.World;

namespace HeroOfEternia.Settlement
{
    /// <summary>
    /// Central settlement simulation manager.
    /// Orchestrates settlement loading, NPC spawning, schedule management,
    /// service availability, prosperity tracking, population simulation,
    /// world events, economy integration, and save/load.
    /// </summary>
    public class SettlementManager
    {
        // Core databases
        private readonly SettlementDatabase _settlementDatabase;
        private readonly BuildingDatabase _buildingDatabase;
        private readonly NpcScheduleExpanded _npcScheduleExpanded;
        private readonly WorldEventFramework _worldEventFramework;

        // Runtime state
        private readonly Dictionary<string, List<BuildingSaveState>> _buildingStates = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, float> _settlementProsperity = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, int> _settlementPopulations = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, float> _settlementSecurity = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<string>> _activeNpcs = new(StringComparer.OrdinalIgnoreCase); // settlementId -> npcIds
        private readonly Dictionary<string, bool> _settlementLoaded = new(StringComparer.OrdinalIgnoreCase);

        private readonly Random _rng = new();
        private bool _isInitialized = false;
        private double _lastDailyUpdate = 0f;
        private const double DailyUpdateInterval = 1.0; // 1 in-game day

        public bool IsInitialized => _isInitialized;
        public int TotalSettlements => _settlementDatabase.SettlementCount;
        public int TotalActiveNpcs => _activeNpcs.Values.Sum(list => list.Count);
        public int ActiveEventCount => _worldEventFramework.ActiveEventCount;

        public SettlementDatabase Database => _settlementDatabase;
        public BuildingDatabase Buildings => _buildingDatabase;
        public NpcScheduleExpanded Schedules => _npcScheduleExpanded;
        public WorldEventFramework Events => _worldEventFramework;

        public SettlementManager()
        {
            _settlementDatabase = new SettlementDatabase();
            _buildingDatabase = new BuildingDatabase();
            _npcScheduleExpanded = new NpcScheduleExpanded();
            _worldEventFramework = new WorldEventFramework();
        }

        /// <summary>Initialize the settlement manager and all subsystems.</summary>
        public void Initialize()
        {
            Logger.Info("SettlementManager: Initializing...");

            // Load databases
            _settlementDatabase.Load();
            _settlementDatabase.LoadTypeDefinitions();
            _buildingDatabase.Load();
            _npcScheduleExpanded.LoadDefaultSchedules();
            _worldEventFramework.Load();

            // Initialize runtime state from database
            foreach (var settlement in _settlementDatabase.GetAllSettlements())
            {
                _settlementProsperity[settlement.SettlementId] = (float)settlement.Prosperity;
                _settlementPopulations[settlement.SettlementId] = settlement.Population;
                _settlementSecurity[settlement.SettlementId] = (float)settlement.Security;
                _settlementLoaded[settlement.SettlementId] = false;
                _activeNpcs[settlement.SettlementId] = new List<string>();
                _buildingStates[settlement.SettlementId] = new List<BuildingSaveState>();

                // Initialize building states from database
                foreach (var buildingId in settlement.BuildingIds)
                {
                    var building = _buildingDatabase.GetBuilding(buildingId);
                    if (building != null)
                    {
                        _buildingStates[settlement.SettlementId].Add(new BuildingSaveState
                        {
                            BuildingId = buildingId,
                            State = BuildingState.Active,
                            UpgradeLevel = building.UpgradeLevel,
                            OwnerNpcId = building.OwnerNpcId,
                            Condition = 1.0f
                        });
                    }
                }
            }

            _isInitialized = true;
            Logger.Info($"SettlementManager: Initialized with {_settlementDatabase.SettlementCount} settlements, {_buildingDatabase.BuildingCount} buildings, {_worldEventFramework.ActiveEventCount} active events.");
        }

        /// <summary>Load a settlement into active memory for simulation.</summary>
        public void LoadSettlement(string settlementId)
        {
            var settlement = _settlementDatabase.GetSettlement(settlementId);
            if (settlement == null) return;

            _settlementLoaded[settlementId] = true;

            // Spawn initial NPCs
            SpawnNpcsForSettlement(settlementId);

            Logger.Info($"SettlementManager: Loaded settlement '{settlement.DisplayName}'.");
        }

        /// <summary>Unload a distant settlement to free resources.</summary>
        public void UnloadSettlement(string settlementId)
        {
            _settlementLoaded[settlementId] = false;
            _activeNpcs[settlementId]?.Clear();

            Logger.Info($"SettlementManager: Unloaded settlement '{settlementId}'.");
        }

        /// <summary>Check if a settlement is currently loaded.</summary>
        public bool IsSettlementLoaded(string settlementId)
        {
            return _settlementLoaded.TryGetValue(settlementId, out var loaded) && loaded;
        }

        /// <summary>Get loaded settlement IDs.</summary>
        public List<string> GetLoadedSettlementIds()
        {
            return _settlementLoaded
                .Where(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .ToList();
        }

        /// <summary>Spawn NPCs for a settlement based on spawn rules.</summary>
        public void SpawnNpcsForSettlement(string settlementId)
        {
            var settlement = _settlementDatabase.GetSettlement(settlementId);
            if (settlement == null) return;

            var rules = settlement.SpawnRules;
            int targetCount = Math.Min(rules.MaxActiveNpcs, settlement.Population);

            var npcList = _activeNpcs.GetValueOrDefault(settlementId);
            if (npcList == null)
            {
                npcList = new List<string>();
                _activeNpcs[settlementId] = npcList;
            }

            // Generate NPC population (simulated IDs for now)
            for (int i = npcList.Count; i < targetCount; i++)
            {
                string npcId = $"npc_{settlementId}_{i}";
                npcList.Add(npcId);
            }

            Logger.Info($"SettlementManager: Spawned {targetCount} NPCs for '{settlement.DisplayName}'.");
        }

        /// <summary>Get active NPC count for a settlement.</summary>
        public int GetActiveNpcCount(string settlementId)
        {
            return _activeNpcs.TryGetValue(settlementId, out var list) ? list.Count : 0;
        }

        /// <summary>
        /// Perform daily update for all loaded settlements.
        /// Updates prosperity, population, services, events, and economy.
        /// </summary>
        public void DailyUpdate(double currentTime)
        {
            if (!_isInitialized) return;

            // Check if a day has passed
            if (currentTime - _lastDailyUpdate < DailyUpdateInterval) return;
            _lastDailyUpdate = currentTime;

            foreach (var settlement in _settlementDatabase.GetAllSettlements())
            {
                if (!_settlementLoaded.GetValueOrDefault(settlement.SettlementId, false))
                    continue;

                UpdateSettlement(settlement);
            }

            // Update world events
            _worldEventFramework.DailyUpdate();

            Logger.Info($"SettlementManager: Daily update completed for loaded settlements.");
        }

        /// <summary>Update a single settlement's simulation.</summary>
        private void UpdateSettlement(SettlementData settlement)
        {
            string id = settlement.SettlementId;

            // Update prosperity
            UpdateProsperity(settlement);

            // Update population
            UpdatePopulation(settlement);

            // Update building states
            UpdateBuildings(settlement);

            // Update services
            UpdateServices(settlement);

            // Random event check
            if (_rng.NextDouble() < 0.05) // 5% chance per day
            {
                _worldEventFramework.TryTriggerRandomEvent(id, settlement);
            }

            // Economy integration via EventBus
            var eventBus = ServiceLocator.Get<EventBus>();
            eventBus?.Publish("SettlementDailyUpdate", new SettlementStateChangedEvent
            {
                SettlementId = id,
                PropertyName = "daily_update",
                OldValue = "",
                NewValue = ""
            });
        }

        /// <summary>Update settlement prosperity based on buildings, population, and events.</summary>
        private void UpdateProsperity(SettlementData settlement)
        {
            string id = settlement.SettlementId;
            float currentProsperity = _settlementProsperity.GetValueOrDefault(id, 0.5f);

            // Base prosperity from population density
            float densityRatio = (float)settlement.Population / Math.Max(1, settlement.MaxPopulation);
            float targetProsperity = Math.Min(1.0f, densityRatio * 1.5f);

            // Bonus from buildings
            float buildingBonus = 0f;
            if (_buildingStates.TryGetValue(id, out var states))
            {
                int activeBuildings = states.Count(s => s.State == BuildingState.Active);
                int totalBuildings = Math.Max(1, settlement.BuildingIds.Count);
                buildingBonus = (float)activeBuildings / totalBuildings * 0.2f;
            }

            // Event effects
            float eventModifier = 1.0f;
            var activeEvents = _worldEventFramework.GetActiveEventsForSettlement(id);
            foreach (var evt in activeEvents)
            {
                if (evt.Effects.TryGetValue("prosperity", out var prosperityEffect))
                    eventModifier += prosperityEffect;
            }

            // Blend towards target
            float newProsperity = currentProsperity + (targetProsperity + buildingBonus - currentProsperity) * 0.1f * eventModifier;
            newProsperity = Math.Clamp(newProsperity, 0f, 1f);

            // Convert to enum
            ProsperityLevel oldLevel = settlement.Prosperity;
            settlement.Prosperity = GetProsperityFromFloat(newProsperity);
            _settlementProsperity[id] = newProsperity;

            if (settlement.Prosperity != oldLevel)
            {
                var eventBus = ServiceLocator.Get<EventBus>();
                eventBus?.Publish("SettlementEconomyChanged", new Economy.SettlementEconomyChangeEvent
                {
                    SettlementId = id,
                    OldProsperity = oldLevel,
                    NewProsperity = settlement.Prosperity,
                    Event = Economy.EconomicEventType.None
                });
            }
        }

        /// <summary>Update settlement population based on prosperity and capacity.</summary>
        private void UpdatePopulation(SettlementData settlement)
        {
            string id = settlement.SettlementId;
            int currentPop = _settlementPopulations.GetValueOrDefault(id, settlement.Population);

            float prosperity = _settlementProsperity.GetValueOrDefault(id, 0.5f);

            // Population growth/decline based on prosperity
            float growthRate = (prosperity - 0.3f) * 0.05f; // Positive above 0.3 prosperity
            int change = (int)(currentPop * growthRate);

            // Clamp to bounds
            int newPop = Math.Clamp(currentPop + change, 1, settlement.MaxPopulation);
            _settlementPopulations[id] = newPop;
            settlement.Population = newPop;
        }

        /// <summary>Update building states for a settlement.</summary>
        private void UpdateBuildings(SettlementData settlement)
        {
            if (!_buildingStates.TryGetValue(settlement.SettlementId, out var states)) return;

            foreach (var state in states)
            {
                var building = _buildingDatabase.GetBuilding(state.BuildingId);
                if (building == null) continue;

                // Apply daily maintenance
                if (state.State == BuildingState.Active)
                {
                    state.Condition = Math.Max(0f, state.Condition - 0.001f); // Minor wear

                    // Check if building should be open based on time (simplified)
                    // Revenue collection would happen here in full implementation
                }
            }
        }

        /// <summary>Update available services for a settlement.</summary>
        private void UpdateServices(SettlementData settlement)
        {
            // Recalculate available services from active buildings
            var availableServices = new List<ServiceType>();
            if (_buildingStates.TryGetValue(settlement.SettlementId, out var states))
            {
                foreach (var state in states)
                {
                    if (state.State != BuildingState.Active) continue;
                    var building = _buildingDatabase.GetBuilding(state.BuildingId);
                    if (building == null) continue;
                    availableServices.AddRange(building.Services);
                }
            }

            settlement.SettlementServices = availableServices.Distinct().ToList();
        }

        /// <summary>Get available services for a settlement.</summary>
        public List<ServiceType> GetAvailableServices(string settlementId)
        {
            var settlement = _settlementDatabase.GetSettlement(settlementId);
            return settlement?.SettlementServices ?? new List<ServiceType>();
        }

        /// <summary>Get buildings for a settlement with runtime state.</summary>
        public List<(BuildingData data, BuildingSaveState state)> GetSettlementBuildings(string settlementId)
        {
            var settlement = _settlementDatabase.GetSettlement(settlementId);
            if (settlement == null) return new List<(BuildingData, BuildingSaveState)>();

            var result = new List<(BuildingData, BuildingSaveState)>();
            var states = _buildingStates.GetValueOrDefault(settlementId, new List<BuildingSaveState>());

            foreach (var state in states)
            {
                var building = _buildingDatabase.GetBuilding(state.BuildingId);
                if (building != null)
                    result.Add((building, state));
            }

            return result;
        }

        /// <summary>Get the active schedule for an NPC at a given time.</summary>
        public NpcScheduleBlock? GetNpcSchedule(string npcId, NpcProfession profession, double timeOfDay)
        {
            var schedules = _npcScheduleExpanded.GetSchedulesForProfession(profession);
            foreach (var schedule in schedules)
            {
                var block = _npcScheduleExpanded.GetActiveBlock(schedule, timeOfDay);
                if (block != null) return block;
            }
            return null;
        }

        /// <summary>Get settlement prosperity as a float (0.0-1.0).</summary>
        public float GetSettlementProsperityFloat(string settlementId)
        {
            return _settlementProsperity.GetValueOrDefault(settlementId, 0.5f);
        }

        /// <summary>Get settlement population.</summary>
        public int GetSettlementPopulation(string settlementId)
        {
            return _settlementPopulations.GetValueOrDefault(settlementId, 0);
        }

        /// <summary>Get settlement security rating.</summary>
        public SecurityRating GetSettlementSecurity(string settlementId)
        {
            var settlement = _settlementDatabase.GetSettlement(settlementId);
            return settlement?.Security ?? SecurityRating.None;
        }

        /// <summary>Handle an emergency event in a settlement.</summary>
        public void HandleEmergency(string settlementId, WorldEventType emergencyType)
        {
            var settlement = _settlementDatabase.GetSettlement(settlementId);
            if (settlement == null) return;

            Logger.Info($"SettlementManager: Emergency '{emergencyType}' in '{settlement.DisplayName}'.");

            // Trigger schedule overrides
            _npcScheduleExpanded.SetEmergencyOverride(true);

            // Trigger world event if applicable
            if (emergencyType == WorldEventType.MonsterAlert || emergencyType == WorldEventType.StormPreparation)
            {
                _worldEventFramework.TriggerEvent(emergencyType, settlementId, settlement, EventSeverity.Major);
            }

            // Reduce security temporarily
            _settlementSecurity[settlementId] = Math.Max(0f, _settlementSecurity.GetValueOrDefault(settlementId, 0.5f) - 0.2f);
        }

        /// <summary>Resolve an emergency and restore normal operations.</summary>
        public void ResolveEmergency(string settlementId)
        {
            _npcScheduleExpanded.SetEmergencyOverride(false);
            Logger.Info($"SettlementManager: Emergency resolved for '{settlementId}'.");
        }

        // ==========================================================
        // SAVE / LOAD
        // ==========================================================

        /// <summary>Get complete save state for all settlements.</summary>
        public SettlementSaveData GetSaveState()
        {
            var saveData = new SettlementSaveData();

            foreach (var settlement in _settlementDatabase.GetAllSettlements())
            {
                saveData.Settlements.Add(new SettlementSaveState
                {
                    SettlementId = settlement.SettlementId,
                    Prosperity = settlement.Prosperity,
                    Population = _settlementPopulations.GetValueOrDefault(settlement.SettlementId, settlement.Population),
                    Security = settlement.Security,
                    BuildingStates = _buildingStates.GetValueOrDefault(settlement.SettlementId, new List<BuildingSaveState>()),
                    ActiveEvents = _worldEventFramework.GetActiveEventsForSettlement(settlement.SettlementId)
                        .Select(e => new WorldEventSaveState
                        {
                            EventId = e.EventId,
                            Type = e.Type,
                            Severity = e.Severity,
                            Phase = e.Phase,
                            ElapsedDays = e.ElapsedDays,
                            Version = 1
                        }).ToList(),
                    EconomyValues = new Dictionary<string, float>
                    {
                        { "prosperity_float", _settlementProsperity.GetValueOrDefault(settlement.SettlementId, 0.5f) },
                        { "security_float", _settlementSecurity.GetValueOrDefault(settlement.SettlementId, 0.5f) }
                    },
                    Version = 1
                });
            }

            saveData.GlobalEvents = _worldEventFramework.GetSaveState();
            saveData.Version = 1;

            return saveData;
        }

        /// <summary>Restore settlements from save state.</summary>
        public void RestoreSaveState(SettlementSaveData saveData)
        {
            if (saveData == null) return;

            foreach (var state in saveData.Settlements)
            {
                var settlement = _settlementDatabase.GetSettlement(state.SettlementId);
                if (settlement == null) continue;

                settlement.Prosperity = state.Prosperity;
                _settlementProsperity[state.SettlementId] = state.EconomyValues.GetValueOrDefault("prosperity_float", 0.5f);
                _settlementPopulations[state.SettlementId] = state.Population;
                settlement.Population = state.Population;
                settlement.Security = state.Security;
                _settlementSecurity[state.SettlementId] = state.EconomyValues.GetValueOrDefault("security_float", 0.5f);

                // Restore building states
                if (state.BuildingStates.Count > 0)
                {
                    _buildingStates[state.SettlementId] = new List<BuildingSaveState>(state.BuildingStates);
                }
            }

            // Restore world events
            _worldEventFramework.RestoreSaveState(saveData.GlobalEvents);

            Logger.Info($"SettlementManager: Restored {saveData.Settlements.Count} settlement states.");
        }

        /// <summary>Get building state for a specific building in a settlement.</summary>
        public BuildingSaveState? GetBuildingState(string settlementId, string buildingId)
        {
            if (!_buildingStates.TryGetValue(settlementId, out var states)) return null;
            return states.Find(s => s.BuildingId == buildingId);
        }

        /// <summary>Set building state for a specific building.</summary>
        public void SetBuildingState(string settlementId, string buildingId, BuildingState state)
        {
            if (!_buildingStates.TryGetValue(settlementId, out var states)) return;

            var buildingState = states.Find(s => s.BuildingId == buildingId);
            if (buildingState != null)
            {
                var oldState = buildingState.State;
                buildingState.State = state;

                var eventBus = ServiceLocator.Get<EventBus>();
                eventBus?.Publish("BuildingStateChanged", new BuildingStateChangedEvent
                {
                    BuildingId = buildingId,
                    SettlementId = settlementId,
                    OldState = oldState,
                    NewState = state
                });
            }
        }

        /// <summary>Upgrade a building in a settlement.</summary>
        public bool UpgradeBuilding(string settlementId, string buildingId)
        {
            var building = _buildingDatabase.GetBuilding(buildingId);
            if (building == null) return false;

            var state = GetBuildingState(settlementId, buildingId);
            if (state == null) return false;

            if (state.UpgradeLevel >= building.MaxUpgradeLevel) return false;

            state.UpgradeLevel++;
            Logger.Info($"SettlementManager: Upgraded '{building.DisplayName}' to level {state.UpgradeLevel} in '{settlementId}'.");
            return true;
        }

        /// <summary>Set current weather condition for schedule adaptation.</summary>
        public void SetCurrentWeather(WeatherCondition weather)
        {
            _npcScheduleExpanded.SetCurrentWeather(weather);
        }

        /// <summary>Set festival mode for schedule overrides.</summary>
        public void SetFestivalMode(bool active)
        {
            _npcScheduleExpanded.SetFestivalOverride(active);
        }

        // ==========================================================
        // HELPERS
        // ==========================================================

        private static ProsperityLevel GetProsperityFromFloat(float value)
        {
            return value switch
            {
                <= 0.1f => ProsperityLevel.Collapsed,
                <= 0.25f => ProsperityLevel.Poor,
                <= 0.4f => ProsperityLevel.Struggling,
                <= 0.55f => ProsperityLevel.Stable,
                <= 0.7f => ProsperityLevel.Prosperous,
                <= 0.85f => ProsperityLevel.Wealthy,
                _ => ProsperityLevel.Booming
            };
        }
    }
}