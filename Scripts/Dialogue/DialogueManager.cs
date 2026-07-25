using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Quest;

namespace HeroOfEternia.Dialogue
{
    /// <summary>
    /// Branching dialogue execution engine.
    /// Handles conversation flow, condition evaluation, choice resolution,
    /// quest hooks, merchant hooks, and narrative state integration.
    /// Supports thousands of dialogue entries with efficient lookups.
    /// </summary>
    public class DialogueManager
    {
        // ==========================================================
        // EVENTS
        // ==========================================================
        public event Action<DialogueEntry>? DialogueDisplayed;
        public event Action<DialogueChoice>? ChoiceSelected;
        public event Action<string>? ConversationStarted;    // conversationId
        public event Action<string>? ConversationEnded;      // conversationId
        public event Action<string>? DialogueAdvanced;       // dialogueId
        public event Action<string, string>? FlagSet;        // (flagName, value)
        public event Action<string, string>? DecisionRecorded; // (decisionId, choice)

        // ==========================================================
        // STATE
        // ==========================================================
        private NarrativeManager? _narrativeManager;
        private QuestManager? _questManager;
        private JournalManager? _journalManager;
        
        private string? _activeConversationId;
        private string? _activeDialogueId;
        private int _currentDepth;
        private int _maxDepth = 10;
        private bool _isInConversation = false;
        private readonly HashSet<string> _visitedDialogues = new(); // prevent infinite loops

        // ==========================================================
        // CONFIGURATION
        // ==========================================================

        /// <summary>
        /// Sets the narrative manager for condition evaluation.
        /// </summary>
        public void SetNarrativeManager(NarrativeManager manager)
        {
            _narrativeManager = manager;
        }

        /// <summary>
        /// Sets the quest manager for quest hooks.
        /// </summary>
        public void SetQuestManager(QuestManager manager)
        {
            _questManager = manager;
        }

        /// <summary>
        /// Sets the journal manager for dialogue logging.
        /// </summary>
        public void SetJournalManager(JournalManager manager)
        {
            _journalManager = manager;
        }

        /// <summary>
        /// Sets the maximum nesting depth for conversations.
        /// </summary>
        public void SetMaxDepth(int depth)
        {
            _maxDepth = Math.Clamp(depth, 1, 100);
        }

        // ==========================================================
        // CONVERSATION FLOW
        // ==========================================================

        /// <summary>
        /// Starts a conversation with an NPC.
        /// Returns the first dialogue entry, or null if conditions not met.
        /// </summary>
        public DialogueEntry? StartConversation(string conversationId)
        {
            var conversation = DialogueDatabase.GetConversation(conversationId);
            if (conversation == null) return null;

            // Check entry conditions
            if (!EvaluateConditions(conversation.EntryConditions))
                return null;

            // Check max depth
            if (conversation.MaxDepth > 0)
                _maxDepth = conversation.MaxDepth;

            // Reset state
            _activeConversationId = conversationId;
            _currentDepth = 0;
            _isInConversation = true;
            _visitedDialogues.Clear();

            // Set on-start flag
            if (!string.IsNullOrEmpty(conversation.OnStartFlag))
                _narrativeManager?.SetGlobalFlag(conversation.OnStartFlag);

            // Get starting dialogue
            var startingDialogue = DialogueDatabase.GetStartingDialogue(conversationId);
            if (startingDialogue == null) return null;

            _activeDialogueId = startingDialogue.DialogueId;
            _visitedDialogues.Add(startingDialogue.DialogueId);

            ConversationStarted?.Invoke(conversationId);
            DisplayDialogue(startingDialogue);

            return startingDialogue;
        }

