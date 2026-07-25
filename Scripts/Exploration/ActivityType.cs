using System;

namespace HeroOfEternia.Exploration
{
    public enum ActivityType
    {
        TreasureHunt,
        PuzzleShrine,
        HiddenChest,
        TimedChallenge,
        ParkourChallenge,
        CombatChallenge,
        SurvivalEvent,
        FishingSpot,
        RareResourceNode,
        ArtifactDiscovery,
        LoreDiscovery,
        AncientMechanism,
        MemoryFragment,
        MagicAnomaly,
        WorldBossHook,
        SeasonalActivity,
        Custom
    }

    public enum ActivityCategory
    {
        Exploration,
        Puzzle,
        Combat,
        Resource,
        Lore,
        Event
    }
}
