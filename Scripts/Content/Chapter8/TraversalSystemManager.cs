using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter8
{
    public enum TraversalType
    {
        GrapplePoint,
        WallClimbZone,
        ZiplineAnchor,
        RopeBridge,
        MovingPlatform
    }

    public class TraversalNodeRecord
    {
        public string NodeId { get; set; } = "";
        public string Name { get; set; } = "";
        public TraversalType Type { get; set; } = TraversalType.GrapplePoint;
        public string LocationZoneId { get; set; } = "";
        public bool IsUnlocked { get; set; } = true;
        public float ReachDistanceMeters { get; set; } = 15f;
    }

    /// <summary>
    /// Advanced Traversal System Manager for Act III & Chapter 8.
    /// Manages Grapple Hooks, Wall Climbing, Ziplines, Moving Platforms, and environmental traversal puzzles.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class TraversalSystemManager : IInitializable
    {
        private readonly Dictionary<string, TraversalNodeRecord> _nodes = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _unlockedTools = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized { get; private set; }

        public event Action<TraversalNodeRecord>? OnTraversalUsed;
        public event Action<string>? OnTraversalToolUnlocked;

        public void Initialize()
        {
            if (IsInitialized) return;

            RegisterDefaultNodes();

            // Register default tools
            UnlockTool("tool_grapple_hook");
            UnlockTool("tool_climbing_claws");

            // Register with ServiceLocator
            ServiceLocator.Register<TraversalSystemManager>(this);

            IsInitialized = true;
            Logger.Info("TraversalSystemManager: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            _nodes.Clear();
            _unlockedTools.Clear();

            ServiceLocator.Unregister<TraversalSystemManager>();
            IsInitialized = false;
            Logger.Info("TraversalSystemManager: Shutdown completed.");
        }

        private void RegisterDefaultNodes()
        {
            // 1. Dread Ravine Grapple Anchor
            RegisterNode(new TraversalNodeRecord
            {
                NodeId = "node_dread_ravine_grapple",
                Name = "Dread Ravine High Iron Grapple Ring",
                Type = TraversalType.GrapplePoint,
                LocationZoneId = "zone_dread_ravine",
                ReachDistanceMeters = 20f
            });

            // 2. Gloomstone Wall Climb
            RegisterNode(new TraversalNodeRecord
            {
                NodeId = "node_gloomstone_wall_climb",
                Name = "Gloomstone Cliff Wall Climbing Face",
                Type = TraversalType.WallClimbZone,
                LocationZoneId = "zone_gloomstone_caverns",
                ReachDistanceMeters = 10f
            });

            // 3. Obsidian Crag Zipline
            RegisterNode(new TraversalNodeRecord
            {
                NodeId = "node_obsidian_crag_zipline",
                Name = "Obsidian Crag Sky Zipline Anchor",
                Type = TraversalType.ZiplineAnchor,
                LocationZoneId = "zone_obsidian_crag_sanctuary",
                ReachDistanceMeters = 30f
            });
        }

        public void RegisterNode(TraversalNodeRecord node)
        {
            if (node != null && !string.IsNullOrEmpty(node.NodeId))
            {
                _nodes[node.NodeId] = node;
            }
        }

        public bool UnlockTool(string toolId)
        {
            if (string.IsNullOrEmpty(toolId)) return false;

            if (_unlockedTools.Add(toolId))
            {
                OnTraversalToolUnlocked?.Invoke(toolId);
                Logger.Info($"TraversalSystemManager: Unlocked traversal tool '{toolId}'.");
                return true;
            }

            return false;
        }

        public bool IsToolUnlocked(string toolId)
        {
            return _unlockedTools.Contains(toolId);
        }

        public bool ExecuteTraversal(string nodeId)
        {
            if (!_nodes.TryGetValue(nodeId, out var node))
            {
                Logger.Warning($"TraversalSystemManager: Node '{nodeId}' not found.");
                return false;
            }

            if (!node.IsUnlocked)
            {
                Logger.Warning($"TraversalSystemManager: Node '{node.Name}' is locked.");
                return false;
            }

            OnTraversalUsed?.Invoke(node);
            Logger.Info($"TraversalSystemManager: Executed traversal '{node.Name}' ({node.Type}).");
            return true;
        }

        public TraversalNodeRecord? GetNode(string nodeId)
        {
            return _nodes.TryGetValue(nodeId, out var n) ? n : null;
        }

        public List<TraversalNodeRecord> GetAllNodes()
        {
            return new List<TraversalNodeRecord>(_nodes.Values);
        }
    }
}
