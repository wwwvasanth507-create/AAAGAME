using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;

namespace HeroOfEternia.Quest
{
    /// <summary>
    /// Central quest orchestrator managing the full quest lifecycle.
    /// Handles acceptance, progress tracking, completion, failure,
    /// abandonment, retry, reward distribution, and save/load.
    /// </summary>
    public class QuestManager
    {
        // ==========================================================
        // EVENTS
        // ==========================================================
        public event Action<QuestInstance>? QuestAccepted;
        public event Action<QuestInstance>? QuestCompleted;
        public event Action<QuestInstance>? QuestFailed;
        public event Action<QuestInstance>? QuestAbandoned;
        public event Action<QuestInstance>? QuestRetried;
        public event Action<QuestInstance>? QuestProgressed;
        public event Action<QuestInstance>? QuestBranchChanged;

        // ==========================================================
        // INTERNAL STATE
        // ==========================================================
        private readonly Dictionary<string, QuestInstance> _activeQuests = new();      // instanceId -> instance
        private readonly Dictionary<string, List<string>> _questInstances = new();      // questId -> instanceIds
        private readonly List<string> _completedQuestIds = new();
        private readonly List<string> _failedQuestIds = new();
        private readonly List<string> _abandonedQuestIds = new();
        private readonly Dictionary<string, int> _repeatCounts = new();                 // questId -> repeat count
        private readonly List<QuestHistoryEntry> _questHistory = new();
        
        private readonly ObjectiveManager _objectiveManager;
        private int _nextInstanceId = 1;

        // ==========================================================
        // CONSTRUCTION
        // ==========================================================

        public QuestManager()
        {
            _objectiveManager = new ObjectiveManager();
            WireObjectiveEvents();
        }

        private void WireObjectiveEvents()
        {
            _objectiveManager.ObjectiveCompleted += OnObjectiveCompleted;
            _objectiveManager.ObjectiveFailed += OnObjectiveFailed;
            _objectiveManager.AllObjectivesCompleted += OnAllObjectivesCompleted;
            _objectiveManager.BranchCompleted += OnBranchCompleted;
        }

        // ==========================================================
        // QUEST LIFECYCLE
        // ==========================================================

        /// <summary>
        /// Accepts a quest, creating a new runtime instance.
        /// Returns the quest instance, or null if prerequisites not met.
        /// </summary>
        public QuestInstance? AcceptQuest(string questId)
        {
            var definition = QuestDatabase.GetQuest(questId);
            if (definition == null) return null;
            if (!definition.IsEnabled) return null;

            // Check prerequisites
            if (!EvaluateGlobalPrerequisites(definition))
                return null;

            // Check repeatability
            if (!CanAcceptQuest(questId))
                return null;

            // Create instance
            var instance = new QuestInstance
            {
                InstanceId = GenerateInstanceId(),
                QuestId = questId,
                State = QuestState.Active,
                ActiveBranchId = definition.Branches.Count > 0 ? definition.Branches[0].BranchId : "",
                AcceptedTime = DateTime.UtcNow,
                TimeRemaining = definition.TimeLimit.HasTimeLimit ? definition.TimeLimit.TotalSeconds : 0f,
                RepeatCount = _repeatCounts.GetValueOrDefault(questId, 0)
            };

            // Initialize objectives
            _objectiveManager.InitializeObjectives(instance);

            // Register
            _activeQuests[instance.InstanceId] = instance;
            if (!_questInstances.ContainsKey(questId))
                _questInstances[questId] = new List<string>();
            _questInstances[questId].Add(instance.InstanceId);

            // Track repeat count
            if (!_repeatCounts.ContainsKey(questId))
                _repeatCounts[questId] = 0;
            _repeatCounts[questId]++;

            QuestAccepted?.Invoke(instance);
            return instance;
        }

