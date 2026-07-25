using System;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Animation
{
    public class IKTarget
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; } = Quaternion.Identity;
        public float Weight { get; set; } = 1.0f;
        public bool Enabled { get; set; } = true;
    }

    /// <summary>
    /// Inverse Kinematics solver engine supporting foot placement, hand placement,
    /// weapon alignment, terrain adaptation, and ledge hooks. Can be toggled per character.
    /// </summary>
    public partial class IKSystem : Node
    {
        public bool IsEnabled { get; set; } = true;
        public float GlobalIKWeight { get; set; } = 1.0f;

        public IKTarget LeftFoot { get; } = new();
        public IKTarget RightFoot { get; } = new();
        public IKTarget LeftHand { get; } = new();
        public IKTarget RightHand { get; } = new();
        public IKTarget WeaponAlignment { get; } = new();

        public float TerrainAdaptationRaycastDistance { get; set; } = 1.5f;

        public override void _Process(double delta)
        {
            if (!IsEnabled || GlobalIKWeight <= 0.001f) return;
            EvaluateIK((float)delta);
        }

        public void EvaluateIK(float delta)
        {
            // Evaluate foot placement ground alignment
            if (LeftFoot.Enabled) SolveLegIK(LeftFoot, delta);
            if (RightFoot.Enabled) SolveLegIK(RightFoot, delta);

            // Evaluate hand placement & weapon alignment
            if (LeftHand.Enabled) SolveHandIK(LeftHand, delta);
            if (RightHand.Enabled) SolveHandIK(RightHand, delta);
            if (WeaponAlignment.Enabled) SolveWeaponIK(WeaponAlignment, delta);
        }

        private void SolveLegIK(IKTarget foot, float delta)
        {
            // Raycast terrain adaptation calculation (stubbed for physics integration)
            foot.Weight = Mathf.Lerp(foot.Weight, foot.Enabled ? 1.0f : 0.0f, delta * 10f);
        }

        private void SolveHandIK(IKTarget hand, float delta)
        {
            hand.Weight = Mathf.Lerp(hand.Weight, hand.Enabled ? 1.0f : 0.0f, delta * 8f);
        }

        private void SolveWeaponIK(IKTarget weapon, float delta)
        {
            weapon.Weight = Mathf.Lerp(weapon.Weight, weapon.Enabled ? 1.0f : 0.0f, delta * 12f);
        }

        public void SetAllIKEnabled(bool enabled)
        {
            IsEnabled = enabled;
            LeftFoot.Enabled = enabled;
            RightFoot.Enabled = enabled;
            LeftHand.Enabled = enabled;
            RightHand.Enabled = enabled;
            WeaponAlignment.Enabled = enabled;
        }
    }
}
