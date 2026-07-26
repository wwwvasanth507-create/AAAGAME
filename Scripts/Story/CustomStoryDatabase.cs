using System;
using System.Collections.Generic;

namespace HeroOfEternia.Story
{
    /// <summary>
    /// Custom narrative arc definition representing player-driven storylines and side sagas.
    /// </summary>
    public class CustomStoryArc
    {
        public string StoryArcId { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int RecommendedLevel { get; set; } = 1;
        public string RegionId { get; set; } = "";
        public List<string> ChapterIds { get; set; } = new();
        public List<string> PrerequisiteFlags { get; set; } = new();
        public List<string> RewardItemIds { get; set; } = new();
        public int RewardXp { get; set; } = 100;
        public int RewardGold { get; set; } = 50;
        public bool IsCompleted { get; set; } = false;
    }

    /// <summary>
    /// Story node representing individual narrative beats or quest checkpoints in custom stories.
    /// </summary>
    public class CustomStoryNode
    {
        public string NodeId { get; set; } = "";
        public string StoryArcId { get; set; } = "";
        public string Title { get; set; } = "";
        public string Summary { get; set; } = "";
        public string PrimaryDialogueId { get; set; } = "";
        public string AssociatedNpcId { get; set; } = "";
        public string AssociatedObjectId { get; set; } = "";
        public string RequiredWorldFlag { get; set; } = "";
        public string CompletionSetFlag { get; set; } = "";
        public string NextNodeId { get; set; } = "";
        public bool IsMilestone { get; set; } = false;
    }

    /// <summary>
    /// Database holding custom player storylines, narrative arcs, and story nodes.
    /// </summary>
    public class CustomStoryDatabase
    {
        private readonly Dictionary<string, CustomStoryArc> _storyArcs = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, CustomStoryNode> _storyNodes = new(StringComparer.OrdinalIgnoreCase);

        public CustomStoryDatabase()
        {
            RegisterDefaultCustomStories();
        }

        public void RegisterDefaultCustomStories()
        {
            // 1. Arc: The Astral Seal Saga
            var astralArc = new CustomStoryArc
            {
                StoryArcId = "arc_astral_seal",
                Title = "The Astral Seal Saga",
                Description = "Uncover the forgotten ancient seals buried deep within Eternia and decide their fate.",
                RecommendedLevel = 5,
                RegionId = "region_vale",
                ChapterIds = new List<string> { "astral_node_01", "astral_node_02", "astral_node_03" },
                RewardItemIds = new List<string> { "astral_amulet_tier2", "essence_crystal_rare" },
                RewardXp = 500,
                RewardGold = 250
            };
            RegisterArc(astralArc);

            RegisterNode(new CustomStoryNode
            {
                NodeId = "astral_node_01",
                StoryArcId = "arc_astral_seal",
                Title = "The Awakening Relic",
                Summary = "Discover the ancient Astral Altar glowing in the whispers of the valley.",
                PrimaryDialogueId = "dlg_astral_relic_inspect",
                AssociatedObjectId = "obj_astral_altar_01",
                CompletionSetFlag = "astral_altar_discovered",
                NextNodeId = "astral_node_02",
                IsMilestone = false
            });

            RegisterNode(new CustomStoryNode
            {
                NodeId = "astral_node_02",
                StoryArcId = "arc_astral_seal",
                Title = "Guardian's Dilemma",
                Summary = "Confront Keeper Orin regarding the seal's true purpose.",
                PrimaryDialogueId = "dlg_keeper_orin_confront",
                AssociatedNpcId = "npc_keeper_orin",
                RequiredWorldFlag = "astral_altar_discovered",
                CompletionSetFlag = "astral_keeper_confronted",
                NextNodeId = "astral_node_03",
                IsMilestone = true
            });

            RegisterNode(new CustomStoryNode
            {
                NodeId = "astral_node_03",
                StoryArcId = "arc_astral_seal",
                Title = "Restoration or Oblivion",
                Summary = "Choose whether to cleanse the Astral Seal or absorb its volatile power.",
                PrimaryDialogueId = "dlg_astral_final_choice",
                AssociatedObjectId = "obj_astral_core_switch",
                RequiredWorldFlag = "astral_keeper_confronted",
                CompletionSetFlag = "astral_saga_completed",
                NextNodeId = "",
                IsMilestone = true
            });

            // 2. Arc: Whispers of the Eternal Crucible
            var crucibleArc = new CustomStoryArc
            {
                StoryArcId = "arc_eternal_crucible",
                Title = "Whispers of the Eternal Crucible",
                Description = "Reignite the sacred embers of the ancient forge to craft heroic-grade relics.",
                RecommendedLevel = 10,
                RegionId = "region_highlands",
                ChapterIds = new List<string> { "crucible_node_01", "crucible_node_02" },
                RewardItemIds = new List<string> { "crucible_hammer", "flame_ingot" },
                RewardXp = 800,
                RewardGold = 400
            };
            RegisterArc(crucibleArc);

            RegisterNode(new CustomStoryNode
            {
                NodeId = "crucible_node_01",
                StoryArcId = "arc_eternal_crucible",
                Title = "Dormant Forge",
                Summary = "Locate the long-lost Crucible Forge in the iron mountains.",
                PrimaryDialogueId = "dlg_crucible_forge_inspect",
                AssociatedObjectId = "obj_crucible_forge_01",
                CompletionSetFlag = "crucible_found",
                NextNodeId = "crucible_node_02",
                IsMilestone = false
            });

            RegisterNode(new CustomStoryNode
            {
                NodeId = "crucible_node_02",
                StoryArcId = "arc_eternal_crucible",
                Title = "Igniting the Embers",
                Summary = "Use elemental keys to awaken the forge furnace.",
                PrimaryDialogueId = "dlg_crucible_ignite",
                AssociatedObjectId = "obj_crucible_igniter",
                RequiredWorldFlag = "crucible_found",
                CompletionSetFlag = "crucible_reignited",
                NextNodeId = "",
                IsMilestone = true
            });
        }

        public void RegisterArc(CustomStoryArc arc)
        {
            if (arc != null && !string.IsNullOrEmpty(arc.StoryArcId))
            {
                _storyArcs[arc.StoryArcId] = arc;
            }
        }

        public void RegisterNode(CustomStoryNode node)
        {
            if (node != null && !string.IsNullOrEmpty(node.NodeId))
            {
                _storyNodes[node.NodeId] = node;
            }
        }

        public CustomStoryArc? GetArc(string arcId)
        {
            return _storyArcs.TryGetValue(arcId, out var arc) ? arc : null;
        }

        public CustomStoryNode? GetNode(string nodeId)
        {
            return _storyNodes.TryGetValue(nodeId, out var node) ? node : null;
        }

        public List<CustomStoryArc> GetAllArcs()
        {
            return new List<CustomStoryArc>(_storyArcs.Values);
        }

        public List<CustomStoryNode> GetNodesForArc(string arcId)
        {
            var list = new List<CustomStoryNode>();
            foreach (var node in _storyNodes.Values)
            {
                if (string.Equals(node.StoryArcId, arcId, StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(node);
                }
            }
            return list;
        }
    }
}
