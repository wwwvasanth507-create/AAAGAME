using System;

namespace HeroOfEternia.Audio
{
    /// <summary>
    /// Audio categories for dynamic mixer routing, independent volume control,
    /// and priority management across the audio engine.
    /// </summary>
    public enum AudioCategory
    {
        Master,
        Music,
        Ambient,
        Environment,
        Combat,
        UI,
        Dialogue,
        NPC,
        Creatures,
        Weather,
        Footsteps,
        Abilities,
        VoiceOver,
        DeveloperDebug
    }

    /// <summary>
    /// Priority level for audio channels when channel limits are reached.
    /// </summary>
    public enum AudioPriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }
}
