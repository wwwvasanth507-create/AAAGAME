using System;
using System.Collections.Generic;

namespace HeroOfEternia.Animation
{
    /// <summary>
    /// Reusable animation states supported across player, NPCs, enemies, and bosses.
    /// </summary>
    public enum AnimationState
    {
        Idle,
        Walk,
        Run,
        Sprint,
        Jump,
        Fall,
        Land,
        Swim,
        Climb,
        Crouch,
        Roll,
        Dodge,
        Attack,
        CastAbility,
        Block,
        HitReaction,
        Stunned,
        Interact,
        Gather,
        Craft,
        Sleep,
        Sit,
        Celebrate,
        Death,
        Respawn,
        Custom
    }

    /// <summary>
    /// Configurable animation blending layers.
    /// </summary>
    public enum AnimationLayerType
    {
        FullBody,
        UpperBody,
        LowerBody,
        Head,
        Hands,
        Facial,
        WeaponLayer,
        AdditiveLayer,
        ProceduralLayer,
        CinematicLayer
    }

    /// <summary>
    /// Playback priority tiers for preemption handling.
    /// </summary>
    public enum AnimationPriority
    {
        Low = 0,
        Normal = 10,
        High = 20,
        Interrupt = 30,
        Critical = 40
    }

    public class AnimationStateConfig
    {
        public AnimationState State { get; set; }
        public string ClipName { get; set; } = string.Empty;
        public float CrossfadeDuration { get; set; } = 0.2f;
        public bool Loop { get; set; } = true;
        public float SpeedScale { get; set; } = 1.0f;
        public AnimationPriority Priority { get; set; } = AnimationPriority.Normal;
        public AnimationLayerType DefaultLayer { get; set; } = AnimationLayerType.FullBody;
        public bool EnablesRootMotion { get; set; } = false;
    }
}
