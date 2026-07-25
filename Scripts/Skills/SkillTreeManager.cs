using Godot;
using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Skills
{
    public class SkillNode
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RequiredSkillId { get; set; } = string.Empty;
        public int RequiredLevel { get; set; } = 1;
        public int MaxRank { get; set; } = 5;
        public int CurrentRank { get; set; } = 0;
        public float StatBonusPerRank { get; set; } = 0.05f;
    }

    public class SkillTreeManager : IInitializable
    {
        private static SkillTreeManager? _instance;
        public static SkillTreeManager Instance => _instance ??= new SkillTreeManager();

        private readonly Dictionary<string, SkillNode> _nodes = new();
        public int AvailableSkillPoints { get; private set; } = 0;
        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            LoadDefaultSkillNodes();
            GD.Print("[SkillTreeManager] Initialized.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
            _nodes.Clear();
        }

        private void LoadDefaultSkillNodes()
        {
            _nodes.Clear();
            AddNode(new SkillNode { Id = "passive_strength", Name = "Might of Eternia", Description = "+5% Physical Damage per rank", MaxRank = 5, RequiredLevel = 1, StatBonusPerRank = 0.05f });
            AddNode(new SkillNode { Id = "passive_vitality", Name = "Iron Health", Description = "+8% Max HP per rank", MaxRank = 5, RequiredLevel = 1, StatBonusPerRank = 0.08f });
            AddNode(new SkillNode { Id = "passive_stamina", Name = "Endless Wind", Description = "+10% Stamina Regen per rank", MaxRank = 5, RequiredLevel = 2, RequiredSkillId = "passive_vitality", StatBonusPerRank = 0.10f });
            AddNode(new SkillNode { Id = "passive_elemental", Name = "Arcane Flame", Description = "+12% Elemental Reaction Damage", MaxRank = 3, RequiredLevel = 3, RequiredSkillId = "passive_strength", StatBonusPerRank = 0.12f });
        }

        private void AddNode(SkillNode node)
        {
            _nodes[node.Id] = node;
        }

        public void AddSkillPoints(int points)
        {
            if (points <= 0) return;
            AvailableSkillPoints += points;
            EventBus.Publish(AvailableSkillPoints);
        }

        public bool UpgradeSkill(string skillId)
        {
            if (!_nodes.TryGetValue(skillId, out var node)) return false;
            if (AvailableSkillPoints < 1) return false;
            if (node.CurrentRank >= node.MaxRank) return false;

            if (!string.IsNullOrEmpty(node.RequiredSkillId))
            {
                if (!_nodes.TryGetValue(node.RequiredSkillId, out var reqNode) || reqNode.CurrentRank == 0)
                {
                    GD.Print($"[SkillTreeManager] Upgrade failed: Requires {node.RequiredSkillId}");
                    return false;
                }
            }

            node.CurrentRank++;
            AvailableSkillPoints--;
            EventBus.Publish(node);
            GD.Print($"[SkillTreeManager] Skill '{node.Name}' upgraded to rank {node.CurrentRank}.");
            return true;
        }

        public void RespecAllSkills()
        {
            int refundedPoints = 0;
            foreach (var node in _nodes.Values)
            {
                refundedPoints += node.CurrentRank;
                node.CurrentRank = 0;
            }

            AvailableSkillPoints += refundedPoints;
            EventBus.Publish(AvailableSkillPoints);
            GD.Print($"[SkillTreeManager] Respec complete. Refunded {refundedPoints} skill points.");
        }

        public SkillNode? GetNode(string skillId)
        {
            return _nodes.TryGetValue(skillId, out var node) ? node : null;
        }

        public IReadOnlyDictionary<string, SkillNode> GetAllNodes() => _nodes;
    }
}
