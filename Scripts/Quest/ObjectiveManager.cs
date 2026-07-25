using System;
using System.Collections.Generic;
using System.Linq;

namespace HeroOfEternia.Quest
{
    /// <summary>
    /// Manages objective progression for quest instances.
    /// Supports unlimited objective chains, branching, optional objectives,
    /// and custom completion checks.
    /// </summary>
    public class ObjectiveManager
    {
        // ==========================================================
        // EVENTS
        // ==========================================================
        public event Action<string, string>? ObjectiveActivated;    // (questId, objectiveId)
        public event Action<string, string>? ObjectiveCompleted;    // (questId, objectiveId)
        public event Action<string, string>? ObjectiveFailed;       // (questId, objectiveId)
        public event Action<string, string, int>? ObjectiveProgress; // (questId, objectiveId, newCount)
        public event Action<string, string>? BranchCompleted;       // (questId, branchId)
        public event Action<string>? AllObjectivesCompleted;        // (questId)

        // ==========================================================
        // OBJECTIVE PROGRESSION
        // ==========================================================

        /// <summary>
        /// Initializes objective states for a quest instance based on its active branch.
        /// </summary>
        public void InitializeObjectives(QuestInstance instance)
        {
            var definition = instance.GetDefinition();
            if (definition == null) return;

            var branch = GetActiveBranch(definition, instance.ActiveBranchId);
            if (branch == null) return;

            instance.ObjectiveStates.Clear();

            foreach (var objDef in branch.Objectives)
            {
                var state = new ObjectiveRuntimeState
                {
                    ObjectiveId = objDef.ObjectiveId,
                    State = objDef.InitialState,
                    CurrentCount = 0,
                    CurrentFloat = 0f,
                    IsCompleted = false,
                    IsFailed = false
                };

                // Check if this objective has prerequisites that are not yet met
                if (objDef.PrerequisiteObjectiveIds.Count > 0)
                {
                    bool allPrereqsMet = objDef.PrerequisiteObjectiveIds
                        .All(prereqId => instance.ObjectiveStates
                            .Any(os => os.ObjectiveId == prereqId && os.IsCompleted));

                    if (!allPrereqsMet)
                        state.State = ObjectiveState.Locked;
                }

                instance.ObjectiveStates.Add(state);
            }

            // Activate the first unlocked objective
            ActivateNextObjectives(instance);
        }

