using System;
using System.Collections.Generic;
using HeroOfEternia.Core;
using HeroOfEternia.Dialogue;

namespace HeroOfEternia.Story
{
    /// <summary>
    /// Central manager for custom player stories, story nodes, dialogue hooks, and narrative state transitions.
    /// Implements IInitializable and registers with ServiceLocator.
    /// </summary>
    public class CustomStoryManager : IInitializable
    {
        private CustomStoryDatabase _database = new();
        private CustomDialogueController _dialogueController = new();

        private readonly HashSet<string> _completedStoryArcIds = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _completedNodeIds = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _activeArcNodes = new(StringComparer.OrdinalIgnoreCase);

        public bool IsInitialized { get; private set; }
        public CustomStoryDatabase Database => _database;
        public CustomDialogueController DialogueController => _dialogueController;

        public event Action<CustomStoryArc>? OnStoryArcStarted;
        public event Action<CustomStoryArc>? OnStoryArcCompleted;
        public event Action<CustomStoryNode>? OnStoryNodeCompleted;

        public void Initialize()
        {
            if (IsInitialized) return;

            _database = new CustomStoryDatabase();
            _dialogueController = new CustomDialogueController();

            // Register with ServiceLocator
            ServiceLocator.Register<CustomStoryManager>(this);

            IsInitialized = true;
            Logger.Info("CustomStoryManager: Initialized successfully and registered with ServiceLocator.");
        }

        public void Shutdown()
        {
            if (!IsInitialized) return;

            _completedStoryArcIds.Clear();
            _completedNodeIds.Clear();
            _activeArcNodes.Clear();

            ServiceLocator.Unregister<CustomStoryManager>();
            IsInitialized = false;
            Logger.Info("CustomStoryManager: Shutdown completed.");
        }

        public bool StartStoryArc(string arcId)
        {
            var arc = _database.GetArc(arcId);
            if (arc == null)
            {
                Logger.Warning($"CustomStoryManager: Cannot start story arc '{arcId}' - Arc not found.");
                return false;
            }

            if (_completedStoryArcIds.Contains(arcId))
            {
                Logger.Warning($"CustomStoryManager: Story arc '{arcId}' is already completed.");
                return false;
            }

            if (arc.ChapterIds.Count > 0)
            {
                _activeArcNodes[arcId] = arc.ChapterIds[0];
            }

            OnStoryArcStarted?.Invoke(arc);
            Logger.Info($"CustomStoryManager: Started story arc '{arc.Title}' ({arcId}).");
            return true;
        }

        public bool AdvanceStoryNode(string nodeId)
        {
            var node = _database.GetNode(nodeId);
            if (node == null)
            {
                Logger.Warning($"CustomStoryManager: Cannot advance node '{nodeId}' - Node not found.");
                return false;
            }

            _completedNodeIds.Add(nodeId);

            // Update world state flag if configured
            if (!string.IsNullOrEmpty(node.CompletionSetFlag))
            {
                try
                {
                    var worldStateManager = ServiceLocator.Get<WorldStateManager>();
                    worldStateManager?.SetFlag(node.CompletionSetFlag, "true");
                }
                catch
                {
                    // WorldStateManager not registered in lightweight unit tests
                }
            }

            OnStoryNodeCompleted?.Invoke(node);

            // Advance active arc tracking
            if (_activeArcNodes.TryGetValue(node.StoryArcId, out var activeNode) && activeNode == nodeId)
            {
                if (!string.IsNullOrEmpty(node.NextNodeId))
                {
                    _activeArcNodes[node.StoryArcId] = node.NextNodeId;
                }
                else
                {
                    // Story arc complete
                    CompleteStoryArc(node.StoryArcId);
                }
            }

            Logger.Info($"CustomStoryManager: Advanced story node '{node.Title}' ({nodeId}).");
            return true;
        }

        public bool CompleteStoryArc(string arcId)
        {
            var arc = _database.GetArc(arcId);
            if (arc == null) return false;

            arc.IsCompleted = true;
            _completedStoryArcIds.Add(arcId);
            _activeArcNodes.Remove(arcId);

            OnStoryArcCompleted?.Invoke(arc);
            Logger.Info($"CustomStoryManager: Completed story arc '{arc.Title}' ({arcId}).");
            return true;
        }

        public string GetActiveNodeForArc(string arcId)
        {
            return _activeArcNodes.TryGetValue(arcId, out var nodeId) ? nodeId : "";
        }

        public bool IsStoryArcCompleted(string arcId) => _completedStoryArcIds.Contains(arcId);
        public bool IsStoryNodeCompleted(string nodeId) => _completedNodeIds.Contains(nodeId);

        public IReadOnlyCollection<string> GetCompletedArcs() => _completedStoryArcIds;
        public IReadOnlyCollection<string> GetCompletedNodes() => _completedNodeIds;
    }
}
