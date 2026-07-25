using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Content.Chapter2
{
    public class Chapter2NpcData
    {
        public string NpcId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Profession { get; set; } = "Citizen";
        public Vector3 DefaultPosition { get; set; }
        public string DialogueTreeId { get; set; } = string.Empty;
        public string VendorTableId { get; set; } = string.Empty;
        public string FactionId { get; set; } = "faction_sylvan_guardians";
    }

    /// <summary>
    /// NPC definitions for Elderwood Grove and Sylvanwood Wilds, registering schedules,
    /// dialogue trees, vendor tables, and faction affinities.
    /// </summary>
    public class Chapter2NpcDefinitions
    {
        private readonly Dictionary<string, Chapter2NpcData> _npcs = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterDefaultChapter2NPCs()
        {
            RegisterNpc(new Chapter2NpcData
            {
                NpcId = "npc_warden_kaelen",
                DisplayName = "Warden Kaelen",
                Profession = "Town Leader",
                DefaultPosition = new Vector3(200, 0, 300),
                DialogueTreeId = "dlg_warden_kaelen_intro"
            });

            RegisterNpc(new Chapter2NpcData
            {
                NpcId = "npc_master_corin",
                DisplayName = "Master Corin",
                Profession = "High Alchemist",
                DefaultPosition = new Vector3(170, 0, 280),
                DialogueTreeId = "dlg_corin_alchemist",
                VendorTableId = "vendor_alchemy_chapter2"
            });

            RegisterNpc(new Chapter2NpcData
            {
                NpcId = "npc_scholar_elora",
                DisplayName = "Scholar Elora",
                Profession = "Ancient Relic Scholar",
                DefaultPosition = new Vector3(200, 5, 350),
                DialogueTreeId = "dlg_elora_scholar"
            });

            RegisterNpc(new Chapter2NpcData
            {
                NpcId = "npc_guildmaster_vance",
                DisplayName = "Guildmaster Vance",
                Profession = "Merchants Guild Lead",
                DefaultPosition = new Vector3(220, 0, 280),
                DialogueTreeId = "dlg_vance_guild"
            });

            RegisterNpc(new Chapter2NpcData
            {
                NpcId = "npc_stranger_vaelen",
                DisplayName = "Suspicious Stranger Vaelen",
                Profession = "Wandering Rogue",
                DefaultPosition = new Vector3(240, 0, 320),
                DialogueTreeId = "dlg_stranger_vaelen",
                FactionId = "faction_shadow_cult"
            });
        }

        public void RegisterNpc(Chapter2NpcData npc)
        {
            if (npc != null && !string.IsNullOrEmpty(npc.NpcId))
            {
                _npcs[npc.NpcId] = npc;
            }
        }

        public Chapter2NpcData? GetNpc(string npcId)
        {
            return _npcs.TryGetValue(npcId, out var npc) ? npc : null;
        }

        public IReadOnlyCollection<Chapter2NpcData> AllNpcs => _npcs.Values;
    }
}
