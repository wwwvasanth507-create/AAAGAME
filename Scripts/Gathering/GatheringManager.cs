using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using HeroOfEternia.Core;
using HeroOfEternia.Inventory;

namespace HeroOfEternia.Gathering
{
    /// <summary>
    /// Event published when a resource node is gathered.
    /// </summary>
    public class GatherEvent
    {
        public string ResourceId { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public int YieldAmount { get; set; }
        public int ExperienceGained { get; set; }
        public bool IsCriticalGather { get; set; }
        public string ProfessionType { get; set; } = string.Empty;
        public string ToolUsed { get; set; } = string.Empty;
    }

    /// <summary>
    /// Event published when a resource node is fully depleted.
    /// </summary>
    public class NodeDepletedEvent
    {
        public string ResourceId { get; set; } = string.Empty;
        public string WorldPositionKey { get; set; } = string.Empty;
        public string ChunkKey { get; set; } = string.Empty;
        public float RespawnTimeSeconds { get; set; }
    }

    /// <summary>
    /// Event published when a resource node respawns.
    /// </summary>
    public class NodeRespawnedEvent
    {
        public string ResourceId { get; set; } = string.Empty;
        public string WorldPositionKey { get; set; } = string.Empty;
        public string ChunkKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// Central gathering manager for validating, executing, and tracking gather actions.
    /// Supports tool validation, animation triggers, resource health, critical gathers, 
    /// bonus yield, node depletion, and respawn hooks.
    /// </summary>
    public class GatheringManager : IInitializable
    {
        private static GatheringManager? _instance;
        public static GatheringManager Instance => _instance ??= new GatheringManager();

        private ResourceDatabase _resourceDb = null!;
        private ProfessionManager _professionManager = null!;
        private bool _isInitialized;

        /// <summary>Tracks active resource node states keyed by position key.</summary>
        private Dictionary<string, ResourceNodeState> _nodeStates = new();

        /// <summary>Tracks nodes pending respawn.</summary>
        private List<ResourceNodeState> _respawnQueue = new();

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _resourceDb = ResourceDatabase.Instance;
            _professionManager = ProfessionManager.Instance;

            if (!_resourceDb.IsInitialized)
                _resourceDb.Initialize();
            if (!_professionManager.IsInitialized)
                _professionManager.Initialize();

            GD.Print("[GatheringManager] Initialized.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
            _nodeStates.Clear();
            _respawnQueue.Clear();
        }

        /// <summary>
        /// Validates if a gather action can be performed.
        /// Returns a result code with failure reason.
        /// </summary>
        public GatherResult ValidateGather(string resourceId, string toolEquipped, int toolTier, 
            InventoryContainer playerInventory, ProfessionType? profession = null)
        {
            var resource = _resourceDb.GetResource(resourceId);
            if (resource == null)
                return new GatherResult(false, "Resource not found in database.");

            // Check if node is depleted
            var nodeState = GetOrCreateNodeState(resourceId, "");
            if (nodeState.IsDepleted)
                return new GatherResult(false, "Resource node is depleted and needs to respawn.");

            // Check tool requirement
            if (resource.ToolRequirement != "None" && 
                !string.IsNullOrEmpty(resource.ToolRequirement))
            {
                string requiredTool = resource.ToolRequirement;
                if (!toolEquipped.Equals(requiredTool, StringComparison.OrdinalIgnoreCase))
                    return new GatherResult(false, $"Requires tool: {requiredTool}");

                if (toolTier < resource.MinimumToolTier)
                    return new GatherResult(false, $"Requires tool tier {resource.MinimumToolTier} or higher.");
            }

            // Check if node has health remaining
            if (nodeState.CurrentHealth <= 0)
                return new GatherResult(false, "Resource node is depleted.");

            return new GatherResult(true, "");
        }

