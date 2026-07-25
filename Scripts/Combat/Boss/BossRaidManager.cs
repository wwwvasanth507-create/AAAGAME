using Godot;
using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Combat.Boss
{
    public class BossRaidManager : IInitializable
    {
        private static BossRaidManager? _instance;
        public static BossRaidManager Instance => _instance ??= new BossRaidManager();

        public string ActiveRaidBossId { get; private set; } = string.Empty;
        public int CurrentRaidPhase { get; private set; } = 1;
        public bool IsRaidActive { get; private set; } = false;

        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            GD.Print("[BossRaidManager] Initialized.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
        }

        public void StartRaidEncounter(string bossId)
        {
            ActiveRaidBossId = bossId;
            CurrentRaidPhase = 1;
            IsRaidActive = true;
            EventBus.Publish(bossId);
            GD.Print($"[BossRaidManager] Raid Encounter started vs {bossId}");
        }

        public void AdvanceRaidPhase()
        {
            if (!IsRaidActive) return;
            CurrentRaidPhase++;
            EventBus.Publish(CurrentRaidPhase);
            GD.Print($"[BossRaidManager] Boss Raid advanced to Phase {CurrentRaidPhase}");
        }

        public void EndRaidEncounter(bool victory)
        {
            IsRaidActive = false;
            EventBus.Publish(victory);
            GD.Print($"[BossRaidManager] Raid Encounter ended. Victory: {victory}");
        }
    }
}
