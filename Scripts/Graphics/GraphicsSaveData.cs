using System;

namespace HeroOfEternia.Graphics
{
    public class GraphicsSaveData
    {
        public GraphicsQualityPreset QualityPreset { get; set; } = GraphicsQualityPreset.High;
        public bool BloomEnabled { get; set; } = true;
        public bool AmbientOcclusionEnabled { get; set; } = true;
        public bool DepthOfFieldEnabled { get; set; } = false;
        public bool MotionBlurEnabled { get; set; } = false;
        public bool VignetteEnabled { get; set; } = true;
        public ShadowQualityLevel ShadowQuality { get; set; } = ShadowQualityLevel.Medium;
        public float TargetRenderScale { get; set; } = 1.0f;
        public int SaveVersion { get; set; } = 18;
    }
}
