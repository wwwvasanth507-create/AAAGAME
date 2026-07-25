using System;
using Godot;

namespace HeroOfEternia.Graphics
{
    /// <summary>
    /// Post-processing manager applying profile settings to WorldEnvironment nodes.
    /// </summary>
    public partial class PostProcessingManager : Node
    {
        public PostProcessingProfile CurrentProfile { get; private set; } = PostProcessingProfile.GetPreset(GraphicsQualityPreset.High);

        public void ApplyProfile(PostProcessingProfile profile)
        {
            CurrentProfile = profile ?? PostProcessingProfile.GetPreset(GraphicsQualityPreset.Medium);
            // Apply to WorldEnvironment Environment resource
        }

        public void SetQualityPreset(GraphicsQualityPreset preset)
        {
            ApplyProfile(PostProcessingProfile.GetPreset(preset));
        }
    }
}
