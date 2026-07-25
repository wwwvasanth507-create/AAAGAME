using System;
using Godot;

namespace HeroOfEternia.Graphics
{
    public enum ShadowQualityLevel
    {
        Off,
        Low,
        Medium,
        High
    }

    /// <summary>
    /// Manager for LOD distance culling, shadow quality limits, GPU instancing toggles,
    /// dynamic resolution scaling, and mobile performance budgets.
    /// </summary>
    public class RenderingOptimizationManager
    {
        public ShadowQualityLevel ShadowQuality { get; set; } = ShadowQualityLevel.Medium;
        public float MaxDrawDistance { get; set; } = 150f;
        public bool EnableGPUInstancing { get; set; } = true;
        public bool EnableDynamicResolution { get; set; } = false;
        public float TargetRenderScale { get; set; } = 1.0f;

        public void ApplyQualitySettings(GraphicsQualityPreset preset)
        {
            switch (preset)
            {
                case GraphicsQualityPreset.Low:
                    ShadowQuality = ShadowQualityLevel.Off;
                    MaxDrawDistance = 80f;
                    EnableGPUInstancing = true;
                    TargetRenderScale = 0.8f;
                    break;
                case GraphicsQualityPreset.Medium:
                    ShadowQuality = ShadowQualityLevel.Low;
                    MaxDrawDistance = 120f;
                    EnableGPUInstancing = true;
                    TargetRenderScale = 0.9f;
                    break;
                default:
                    ShadowQuality = ShadowQualityLevel.High;
                    MaxDrawDistance = 200f;
                    EnableGPUInstancing = true;
                    TargetRenderScale = 1.0f;
                    break;
            }
        }
    }
}
