using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Graphics
{
    /// <summary>
    /// Environmental lighting orchestrator managing time-of-day lighting blends,
    /// interior/dungeon lighting overrides, and smooth profile transitions.
    /// </summary>
    public partial class LightingManager : Node
    {
        private LightingProfile _currentProfile = LightingProfile.CreateDefaultDayProfile();
        private LightingProfile _targetProfile = LightingProfile.CreateDefaultDayProfile();
        private float _transitionTime = 0.0f;
        private float _transitionDuration = 2.0f;
        private bool _isTransitioning = false;

        private readonly Dictionary<LightingContext, LightingProfile> _profileRegistry = new();

        public LightingProfile CurrentProfile => _currentProfile;

        public override void _Ready()
        {
            RegisterProfile(LightingProfile.CreateDefaultDayProfile());
            RegisterProfile(LightingProfile.CreateDefaultNightProfile());
            RegisterProfile(LightingProfile.CreateDefaultDungeonProfile());
        }

        public void RegisterProfile(LightingProfile profile)
        {
            if (profile != null)
            {
                _profileRegistry[profile.Context] = profile;
            }
        }

        public void TransitionToContext(LightingContext context, float duration = 2.0f)
        {
            if (_profileRegistry.TryGetValue(context, out var target))
            {
                _targetProfile = target;
                _transitionDuration = Math.Max(0.1f, duration);
                _transitionTime = 0.0f;
                _isTransitioning = true;
            }
        }

        public override void _Process(double delta)
        {
            if (!_isTransitioning) return;

            _transitionTime += (float)delta;
            float t = Math.Clamp(_transitionTime / _transitionDuration, 0.0f, 1.0f);
            _currentProfile = LightingProfile.Lerp(_currentProfile, _targetProfile, t);

            if (t >= 1.0f)
            {
                _isTransitioning = false;
            }
        }
    }
}
