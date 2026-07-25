using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Exploration
{
    public enum PuzzleMechanismType
    {
        PressurePlate,
        Switch,
        Lever,
        RotatingObject,
        PatternMatching,
        RuneActivation,
        LightReflection,
        WeightBased,
        ElementInteraction,
        Timed,
        MultiStage
    }

    public class PuzzleState
    {
        public string PuzzleId { get; set; } = string.Empty;
        public PuzzleMechanismType Type { get; set; } = PuzzleMechanismType.Lever;
        public int CurrentStage { get; set; } = 0;
        public int TotalStages { get; set; } = 1;
        public bool IsSolved { get; set; } = false;
        public Dictionary<string, bool> ComponentStates { get; set; } = new();
    }

    /// <summary>
    /// Reusable puzzle engine supporting pressure plates, levers, rune activation,
    /// light reflection, multi-stage puzzles, and state persistence.
    /// </summary>
    public class PuzzleManager
    {
        private readonly Dictionary<string, PuzzleState> _puzzles = new(StringComparer.OrdinalIgnoreCase);

        public event Action<string>? OnPuzzleSolved;

        public void RegisterPuzzle(string puzzleId, PuzzleMechanismType type, int totalStages = 1)
        {
            _puzzles[puzzleId] = new PuzzleState
            {
                PuzzleId = puzzleId,
                Type = type,
                TotalStages = totalStages
            };
        }

        public bool ToggleComponent(string puzzleId, string componentId)
        {
            if (!_puzzles.TryGetValue(puzzleId, out var puzzle) || puzzle.IsSolved) return false;

            bool state = puzzle.ComponentStates.TryGetValue(componentId, out var current) && current;
            puzzle.ComponentStates[componentId] = !state;

            return CheckPuzzleCompletion(puzzle);
        }

        public bool AdvanceStage(string puzzleId)
        {
            if (!_puzzles.TryGetValue(puzzleId, out var puzzle) || puzzle.IsSolved) return false;

            puzzle.CurrentStage++;
            if (puzzle.CurrentStage >= puzzle.TotalStages)
            {
                puzzle.IsSolved = true;
                OnPuzzleSolved?.Invoke(puzzleId);
                EventBus.Publish(new HeroOfEternia.World.Content.DiscoveryEvent { LocationId = puzzleId, DisplayName = $"Solved {puzzleId}" });
                return true;
            }
            return false;
        }

        private bool CheckPuzzleCompletion(PuzzleState puzzle)
        {
            // All registered components must be true for auto-solve
            foreach (var val in puzzle.ComponentStates.Values)
            {
                if (!val) return false;
            }

            if (puzzle.ComponentStates.Count > 0)
            {
                puzzle.IsSolved = true;
                OnPuzzleSolved?.Invoke(puzzle.PuzzleId);
                return true;
            }
            return false;
        }

        public bool IsSolved(string puzzleId)
        {
            return _puzzles.TryGetValue(puzzleId, out var puzzle) && puzzle.IsSolved;
        }

        public void ResetPuzzle(string puzzleId)
        {
            if (_puzzles.TryGetValue(puzzleId, out var puzzle))
            {
                puzzle.IsSolved = false;
                puzzle.CurrentStage = 0;
                puzzle.ComponentStates.Clear();
            }
        }
    }
}
