using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Content.Prologue
{
    public class StarterExplorationNode
    {
        public string NodeId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public Vector3 Position { get; set; }
        public string NodeType { get; set; } = "Chest";
        public string RewardId { get; set; } = string.Empty;
    }

    public class StarterExplorationContent
    {
        private readonly List<StarterExplorationNode> _nodes = new();

        public void RegisterDefaultExplorationContent()
        {
            _nodes.Add(new StarterExplorationNode
            {
                NodeId = "exp_chest_watchtower",
                DisplayName = "Old Watchtower Chest",
                Position = new Vector3(30, 10, -50),
                NodeType = "HiddenChest",
                RewardId = "item_weapon_rusty_sword"
            });

            _nodes.Add(new StarterExplorationNode
            {
                NodeId = "exp_puzzle_shrine_rune",
                DisplayName = "Oakvale Rune Shrine",
                Position = new Vector3(0, 5, -50),
                NodeType = "PuzzleShrine",
                RewardId = "item_potion_healing_salve"
            });

            _nodes.Add(new StarterExplorationNode
            {
                NodeId = "exp_lore_tablet_creation",
                DisplayName = "Ancient Stone Tablet",
                Position = new Vector3(-80, 0, -90),
                NodeType = "LoreTablet",
                RewardId = "lore_tablet_creation"
            });

            _nodes.Add(new StarterExplorationNode
            {
                NodeId = "exp_viewpoint_cliff",
                DisplayName = "Oakvale Valley Vista",
                Position = new Vector3(60, 20, 40),
                NodeType = "ScenicViewpoint",
                RewardId = "xp_100"
            });
        }

        public IReadOnlyList<StarterExplorationNode> AllNodes => _nodes;
    }
}
