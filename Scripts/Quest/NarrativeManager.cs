using System;
using System.Collections.Generic;
using System.Linq;

namespace HeroOfEternia.Quest
{
    /// <summary>
    /// Central narrative state tracker.
    /// Manages quest states, dialogue variables, world variables,
    /// NPC variables, player decisions, global/regional flags,
    /// and future story chapter tracking.
    /// All data-driven - no hardcoded story content.
    /// </summary>
    public class NarrativeManager
    {
        // ==========================================================
        // EVENTS
        // ==========================================================
        public event Action<string, string>? GlobalFlagSet;       // (flagName, value)
        public event Action<string, string>? RegionalFlagSet;     // (regionId, flagName)
        public event Action<string, object>? WorldVariableChanged; // (varName, value)
        public event Action<string, object>? NpcVariableChanged;  // (npcId, value)
        public event Action<string, object>? DialogueVariableChanged; // (varName, value)
        public event Action<string>? StoryChapterUnlocked;        // chapterId
        public event Action? NarrativeStateChanged;

        // ==========================================================
        // NARRATIVE STATE
        // ==========================================================
        private readonly Dictionary<string, string> _globalFlags = new();           // flag -> value
        private readonly Dictionary<string, Dictionary<string, string>> _regionalFlags = new(); // region -> {flag -> value}
        private readonly Dictionary<string, object> _worldVariables = new();        // varName -> value
        private readonly Dictionary<string, object> _npcVariables = new();          // npcId -> value
        private readonly Dictionary<string, object> _dialogueVariables = new();     // varName -> value
        private readonly Dictionary<string, PlayerDecision> _playerDecisions = new(); // decisionId -> decision
        private readonly List<string> _unlockedStoryChapters = new();               // chapter IDs
        private readonly Dictionary<string, QuestState> _questStates = new();       // questId -> state

        // ==========================================================
        // GLOBAL FLAGS
        // ==========================================================

        /// <summary>
        /// Sets a global narrative flag.
        /// </summary>
        public void SetGlobalFlag(string flagName, string value = "true")
        {
            _globalFlags[flagName] = value;
            GlobalFlagSet?.Invoke(flagName, value);
            NarrativeStateChanged?.Invoke();
        }

        /// <summary>
        /// Gets a global narrative flag value.
        /// </summary>
        public string? GetGlobalFlag(string flagName)
        {
            _globalFlags.TryGetValue(flagName, out var value);
            return value;
        }

        /// <summary>
        /// Checks if a global flag exists.
        /// </summary>
        public bool HasGlobalFlag(string flagName)
        {
            return _globalFlags.ContainsKey(flagName);
        }

        /// <summary>
        /// Removes a global flag.
        /// </summary>
        public void RemoveGlobalFlag(string flagName)
        {
            _globalFlags.Remove(flagName);
            NarrativeStateChanged?.Invoke();
        }

        /// <summary>
        /// Gets all global flags.
        /// </summary>
        public Dictionary<string, string> GetAllGlobalFlags()
        {
            return new Dictionary<string, string>(_globalFlags);
        }

        // ==========================================================
        // REGIONAL FLAGS
        // ==========================================================

        /// <summary>
        /// Sets a regional narrative flag.
        /// </summary>
        public void SetRegionalFlag(string regionId, string flagName, string value = "true")
        {
            if (!_regionalFlags.ContainsKey(regionId))
                _regionalFlags[regionId] = new Dictionary<string, string>();

            _regionalFlags[regionId][flagName] = value;
            RegionalFlagSet?.Invoke(regionId, flagName);
            NarrativeStateChanged?.Invoke();
        }

        /// <summary>
        /// Gets a regional flag value.
        /// </summary>
        public string? GetRegionalFlag(string regionId, string flagName)
        {
            if (_regionalFlags.TryGetValue(regionId, out var flags))
            {
                flags.TryGetValue(flagName, out var value);
                return value;
            }
            return null;
        }

