using System;
using System.Collections.Generic;

namespace HeroOfEternia.Dialogue
{
    public class DialogueChoiceResult
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
        public string NextDialogueId { get; set; } = "";
        public string SetFlag { get; set; } = "";
        public string QuestHookId { get; set; } = "";
        public bool ConversationEnded { get; set; } = false;
    }

    /// <summary>
    /// Interactive controller for managing custom dialogue trees, multi-choice options, emotion hooks, and narrative callbacks.
    /// </summary>
    public class CustomDialogueController
    {
        private readonly List<string> _dialogueHistory = new();
        private readonly Dictionary<string, string> _recordedDecisions = new(StringComparer.OrdinalIgnoreCase);

        public DialogueEntry? CurrentDialogue { get; private set; }
        public bool IsInConversation => CurrentDialogue != null;

        public event Action<DialogueEntry>? OnDialogueStarted;
        public event Action<DialogueEntry>? OnDialogueAdvanced;
        public event Action<string, string>? OnDecisionRecorded;
        public event Action? OnConversationEnded;

        public CustomDialogueController()
        {
        }

        public bool StartConversation(string conversationId, Dictionary<string, string>? worldFlags = null)
        {
            var conv = DialogueDatabase.GetConversation(conversationId);
            if (conv == null)
            {
                Core.Logger.Warning($"CustomDialogueController: Conversation '{conversationId}' not found in database.");
                return false;
            }

            var startDialogue = DialogueDatabase.GetDialogue(conv.StartingDialogueId);
            if (startDialogue == null)
            {
                Core.Logger.Warning($"CustomDialogueController: Starting dialogue '{conv.StartingDialogueId}' not found.");
                return false;
            }

            CurrentDialogue = startDialogue;
            _dialogueHistory.Clear();
            _dialogueHistory.Add(startDialogue.DialogueId);

            OnDialogueStarted?.Invoke(CurrentDialogue);
            Core.Logger.Info($"CustomDialogueController: Started conversation '{conversationId}' at dialogue '{startDialogue.DialogueId}'.");
            return true;
        }

        public List<DialogueChoice> GetAvailableChoices(Dictionary<string, string>? worldFlags = null)
        {
            if (CurrentDialogue == null) return new List<DialogueChoice>();

            var validChoices = new List<DialogueChoice>();
            foreach (var choice in CurrentDialogue.Choices)
            {
                if (EvaluateConditions(choice.Conditions, worldFlags))
                {
                    validChoices.Add(choice);
                }
            }
            return validChoices;
        }

        public DialogueChoiceResult SelectChoice(int choiceIndex, Dictionary<string, string>? worldFlags = null)
        {
            var result = new DialogueChoiceResult();
            if (CurrentDialogue == null)
            {
                result.Message = "No active dialogue.";
                return result;
            }

            var choices = GetAvailableChoices(worldFlags);
            if (choiceIndex < 0 || choiceIndex >= choices.Count)
            {
                result.Message = $"Invalid choice index {choiceIndex}. Available choices: {choices.Count}";
                return result;
            }

            var selected = choices[choiceIndex];

            // Record decisions and flags
            if (!string.IsNullOrEmpty(selected.RecordDecision))
            {
                _recordedDecisions[selected.RecordDecision] = selected.RecordChoice;
                OnDecisionRecorded?.Invoke(selected.RecordDecision, selected.RecordChoice);
            }

            result.SetFlag = selected.SetFlag;
            result.QuestHookId = selected.QuestHookId;
            result.Success = true;

            // Check next dialogue
            if (string.IsNullOrEmpty(selected.NextDialogueId) || selected.NextDialogueId.Equals("END", StringComparison.OrdinalIgnoreCase))
            {
                EndConversation();
                result.ConversationEnded = true;
                result.NextDialogueId = "";
            }
            else
            {
                var next = DialogueDatabase.GetDialogue(selected.NextDialogueId);
                if (next != null)
                {
                    CurrentDialogue = next;
                    _dialogueHistory.Add(next.DialogueId);
                    result.NextDialogueId = next.DialogueId;
                    OnDialogueAdvanced?.Invoke(CurrentDialogue);
                }
                else
                {
                    EndConversation();
                    result.ConversationEnded = true;
                }
            }

            return result;
        }

        public bool AdvanceDialogue()
        {
            if (CurrentDialogue == null) return false;

            if (CurrentDialogue.IsEndOfConversation || string.IsNullOrEmpty(CurrentDialogue.NextDialogueId))
            {
                EndConversation();
                return false;
            }

            var next = DialogueDatabase.GetDialogue(CurrentDialogue.NextDialogueId);
            if (next != null)
            {
                CurrentDialogue = next;
                _dialogueHistory.Add(next.DialogueId);
                OnDialogueAdvanced?.Invoke(CurrentDialogue);
                return true;
            }

            EndConversation();
            return false;
        }

        public void EndConversation()
        {
            CurrentDialogue = null;
            OnConversationEnded?.Invoke();
            Core.Logger.Info("CustomDialogueController: Conversation ended.");
        }

        public IReadOnlyList<string> GetDialogueHistory() => _dialogueHistory.AsReadOnly();
        public IReadOnlyDictionary<string, string> GetRecordedDecisions() => _recordedDecisions;

        private bool EvaluateConditions(List<DialogueCondition> conditions, Dictionary<string, string>? worldFlags)
        {
            if (conditions == null || conditions.Count == 0) return true;
            if (worldFlags == null) return true;

            foreach (var cond in conditions)
            {
                bool hasFlag = worldFlags.TryGetValue(cond.Parameter, out var val) && string.Equals(val, cond.ExpectedValue, StringComparison.OrdinalIgnoreCase);
                if (cond.Negate) hasFlag = !hasFlag;
                if (!hasFlag) return false;
            }
            return true;
        }
    }
}
