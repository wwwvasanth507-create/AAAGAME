using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.World;

namespace HeroOfEternia.Settlement
{
    /// <summary>
    /// Expanded NPC schedule manager.
    /// Supports per-profession schedules, weather adaptation, festival/emergency overrides,
    /// and integration with the world time system.
    /// </summary>
    public class NpcScheduleExpanded
    {
        private readonly Dictionary<string, NpcScheduleDefinition> _schedules = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<NpcProfession, List<string>> _schedulesByProfession = new();
        private NpcScheduleDefinition? _defaultSchedule = null;
        private WeatherCondition _currentWeather = WeatherCondition.Clear;
        private bool _hasFestivalOverride = false;
        private bool _hasEmergencyOverride = false;

        /// <summary>Register a schedule definition.</summary>
        public void RegisterSchedule(NpcScheduleDefinition schedule)
        {
            if (schedule == null || string.IsNullOrEmpty(schedule.ScheduleId)) return;
            _schedules[schedule.ScheduleId] = schedule;

            if (!_schedulesByProfession.ContainsKey(schedule.Profession))
                _schedulesByProfession[schedule.Profession] = new List<string>();
            _schedulesByProfession[schedule.Profession].Add(schedule.ScheduleId);

            if (schedule.IsDefault)
                _defaultSchedule = schedule;
        }

        /// <summary>Get a schedule by ID.</summary>
        public NpcScheduleDefinition? GetSchedule(string scheduleId)
        {
            return _schedules.TryGetValue(scheduleId, out var schedule) ? schedule : null;
        }

        /// <summary>Get schedules for a profession.</summary>
        public List<NpcScheduleDefinition> GetSchedulesForProfession(NpcProfession profession)
        {
            if (!_schedulesByProfession.TryGetValue(profession, out var ids))
                return _defaultSchedule != null ? new List<NpcScheduleDefinition> { _defaultSchedule } : new List<NpcScheduleDefinition>();
            return ids.Select(id => _schedules[id]).ToList();
        }

        /// <summary>Get the default schedule.</summary>
        public NpcScheduleDefinition? GetDefaultSchedule() => _defaultSchedule;

        /// <summary>Set current weather for schedule adaptation.</summary>
        public void SetCurrentWeather(WeatherCondition weather) => _currentWeather = weather;

        /// <summary>Set festival override flag.</summary>
        public void SetFestivalOverride(bool active) => _hasFestivalOverride = active;

        /// <summary>Set emergency override flag.</summary>
        public void SetEmergencyOverride(bool active) => _hasEmergencyOverride = active;

        /// <summary>
        /// Get the active schedule block for a given NPC schedule and time.
        /// Respects weather, festival, and emergency overrides.
        /// </summary>
        public NpcScheduleBlock? GetActiveBlock(NpcScheduleDefinition schedule, double timeOfDay)
        {
            if (schedule == null) return null;

            NpcScheduleBlock? best = null;
            int bestPriority = -1;

            foreach (var block in schedule.Blocks)
            {
                // Match time window
                bool inWindow = timeOfDay >= block.TimeStart && timeOfDay < block.TimeEnd;
                if (!inWindow) continue;

                // Check event requirement
                if (block.RequiredEvent != WorldEventType.None)
                {
                    bool eventActive = false;
                    switch (block.RequiredEvent)
                    {
                        case WorldEventType.Festival:
                            eventActive = _hasFestivalOverride;
                            break;
                        case WorldEventType.MonsterAlert:
                        case WorldEventType.StormPreparation:
                            eventActive = _hasEmergencyOverride;
                            break;
                        default:
                            eventActive = _hasEmergencyOverride;
                            break;
                    }
                    if (!eventActive) continue;
                }

                // Check weather requirement
                if (block.RequiredWeather != WeatherCondition.None &&
                    block.RequiredWeather != _currentWeather)
                    continue;

                // Weather override: stay indoors during storms
                if (_currentWeather == WeatherCondition.Storm || 
                    _currentWeather == WeatherCondition.Blizzard ||
                    _currentWeather == WeatherCondition.Sandstorm ||
                    _currentWeather == WeatherCondition.AshFall)
                {
                    if (block.Activity != ScheduleActivity.Sleep &&
                        block.Activity != ScheduleActivity.Idle &&
                        block.LocationTag != "home" &&
                        !block.IsOverride)
                        continue;
                }

                if (block.Priority > bestPriority)
                {
                    best = block;
                    bestPriority = block.Priority;
                }
            }

            return best;
        }