        /// <summary>
        /// Checks if a regional flag exists.
        /// </summary>
        public bool HasRegionalFlag(string regionId, string flagName)
        {
            return _regionalFlags.TryGetValue(regionId, out var flags) && flags.ContainsKey(flagName);
        }

        /// <summary>
        /// Gets all flags for a region.
        /// </summary>
        public Dictionary<string, string> GetRegionalFlags(string regionId)
        {
            if (_regionalFlags.TryGetValue(regionId, out var flags))
                return new Dictionary<string, string>(flags);
            return new Dictionary<string, string>();
        }

        // ==========================================================
        // WORLD VARIABLES
        // ==========================================================

        /// <summary>
        /// Sets a world variable (e.g., "time_of_day", "season", "world_state").
        /// </summary>
        public void SetWorldVariable(string varName, object value)
        {
            _worldVariables[varName] = value;
            WorldVariableChanged?.Invoke(varName, value);
            NarrativeStateChanged?.Invoke();
        }

        /// <summary>
        /// Gets a world variable.
        /// </summary>
        public object? GetWorldVariable(string varName)
        {
            _worldVariables.TryGetValue(varName, out var value);
            return value;
        }

        /// <summary>
        /// Gets a world variable as a string.
        /// </summary>
        public string? GetWorldVariableString(string varName)
        {
            if (_worldVariables.TryGetValue(varName, out var value))
                return value?.ToString();
            return null;
        }

        /// <summary>
        /// Gets a world variable as a float.
        /// </summary>
        public float GetWorldVariableFloat(string varName, float defaultValue = 0f)
        {
            if (_worldVariables.TryGetValue(varName, out var value))
            {
                if (value is float f) return f;
                if (value is int i) return i;
                if (value is double d) return (float)d;
                if (float.TryParse(value?.ToString(), out var parsed)) return parsed;
            }
            return defaultValue;
        }

        // ==========================================================
        // NPC VARIABLES
        // ==========================================================

        /// <summary>
        /// Sets a variable on an NPC (e.g., "mood", "trust", "has_met_player").
        /// </summary>
        public void SetNpcVariable(string npcId, string varName, object value)
        {
            string key = $"{npcId}.{varName}";
            _npcVariables[key] = value;
            NpcVariableChanged?.Invoke(npcId, value);
            NarrativeStateChanged?.Invoke();
        }

        /// <summary>
        /// Gets an NPC variable.
        /// </summary>
        public object? GetNpcVariable(string npcId, string varName)
        {
            string key = $"{npcId}.{varName}";
            _npcVariables.TryGetValue(key, out var value);
            return value;
        }

        /// <summary>
        /// Gets an NPC variable as a string.
        /// </summary>
        public string? GetNpcVariableString(string npcId, string varName)
        {
            var val = GetNpcVariable(npcId, varName);
            return val?.ToString();
        }

        /// <summary>
        /// Gets an NPC variable as a float.
        /// </summary>
        public float GetNpcVariableFloat(string npcId, string varName, float defaultValue = 0f)
        {
            var val = GetNpcVariable(npcId, varName);
            if (val is float f) return f;
            if (val is int i) return i;
            if (float.TryParse(val?.ToString(), out var parsed)) return parsed;
            return defaultValue;
        }

        // ==========================================================
        // DIALOGUE VARIABLES
        // ==========================================================

        /// <summary>
        /// Sets a dialogue-scoped variable (resets per conversation).
        /// </summary>
        public void SetDialogueVariable(string varName, object value)
        {
            _dialogueVariables[varName] = value;
            DialogueVariableChanged?.Invoke(varName, value);
        }

        /// <summary>
        /// Gets a dialogue variable.
        /// </summary>
        public object? GetDialogueVariable(string varName)
        {
            _dialogueVariables.TryGetValue(varName, out var value);
            return value;
        }

        /// <summary>
        /// Clears all dialogue variables (called at end of conversation).
        /// </summary>
        public void ClearDialogueVariables()
        {
            _dialogueVariables.Clear();
        }

