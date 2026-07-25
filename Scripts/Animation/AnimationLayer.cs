using System;
using Godot;

namespace HeroOfEternia.Animation
{
    /// <summary>
    /// Represents an independent animation blending layer with mask and weight controls.
    /// </summary>
    public class AnimationLayer
    {
        public AnimationLayerType LayerType { get; }
        public float Weight { get; set; } = 1.0f;
        public bool IsAdditive { get; set; } = false;
        public string BoneMask { get; set; } = string.Empty;
        public AnimationState CurrentState { get; private set; } = AnimationState.Idle;
        public AnimationPriority CurrentPriority { get; private set; } = AnimationPriority.Low;

        public AnimationLayer(AnimationLayerType type, float weight = 1.0f, bool isAdditive = false, string boneMask = "")
        {
            LayerType = type;
            Weight = Math.Clamp(weight, 0.0f, 1.0f);
            IsAdditive = isAdditive;
            BoneMask = boneMask;
        }

        public bool SetState(AnimationState state, AnimationPriority priority)
        {
            if (priority >= CurrentPriority || CurrentState == state)
            {
                CurrentState = state;
                CurrentPriority = priority;
                return true;
            }
            return false;
        }

        public void ResetPriority()
        {
            CurrentPriority = AnimationPriority.Low;
        }
    }
}
