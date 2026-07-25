using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Prologue
{
    public enum TutorialStep
    {
        Movement,
        CameraControl,
        Interaction,
        Dialogue,
        CombatBasics,
        Gathering,
        Crafting,
        InventoryEquipment,
        MapJournal,
        SaveSystemCompleted
    }

    /// <summary>
    /// Contextual tutorial flow controller guiding players smoothly through basic controls,
    /// combat, inventory, crafting, and UI without intrusive popups.
    /// </summary>
    public class IntroductionFlowManager
    {
        public TutorialStep CurrentStep { get; private set; } = TutorialStep.Movement;
        private readonly HashSet<TutorialStep> _completedSteps = new();

        public event Action<TutorialStep>? OnStepCompleted;
        public event Action<TutorialStep>? OnStepStarted;

        public bool CompleteStep(TutorialStep step)
        {
            if (_completedSteps.Add(step))
            {
                OnStepCompleted?.Invoke(step);
                AdvanceToNextStep();
                return true;
            }
            return false;
        }

        private void AdvanceToNextStep()
        {
            if (CurrentStep < TutorialStep.SaveSystemCompleted)
            {
                CurrentStep++;
                OnStepStarted?.Invoke(CurrentStep);
            }
        }

        public bool IsStepCompleted(TutorialStep step) => _completedSteps.Contains(step);
        public IReadOnlySet<TutorialStep> CompletedSteps => _completedSteps;

        public void LoadCompletedSteps(IEnumerable<TutorialStep> steps)
        {
            _completedSteps.Clear();
            if (steps != null)
            {
                foreach (var s in steps) _completedSteps.Add(s);
            }
        }
    }
}