        /// <summary>
        /// Advances an objective's count by the specified amount.
        /// Returns true if the objective was completed as a result.
        /// </summary>
        public bool AdvanceObjective(QuestInstance instance, string objectiveId, int amount = 1)
        {
            var state = instance.ObjectiveStates.FirstOrDefault(os => os.ObjectiveId == objectiveId);
            if (state == null || state.IsCompleted || state.IsFailed) return false;
            if (state.State == ObjectiveState.Locked) return false;

            state.CurrentCount += amount;
            ObjectiveProgress?.Invoke(instance.QuestId, objectiveId, state.CurrentCount);

            var definition = instance.GetDefinition();
            if (definition == null) return false;

            var branch = GetActiveBranch(definition, instance.ActiveBranchId);
            if (branch == null) return false;

            var objDef = branch.Objectives.FirstOrDefault(o => o.ObjectiveId == objectiveId);
            if (objDef == null) return false;

            // Check if objective is complete
            if (state.CurrentCount >= objDef.RequiredCount)
            {
                CompleteObjective(instance, objectiveId);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Advances an objective's float value (for timed objectives, distances, etc.).
        /// Returns true if the objective was completed as a result.
        /// </summary>
        public bool AdvanceObjectiveFloat(QuestInstance instance, string objectiveId, float amount)
        {
            var state = instance.ObjectiveStates.FirstOrDefault(os => os.ObjectiveId == objectiveId);
            if (state == null || state.IsCompleted || state.IsFailed) return false;
            if (state.State == ObjectiveState.Locked) return false;

            state.CurrentFloat += amount;
            ObjectiveProgress?.Invoke(instance.QuestId, objectiveId, (int)state.CurrentFloat);

            var definition = instance.GetDefinition();
            if (definition == null) return false;

            var branch = GetActiveBranch(definition, instance.ActiveBranchId);
            if (branch == null) return false;

            var objDef = branch.Objectives.FirstOrDefault(o => o.ObjectiveId == objectiveId);
            if (objDef == null) return false;

            if (state.CurrentFloat >= objDef.RequiredFloat)
            {
                CompleteObjective(instance, objectiveId);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets an objective's count to a specific value.
        /// </summary>
        public bool SetObjectiveCount(QuestInstance instance, string objectiveId, int count)
        {
            var state = instance.ObjectiveStates.FirstOrDefault(os => os.ObjectiveId == objectiveId);
            if (state == null || state.IsCompleted || state.IsFailed) return false;
            if (state.State == ObjectiveState.Locked) return false;

            state.CurrentCount = count;
            ObjectiveProgress?.Invoke(instance.QuestId, objectiveId, count);

            var definition = instance.GetDefinition();
            if (definition == null) return false;

            var branch = GetActiveBranch(definition, instance.ActiveBranchId);
            if (branch == null) return false;

            var objDef = branch.Objectives.FirstOrDefault(o => o.ObjectiveId == objectiveId);
            if (objDef == null) return false;

            if (state.CurrentCount >= objDef.RequiredCount)
            {
                CompleteObjective(instance, objectiveId);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Completes an objective and triggers branching logic.
        /// </summary>
        public void CompleteObjective(QuestInstance instance, string objectiveId)
        {
            var state = instance.ObjectiveStates.FirstOrDefault(os => os.ObjectiveId == objectiveId);
            if (state == null || state.IsCompleted) return;

            state.IsCompleted = true;
            state.State = ObjectiveState.Completed;
            ObjectiveCompleted?.Invoke(instance.QuestId, objectiveId);

            var definition = instance.GetDefinition();
            if (definition == null) return;

            var branch = GetActiveBranch(definition, instance.ActiveBranchId);
            if (branch == null) return;

            var objDef = branch.Objectives.FirstOrDefault(o => o.ObjectiveId == objectiveId);
            if (objDef == null) return;

            // Check for branch transition
            if (!string.IsNullOrEmpty(objDef.OnCompleteBranchId))
            {
                TransitionToBranch(instance, objDef.OnCompleteBranchId);
                return;
            }

            // Activate next objectives in chain
            ActivateNextObjectives(instance);

            // Check if all objectives in current branch are complete
            if (AreAllBranchObjectivesComplete(instance, branch))
            {
                BranchCompleted?.Invoke(instance.QuestId, branch.BranchId);

                // Check for next branch
                if (!string.IsNullOrEmpty(branch.OnCompleteBranchId))
                {
                    TransitionToBranch(instance, branch.OnCompleteBranchId);
                }
                else
                {
                    AllObjectivesCompleted?.Invoke(instance.QuestId);
                }
            }
        }

        /// <summary>
        /// Fails an objective and triggers failure branching.
        /// </summary>
        public void FailObjective(QuestInstance instance, string objectiveId)
        {
            var state = instance.ObjectiveStates.FirstOrDefault(os => os.ObjectiveId == objectiveId);
            if (state == null || state.IsFailed) return;

            state.IsFailed = true;
            state.State = ObjectiveState.Failed;
            ObjectiveFailed?.Invoke(instance.QuestId, objectiveId);

            var definition = instance.GetDefinition();
            if (definition == null) return;

            var branch = GetActiveBranch(definition, instance.ActiveBranchId);
            if (branch == null) return;

            var objDef = branch.Objectives.FirstOrDefault(o => o.ObjectiveId == objectiveId);
            if (objDef == null) return;

            // Check for failure branch transition
            if (!string.IsNullOrEmpty(objDef.OnFailBranchId))
            {
                TransitionToBranch(instance, objDef.OnFailBranchId);
            }
            else if (!string.IsNullOrEmpty(branch.OnFailBranchId))
            {
                TransitionToBranch(instance, branch.OnFailBranchId);
            }
        }

        // ==========================================================
        // BRANCH MANAGEMENT
        // ==========================================================

        /// <summary>
        /// Transitions the quest to a new branch.
        /// </summary>
        public void TransitionToBranch(QuestInstance instance, string branchId)
        {
            var definition = instance.GetDefinition();
            if (definition == null) return;

            var branch = definition.Branches.FirstOrDefault(b => b.BranchId == branchId);
            if (branch == null) return;

            // Check branch conditions
            if (!EvaluateBranchConditions(branch, instance))
                return;

            instance.ActiveBranchId = branchId;
            InitializeObjectives(instance);
        }

        /// <summary>
        /// Gets the active branch for a quest instance.
        /// </summary>
        public QuestBranch? GetActiveBranch(QuestDefinition definition, string branchId)
        {
            if (string.IsNullOrEmpty(branchId) && definition.Branches.Count > 0)
                return definition.Branches[0]; // default to first branch

            return definition.Branches.FirstOrDefault(b => b.BranchId == branchId);
        }

        /// <summary>
        /// Evaluates whether a branch's conditions are met.
        /// </summary>
        public bool EvaluateBranchConditions(QuestBranch branch, QuestInstance instance)
        {
            foreach (var condition in branch.Conditions)
            {
                if (!EvaluatePrerequisite(condition, instance))
                    return false;
            }
            return true;
        }

        // ==========================================================
        // PREREQUISITE EVALUATION
        // ==========================================================

        /// <summary>
        /// Evaluates a single prerequisite condition.
        /// </summary>
        public bool EvaluatePrerequisite(QuestPrerequisite prereq, QuestInstance instance)
        {
            switch (prereq.PrerequisiteType.ToLowerInvariant())
            {
                case "quest_completed":
                    return instance.QuestId == prereq.RequiredId && instance.State == QuestState.Completed;

                case "quest_active":
                    return instance.QuestId == prereq.RequiredId && instance.State == QuestState.Active;

                case "objective_completed":
                    return instance.ObjectiveStates.Any(os => os.ObjectiveId == prereq.RequiredId && os.IsCompleted);

                case "objective_active":
                    return instance.ObjectiveStates.Any(os => os.ObjectiveId == prereq.RequiredId && os.State == ObjectiveState.Active);

                case "variable":
                    return instance.QuestVariables.TryGetValue(prereq.RequiredId, out var val) &&
                           float.TryParse(val, out var fVal) && fVal >= prereq.RequiredValue;

                case "flag":
                    return instance.QuestVariables.ContainsKey(prereq.RequiredId);

                case "custom":
                    // Custom checks are handled by external systems via hooks
                    return true; // default to true for custom checks

                default:
                    return true;
            }
        }

        // ==========================================================
        // HELPERS
        // ==========================================================

        private void ActivateNextObjectives(QuestInstance instance)
        {
            foreach (var state in instance.ObjectiveStates)
            {
                if (state.State != ObjectiveState.Locked) continue;

                var definition = instance.GetDefinition();
                if (definition == null) continue;

                var branch = GetActiveBranch(definition, instance.ActiveBranchId);
                if (branch == null) continue;

                var objDef = branch.Objectives.FirstOrDefault(o => o.ObjectiveId == state.ObjectiveId);
                if (objDef == null) continue;

                // Check if all prerequisites are met
                bool allPrereqsMet = objDef.PrerequisiteObjectiveIds
                    .All(prereqId => instance.ObjectiveStates
                        .Any(os => os.ObjectiveId == prereqId && os.IsCompleted));

                if (allPrereqsMet)
                {
                    state.State = objDef.IsOptional ? ObjectiveState.Optional : ObjectiveState.Active;
                    ObjectiveActivated?.Invoke(instance.QuestId, state.ObjectiveId);
                }
            }
        }

        private bool AreAllBranchObjectivesComplete(QuestInstance instance, QuestBranch branch)
        {
            return branch.Objectives
                .Where(o => !o.IsOptional) // optional objectives don't block completion
                .All(o => instance.ObjectiveStates
                    .Any(os => os.ObjectiveId == o.ObjectiveId && os.IsCompleted));
        }

        /// <summary>
        /// Gets progress info for a specific objective.
        /// </summary>
        public ObjectiveProgressInfo GetObjectiveProgress(QuestInstance instance, string objectiveId)
        {
            var state = instance.ObjectiveStates.FirstOrDefault(os => os.ObjectiveId == objectiveId);
            if (state == null) return new ObjectiveProgressInfo();

            var definition = instance.GetDefinition();
            if (definition == null) return new ObjectiveProgressInfo();

            var branch = GetActiveBranch(definition, instance.ActiveBranchId);
            if (branch == null) return new ObjectiveProgressInfo();

            var objDef = branch.Objectives.FirstOrDefault(o => o.ObjectiveId == objectiveId);
            if (objDef == null) return new ObjectiveProgressInfo();

            return new ObjectiveProgressInfo
            {
                ObjectiveId = objectiveId,
                State = state.State,
                CurrentCount = state.CurrentCount,
                RequiredCount = objDef.RequiredCount,
                CurrentFloat = state.CurrentFloat,
                RequiredFloat = objDef.RequiredFloat,
                IsCompleted = state.IsCompleted,
                IsOptional = objDef.IsOptional,
                DescriptionKey = objDef.DescriptionKey
            };
        }

        /// <summary>
        /// Gets progress info for all objectives in a quest instance.
        /// </summary>
        public List<ObjectiveProgressInfo> GetAllObjectiveProgress(QuestInstance instance)
        {
            var result = new List<ObjectiveProgressInfo>();
            var definition = instance.GetDefinition();
            if (definition == null) return result;

            var branch = GetActiveBranch(definition, instance.ActiveBranchId);
            if (branch == null) return result;

            foreach (var objDef in branch.Objectives)
            {
                result.Add(GetObjectiveProgress(instance, objDef.ObjectiveId));
            }

            return result;
        }
    }

    // ==========================================================
    // OBJECTIVE PROGRESS INFO (for UI)
    // ==========================================================
    public class ObjectiveProgressInfo
    {
        public string ObjectiveId { get; set; } = "";
        public ObjectiveState State { get; set; } = ObjectiveState.Locked;
        public int CurrentCount { get; set; } = 0;
        public int RequiredCount { get; set; } = 1;
        public float CurrentFloat { get; set; } = 0f;
        public float RequiredFloat { get; set; } = 0f;
        public bool IsCompleted { get; set; } = false;
        public bool IsOptional { get; set; } = false;
        public string DescriptionKey { get; set; } = "";

        public float ProgressPercent
        {
            get
            {
                if (RequiredCount > 1) return Math.Clamp((float)CurrentCount / RequiredCount, 0f, 1f);
                if (RequiredFloat > 0f) return Math.Clamp(CurrentFloat / RequiredFloat, 0f, 1f);
                return IsCompleted ? 1f : 0f;
            }
        }

        public string ProgressText
        {
            get
            {
                if (RequiredCount > 1) return $"{CurrentCount}/{RequiredCount}";
                if (RequiredFloat > 0f) return $"{CurrentFloat:F1}/{RequiredFloat:F1}";
                return IsCompleted ? "✓" : "○";
            }
        }
    }
}