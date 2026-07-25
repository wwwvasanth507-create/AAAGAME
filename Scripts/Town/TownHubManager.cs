using Godot;
using System;
using HeroOfEternia.Core;

namespace HeroOfEternia.Town
{
    public enum NpcType
    {
        Blacksmith = 0,
        Merchant = 1,
        Enchanter = 2,
        QuestGiver = 3,
        Innkeeper = 4
    }

    public class TownHubManager : IInitializable
    {
        private static TownHubManager? _instance;
        public static TownHubManager Instance => _instance ??= new TownHubManager();

        public bool IsInTownHub { get; private set; } = false;
        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            GD.Print("[TownHubManager] Initialized.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
        }

        public void EnterTownHub()
        {
            IsInTownHub = true;
            EventBus.Publish(true);
            GD.Print("[TownHubManager] Entered Town Hub safe zone.");
        }

        public void ExitTownHub()
        {
            IsInTownHub = false;
            EventBus.Publish(false);
            GD.Print("[TownHubManager] Exited Town Hub.");
        }

        public void InteractWithNpc(NpcType npc)
        {
            EventBus.Publish(npc);
            GD.Print($"[TownHubManager] Interacting with NPC: {npc}");
        }
    }
}