        /// <summary>
        /// Build default schedules for all professions.
        /// </summary>
        public void LoadDefaultSchedules()
        {
            // Farmer schedule
            RegisterSchedule(new NpcScheduleDefinition
            {
                ScheduleId = "farmer_default",
                DisplayName = "Farmer Daily Routine",
                Profession = NpcProfession.Farmer,
                IsDefault = false,
                Blocks = new List<NpcScheduleBlock>
                {
                    new() { Activity = ScheduleActivity.Sleep,        TimeStart = 0.00, TimeEnd = 0.20, LocationTag = "home",    Priority = 1 },
                    new() { Activity = ScheduleActivity.Wake,         TimeStart = 0.20, TimeEnd = 0.22, LocationTag = "home",    Priority = 1 },
                    new() { Activity = ScheduleActivity.Breakfast,    TimeStart = 0.22, TimeEnd = 0.27, LocationTag = "home",    Priority = 1 },
                    new() { Activity = ScheduleActivity.TravelToWork, TimeStart = 0.27, TimeEnd = 0.30, LocationTag = "farm",    Priority = 1 },
                    new() { Activity = ScheduleActivity.Work,         TimeStart = 0.30, TimeEnd = 0.55, LocationTag = "farm",    Priority = 2 },
                    new() { Activity = ScheduleActivity.Lunch,        TimeStart = 0.55, TimeEnd = 0.60, LocationTag = "farm",    Priority = 1 },
                    new() { Activity = ScheduleActivity.Work,         TimeStart = 0.60, TimeEnd = 0.75, LocationTag = "farm",    Priority = 2 },
                    new() { Activity = ScheduleActivity.GoHome,       TimeStart = 0.75, TimeEnd = 0.78, LocationTag = "home",    Priority = 1 },
                    new() { Activity = ScheduleActivity.Relax,        TimeStart = 0.78, TimeEnd = 0.82, LocationTag = "inn",     Priority = 1 },
                    new() { Activity = ScheduleActivity.Socialize,    TimeStart = 0.82, TimeEnd = 0.88, LocationTag = "inn",     Priority = 1 },
                    new() { Activity = ScheduleActivity.Sleep,        TimeStart = 0.88, TimeEnd = 1.00, LocationTag = "home",    Priority = 1 },
                    // Weather override
                    new() { Activity = ScheduleActivity.Idle,         TimeStart = 0.20, TimeEnd = 0.78, LocationTag = "home",    Priority = 5, RequiredWeather = WeatherCondition.Storm, IsOverride = true },
                }
            });

            // Merchant schedule
            RegisterSchedule(new NpcScheduleDefinition
            {
                ScheduleId = "merchant_default",
                DisplayName = "Merchant Daily Routine",
                Profession = NpcProfession.Merchant,
                IsDefault = false,
                Blocks = new List<NpcScheduleBlock>
                {
                    new() { Activity = ScheduleActivity.Sleep,        TimeStart = 0.00, TimeEnd = 0.22, LocationTag = "home",    Priority = 1 },
                    new() { Activity = ScheduleActivity.Wake,         TimeStart = 0.22, TimeEnd = 0.24, LocationTag = "home",    Priority = 1 },
                    new() { Activity = ScheduleActivity.Breakfast,    TimeStart = 0.24, TimeEnd = 0.27, LocationTag = "home",    Priority = 1 },
                    new() { Activity = ScheduleActivity.TravelToWork, TimeStart = 0.27, TimeEnd = 0.30, LocationTag = "shop",    Priority = 1 },
                    new() { Activity = ScheduleActivity.Work,         TimeStart = 0.30, TimeEnd = 0.55, LocationTag = "shop",    Priority = 2 },
                    new() { Activity = ScheduleActivity.Lunch,        TimeStart = 0.55, TimeEnd = 0.60, LocationTag = "inn",     Priority = 1 },
                    new() { Activity = ScheduleActivity.Work,         TimeStart = 0.60, TimeEnd = 0.75, LocationTag = "shop",    Priority = 2 },
                    new() { Activity = ScheduleActivity.Shopping,     TimeStart = 0.75, TimeEnd = 0.80, LocationTag = "market",  Priority = 1 },
                    new() { Activity = ScheduleActivity.GoHome,       TimeStart = 0.80, TimeEnd = 0.82, LocationTag = "home",    Priority = 1 },
                    new() { Activity = ScheduleActivity.Relax,        TimeStart = 0.82, TimeEnd = 0.90, LocationTag = "inn",     Priority = 1 },
                    new() { Activity = ScheduleActivity.Sleep,        TimeStart = 0.90, TimeEnd = 1.00, LocationTag = "home",    Priority = 1 },
                    // Festival override
                    new() { Activity = ScheduleActivity.Festival,     TimeStart = 0.30, TimeEnd = 0.80, LocationTag = "square",  Priority = 10, RequiredEvent = WorldEventType.Festival, IsOverride = true },
                }
            });

            // Blacksmith schedule
            RegisterSchedule(new NpcScheduleDefinition
            {
                ScheduleId = "blacksmith_default",
                DisplayName = "Blacksmith Daily Routine",
                Profession = NpcProfession.Blacksmith,
                IsDefault = false,
                Blocks = new List<NpcScheduleBlock>
                {
                    new() { Activity = ScheduleActivity.Sleep,        TimeStart = 0.00, TimeEnd = 0.20, LocationTag = "home",    Priority = 1 },
                    new() { Activity = ScheduleActivity.Wake,         TimeStart = 0.20, TimeEnd = 0.22, LocationTag = "home",    Priority = 1 },
                    new() { Activity = ScheduleActivity.Breakfast,    TimeStart = 0.22, TimeEnd = 0.25, LocationTag = "home",    Priority = 1 },
                    new() { Activity = ScheduleActivity.TravelToWork, TimeStart = 0.25, TimeEnd = 0.28, LocationTag = "blacksmith", Priority = 1 },
                    new() { Activity = ScheduleActivity.Work,         TimeStart = 0.28, TimeEnd = 0.55, LocationTag = "blacksmith", Priority = 2 },
                    new() { Activity = ScheduleActivity.Lunch,        TimeStart = 0.55, TimeEnd = 0.60, LocationTag = "inn",     Priority = 1 },
                    new() { Activity = ScheduleActivity.Work,         TimeStart = 0.60, TimeEnd = 0.78, LocationTag = "blacksmith", Priority = 2 },
                    new() { Activity = ScheduleActivity.GoHome,       TimeStart = 0.78, TimeEnd = 0.80, LocationTag = "home",    Priority = 1 },
                    new() { Activity = ScheduleActivity.Socialize,    TimeStart = 0.80, TimeEnd = 0.88, LocationTag = "inn",     Priority = 1 },
                    new() { Activity = ScheduleActivity.Sleep,        TimeStart = 0.88, TimeEnd = 1.00, LocationTag = "home",    Priority = 1 },
                }
            });

            // Guard schedule
            RegisterSchedule(new NpcScheduleDefinition
            {
                ScheduleId = "guard_default",
                DisplayName = "Guard Daily Routine",
                Profession = NpcProfession.Guard,
                IsDefault = false,
                Blocks = new List<NpcScheduleBlock>
                {
                    new() { Activity = ScheduleActivity.Sleep,        TimeStart = 0.00, TimeEnd = 0.20, LocationTag = "barracks", Priority = 1 },
                    new() { Activity = ScheduleActivity.Wake,         TimeStart = 0.20, TimeEnd = 0.22, LocationTag = "barracks", Priority = 1 },
                    new() { Activity = ScheduleActivity.Breakfast,    TimeStart = 0.22, TimeEnd = 0.27, LocationTag = "barracks", Priority = 1 },
                    new() { Activity = ScheduleActivity.Patrol,       TimeStart = 0.27, TimeEnd = 0.55, LocationTag = "gate",    Priority = 2 },
                    new() { Activity = ScheduleActivity.Lunch,        TimeStart = 0.55, TimeEnd = 0.60, LocationTag = "barracks", Priority = 1 },
                    new() { Activity = ScheduleActivity.Patrol,       TimeStart = 0.60, TimeEnd = 0.80, LocationTag = "market",  Priority = 2 },
                    new() { Activity = ScheduleActivity.Train,        TimeStart = 0.80, TimeEnd = 0.87, LocationTag = "training", Priority = 1 },
                    new() { Activity = ScheduleActivity.Sleep,        TimeStart = 0.87, TimeEnd = 1.00, LocationTag = "barracks", Priority = 1 },
                    // Monster alert override
                    new() { Activity = ScheduleActivity.Emergency,    TimeStart = 0.0,  TimeEnd = 1.00, LocationTag = "gate",    Priority = 10, RequiredEvent = WorldEventType.MonsterAlert, IsOverride = true },
                }
            });

            // Default civilian schedule (fallback for all other professions)
            RegisterSchedule(new NpcScheduleDefinition
            {
                ScheduleId = "civilian_default",
                DisplayName = "Civilian Daily Routine",
                Profession = NpcProfession.None,
                IsDefault = true,
                Blocks = new List<NpcScheduleBlock>
                {
                    new() { Activity = ScheduleActivity.Sleep,        TimeStart = 0.00, TimeEnd = 0.22, LocationTag = "home",    Priority = 1 },
                    new() { Activity = ScheduleActivity.Wake,         TimeStart = 0.22, TimeEnd = 0.25, LocationTag = "home",    Priority = 1 },
                    new() { Activity = ScheduleActivity.Breakfast,    TimeStart = 0.25, TimeEnd = 0.30, LocationTag = "home",    Priority = 1 },
                    new() { Activity = ScheduleActivity.Socialize,    TimeStart = 0.30, TimeEnd = 0.40, LocationTag = "market",  Priority = 1 },
                    new() { Activity = ScheduleActivity.Shopping,     TimeStart = 0.40, TimeEnd = 0.50, LocationTag = "market",  Priority = 1 },
                    new() { Activity = ScheduleActivity.Work,         TimeStart = 0.50, TimeEnd = 0.70, LocationTag = "workplace", Priority = 1 },
                    new() { Activity = ScheduleActivity.Relax,        TimeStart = 0.70, TimeEnd = 0.80, LocationTag = "inn",     Priority = 1 },
                    new() { Activity = ScheduleActivity.Socialize,    TimeStart = 0.80, TimeEnd = 0.88, LocationTag = "inn",     Priority = 1 },
                    new() { Activity = ScheduleActivity.Sleep,        TimeStart = 0.88, TimeEnd = 1.00, LocationTag = "home",    Priority = 1 },
                    // Weather override
                    new() { Activity = ScheduleActivity.Idle,         TimeStart = 0.25, TimeEnd = 0.80, LocationTag = "home",    Priority = 5, RequiredWeather = WeatherCondition.Storm, IsOverride = true },
                    // Festival override
                    new() { Activity = ScheduleActivity.Festival,     TimeStart = 0.25, TimeEnd = 0.85, LocationTag = "square",  Priority = 10, RequiredEvent = WorldEventType.Festival, IsOverride = true },
                }
            });

            // Priest schedule
            RegisterSchedule(new NpcScheduleDefinition
            {
                ScheduleId = "priest_default",
                DisplayName = "Priest Daily Routine",
                Profession = NpcProfession.Priest,
                IsDefault = false,
                Blocks = new List<NpcScheduleBlock>
                {
                    new() { Activity = ScheduleActivity.Sleep,        TimeStart = 0.00, TimeEnd = 0.18, LocationTag = "temple",  Priority = 1 },
                    new() { Activity = ScheduleActivity.Wake,         TimeStart = 0.18, TimeEnd = 0.20, LocationTag = "temple",  Priority = 1 },
                    new() { Activity = ScheduleActivity.Worship,      TimeStart = 0.20, TimeEnd = 0.30, LocationTag = "temple",  Priority = 2 },
                    new() { Activity = ScheduleActivity.Breakfast,    TimeStart = 0.30, TimeEnd = 0.35, LocationTag = "temple",  Priority = 1 },
                    new() { Activity = ScheduleActivity.Work,         TimeStart = 0.35, TimeEnd = 0.55, LocationTag = "temple",  Priority = 2 },
                    new() { Activity = ScheduleActivity.Lunch,        TimeStart = 0.55, TimeEnd = 0.60, LocationTag = "temple",  Priority = 1 },
                    new() { Activity = ScheduleActivity.Work,         TimeStart = 0.60, TimeEnd = 0.75, LocationTag = "temple",  Priority = 2 },
                    new() { Activity = ScheduleActivity.Study,        TimeStart = 0.75, TimeEnd = 0.85, LocationTag = "temple",  Priority = 1 },
                    new() { Activity = ScheduleActivity.Sleep,        TimeStart = 0.85, TimeEnd = 1.00, LocationTag = "temple",  Priority = 1 },
                }
            });
        }

