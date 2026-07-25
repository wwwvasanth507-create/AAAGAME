using System;

namespace HeroOfEternia.Animation
{
    public class AnimationSaveData
    {
        public bool FootIKEnabled { get; set; } = true;
        public bool HandIKEnabled { get; set; } = true;
        public bool ProceduralLookAtEnabled { get; set; } = true;
        public bool WeaponSwayEnabled { get; set; } = true;
        public bool RootMotionEnabled { get; set; } = true;
        public float GlobalIKWeight { get; set; } = 1.0f;
        public bool DebugVisualization { get; set; } = false;
        public int SaveVersion { get; set; } = 17;
    }
}
