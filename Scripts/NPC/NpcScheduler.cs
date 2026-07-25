using System;
using System.Collections.Generic;
using HeroOfEternia.World;

namespace HeroOfEternia.NPC
{
    public enum SchedulePeriod
    {
        Morning,
        Afternoon,
        Evening,
        Night
    }

    public enum ScheduleOverrideType
    {
        None,
        Weather,
        Festival,
        Emergency
    }

    /// <summary>
    /// A single time-block entry in an NPC's daily routine.
    /// </summary>
    public class ScheduleBlock
    {
        public SchedulePeriod Period { get; set; }
        public double TimeStart { get; set; }   // 0.0–1.0 fraction of day
        public double TimeEnd { get; set; }
        public NpcStateEnum TargetState { get; set; } = NpcStateEnum.Idle;
        public string LocationTag { get; set; } = "home"; // e.g. "market", "farm", "inn"
        public ScheduleOverrideType OverrideType { get; set; } = ScheduleOverrideType.None;
        public int Priority { get; set; } = 0; // Higher priority wins on conflict
    }

    /// <summary>
    /// Evaluates the active schedule block for an NPC given the current world time fraction.
    /// Respects weather, festival, and emergency overrides.
    /// </summary>
    public class NpcScheduler
    {
        private readonly List<ScheduleBlock> _blocks = new();
        private ScheduleOverrideType _activeOverride = ScheduleOverrideType.None;

        public void AddBlock(ScheduleBlock block) => _blocks.Add(block);

        public void SetOverride(ScheduleOverrideType overrideType) => _activeOverride = overrideType;

        public void ClearOverride() => _activeOverride = ScheduleOverrideType.None;

        /// <summary>
        /// Returns the highest-priority active schedule block for the given time fraction.
        /// </summary>
        public ScheduleBlock? GetActiveBlock(double timeOfDay)
        {
            ScheduleBlock? best = null;
            int bestPriority = -1;

            foreach (var block in _blocks)
            {
                // Match time window
                bool inWindow = timeOfDay >= block.TimeStart && timeOfDay < block.TimeEnd;
                if (!inWindow) continue;

                // Override type filter
                if (block.OverrideType != ScheduleOverrideType.None &&
                    block.OverrideType != _activeOverride)
                    continue;

                if (block.Priority > bestPriority)
                {
                    best = block;
                    bestPriority = block.Priority;
                }
            }
            return best;
        }

        /// <summary>
        /// Builds a default civilian daily schedule.
        /// </summary>
        public static NpcScheduler BuildDefaultCivilianSchedule()
        {
            var scheduler = new NpcScheduler();
            scheduler.AddBlock(new ScheduleBlock { Period = SchedulePeriod.Night,     TimeStart = 0.00, TimeEnd = 0.20, TargetState = NpcStateEnum.Sleeping,  LocationTag = "home",   Priority = 1 });
            scheduler.AddBlock(new ScheduleBlock { Period = SchedulePeriod.Morning,   TimeStart = 0.20, TimeEnd = 0.45, TargetState = NpcStateEnum.Working,   LocationTag = "market", Priority = 1 });
            scheduler.AddBlock(new ScheduleBlock { Period = SchedulePeriod.Afternoon, TimeStart = 0.45, TimeEnd = 0.65, TargetState = NpcStateEnum.Working,   LocationTag = "farm",   Priority = 1 });
            scheduler.AddBlock(new ScheduleBlock { Period = SchedulePeriod.Evening,   TimeStart = 0.65, TimeEnd = 0.80, TargetState = NpcStateEnum.Eating,    LocationTag = "inn",    Priority = 1 });
            scheduler.AddBlock(new ScheduleBlock { Period = SchedulePeriod.Night,     TimeStart = 0.80, TimeEnd = 1.00, TargetState = NpcStateEnum.Sleeping,  LocationTag = "home",   Priority = 1 });

            // Festival override example
            scheduler.AddBlock(new ScheduleBlock { Period = SchedulePeriod.Evening, TimeStart = 0.65, TimeEnd = 0.90,
                TargetState = NpcStateEnum.Celebrating, LocationTag = "square",
                OverrideType = ScheduleOverrideType.Festival, Priority = 10 });

            // Storm override — stay indoors
            scheduler.AddBlock(new ScheduleBlock { Period = SchedulePeriod.Morning, TimeStart = 0.20, TimeEnd = 0.80,
                TargetState = NpcStateEnum.Idle, LocationTag = "home",
                OverrideType = ScheduleOverrideType.Weather, Priority = 5 });

            return scheduler;
        }
    }
}