        // ==========================================================
        // PLAYER DECISIONS
        // ==========================================================

        /// <summary>
        /// Records a player decision.
        /// </summary>
        public void RecordDecision(string decisionId, string choice, string context = "")
        {
            _playerDecisions[decisionId] = new PlayerDecision
            {
                DecisionId = decisionId,
                Choice = choice,
                Context = context,
                Timestamp = DateTime.UtcNow
            };
            NarrativeStateChanged?.Invoke();
        }

        /// <summary>
        /// Gets a recorded player decision.
        /// </summary>
        public PlayerDecision? GetDecision(string decisionId)
        {
            _playerDecisions.TryGetValue(decisionId, out var decision);
            return decision;
        }

        /// <summary>
        /// Checks if a player has made a specific choice.
        /// </summary>
        public bool DidPlayerChoose(string decisionId, string choice)
        {
            if (_playerDecisions.TryGetValue(decisionId, out var decision))
                return decision.Choice == choice;
            return false;
        }

        /// <summary>
        /// Gets all player decisions.
        /// </summary>
        public List<PlayerDecision> GetAllDecisions()
        {
            return _playerDecisions.Values.ToList();
        }

        // ==========================================================
        // QUEST STATE TRACKING
        // ==========================================================

        /// <summary>
        /// Updates the tracked state of a quest.
        /// </summary>
        public void SetQuestState(string questId, QuestState state)
        {
            _questStates[questId] = state;
            NarrativeStateChanged?.Invoke();
        }

        /// <summary>
        /// Gets the tracked state of a quest.
        /// </summary>
        public QuestState? GetQuestState(string questId)
        {
            if (_questStates.TryGetValue(questId, out var state))
                return state;
            return null;
        }

        /// <summary>
        /// Checks if a quest has been completed (narrative tracking).
        /// </summary>
        public bool IsQuestCompleted(string questId)
        {
            return _questStates.TryGetValue(questId, out var state) && state == QuestState.Completed;
        }

        // ==========================================================
        // STORY CHAPTERS
        // ==========================================================

        /// <summary>
        /// Unlocks a story chapter.
        /// </summary>
        public void UnlockStoryChapter(string chapterId)
        {
            if (!_unlockedStoryChapters.Contains(chapterId))
            {
                _unlockedStoryChapters.Add(chapterId);
                StoryChapterUnlocked?.Invoke(chapterId);
                NarrativeStateChanged?.Invoke();
            }
        }

        /// <summary>
        /// Checks if a story chapter is unlocked.
        /// </summary>
        public bool IsStoryChapterUnlocked(string chapterId)
        {
            return _unlockedStoryChapters.Contains(chapterId);
        }

        /// <summary>
        /// Gets all unlocked story chapters.
        /// </summary>
        public List<string> GetUnlockedStoryChapters()
        {
            return _unlockedStoryChapters.ToList();
        }

        // ==========================================================
        // CONDITION EVALUATION
        // ==========================================================

        /// <summary>
        /// Evaluates a condition string against the current narrative state.
        /// Format: "flag:flagName" or "!flag:flagName" or "variable:varName=value"
        /// </summary>
        public bool EvaluateCondition(string condition)
        {
            if (string.IsNullOrEmpty(condition)) return true;

            // Negation
            bool negate = condition.StartsWith("!");
            string actual = negate ? condition.Substring(1) : condition;

            bool result = EvaluateConditionInternal(actual);
            return negate ? !result : result;
        }