        /// <summary>
        /// Get the location tag for a schedule block based on the NPC's profession and current assignments.
        /// Resolves generic tags like "workplace", "shop" to actual building references.
        /// </summary>
        public string ResolveLocationTag(string locationTag, NpcProfession profession, List<string> availableBuildings)
        {
            if (availableBuildings == null || availableBuildings.Count == 0)
                return locationTag;

            switch (locationTag)
            {
                case "workplace":
                    return profession switch
                    {
                        NpcProfession.Farmer => "farm",
                        NpcProfession.Blacksmith => "blacksmith",
                        NpcProfession.Merchant => "shop",
                        NpcProfession.Guard => "barracks",
                        NpcProfession.Priest => "temple",
                        NpcProfession.Healer => "hospital",
                        NpcProfession.Alchemist => "alchemist",
                        NpcProfession.Librarian => "library",
                        NpcProfession.Sailor => "dock",
                        NpcProfession.Miner => "mine",
                        _ => "workplace"
                    };

                case "shop":
                    return availableBuildings.Find(b => b.Contains("merchant") || b.Contains("shop")) ?? "market";

                case "farm":
                    return availableBuildings.Find(b => b.Contains("farm")) ?? "workplace";

                case "blacksmith":
                    return availableBuildings.Find(b => b.Contains("blacksmith")) ?? "workplace";

                case "barracks":
                    return availableBuildings.Find(b => b.Contains("barracks") || b.Contains("guard")) ?? "workplace";

                case "temple":
                    return availableBuildings.Find(b => b.Contains("temple")) ?? "workplace";

                case "training":
                    return availableBuildings.Find(b => b.Contains("training") || b.Contains("barracks")) ?? "barracks";

                case "inn":
                    return availableBuildings.Find(b => b.Contains("inn") || b.Contains("tavern")) ?? "inn";

                case "market":
                    return availableBuildings.Find(b => b.Contains("market")) ?? "market";

                case "home":
                    return "home";

                default:
                    return locationTag;
            }
        }
    }
}