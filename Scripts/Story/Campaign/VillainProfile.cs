using System;
using System.Collections.Generic;

namespace HeroOfEternia.Story.Campaign
{
    public enum VillainRank
    {
        PrimaryVillain,
        RegionalVillain,
        EliteCommander,
        AncientEvil,
        MonsterLeader,
        CorruptedHero,
        ExpansionVillain
    }

    public class VillainProfile
    {
        public string VillainId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public VillainRank Rank { get; set; } = VillainRank.RegionalVillain;
        public string FactionId { get; set; } = string.Empty;
        public string Goals { get; set; } = string.Empty;
        public string Motivations { get; set; } = string.Empty;
        public List<string> ResourcesControlled { get; set; } = new();
        public string CombatArchetype { get; set; } = "DarkMage";
        public float GlobalInfluenceScore { get; set; } = 0.5f;
        public bool IsDefeated { get; set; } = false;
        public string DlcModuleId { get; set; } = string.Empty;
    }

    public class VillainDatabase
    {
        private readonly Dictionary<string, VillainProfile> _villains = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterVillain(VillainProfile villain)
        {
            if (villain != null && !string.IsNullOrEmpty(villain.VillainId))
            {
                _villains[villain.VillainId] = villain;
            }
        }

        public VillainProfile? GetVillain(string villainId)
        {
            return _villains.TryGetValue(villainId, out var v) ? v : null;
        }

        public IReadOnlyCollection<VillainProfile> GetAllVillains() => _villains.Values;

        public void RegisterDefaultVillains()
        {
            RegisterVillain(new VillainProfile
            {
                VillainId = "villain_malakor_voidlord",
                DisplayName = "Malakor the Void Lord",
                Rank = VillainRank.PrimaryVillain,
                FactionId = "faction_shadow_cult",
                Goals = "Shatter the ancient seals of Eternia and merge the mortal realm with the Void.",
                Motivations = "Revenge against the Ancient Titans for imprisoning him during the First War.",
                CombatArchetype = "VoidSorcerer",
                GlobalInfluenceScore = 1.0f
            });

            RegisterVillain(new VillainProfile
            {
                VillainId = "villain_baron_skarr",
                DisplayName = "Baron Skarr",
                Rank = VillainRank.RegionalVillain,
                FactionId = "faction_bandit_syndicate",
                Goals = "Control trade routes in Sylvanwood Wilds and plunder Oakvale.",
                CombatArchetype = "DualDaggerRogue",
                GlobalInfluenceScore = 0.4f
            });
        }
    }
}
