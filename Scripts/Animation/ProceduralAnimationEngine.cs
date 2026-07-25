using System;
using Godot;

namespace HeroOfEternia.Animation
{
    /// <summary>
    /// Procedural animation engine managing head look-at, breathing, weapon sway,
    /// secondary motion, aim adjustment, and idle variations.
    /// </summary>
    public partial class ProceduralAnimationEngine : Node
    {
        public bool IsEnabled { get; set; } = true;

        // Head look-at target
        public Vector3 LookAtTarget { get; set; } = Vector3.Zero;
        public float LookAtWeight { get; set; } = 0.0f;
        public float MaxLookAtAngleDegrees { get; set; } = 75f;

        // Idle Breathing
        public float BreathingFrequency { get; set; } = 1.2f;
        public float BreathingAmplitude { get; set; } = 0.03f;
        private float _breathingTime = 0.0f;

        // Weapon Sway
        public Vector2 MovementInputVelocity { get; set; } = Vector2.Zero;
        public Vector3 CurrentSwayOffset { get; private set; } = Vector3.Zero;

        // Aim Adjustment
        public float AimPitchDegrees { get; set; } = 0.0f;

        public override void _Process(double delta)
        {
            if (!IsEnabled) return;

            float dt = (float)delta;
            UpdateBreathing(dt);
            UpdateWeaponSway(dt);
        }

        private void UpdateBreathing(float delta)
        {
            _breathingTime += delta * BreathingFrequency;
            float offset = Mathf.Sin(_breathingTime) * BreathingAmplitude;
            // Applied during bone evaluation phase
        }

        private void UpdateWeaponSway(float delta)
        {
            Vector3 targetSway = new Vector3(
                -MovementInputVelocity.X * 0.05f,
                -Mathf.Abs(MovementInputVelocity.Y) * 0.03f,
                0f
            );
            CurrentSwayOffset = CurrentSwayOffset.Lerp(targetSway, delta * 10f);
        }

        public void SetLookAtTarget(Vector3 targetPosition, float weight = 1.0f)
        {
            LookAtTarget = targetPosition;
            LookAtWeight = Math.Clamp(weight, 0.0f, 1.0f);
        }

        public void ClearLookAt()
        {
            LookAtWeight = 0.0f;
        }
    }
}