        /// <summary>
        /// Advances the conversation to the next dialogue.
        /// Returns the next dialogue entry, or null if conversation ends.
        /// </summary>
        public DialogueEntry? AdvanceDialogue(string dialogueId)
        {
            if (!_isInConversation) return null;

            var dialogue = DialogueDatabase.GetDialogue(dialogueId);
            if (dialogue == null) return null;

            // Check depth
            _currentDepth++;
            if (_currentDepth > _maxDepth)
            {
                EndConversation();
                return null;
            }

            // Check for end of conversation
            if (dialogue.IsEndOfConversation)
            {
                EndConversation();
                return null;
            }

            // Auto-advance if no choices
            if (dialogue.Choices.Count == 0 && !string.IsNullOrEmpty(dialogue.NextDialogueId))
            {
                return GoToDialogue(dialogue.NextDialogueId);
            }

            // If there are choices, wait for player input
            if (dialogue.Choices.Count > 0)
            {
                // Filter available choices
                var availableChoices = GetAvailableChoices(dialogue);
                if (availableChoices.Count == 0)
                {
                    // No available choices - end conversation or auto-advance
                    if (!string.IsNullOrEmpty(dialogue.NextDialogueId))
                        return GoToDialogue(dialogue.NextDialogueId);
                    
                    EndConversation();
                    return null;
                }

                // If only one choice and it has no conditions, auto-select
                if (availableChoices.Count == 1 && availableChoices[0].Conditions.Count == 0)
                {
                    return SelectChoice(dialogue.DialogueId, availableChoices[0].ChoiceId);
                }

                // Return current dialogue with choices for UI to display
                DialogueAdvanced?.Invoke(dialogueId);
                return dialogue;
            }

            // No next dialogue - end conversation
            EndConversation();
            return null;
        }

        /// <summary>
        /// Selects a choice in the current dialogue.
        /// Returns the next dialogue entry after the choice.
        /// </summary>
        public DialogueEntry? SelectChoice(string dialogueId, string choiceId)
        {
            if (!_isInConversation) return null;

            var dialogue = DialogueDatabase.GetDialogue(dialogueId);
            if (dialogue == null) return null;

            var choice = dialogue.Choices.FirstOrDefault(c => c.ChoiceId == choiceId);
            if (choice == null) return null;

            // Check choice conditions
            if (!EvaluateConditions(choice.Conditions))
                return null;

            // Apply choice effects
            ApplyChoiceEffects(choice);

            ChoiceSelected?.Invoke(choice);

            // Navigate to next dialogue
            if (!string.IsNullOrEmpty(choice.NextDialogueId))
                return GoToDialogue(choice.NextDialogueId);

            // If no next dialogue, advance current
            return AdvanceDialogue(dialogueId);
        }

        /// <summary>
        /// Ends the current conversation.
        /// </summary>
        public void EndConversation()
        {
            if (!_isInConversation) return;

            var conversationId = _activeConversationId;
            
            // Set on-end flag
            if (!string.IsNullOrEmpty(conversationId))
            {
                var conversation = DialogueDatabase.GetConversation(conversationId);
                if (conversation != null && !string.IsNullOrEmpty(conversation.OnEndFlag))
                    _narrativeManager?.SetGlobalFlag(conversation.OnEndFlag);
            }

            _isInConversation = false;
            _activeConversationId = null;
            _activeDialogueId = null;
            _currentDepth = 0;
            _visitedDialogues.Clear();

            // Clear dialogue-scoped variables
            _narrativeManager?.ClearDialogueVariables();

            if (conversationId != null)
                ConversationEnded?.Invoke(conversationId);
        }

        // ==========================================================
        // DIALOGUE NAVIGATION
        // ==========================================================

        private DialogueEntry? GoToDialogue(string dialogueId)
        {
            // Prevent infinite loops
            if (_visitedDialogues.Contains(dialogueId))
            {
                Godot.GD.PushWarning($"[DialogueManager] Loop detected: {dialogueId} already visited");
                EndConversation();
                return null;
            }

            var dialogue = DialogueDatabase.GetDialogue(dialogueId);
            if (dialogue == null)
            {
                EndConversation();
                return null;
            }

            _activeDialogueId = dialogueId;
            _visitedDialogues.Add(dialogueId);

            // Apply quest hooks on display
            ApplyQuestHooks(dialogue);

            DialogueAdvanced?.Invoke(dialogueId);
            DisplayDialogue(dialogue);

            return dialogue;
        }

