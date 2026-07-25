using System;
using Godot;

namespace HeroOfEternia.Graphics
{
    public enum GraphicsQualityPreset
    {
        Low,
        Medium,
        High,
        Custom
    }

    /// <summary>
    /// Post-processing profile configuration model for Bloom, AO, DOF, Motion Blur, and Vignette.
    /// </summary>
    public class PostProcessingProfile
    {
        public GraphicsQualityPreset QualityPreset { get; set; } = GraphicsQualityPreset.High;
        public bool EnableBloom { get; set; } = true;
        public float BloomIntensity { get; set; } = 0.5f;
        public bool EnableAO { get; set; } = true;
        public float AOIntensity { get; set; } = 1.0f;
        public bool EnableDOF { get; set; } = false;
        public float DOFFarDistance { get; set; } = 50f;
        public bool EnableMotionBlur { get; set; } = false;
        public bool EnableVignette { get; set; } = true;
        public float VignetteIntensity { get; set; } = 0.3f;
        public float Exposure { get; set; } = 1.0f;

        public static PostProcessingProfile GetPreset(GraphicsQualityPreset preset)
        {
            return preset switch
            {
                GraphicsQualityPreset.Low => new PostProcessingProfile
                {
                    QualityPreset = GraphicsQualityPreset.Low,
                    EnableBloom = false,
                    EnableAO = false,
                    EnableDOF = false,
                    EnableMotionBlur = false,
                    EnableVignette = false,
                    Exposure = 1.0f
                },
                GraphicsQualityPreset.Medium => new PostProcessingProfile
                {
                    QualityPreset = GraphicsQualityPreset.Medium,
                    EnableBloom = true,
                    BloomIntensity = 0.3f,
                    EnableAO = false,
                    EnableDOF = false,
                    EnableMotionBlur = false,
                    EnableVignette = true,
                    VignetteIntensity = 0.2f,
                    Exposure = 1.0f
                },
                _ => new PostProcessingProfile
                {
                    QualityPreset = GraphicsQualityPreset.High,
                    EnableBloom = true,
                    BloomIntensity = 0.5f,
                    EnableAO = true,
                    AOIntensity = 1.0f,
                    EnableDOF = true,
                    EnableMotionBlur = false,
                    EnableVignette = true,
                    VignetteIntensity = 0.3f,
                    Exposure = 1.0f
                }
            };
        }
    }
}
