using System;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// Translates multi-touch coordinates, virtual thumbsticks, and external controller states.
    /// </summary>
    public class InputManager
    {
        public float JoystickAxisX { get; private set; } = 0f;
        public float JoystickAxisY { get; private set; } = 0f;
        public bool ActionFirePressed { get; private set; } = false;

        public event Action? OnInputUpdated;

        public void ProcessJoystickTouch(float normalizedX, float normalizedY)
        {
            JoystickAxisX = Math.Clamp(normalizedX, -1f, 1f);
            JoystickAxisY = Math.Clamp(normalizedY, -1f, 1f);
            OnInputUpdated?.Invoke();
        }

        public void SetFireState(bool pressed)
        {
            ActionFirePressed = pressed;
            OnInputUpdated?.Invoke();
        }

        public void ResetInputs()
        {
            JoystickAxisX = 0f;
            JoystickAxisY = 0f;
            ActionFirePressed = false;
            OnInputUpdated?.Invoke();
        }
    }
}
