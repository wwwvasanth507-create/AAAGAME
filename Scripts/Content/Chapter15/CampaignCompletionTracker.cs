using System;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter15
{
    /// <summary>
    /// Campaign Completion & Play Statistics Tracker for Hero of Eternia.
    /// Manages campaign completion timestamps, total play time, completion percentage, unlocked titles (Champion of Sol), and stat tracking.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class CampaignCompletionTracker : IInitializable
    {
        public bool IsInitialized { get; private set; }
        public bool IsCampaignCompleted { get; private set; } = false;
        public string CompletionTimestamp { get; private set; } = "";
        public float TotalPlayTimeHours { get; private set; } = 48.5f;
        public float CompletionPercentage { get; private set; } = 100.0f;
        public string AwardedTitle { get; private set; } = "Champion of Sol";

        public event Action? OnCampaignCompleted;

        public void Initialize()
        {
            if (IsInitialized) return;

            // Register with ServiceLocator
            ServiceLocator.Register<CampaignCompletionTracker>(this);

            IsInitialized = true;
            Logger.Info("CampaignCompletionTracker: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            ServiceLocator.Unregister<CampaignCompletionTracker>();
            IsInitialized = false;
            Logger.Info("CampaignCompletionTracker: Shutdown completed.");
        }

        public void RecordCampaignCompletion(float playTimeHours, string difficultyCompleted = "Normal")
        {
            if (!IsInitialized || IsCampaignCompleted) return;

            IsCampaignCompleted = true;
            TotalPlayTimeHours = playTimeHours > 0 ? playTimeHours : 48.5f;
            CompletionTimestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC");
            AwardedTitle = "Champion of Sol";

            OnCampaignCompleted?.Invoke();
            Logger.Info($"CampaignCompletionTracker: CAMPAIGN COMPLETED ON {difficultyCompleted}! Playtime: {TotalPlayTimeHours:F1} hrs. Awarded Title: '{AwardedTitle}'.");
        }
    }
}