        /// <summary>
        /// Executes a gather action. Returns the result with yield and experience.
        /// </summary>
        public GatherResult ExecuteGather(string resourceId, string playerId, string toolEquipped, 
            int toolTier, InventoryContainer playerInventory, ProfessionType profession)
        {
            // Validate first
            var validation = ValidateGather(resourceId, toolEquipped, toolTier, playerInventory, profession);
            if (!validation.Success)
                return validation;

            var resource = _resourceDb.GetResource(resourceId)!;
            var nodeState = GetOrCreateNodeState(resourceId, "");

            // Calculate gather speed modifications from profession bonuses
            float gatherSpeedMod = _professionManager.GetBonus(profession, "gather_speed", 1.0f);
            float effectiveGatherTime = resource.BaseGatherTime / gatherSpeedMod;

            // Calculate bonus yield from profession bonuses
            float yieldMod = 1.0f + _professionManager.GetBonus(profession, "yield_bonus", 0.0f);

            // Check for critical gather
            float criticalChance = _professionManager.GetBonus(profession, "critical_chance", 0.0f);
            bool isCritical = new Random().NextDouble() < criticalChance;
            int criticalMultiplier = isCritical ? 2 : 1;

            // Calculate final yield
            int baseYield = resource.BaseYield;
            int finalYield = Mathf.RoundToInt(baseYield * yieldMod) * criticalMultiplier;

            // Damage the node
            nodeState.CurrentHealth--;

            // Award profession experience
            int baseXp = resource.BaseExperience;
            int xpGained = Mathf.RoundToInt(baseXp * gatherSpeedMod);

            if (profession != ProfessionType.Woodcutting && IsResourceRelatedProfession(resource, profession))
            {
                _professionManager.AddExperience(profession, xpGained);
            }

            // Add items to player inventory
            if (playerInventory != null && finalYield > 0)
            {
                playerInventory.AddItem(resourceId, finalYield);
            }

            // Check if node is now depleted
            if (nodeState.CurrentHealth <= 0 && resource.IsDepletable)
            {
                nodeState.IsDepleted = true;
                nodeState.RemainingRespawnTime = resource.RespawnTimeSeconds;

                var depletedEvent = new NodeDepletedEvent
                {
                    ResourceId = resourceId,
                    WorldPositionKey = "",
                    ChunkKey = nodeState.ChunkKey,
                    RespawnTimeSeconds = resource.RespawnTimeSeconds
                };
                EventBus.Publish(depletedEvent);
            }

            var gatherEvent = new GatherEvent
            {
                ResourceId = resourceId,
                PlayerId = playerId,
                YieldAmount = finalYield,
                ExperienceGained = xpGained,
                IsCriticalGather = isCritical,
                ProfessionType = profession.ToString(),
                ToolUsed = toolEquipped
            };
            EventBus.Publish(gatherEvent);

            return new GatherResult(true, "")
            {
                YieldAmount = finalYield,
                ExperienceGained = xpGained,
                IsCriticalGather = isCritical,
                EffectiveGatherTime = effectiveGatherTime,
                NodeDepleted = nodeState.IsDepleted
            };
        }

        /// <summary>
        /// Updates respawn timers. Call this from a game loop tick.
        /// </summary>
        public void UpdateRespawnTimers(float deltaTime)
        {
            var nodesToRespawn = new List<ResourceNodeState>();

            foreach (var node in _respawnQueue)
            {
                node.RemainingRespawnTime -= deltaTime;
                if (node.RemainingRespawnTime <= 0)
                {
                    node.IsDepleted = false;
                    node.CurrentHealth = GetResourceMaxHealth(node.ResourceId);
                    node.RemainingRespawnTime = 0;
                    nodesToRespawn.Add(node);

                    var respawnEvent = new NodeRespawnedEvent
                    {
                        ResourceId = node.ResourceId,
                        WorldPositionKey = node.WorldPositionKey,
                        ChunkKey = node.ChunkKey
                    };
                    EventBus.Publish(respawnEvent);
                }
            }

            foreach (var node in nodesToRespawn)
            {
                _respawnQueue.Remove(node);
            }
        }

