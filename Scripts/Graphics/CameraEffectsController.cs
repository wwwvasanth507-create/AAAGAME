using System;
using Godot;

namespace HeroOfEternia.Graphics
{
    /// <summary>
    /// Controller for camera shake impulses, impact zooms, screen damage flashes,
    /// screen fades, and environmental blur.
    /// </summary>
    public partial class CameraEffectsController : Node
    {
        public float CurrentShakeIntensity { get; private set; } = 0.0f;
        private float _shakeDecay = 5.0f;

        public event Action<float>? OnDamageFlashTriggered;
        public event Action<float, float>? OnScreenFadeTriggered;

        public override void _Process(double delta)
        {
            if (CurrentShakeIntensity > 0.001f)
            {
                CurrentShakeIntensity = Mathf.Lerp(CurrentShakeIntensity, 0.0f, (float)delta * _shakeDecay);
            }
        }

        public void TriggerCameraShake(float intensity = 1.0f, float decayRate = 5.0f)
        {
            CurrentShakeIntensity = Math.Max(CurrentShakeIntensity, intensity);
            _shakeDecay = Math.Max(1.0f, decayRate);
        }

        public void TriggerDamageFlash(float opacity = 0.5f)
        {
            OnDamageFlashTriggered?.Invoke(Math.Clamp(opacity, 0.0f, 1.0f));
        }

        public void TriggerScreenFade(float targetAlpha = 1.0f, float duration = 1.0f)
        {
            OnScreenFadeTriggered?.Invoke(Math.Clamp(targetAlpha, 0.0f, 1.0f), duration);
        }
    }
}