        private bool EvaluateConditionInternal(string condition)
        {
            // Flag check: "flag:flagName" or "flag:flagName=value"
            if (condition.StartsWith("flag:"))
            {
                string flagPart = condition.Substring(5);
                int eqIndex = flagPart.IndexOf('=');
                if (eqIndex >= 0)
                {
                    string flagName = flagPart.Substring(0, eqIndex);
                    string expectedValue = flagPart.Substring(eqIndex + 1);
                    return _globalFlags.TryGetValue(flagName, out var val) && val == expectedValue;
                }
                return _globalFlags.ContainsKey(flagPart);
            }

            // Regional flag: "region:regionId:flagName" or "region:regionId:flagName=value"
            if (condition.StartsWith("region:"))
            {
                string regionPart = condition.Substring(7);
                var parts = regionPart.Split(':');
                if (parts.Length >= 2)
                {
                    string regionId = parts[0];
                    string flagPart = parts[1];
                    int eqIndex = flagPart.IndexOf('=');
                    if (eqIndex >= 0)
                    {
                        string flagName = flagPart.Substring(0, eqIndex);
                        string expectedValue = flagPart.Substring(eqIndex + 1);
                        return _regionalFlags.TryGetValue(regionId, out var flags) &&
                               flags.TryGetValue(flagName, out var val) && val == expectedValue;
                    }
                    return _regionalFlags.TryGetValue(regionId, out var flags) && flags.ContainsKey(flagPart);
                }
                return false;
            }

            // Variable check: "var:varName=value" or "var:varName>value"
            if (condition.StartsWith("var:"))
            {
                string varPart = condition.Substring(4);
                return EvaluateVariableCondition(varPart, _worldVariables);
            }

            // NPC variable: "npc:npcId:varName=value"
            if (condition.StartsWith("npc:"))
            {
                string npcPart = condition.Substring(4);
                var parts = npcPart.Split(':');
                if (parts.Length >= 2)
                {
                    string npcId = parts[0];
                    string varPart = parts[1];
                    string key = $"{npcId}.{varPart}";
                    return EvaluateVariableCondition(varPart, _npcVariables);
                }
                return false;
            }

            // Quest state: "quest:questId=completed" or "quest:questId=active"
            if (condition.StartsWith("quest:"))
            {
                string questPart = condition.Substring(6);
                var parts = questPart.Split('=');
                if (parts.Length == 2)
                {
                    string questId = parts[0];
                    string expectedState = parts[1];
                    if (_questStates.TryGetValue(questId, out var state))
                    {
                        return state.ToString().ToLowerInvariant() == expectedState.ToLowerInvariant();
                    }
                    return false;
                }
                return _questStates.ContainsKey(questPart);
            }

            // Chapter check: "chapter:chapterId"
            if (condition.StartsWith("chapter:"))
            {
                string chapterId = condition.Substring(8);
                return _unlockedStoryChapters.Contains(chapterId);
            }

            // Decision check: "decision:decisionId=choice"
            if (condition.StartsWith("decision:"))
            {
                string decisionPart = condition.Substring(9);
                var parts = decisionPart.Split('=');
                if (parts.Length == 2)
                {
                    return DidPlayerChoose(parts[0], parts[1]);
                }
                return _playerDecisions.ContainsKey(decisionPart);
            }

            // Default: unknown conditions return true
            return true;
        }

        private bool EvaluateVariableCondition(string varPart, Dictionary<string, object> variables)
        {
            // Check for operators: =, >, <, >=, <=, !=
            string[] operators = { ">=", "<=", "!=", "=", ">", "<" };
            string op = "=";
            int opIndex = -1;

            foreach (var oper in operators)
            {
                int idx = varPart.IndexOf(oper, StringComparison.Ordinal);
                if (idx >= 0)
                {
                    op = oper;
                    opIndex = idx;
                    break;
                }
            }

            if (opIndex < 0)
            {
                // Just check existence
                return variables.ContainsKey(varPart);
            }

            string varName = varPart.Substring(0, opIndex);
            string expectedValue = varPart.Substring(opIndex + op.Length);

            if (!variables.TryGetValue(varName, out var actualValue))
                return false;

            string actualStr = actualValue?.ToString() ?? "";

            // Try numeric comparison
            if (float.TryParse(actualStr, out var actualFloat) &&
                float.TryParse(expectedValue, out var expectedFloat))
            {
                return op switch
                {
                    "=" => Math.Abs(actualFloat - expectedFloat) < 0.001f,
                    "!=" => Math.Abs(actualFloat - expectedFloat) >= 0.001f,
                    ">" => actualFloat > expectedFloat,
                    "<" => actualFloat < expectedFloat,
                    ">=" => actualFloat >= expectedFloat,
                    "<=" => actualFloat <= expectedFloat,
                    _ => false
                };
            }

            // String comparison
            return op switch
            {
                "=" => actualStr == expectedValue,
                "!=" => actualStr != expectedValue,
                _ => false
            };
        }

