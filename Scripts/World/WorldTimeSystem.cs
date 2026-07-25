using System;
using HeroOfEternia.Core;

namespace HeroOfEternia.World
{
    public enum DayCycleStage
    {
        Sunrise,
        Day,
        Sunset,
        Night
    }

    public enum SeasonType
    {
        Spring,
        Summer,
        Autumn,
        Winter
    }

    /// <summary>
    /// Tracks in-game progression: Day/Night fractional intervals, Sunrise/Sunset triggers,
    /// seasonal shifts, and future lunar progression hooks.
    /// </summary>
    public class WorldTimeSystem
    {
        public double TimeOfDay { get; private set; } = 0.22; // 0.0 to 1.0 (0.0 is Midnight, 0.5 is Noon)
        public double DayLengthSeconds { get; set; } = 1200.0; // 20 real-life minutes for 24-hr loop
        
        public int DayCount { get; private set; } = 1;
        public SeasonType CurrentSeason { get; private set; } = SeasonType.Spring;

        /// <summary>
        /// Advances in-game time by frame delta tick value.
        /// </summary>
        public void Update(double delta)
        {
            if (DayLengthSeconds <= 0.0) return;

            TimeOfDay += delta / DayLengthSeconds;
            if (TimeOfDay >= 1.0)
            {
                TimeOfDay -= 1.0;
                DayCount++;
                EvaluateSeasonalProgress();
                Logger.Info($"WorldTimeSystem: A new day has dawned. Day: {DayCount}, Season: {CurrentSeason}");
            }
        }

        /// <summary>
        /// Returns the current state cycle stage.
        /// </summary>
        public DayCycleStage GetCycleStage()
        {
            // Sunrise: 0.20 to 0.28 (roughly 4:48 AM to 6:43 AM)
            // Sunset: 0.72 to 0.80 (roughly 5:16 PM to 7:12 PM)
            if (TimeOfDay >= 0.20 && TimeOfDay < 0.28) return DayCycleStage.Sunrise;
            if (TimeOfDay >= 0.28 && TimeOfDay < 0.72) return DayCycleStage.Day;
            if (TimeOfDay >= 0.72 && TimeOfDay < 0.80) return DayCycleStage.Sunset;
            return DayCycleStage.Night;
        }

        /// <summary>
        /// Restores time state directly (useful for loading saves).
        /// </summary>
        public void SetTimeState(double timeOfDay, int dayCount)
        {
            TimeOfDay = Math.Clamp(timeOfDay, 0.0, 0.9999);
            DayCount = Math.Max(1, dayCount);
            EvaluateSeasonalProgress();
        }

        private void EvaluateSeasonalProgress()
        {
            // Shift season every 30 in-game days
            int seasonIndex = ((DayCount - 1) / 30) % 4;
            CurrentSeason = (SeasonType)seasonIndex;
        }
    }
}
