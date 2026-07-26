using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter15
{
    public enum EndingChoice
    {
        Restoration_SolWarden,   // Primary Choice: Rebuild the Celestial Sol Order & restore Eternia's sun towers
        Transformation_VoidBalance, // Secondary Choice: Balance Sol & Void, creating an ethereal harmony
        Purge_PrimalDawn        // Tertiary Choice: Purge all celestial relics, returning Eternia to raw mortal nature
    }

    /// <summary>
    /// Ending Sequence & Cinematic Resolution Controller for Chapter 15.
    /// Manages post-Malakor dialogue resolution, character epilogues, world restoration state (WorldState_DawnOfSol),
    /// and regional victory flags.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class EndingSequenceManager : IInitializable
    {
        private EndingChoice _chosenEnding = EndingChoice.Restoration_SolWarden;
        private bool _isEndingTriggered = false;

        public bool IsInitialized { get; private set; }
        public bool IsEndingTriggered => _isEndingTriggered;
        public EndingChoice ChosenEnding => _chosenEnding;

        public event Action<EndingChoice>? OnEndingSequenceStarted;
        public event Action<EndingChoice>? OnEndingSequenceCompleted;

        public void Initialize()
        {
            if (IsInitialized) return;

            _chosenEnding = EndingChoice.Restoration_SolWarden;
            _isEndingTriggered = false;

            // Register with ServiceLocator
            ServiceLocator.Register<EndingSequenceManager>(this);

            IsInitialized = true;
            Logger.Info("EndingSequenceManager: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            ServiceLocator.Unregister<EndingSequenceManager>();
            IsInitialized = false;
            Logger.Info("EndingSequenceManager: Shutdown completed.");
        }

        public void TriggerEndingSequence(EndingChoice choice)
        {
            if (!IsInitialized || _isEndingTriggered) return;

            _chosenEnding = choice;
            _isEndingTriggered = true;

            OnEndingSequenceStarted?.Invoke(_chosenEnding);
            Logger.Info($"EndingSequenceManager: Triggered ending sequence with choice '{choice}'. Rebuilding Eternia's world state!");
        }

        public void CompleteEndingSequence()
        {
            if (!IsInitialized || !_isEndingTriggered) return;

            OnEndingSequenceCompleted?.Invoke(_chosenEnding);
            Logger.Info($"EndingSequenceManager: Ending sequence for '{_chosenEnding}' completed successfully. Proceeding to Epilogue!");
        }
    }
}
