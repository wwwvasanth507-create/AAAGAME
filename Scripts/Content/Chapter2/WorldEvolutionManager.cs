using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Content.Chapter2
{
    public enum WorldPhase
    {
        OakvalePeace,
        BlightSpreading,
        AethelgardUnsealed,
        ShadowSiege
    }

    /// <summary>
    /// Dynamic world state evolution engine reacting to chapter progression,
    /// triggering NPC migrations, merchant stock upgrades, and regional hazard shifts.
    /// </summary>
    public class WorldEvolutionManager
    {
        public WorldPhase CurrentWorldPhase { get; private set; } = WorldPhase.OakvalePeace;

        public event Action<WorldPhase>? OnWorldPhaseChanged;

        public void AdvanceWorldPhase(WorldPhase newPhase)
        {
            if (newPhase > CurrentWorldPhase)
            {
                CurrentWorldPhase = newPhase;
                Logger.Info($"WorldEvolutionManager: Advanced world phase to '{newPhase}'");
                OnWorldPhaseChanged?.Invoke(newPhase);
            }
        }
    }
}