        private void DisplayDialogue(DialogueEntry dialogue)
        {
            // Log to journal
            _journalManager?.LogDialogue(
                dialogue.SpeakerId,
                dialogue.SpeakerId,
                dialogue.TextKey
            );

            DialogueDisplayed?.Invoke(dialogue);
        }

        // ==========================================================
        // CHOICE EFFECTS
        // ==========================================================

        private void ApplyChoiceEffects(DialogueChoice choice)
        {
            // Set flags
            if (!string.IsNullOrEmpty(choice.SetFlag))
            {
                _narrativeManager?.SetGlobalFlag(choice.SetFlag, choice.SetFlagValue);
                FlagSet?.Invoke(choice.SetFlag, choice.SetFlagValue);
            }

            // Record decisions
            if (!string.IsNullOrEmpty(choice.RecordDecision))
            {
                _narrativeManager?.RecordDecision(
                    choice.RecordDecision,
                    choice.RecordChoice,
                    $"conversation:{_activeConversationId}"
                );
                DecisionRecorded?.Invoke(choice.RecordDecision, choice.RecordChoice);
            }

            // Set variables
            if (!string.IsNullOrEmpty(choice.SetVariable))
            {
                _narrativeManager?.SetDialogueVariable(choice.SetVariable, choice.SetVariableValue);
            }

            // Quest hooks
            if (!string.IsNullOrEmpty(choice.QuestHookId) && !string.IsNullOrEmpty(choice.QuestHookAction))
            {
                ApplyQuestAction(choice.QuestHookId, choice.QuestHookAction);
            }

            // Rewards
            if (choice.Rewards.Count > 0)
            {
                foreach (var reward in choice.Rewards)
                {
                    Godot.GD.Print($"[DialogueManager] Choice reward: {reward.Type} {reward.RewardId} x{reward.Quantity}");
                }
            }

            // Merchant/Service hooks
            if (!string.IsNullOrEmpty(choice.MerchantHookId))
            {
                Godot.GD.Print($"[DialogueManager] Open merchant: {choice.MerchantHookId}");
            }
            if (!string.IsNullOrEmpty(choice.ServiceHookId))
            {
                Godot.GD.Print($"[DialogueManager] Open service: {choice.ServiceHookId} ({choice.ServiceAction})");
            }
        }

        // ==========================================================
        // QUEST HOOKS
        // ==========================================================

        private void ApplyQuestHooks(DialogueEntry dialogue)
        {
            if (string.IsNullOrEmpty(dialogue.QuestHookId)) return;

            switch (dialogue.QuestHookAction.ToLowerInvariant())
            {
                case "advance_objective":
                    if (!string.IsNullOrEmpty(dialogue.QuestObjectiveId))
                        _questManager?.AdvanceObjective(dialogue.QuestHookId, dialogue.QuestObjectiveId);
                    break;

                case "complete_objective":
                    if (!string.IsNullOrEmpty(dialogue.QuestObjectiveId))
                    {
                        var instance = _questManager?.GetActiveInstance(dialogue.QuestHookId);
                        if (instance != null)
                            _questManager?.GetObjectiveManager().CompleteObjective(instance, dialogue.QuestObjectiveId);
                    }
                    break;

                case "set_flag":
                    _narrativeManager?.SetGlobalFlag($"quest_{dialogue.QuestHookId}_dialogue_{dialogue.DialogueId}");
                    break;
            }
        }

        private void ApplyQuestAction(string questId, string action)
        {
            switch (action.ToLowerInvariant())
            {
                case "accept":
                    _questManager?.AcceptQuest(questId);
                    break;

                case "advance":
                    // Advance first objective
                    var instance = _questManager?.GetActiveInstance(questId);
                    if (instance != null && instance.ObjectiveStates.Count > 0)
                    {
                        var firstActive = instance.ObjectiveStates
                            .FirstOrDefault(os => os.State == ObjectiveState.Active && !os.IsCompleted);
                        if (firstActive != null)
                            _questManager?.AdvanceObjective(questId, firstActive.ObjectiveId);
                    }
                    break;

                case "complete":
                    instance = _questManager?.GetActiveInstance(questId);
                    if (instance != null)
                        _questManager?.CompleteQuest(instance.InstanceId);
                    break;

                case "fail":
                    instance = _questManager?.GetActiveInstance(questId);
                    if (instance != null)
                        _questManager?.FailQuest(instance.InstanceId);
                    break;
            }
        }