        /// <summary>
        /// Checks if a quest can be accepted based on prerequisites and repeatability.
        /// </summary>
        public bool CanAcceptQuest(string questId)
        {
            var definition = QuestDatabase.GetQuest(questId);
            if (definition == null) return false;

            // Check if already active
            if (_questInstances.TryGetValue(questId, out var instances))
            {
                foreach (var instId in instances)
                {
                    if (_activeQuests.TryGetValue(instId, out var inst) && inst.State == QuestState.Active)
                        return false;
                }
            }

            // Check repeatability
            if (!definition.Repeatable)
            {
                if (_completedQuestIds.Contains(questId) || _failedQuestIds.Contains(questId))
                    return false;
            }
            else
            {
                // Check max repeat count
                int currentRepeats = _repeatCounts.GetValueOrDefault(questId, 0);
                if (definition.MaxRepeatCount > 0 && currentRepeats >= definition.MaxRepeatCount)
                    return false;

                // Check schedule (daily/weekly)
                if (!string.IsNullOrEmpty(definition.RepeatSchedule))
                {
                    if (definition.RepeatSchedule == "daily" && !IsNewDay())
                        return false;
                    if (definition.RepeatSchedule == "weekly" && !IsNewWeek())
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the active quest instance for a given quest ID.
        /// Returns null if the quest is not currently active.
        /// </summary>
        public QuestInstance? GetActiveInstance(string questId)
        {
            if (!_questInstances.TryGetValue(questId, out var instances))
                return null;

            foreach (var instId in instances)
            {
                if (_activeQuests.TryGetValue(instId, out var inst) && inst.State == QuestState.Active)
                    return inst;
            }

            return null;
        }

        /// <summary>
        /// Gets all active quest instances.
        /// </summary>
        public List<QuestInstance> GetActiveQuests()
        {
            return _activeQuests.Values
                .Where(q => q.State == QuestState.Active)
                .ToList();
        }

        /// <summary>
        /// Gets quests by category that are available for the player to accept.
        /// </summary>
        public List<QuestDefinition> GetAvailableQuests(QuestCategory? category = null)
        {
            var allQuests = category.HasValue
                ? QuestDatabase.GetQuestsByCategory(category.Value)
                : QuestDatabase.GetEnabledQuests();

            return allQuests.Where(q => CanAcceptQuest(q.QuestId)).ToList();
        }

        // ==========================================================
        // OBJECTIVE ADVANCEMENT
        // ==========================================================

        /// <summary>
        /// Advances an objective's count for the active instance of a quest.
        /// </summary>
        public bool AdvanceObjective(string questId, string objectiveId, int amount = 1)
        {
            var instance = GetActiveInstance(questId);
            if (instance == null) return false;

            bool result = _objectiveManager.AdvanceObjective(instance, objectiveId, amount);
            if (result) QuestProgressed?.Invoke(instance);
            return result;
        }

        /// <summary>
        /// Advances an objective's float value for the active instance.
        /// </summary>
        public bool AdvanceObjectiveFloat(string questId, string objectiveId, float amount)
        {
            var instance = GetActiveInstance(questId);
            if (instance == null) return false;

            bool result = _objectiveManager.AdvanceObjectiveFloat(instance, objectiveId, amount);
            if (result) QuestProgressed?.Invoke(instance);
            return result;
        }

        /// <summary>
        /// Sets an objective's count directly for the active instance.
        /// </summary>
        public bool SetObjectiveCount(string questId, string objectiveId, int count)
        {
            var instance = GetActiveInstance(questId);
            if (instance == null) return false;

            bool result = _objectiveManager.SetObjectiveCount(instance, objectiveId, count);
            if (result) QuestProgressed?.Invoke(instance);
            return result;
        }

        // ==========================================================
        // QUEST COMPLETION / FAILURE / ABANDONMENT
        // ==========================================================

        /// <summary>
        /// Completes a quest, distributing rewards.
        /// </summary>
        public void CompleteQuest(string instanceId)
        {
            if (!_activeQuests.TryGetValue(instanceId, out var instance)) return;
            if (instance.State != QuestState.Active) return;

            instance.State = QuestState.Completed;
            instance.HasCompleted = true;
            instance.CompletedTime = DateTime.UtcNow;

            // Add to completed list
            if (!_completedQuestIds.Contains(instance.QuestId))
                _completedQuestIds.Add(instance.QuestId);

            // Add to history
            AddToHistory(instance, QuestState.Completed);

            // Distribute rewards
            DistributeRewards(instance);

            // Remove from active
            _activeQuests.Remove(instanceId);

            QuestCompleted?.Invoke(instance);
        }

        /// <summary>
        /// Fails a quest.
        /// </summary>
        public void FailQuest(string instanceId)
        {
            if (!_activeQuests.TryGetValue(instanceId, out var instance)) return;
            if (instance.State != QuestState.Active) return;

            instance.State = QuestState.Failed;
            instance.HasFailed = true;

            if (!_failedQuestIds.Contains(instance.QuestId))
                _failedQuestIds.Add(instance.QuestId);

            AddToHistory(instance, QuestState.Failed);

            // Apply failure penalties
            ApplyFailurePenalties(instance);

            _activeQuests.Remove(instanceId);

            QuestFailed?.Invoke(instance);
        }

        /// <summary>
        /// Abandons a quest.
        /// </summary>
        public void AbandonQuest(string instanceId)
        {
            if (!_activeQuests.TryGetValue(instanceId, out var instance)) return;
            if (instance.State != QuestState.Active) return;

            instance.State = QuestState.Abandoned;

            if (!_abandonedQuestIds.Contains(instance.QuestId))
                _abandonedQuestIds.Add(instance.QuestId);

            AddToHistory(instance, QuestState.Abandoned);

            _activeQuests.Remove(instanceId);

            QuestAbandoned?.Invoke(instance);
        }

        /// <summary>
        /// Retries a failed or abandoned quest.
        /// </summary>
        public QuestInstance? RetryQuest(string questId)
        {
            // Remove from failed/abandoned lists
            _failedQuestIds.Remove(questId);
            _abandonedQuestIds.Remove(questId);

            var instance = AcceptQuest(questId);
            if (instance != null)
            {
                instance.State = QuestState.RetryReady;
                QuestRetried?.Invoke(instance);
            }

            return instance;
        }

        // ==========================================================
        // TIME MANAGEMENT
        // ==========================================================

        /// <summary>
        /// Updates timed quests, checking time limits and advancing survival objectives.
        /// Call from GameLoop tick.
        /// </summary>
        public void Update(float deltaTime)
        {
            var toRemove = new List<string>();

            foreach (var kvp in _activeQuests)
            {
                var instance = kvp.Value;
                if (instance.State != QuestState.Active) continue;

                var definition = instance.GetDefinition();
                if (definition == null) continue;

                // Update time limit
                if (definition.TimeLimit.HasTimeLimit)
                {
                    instance.TimeRemaining -= deltaTime;
                    if (instance.TimeRemaining <= 0f && definition.TimeLimit.FailOnExpire)
                    {
                        toRemove.Add(kvp.Key);
                        FailQuest(kvp.Key);
                        continue;
                    }
                }

                // Update survival objectives
                foreach (var state in instance.ObjectiveStates)
                {
                    if (state.State != ObjectiveState.Active || state.IsCompleted) continue;

                    var branch = _objectiveManager.GetActiveBranch(definition, instance.ActiveBranchId);
                    if (branch == null) continue;

                    var objDef = branch.Objectives.FirstOrDefault(o => o.ObjectiveId == state.ObjectiveId);
                    if (objDef == null) continue;

                    if (objDef.Type == ObjectiveType.Survive)
                    {
                        state.CurrentFloat += deltaTime;
                        if (state.CurrentFloat >= objDef.RequiredFloat)
                        {
                            _objectiveManager.CompleteObjective(instance, state.ObjectiveId);
                        }
                    }
                }

                // Track play time
                instance.TotalPlayTimeOnQuest += deltaTime;
            }
        }

        // ==========================================================
        // REWARD DISTRIBUTION
        // ==========================================================

        private void DistributeRewards(QuestInstance instance)
        {
            var definition = instance.GetDefinition();
            if (definition == null) return;

            foreach (var reward in definition.CompletionRewards)
            {
                ApplyReward(reward);
            }

            // Check for optional objective rewards
            foreach (var state in instance.ObjectiveStates)
            {
                if (state.IsCompleted)
                {
                    var branch = _objectiveManager.GetActiveBranch(definition, instance.ActiveBranchId);
                    if (branch == null) continue;

                    var objDef = branch.Objectives.FirstOrDefault(o => o.ObjectiveId == state.ObjectiveId);
                    if (objDef != null && objDef.IsOptional)
                    {
                        foreach (var reward in definition.OptionalObjectiveRewards)
                        {
                            ApplyReward(reward);
                        }
                    }
                }
            }
        }

        private void ApplyReward(QuestReward reward)
        {
            if (!reward.Guaranteed)
            {
                var random = new Random();
                if (random.NextDouble() > reward.Chance) return;
            }

            // Handle choice groups
            if (reward.ChoiceGroup.Count > 0)
            {
                // Choice groups are resolved by UI - for now, apply first option
                ApplyReward(reward.ChoiceGroup[0]);
                return;
            }

            // TODO: Integrate with actual systems (inventory, progression, reputation)
            // For now, log the reward
            Godot.GD.Print($"[QuestManager] Reward: {reward.Type} {reward.RewardId} x{reward.Quantity} value={reward.FloatValue}");
        }

        private void ApplyFailurePenalties(QuestInstance instance)
        {
            var definition = instance.GetDefinition();
            if (definition == null) return;

            foreach (var penalty in definition.FailurePenalties)
            {
                // Apply negative rewards (penalties)
                Godot.GD.Print($"[QuestManager] Penalty: {penalty.Type} {penalty.RewardId} x{penalty.Quantity}");
            }
        }

        // ==========================================================
        // PREREQUISITE EVALUATION
        // ==========================================================

        private bool EvaluateGlobalPrerequisites(QuestDefinition definition)
        {
            foreach (var prereq in definition.Prerequisites)
            {
                switch (prereq.PrerequisiteType.ToLowerInvariant())
                {
                    case "quest_completed":
                        if (!_completedQuestIds.Contains(prereq.RequiredId))
                            return false;
                        break;

                    case "quest_not_completed":
                        if (_completedQuestIds.Contains(prereq.RequiredId))
                            return false;
                        break;

                    case "quest_active":
                        if (!_activeQuests.Values.Any(q => q.QuestId == prereq.RequiredId && q.State == QuestState.Active))
                            return false;
                        break;

                    case "level":
                        // TODO: Check player level against RequiredValue
                        break;

                    case "faction":
                        // TODO: Check faction membership
                        break;

                    case "reputation":
                        // TODO: Check reputation threshold
                        break;

                    case "item":
                        // TODO: Check if player has item
                        break;

                    case "ability":
                        // TODO: Check if player has ability
                        break;

                    case "flag":
                        // TODO: Check global flag
                        break;

                    case "custom":
                        // Custom checks are handled externally
                        break;
                }
            }
            return true;
        }

        // ==========================================================
        // EVENT HANDLERS
        // ==========================================================

        private void OnObjectiveCompleted(string questId, string objectiveId)
        {
            var instance = GetActiveInstance(questId);
            if (instance != null)
                QuestProgressed?.Invoke(instance);
        }

        private void OnObjectiveFailed(string questId, string objectiveId)
        {
            var definition = QuestDatabase.GetQuest(questId);
            if (definition == null) return;

            // Check if any failure condition triggers quest failure
            foreach (var condition in definition.FailureConditions)
            {
                if (condition.Type == "objective_failed" && condition.CustomCheckId == objectiveId)
                {
                    var instance = GetActiveInstance(questId);
                    if (instance != null)
                        FailQuest(instance.InstanceId);
                    return;
                }
            }
        }

        private void OnAllObjectivesCompleted(string questId)
        {
            var instance = GetActiveInstance(questId);
            if (instance != null)
                CompleteQuest(instance.InstanceId);
        }

        private void OnBranchCompleted(string questId, string branchId)
        {
            var instance = GetActiveInstance(questId);
            if (instance != null)
                QuestBranchChanged?.Invoke(instance);
        }

        // ==========================================================
        // JOURNAL / HISTORY
        // ==========================================================

        private void AddToHistory(QuestInstance instance, QuestState finalState)
        {
            var definition = instance.GetDefinition();
            _questHistory.Add(new QuestHistoryEntry
            {
                QuestId = instance.QuestId,
                QuestName = definition?.DisplayName ?? instance.QuestId,
                Category = definition?.Category ?? QuestCategory.Side,
                FinalState = finalState,
                AcceptedTime = instance.AcceptedTime,
                CompletionTime = instance.CompletedTime,
                TimeSpentSeconds = instance.TotalPlayTimeOnQuest,
                CompletedBy = instance.ActiveBranchId
            });
        }

        /// <summary>
        /// Gets the quest history.
        /// </summary>
        public List<QuestHistoryEntry> GetQuestHistory()
        {
            return _questHistory.ToList();
        }

        /// <summary>
        /// Gets completed quest IDs.
        /// </summary>
        public List<string> GetCompletedQuestIds()
        {
            return _completedQuestIds.ToList();
        }

        /// <summary>
        /// Gets failed quest IDs.
        /// </summary>
        public List<string> GetFailedQuestIds()
        {
            return _failedQuestIds.ToList();
        }

        /// <summary>
        /// Gets abandoned quest IDs.
        /// </summary>
        public List<string> GetAbandonedQuestIds()
        {
            return _abandonedQuestIds.ToList();
        }

        // ==========================================================
        // OBJECTIVE MANAGER ACCESS
        // ==========================================================

        public ObjectiveManager GetObjectiveManager() => _objectiveManager;

        /// <summary>
        /// Gets progress info for all objectives in a quest.
        /// </summary>
        public List<ObjectiveProgressInfo> GetQuestProgress(string questId)
        {
            var instance = GetActiveInstance(questId);
            if (instance == null) return new List<ObjectiveProgressInfo>();

            return _objectiveManager.GetAllObjectiveProgress(instance);
        }

        /// <summary>
        /// Checks if a quest has been completed.
        /// </summary>
        public bool IsQuestCompleted(string questId) => _completedQuestIds.Contains(questId);

        /// <summary>
        /// Checks if a quest has been failed.
        /// </summary>
        public bool IsQuestFailed(string questId) => _failedQuestIds.Contains(questId);

        // ==========================================================
        // SAVE / LOAD
        // ==========================================================

        /// <summary>
        /// Captures save state.
        /// </summary>
        public QuestSaveData GetSaveData()
        {
            var saveData = new QuestSaveData
            {
                ActiveQuests = _activeQuests.Values.Where(q => q.State == QuestState.Active).ToList(),
                CompletedQuestIds = _completedQuestIds.ToList(),
                FailedQuestIds = _failedQuestIds.ToList(),
                AbandonedQuestIds = _abandonedQuestIds.ToList(),
                RepeatCounts = new Dictionary<string, int>(_repeatCounts),
                QuestHistory = _questHistory.ToList()
            };

            return saveData;
        }

        /// <summary>
        /// Restores state from save data.
        /// </summary>
        public void LoadSaveData(QuestSaveData saveData)
        {
            if (saveData == null) return;

            _activeQuests.Clear();
            _questInstances.Clear();
            _completedQuestIds.Clear();
            _failedQuestIds.Clear();
            _abandonedQuestIds.Clear();
            _repeatCounts.Clear();
            _questHistory.Clear();

            foreach (var quest in saveData.ActiveQuests)
            {
                _activeQuests[quest.InstanceId] = quest;
                if (!_questInstances.ContainsKey(quest.QuestId))
                    _questInstances[quest.QuestId] = new List<string>();
                _questInstances[quest.QuestId].Add(quest.InstanceId);
            }

            _completedQuestIds.AddRange(saveData.CompletedQuestIds);
            _failedQuestIds.AddRange(saveData.FailedQuestIds);
            _abandonedQuestIds.AddRange(saveData.AbandonedQuestIds);

            foreach (var kvp in saveData.RepeatCounts)
                _repeatCounts[kvp.Key] = kvp.Value;

            _questHistory.AddRange(saveData.QuestHistory);
        }

        /// <summary>
        /// Clears all state (for testing).
        /// </summary>
        public void Clear()
        {
            _activeQuests.Clear();
            _questInstances.Clear();
            _completedQuestIds.Clear();
            _failedQuestIds.Clear();
            _abandonedQuestIds.Clear();
            _repeatCounts.Clear();
            _questHistory.Clear();
            _nextInstanceId = 1;
        }

        // ==========================================================
        // HELPERS
        // ==========================================================

        private string GenerateInstanceId()
        {
            return $"qinst_{_nextInstanceId++}_{DateTime.UtcNow.Ticks}";
        }

        private static bool IsNewDay()
        {
            // Simplified: returns true if a new calendar day has started
            // In production, this would track the last reset time
            return true;
        }

        private static bool IsNewWeek()
        {
            // Simplified: returns true if a new week has started
            return true;
        }

        /// <summary>
        /// Gets the number of active quests.
        /// </summary>
        public int ActiveQuestCount => _activeQuests.Count;
    }
}