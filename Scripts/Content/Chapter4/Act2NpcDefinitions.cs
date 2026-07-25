using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter4
{
    public class Act2NpcDefinition
    {
        public string NpcId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string DialogueTreeId { get; set; } = string.Empty;
        public bool IsVendor { get; set; } = false;
    }

    /// <summary>
    /// Act II NPC roster for Eastern Ridgeline and Mirkwood Swamps.
    /// Includes Ranger Commander, Swamp herbalist, and the first Malakor faction agent.
    /// </summary>
    public class Act2NpcDefinitions
    {
        private readonly Dictionary<string, Act2NpcDefinition> _npcs = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterNpcs()
        {
            RegisterNpc(new Act2NpcDefinition
            {
                NpcId = "npc_commander_harek",
                DisplayName = "Commander Harek Stonewall",
                Role = "Valen Crown Ranger Commander",
                Location = "poi_ridgeline_watchtower",
                DialogueTreeId = "dlg_harek_act2_intro",
                IsVendor = false
            });

            RegisterNpc(new Act2NpcDefinition
            {
                NpcId = "npc_elda_swampwarden",
                DisplayName = "Elda the Swamp Warden",
                Role = "Mirkwood Herbalist & Guide",
                Location = "region_mirkwood_swamps",
                DialogueTreeId = "dlg_elda_intro",
                IsVendor = true
            });

            RegisterNpc(new Act2NpcDefinition
            {
                NpcId = "npc_shadow_emissary",
                DisplayName = "Emissary Null",
                Role = "Malakor's Secret Agent",
                Location = "poi_ridgeline_waystone",
                DialogueTreeId = "dlg_emissary_act2_warning",
                IsVendor = false
            });

            RegisterNpc(new Act2NpcDefinition
            {
                NpcId = "npc_ridgeline_smith",
                DisplayName = "Forge-Master Brynn",
                Role = "Ridgeline Blacksmith & Tier 2 Vendor",
                Location = "poi_ridgeline_watchtower",
                DialogueTreeId = "dlg_brynn_shop",
                IsVendor = true
            });

            Logger.Info($"Act2NpcDefinitions: {_npcs.Count} NPCs registered for Act II.");
        }

        public void RegisterNpc(Act2NpcDefinition npc)
        {
            if (npc != null && !string.IsNullOrEmpty(npc.NpcId))
                _npcs[npc.NpcId] = npc;
        }

        public Act2NpcDefinition? GetNpc(string npcId)
            => _npcs.TryGetValue(npcId, out var n) ? n : null;

        public IReadOnlyCollection<Act2NpcDefinition> AllNpcs => _npcs.Values;
    }
}