        // ==========================================================
        // CONDITION EVALUATION
        // ==========================================================

        /// <summary>
        /// Gets available choices for a dialogue entry based on conditions.
        /// </summary>
        public List<DialogueChoice> GetAvailableChoices(DialogueEntry dialogue)
        {
            return dialogue.Choices
                .Where(c => EvaluateConditions(c.Conditions))
                .ToList();
        }

        /// <summary>
        /// Evaluates a list of dialogue conditions.
        /// </summary>
        public bool EvaluateConditions(List<DialogueCondition> conditions)
        {
            if (conditions == null || conditions.Count == 0) return true;

            foreach (var condition in conditions)
            {
                if (!EvaluateCondition(condition))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Evaluates a single dialogue condition.
        /// </summary>
        public bool EvaluateCondition(DialogueCondition condition)
        {
            if (condition == null) return true;

            bool result = condition.Type.ToLowerInvariant() switch
            {
                "flag" => EvaluateFlagCondition(condition),
                "quest" => EvaluateQuestCondition(condition),
                "reputation" => EvaluateReputationCondition(condition),
                "faction" => EvaluateFactionCondition(condition),
                "skill" => EvaluateSkillCondition(condition),
                "variable" => EvaluateVariableCondition(condition),
                "npc_variable" => EvaluateNpcVariableCondition(condition),
                "decision" => EvaluateDecisionCondition(condition),
                "chapter" => EvaluateChapterCondition(condition),
                "level" => EvaluateLevelCondition(condition),
                "time" => EvaluateTimeCondition(condition),
                "weather" => EvaluateWeatherCondition(condition),
                "custom" => true, // custom conditions handled externally
                _ => true // unknown conditions default to true
            };

            return condition.Negate ? !result : result;
        }

        private bool EvaluateFlagCondition(DialogueCondition condition)
        {
            if (_narrativeManager == null) return true;

            string flagName = condition.Parameter;
            if (!string.IsNullOrEmpty(condition.ExpectedValue))
            {
                var value = _narrativeManager.GetGlobalFlag(flagName);
                return value == condition.ExpectedValue;
            }

            return _narrativeManager.HasGlobalFlag(flagName);
        }

        private bool EvaluateQuestCondition(DialogueCondition condition)
        {
            if (_questManager == null) return true;

            string questId = condition.Parameter;
            return condition.ExpectedValue.ToLowerInvariant() switch
            {
                "completed" => _questManager.IsQuestCompleted(questId),
                "failed" => _questManager.IsQuestFailed(questId),
                "active" => _questManager.GetActiveInstance(questId) != null,
                "available" => _questManager.CanAcceptQuest(questId),
                _ => _questManager.GetActiveInstance(questId) != null
            };
        }

        private bool EvaluateReputationCondition(DialogueCondition condition)
        {
            // TODO: Integrate with ReputationManager
            return true;
        }

        private bool EvaluateFactionCondition(DialogueCondition condition)
        {
            // TODO: Integrate with FactionDatabase
            return true;
        }

        private bool EvaluateSkillCondition(DialogueCondition condition)
        {
            // TODO: Integrate with skill system for skill checks
            return true;
        }

        private bool EvaluateVariableCondition(DialogueCondition condition)
        {
            if (_narrativeManager == null) return true;

            var value = _narrativeManager.GetWorldVariableString(condition.Parameter);
            if (string.IsNullOrEmpty(condition.ExpectedValue))
                return value != null;

            return value == condition.ExpectedValue;
        }

        private bool EvaluateNpcVariableCondition(DialogueCondition condition)
        {
            if (_narrativeManager == null) return true;

            // Format: npcId:varName
            var parts = condition.Parameter.Split(':');
            if (parts.Length < 2) return true;

            var value = _narrativeManager.GetNpcVariableString(parts[0], parts[1]);
            if (string.IsNullOrEmpty(condition.ExpectedValue))
                return value != null;

            return value == condition.ExpectedValue;
        }

        private bool EvaluateDecisionCondition(DialogueCondition condition)
        {
            if (_narrativeManager == null) return true;

            // Format: decisionId=choice
            var parts = condition.Parameter.Split('=');
            if (parts.Length < 2) return _narrativeManager.GetDecision(condition.Parameter) != null;

            return _narrativeManager.DidPlayerChoose(parts[0], parts[1]);
        }

        private bool EvaluateChapterCondition(DialogueCondition condition)
        {
            if (_narrativeManager == null) return true;
            return _narrativeManager.IsStoryChapterUnlocked(condition.Parameter);
        }

        private bool EvaluateLevelCondition(DialogueCondition condition)
        {
            // TODO: Check player level
            if (int.TryParse(condition.ExpectedValue, out var requiredLevel))
            {
                // return playerLevel >= requiredLevel;
                return true;
            }
            return true;
        }

        private bool EvaluateTimeCondition(DialogueCondition condition)
        {
            // TODO: Check time of day
            return true;
        }

        private bool EvaluateWeatherCondition(DialogueCondition condition)
        {
            // TODO: Check weather
            return true;
        }

        // ==========================================================
        // STATE QUERIES
        // ==========================================================

        /// <summary>
        /// Checks if a conversation is currently active.
        /// </summary>
        public bool IsInConversation => _isInConversation;

        /// <summary>
        /// Gets the active conversation ID.
        /// </summary>
        public string? ActiveConversationId => _activeConversationId;

        /// <summary>
        /// Gets the active dialogue ID.
        /// </summary>
        public string? ActiveDialogueId => _activeDialogueId;

        /// <summary>
        /// Gets the current dialogue entry.
        /// </summary>
        public DialogueEntry? GetCurrentDialogue()
        {
            if (_activeDialogueId == null) return null;
            return DialogueDatabase.GetDialogue(_activeDialogueId);
        }

        /// <summary>
        /// Gets available choices for the current dialogue.
        /// </summary>
        public List<DialogueChoice> GetCurrentChoices()
        {
            var dialogue = GetCurrentDialogue();
            if (dialogue == null) return new List<DialogueChoice>();

            return GetAvailableChoices(dialogue);
        }

        // ==========================================================
        // SAVE / LOAD
        // ==========================================================

        /// <summary>
        /// Captures dialogue manager state for saving.
        /// </summary>
        public DialogueManagerSaveData GetSaveData()
        {
            return new DialogueManagerSaveData
            {
                ActiveConversationId = _activeConversationId,
                ActiveDialogueId = _activeDialogueId,
                CurrentDepth = _currentDepth,
                IsInConversation = _isInConversation,
                VisitedDialogues = _visitedDialogues.ToList()
            };
        }

        /// <summary>
        /// Restores dialogue manager state from saved data.
        /// </summary>
        public void LoadSaveData(DialogueManagerSaveData saveData)
        {
            if (saveData == null) return;

            _activeConversationId = saveData.ActiveConversationId;
            _activeDialogueId = saveData.ActiveDialogueId;
            _currentDepth = saveData.CurrentDepth;
            _isInConversation = saveData.IsInConversation;
            _visitedDialogues.Clear();
            _visitedDialogues.UnionWith(saveData.VisitedDialogues);
        }

        /// <summary>
        /// Clears all state (for testing).
        /// </summary>
        public void Clear()
        {
            _activeConversationId = null;
            _activeDialogueId = null;
            _currentDepth = 0;
            _isInConversation = false;
            _visitedDialogues.Clear();
        }
    }

    // ==========================================================
    // SAVE DATA
    // ==========================================================

    public class DialogueManagerSaveData
    {
        public string? ActiveConversationId { get; set; }
        public string? ActiveDialogueId { get; set; }
        public int CurrentDepth { get; set; }
        public bool IsInConversation { get; set; }
        public List<string> VisitedDialogues { get; set; } = new();
    }
}