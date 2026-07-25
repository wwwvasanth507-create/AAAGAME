using System;
using System.Collections.Generic;

namespace HeroOfEternia.Animation
{
    public enum CharacterArchetype
    {
        Player,
        NPC,
        Merchant,
        Guard,
        Bandit,
        Animal,
        Monster,
        Boss,
        FlyingCreature,
        SwimmingCreature
    }

    /// <summary>
    /// Data-driven profile defining state-to-clip mappings, speed multipliers,
    /// and transition settings for a specific character archetype.
    /// </summary>
    public class CharacterAnimationProfile
    {
        public string ProfileId { get; set; } = string.Empty;
        public CharacterArchetype Archetype { get; set; } = CharacterArchetype.Player;

        private readonly Dictionary<AnimationState, AnimationStateConfig> _stateConfigs = new();

        public void MapState(AnimationState state, string clipName, float crossfade = 0.2f, bool loop = true, float speed = 1.0f, AnimationPriority priority = AnimationPriority.Normal)
        {
            _stateConfigs[state] = new AnimationStateConfig
            {
                State = state,
                ClipName = clipName,
                CrossfadeDuration = crossfade,
                Loop = loop,
                SpeedScale = speed,
                Priority = priority
            };
        }

        public AnimationStateConfig? GetConfig(AnimationState state)
        {
            return _stateConfigs.TryGetValue(state, out var config) ? config : null;
        }

        public static CharacterAnimationProfile CreateDefaultPlayerProfile()
        {
            var p = new CharacterAnimationProfile { ProfileId = "player_default", Archetype = CharacterArchetype.Player };
            p.MapState(AnimationState.Idle, "player_idle", 0.2f, true);
            p.MapState(AnimationState.Walk, "player_walk", 0.2f, true);
            p.MapState(AnimationState.Run, "player_run", 0.15f, true);
            p.MapState(AnimationState.Sprint, "player_sprint", 0.15f, true);
            p.MapState(AnimationState.Jump, "player_jump", 0.1f, false, 1.0f, AnimationPriority.High);
            p.MapState(AnimationState.Attack, "player_attack_01", 0.05f, false, 1.2f, AnimationPriority.High);
            p.MapState(AnimationState.HitReaction, "player_hit", 0.05f, false, 1.0f, AnimationPriority.Interrupt);
            p.MapState(AnimationState.Death, "player_death", 0.1f, false, 1.0f, AnimationPriority.Critical);
            return p;
        }

        public static CharacterAnimationProfile CreateDefaultBossProfile()
        {
            var p = new CharacterAnimationProfile { ProfileId = "boss_titan", Archetype = CharacterArchetype.Boss };
            p.MapState(AnimationState.Idle, "boss_titan_idle", 0.3f, true);
            p.MapState(AnimationState.Walk, "boss_titan_stomp", 0.3f, true);
            p.MapState(AnimationState.Attack, "boss_titan_smash", 0.1f, false, 0.9f, AnimationPriority.High);
            p.MapState(AnimationState.Stunned, "boss_titan_stagger", 0.1f, false, 1.0f, AnimationPriority.Interrupt);
            p.MapState(AnimationState.Death, "boss_titan_collapse", 0.2f, false, 1.0f, AnimationPriority.Critical);
            return p;
        }
    }
}
