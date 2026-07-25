using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Content.Prologue
{
    public class StarterNpcData
    {
        public string NpcId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Profession { get; set; } = "Villager";
        public Vector3 DefaultPosition { get; set; }
        public string DialogueTreeId { get; set; } = string.Empty;
        public string VendorTableId { get; set; } = string.Empty;
        public string FactionId { get; set; } = "faction_valen_crown";
    }

    /// <summary>
    /// Starter NPC definitions for Oakvale Village, registering schedule anchors,
    /// dialogue trees, vendor tables, and faction relationships.
    /// </summary>
    public class StarterNpcDefinitions
    {
        private readonly Dictionary<string, StarterNpcData> _npcs = new(StringComparer.OrdinalIgnoreCase);

        public void RegisterDefaultStarterNPCs()
        {
            RegisterNpc(new StarterNpcData
            {
                NpcId = "npc_elder_alden",
                DisplayName = "Elder Alden",
                Profession = "Village Elder",
                DefaultPosition = new Vector3(0, 0, -5),
                DialogueTreeId = "dlg_elder_alden_intro"
            });

            RegisterNpc(new StarterNpcData
            {
                NpcId = "npc_blacksmith_thorin",
                DisplayName = "Thorin Ironhand",
                Profession = "Blacksmith",
                DefaultPosition = new Vector3(-25, 0, 15),
                DialogueTreeId = "dlg_thorin_blacksmith",
                VendorTableId = "vendor_blacksmith_starter"
            });

            RegisterNpc(new StarterNpcData
            {
                NpcId = "npc_merchant_gideon",
                DisplayName = "Gideon the Merchant",
                Profession = "General Merchant",
                DefaultPosition = new Vector3(5, 0, 10),
                DialogueTreeId = "dlg_gideon_merchant",
                VendorTableId = "vendor_general_starter"
            });

            RegisterNpc(new StarterNpcData
            {
                NpcId = "npc_captain_valerius",
                DisplayName = "Captain Valerius",
                Profession = "Guard Captain",
                DefaultPosition = new Vector3(30, 0, -20),
                DialogueTreeId = "dlg_valerius_training"
            });

            RegisterNpc(new StarterNpcData
            {
                NpcId = "npc_hunter_lyra",
                DisplayName = "Lyra the Hunter",
                Profession = "Hunter",
                DefaultPosition = new Vector3(-40, 0, -30),
                DialogueTreeId = "dlg_lyra_hunter"
            });
        }

        public void RegisterNpc(StarterNpcData npc)
        {
            if (npc != null && !string.IsNullOrEmpty(npc.NpcId))
            {
                _npcs[npc.NpcId] = npc;
            }
        }

        public StarterNpcData? GetNpc(string npcId)
        {
            return _npcs.TryGetValue(npcId, out var npc) ? npc : null;
        }

        public IReadOnlyCollection<StarterNpcData> AllNpcs => _npcs.Values;
    }
}