        // ==========================================================
        // SAVE / LOAD
        // ==========================================================

        /// <summary>
        /// Captures narrative state for saving.
        /// </summary>
        public NarrativeSaveData GetSaveData()
        {
            return new NarrativeSaveData
            {
                GlobalFlags = new Dictionary<string, string>(_globalFlags),
                RegionalFlags = _regionalFlags.ToDictionary(
                    kvp => kvp.Key,
                    kvp => new Dictionary<string, string>(kvp.Value)),
                WorldVariables = _worldVariables.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.ToString() ?? ""),
                NpcVariables = _npcVariables.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.ToString() ?? ""),
                PlayerDecisions = _playerDecisions.Values.ToList(),
                UnlockedStoryChapters = _unlockedStoryChapters.ToList(),
                QuestStates = _questStates.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.ToString())
            };
        }

        /// <summary>
        /// Restores narrative state from saved data.
        /// </summary>
        public void LoadSaveData(NarrativeSaveData saveData)
        {
            if (saveData == null) return;

            _globalFlags.Clear();
            _regionalFlags.Clear();
            _worldVariables.Clear();
            _npcVariables.Clear();
            _playerDecisions.Clear();
            _unlockedStoryChapters.Clear();
            _questStates.Clear();

            foreach (var kvp in saveData.GlobalFlags)
                _globalFlags[kvp.Key] = kvp.Value;

            foreach (var kvp in saveData.RegionalFlags)
                _regionalFlags[kvp.Key] = new Dictionary<string, string>(kvp.Value);

            foreach (var kvp in saveData.WorldVariables)
                _worldVariables[kvp.Key] = kvp.Value;

            foreach (var kvp in saveData.NpcVariables)
                _npcVariables[kvp.Key] = kvp.Value;

            foreach (var decision in saveData.PlayerDecisions)
                _playerDecisions[decision.DecisionId] = decision;

            _unlockedStoryChapters.AddRange(saveData.UnlockedStoryChapters);

            foreach (var kvp in saveData.QuestStates)
            {
                if (Enum.TryParse<QuestState>(kvp.Value, out var state))
                    _questStates[kvp.Key] = state;
            }
        }

        /// <summary>
        /// Clears all narrative state (for testing).
        /// </summary>
        public void Clear()
        {
            _globalFlags.Clear();
            _regionalFlags.Clear();
            _worldVariables.Clear();
            _npcVariables.Clear();
            _dialogueVariables.Clear();
            _playerDecisions.Clear();
            _unlockedStoryChapters.Clear();
            _questStates.Clear();
        }
    }

    // ==========================================================
    // DATA MODELS
    // ==========================================================

    public class PlayerDecision
    {
        public string DecisionId { get; set; } = "";
        public string Choice { get; set; } = "";
        public string Context { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }

    public class NarrativeSaveData
    {
        public Dictionary<string, string> GlobalFlags { get; set; } = new();
        public Dictionary<string, Dictionary<string, string>> RegionalFlags { get; set; } = new();
        public Dictionary<string, string> WorldVariables { get; set; } = new();
        public Dictionary<string, string> NpcVariables { get; set; } = new();
        public List<PlayerDecision> PlayerDecisions { get; set; } = new();
        public List<string> UnlockedStoryChapters { get; set; } = new();
        public Dictionary<string, string> QuestStates { get; set; } = new();
    }
}