using System;
using System.Collections.Generic;

namespace HeroOfEternia.Content.Chapter10
{
    public enum PuzzleCategory
    {
        SunRuneDial,
        LightPrismReflect,
        WaterLevelValve,
        WeightPlateSequence
    }

    public class PuzzleSequenceDefinition
    {
        public string PuzzleId { get; set; } = "";
        public string Name { get; set; } = "";
        public PuzzleCategory Category { get; set; } = PuzzleCategory.SunRuneDial;
        public string LocationChamberId { get; set; } = "";
        public List<string> SolutionSequence { get; set; } = new();
        public bool IsSolved { get; set; } = false;
        public string UnlocksChamberId { get; set; } = "";
    }

    /// <summary>
    /// Layered Puzzle Sequence Manager for Chapter 10 & Act III Finale.
    /// Evaluates rune combinations, light prism arrays, water level control switches, and weight plates in the Temple of the Eternal Sun.
    /// </summary>
    public class TemplePuzzleSequence
    {
        private readonly Dictionary<string, PuzzleSequenceDefinition> _puzzles = new(StringComparer.OrdinalIgnoreCase);

        public event Action<PuzzleSequenceDefinition>? OnPuzzleSolved;

        public TemplePuzzleSequence()
        {
            InitializePuzzles();
        }

        private void InitializePuzzles()
        {
            // 1. Entrance Sun Dial
            RegisterPuzzle(new PuzzleSequenceDefinition
            {
                PuzzleId = "puzzle_entrance_sun_dial",
                Name = "Rune Dial of the Sun",
                Category = PuzzleCategory.SunRuneDial,
                LocationChamberId = "chamber_temple_entrance",
                SolutionSequence = new List<string> { "rune_sun", "rune_dawn", "rune_noon" },
                UnlocksChamberId = "chamber_sun_court"
            });

            // 2. Light Reflection Array
            RegisterPuzzle(new PuzzleSequenceDefinition
            {
                PuzzleId = "puzzle_light_reflection_array",
                Name = "Water Prism Reflection Array",
                Category = PuzzleCategory.LightPrismReflect,
                LocationChamberId = "chamber_water_prism",
                SolutionSequence = new List<string> { "prism_rotate_90", "prism_align_center", "prism_focus_lens" },
                UnlocksChamberId = "chamber_subterranean_sanctum"
            });

            // 3. Weight Plate Sequence
            RegisterPuzzle(new PuzzleSequenceDefinition
            {
                PuzzleId = "puzzle_weight_plate_sequence",
                Name = "Weight Plate Balance Sequence",
                Category = PuzzleCategory.WeightPlateSequence,
                LocationChamberId = "chamber_subterranean_sanctum",
                SolutionSequence = new List<string> { "plate_left_heavy", "plate_right_light" },
                UnlocksChamberId = "chamber_astral_vault"
            });
        }

        public void RegisterPuzzle(PuzzleSequenceDefinition puzzle)
        {
            if (puzzle != null && !string.IsNullOrEmpty(puzzle.PuzzleId))
            {
                _puzzles[puzzle.PuzzleId] = puzzle;
            }
        }

        public bool SolvePuzzle(string puzzleId, List<string> inputSequence)
        {
            if (!_puzzles.TryGetValue(puzzleId, out var puzzle))
            {
                Core.Logger.Warning($"TemplePuzzleSequence: Puzzle '{puzzleId}' not found.");
                return false;
            }

            if (puzzle.IsSolved) return true;

            // Check solution match
            if (inputSequence != null && SequenceMatches(puzzle.SolutionSequence, inputSequence))
            {
                puzzle.IsSolved = true;
                OnPuzzleSolved?.Invoke(puzzle);
                Core.Logger.Info($"TemplePuzzleSequence: Solved puzzle '{puzzle.Name}' ({puzzleId})! Unlocked chamber: {puzzle.UnlocksChamberId}.");
                return true;
            }

            Core.Logger.Warning($"TemplePuzzleSequence: Solution sequence incorrect for puzzle '{puzzle.Name}'.");
            return false;
        }

        private bool SequenceMatches(List<string> expected, List<string> actual)
        {
            if (expected.Count != actual.Count) return false;
            for (int i = 0; i < expected.Count; i++)
            {
                if (!string.Equals(expected[i], actual[i], StringComparison.OrdinalIgnoreCase))
                    return false;
            }
            return true;
        }

        public PuzzleSequenceDefinition? GetPuzzle(string puzzleId)
        {
            return _puzzles.TryGetValue(puzzleId, out var p) ? p : null;
        }

        public List<PuzzleSequenceDefinition> GetAllPuzzles()
        {
            return new List<PuzzleSequenceDefinition>(_puzzles.Values);
        }
    }
}
