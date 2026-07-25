using System;
using Godot;

namespace HeroOfEternia.Animation
{
    /// <summary>
    /// Root motion extraction engine applying animation position and rotation deltas to character movement.
    /// Supports network-ready position sync hooks for future multiplayer.
    /// </summary>
    public partial class RootMotionController : Node
    {
        public bool RootMotionEnabled { get; set; } = false;
        public Vector3 CurrentRootDeltaPosition { get; private set; } = Vector3.Zero;
        public Quaternion CurrentRootDeltaRotation { get; private set; } = Quaternion.Identity;

        public event Action<Vector3, Quaternion>? OnRootMotionExtracted;

        public void ProcessRootMotion(Vector3 deltaPosition, Quaternion deltaRotation, float speedScale = 1.0f)
        {
            if (!RootMotionEnabled) return;

            CurrentRootDeltaPosition = deltaPosition * speedScale;
            CurrentRootDeltaRotation = deltaRotation;

            OnRootMotionExtracted?.Invoke(CurrentRootDeltaPosition, CurrentRootDeltaRotation);
        }

        public void ResetDeltas()
        {
            CurrentRootDeltaPosition = Vector3.Zero;
            CurrentRootDeltaRotation = Quaternion.Identity;
        }
    }
}