        /// <summary>
        /// Registers a resource node in the world.
        /// </summary>
        public void RegisterNode(string resourceId, string worldPositionKey, string chunkKey)
        {
            var resource = _resourceDb.GetResource(resourceId);
            if (resource == null) return;

            var nodeState = new ResourceNodeState
            {
                ResourceId = resourceId,
                WorldPositionKey = worldPositionKey,
                CurrentHealth = resource.NodeHealth,
                IsDepleted = false,
                RemainingRespawnTime = 0,
                IsModified = false,
                ChunkKey = chunkKey,
                CellIndex = 0
            };

            _nodeStates[worldPositionKey] = nodeState;
        }

        /// <summary>
        /// Gets the current state of a resource node.
        /// </summary>
        public ResourceNodeState? GetNodeState(string worldPositionKey)
        {
            return _nodeStates.TryGetValue(worldPositionKey, out var state) ? state : null;
        }

        /// <summary>
        /// Gets or creates a node state for tracking.
        /// </summary>
        private ResourceNodeState GetOrCreateNodeState(string resourceId, string worldPositionKey)
        {
            if (!string.IsNullOrEmpty(worldPositionKey) && _nodeStates.TryGetValue(worldPositionKey, out var existing))
                return existing;

            var resource = _resourceDb.GetResource(resourceId);
            var newState = new ResourceNodeState
            {
                ResourceId = resourceId,
                WorldPositionKey = worldPositionKey,
                CurrentHealth = resource?.NodeHealth ?? 1,
                IsDepleted = false,
                RemainingRespawnTime = 0,
                IsModified = false,
                ChunkKey = "",
                CellIndex = 0
            };

            if (!string.IsNullOrEmpty(worldPositionKey))
            {
                _nodeStates[worldPositionKey] = newState;
            }

            return newState;
        }

        private int GetResourceMaxHealth(string resourceId)
        {
            var resource = _resourceDb.GetResource(resourceId);
            return resource?.NodeHealth ?? 1;
        }

        private bool IsResourceRelatedProfession(ResourceDefinition resource, ProfessionType profession)
        {
            // Map resource categories to professions
            return profession switch
            {
                ProfessionType.Woodcutting => resource.Category == "Wood",
                ProfessionType.Mining => resource.Category == "Ore" || resource.Category == "Stone" || resource.Category == "Crystal",
                ProfessionType.Fishing => resource.Category == "Food" && resource.Subcategory == "Fish",
                ProfessionType.Farming => resource.Category == "Plant" || resource.Category == "Food",
                _ => true
            };
        }

        /// <summary>Exports all node states for save serialization.</summary>
        public List<ResourceNodeState> ExportNodeStates()
        {
            var states = new List<ResourceNodeState>();
            foreach (var kvp in _nodeStates)
            {
                states.Add(new ResourceNodeState
                {
                    ResourceId = kvp.Value.ResourceId,
                    WorldPositionKey = kvp.Key,
                    CurrentHealth = kvp.Value.CurrentHealth,
                    IsDepleted = kvp.Value.IsDepleted,
                    RemainingRespawnTime = kvp.Value.RemainingRespawnTime,
                    IsModified = kvp.Value.IsModified,
                    ChunkKey = kvp.Value.ChunkKey,
                    CellIndex = kvp.Value.CellIndex
                });
            }
            return states;
        }

        /// <summary>Restores node states from save data.</summary>
        public void RestoreNodeStates(List<ResourceNodeState> states)
        {
            if (states == null) return;
            _nodeStates.Clear();
            _respawnQueue.Clear();

            foreach (var state in states)
            {
                _nodeStates[state.WorldPositionKey] = state;
                if (state.IsDepleted && state.RemainingRespawnTime > 0)
                {
                    _respawnQueue.Add(state);
                }
            }
        }

        public bool IsInitialized => _isInitialized;
    }

    /// <summary>
    /// Result of a gather validation or execution.
    /// </summary>
    public class GatherResult
    {
        public bool Success { get; set; }
        public string FailureReason { get; set; } = string.Empty;
        public int YieldAmount { get; set; }
        public int ExperienceGained { get; set; }
        public bool IsCriticalGather { get; set; }
        public float EffectiveGatherTime { get; set; }
        public bool NodeDepleted { get; set; }

        public GatherResult(bool success, string failureReason)
        {
            Success = success;
            FailureReason = failureReason;
        }
    }
}