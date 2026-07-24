using System;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// Monitors framerates and dynamically lowers rendering parameters to keep thermals and batteries optimized.
    /// </summary>
    public class PerformanceManager
    {
        private float _fpsAccumulator = 60.0f;
        public float TargetFps { get; private set; } = 60.0f;
        public float CurrentResolutionScale { get; private set; } = 1.0f;

        public event Action<float>? OnResolutionScaleChanged;

        public void Initialize(float targetFps)
        {
            TargetFps = targetFps;
            Logger.Info($"PerformanceManager: Ticking targets registered at {TargetFps} FPS.");
        }

        public void ReportFrameTime(double frameDeltaSeconds)
        {
            float instantFps = (float)(1.0 / frameDeltaSeconds);
            // Low-pass filter to smooth out instantaneous frame time spikes
            _fpsAccumulator = _fpsAccumulator * 0.95f + instantFps * 0.05f;

            if (_fpsAccumulator < TargetFps * 0.8f)
            {
                AdaptPerformance(lower: true);
            }
            else if (_fpsAccumulator > TargetFps * 0.95f)
            {
                AdaptPerformance(lower: false);
            }
        }

        private void AdaptPerformance(bool lower)
        {
            float oldScale = CurrentResolutionScale;
            if (lower)
            {
                CurrentResolutionScale = Math.Max(0.5f, CurrentResolutionScale - 0.05f);
            }
            else
            {
                CurrentResolutionScale = Math.Min(1.0f, CurrentResolutionScale + 0.02f);
            }

            if (Math.Abs(oldScale - CurrentResolutionScale) > 0.001f)
            {
                Logger.Info($"PerformanceManager: Dynamic resolution scale adjusted to {CurrentResolutionScale}x");
                OnResolutionScaleChanged?.Invoke(CurrentResolutionScale);
            }
        }
    }
}
