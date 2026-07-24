using System;
using System.Collections.Generic;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// Governs stacked layout cards (Main Menu overlays, HUD screens, popups, and pause panels).
    /// </summary>
    public class UIManager
    {
        private readonly Stack<string> _screenStack = new Stack<string>();
        public string CurrentScreen => _screenStack.Count > 0 ? _screenStack.Peek() : "None";

        public event Action<string>? OnScreenOpened;
        public event Action? OnScreenClosed;

        public void PushScreen(string screenName)
        {
            Logger.Info($"UIManager: Loading interface layer: {screenName}");
            _screenStack.Push(screenName);
            OnScreenOpened?.Invoke(screenName);
        }

        public void PopScreen()
        {
            if (_screenStack.Count > 0)
            {
                string closed = _screenStack.Pop();
                Logger.Info($"UIManager: Hiding interface layer: {closed}");
                OnScreenClosed?.Invoke();
            }
        }

        public void ClearUI()
        {
            Logger.Info("UIManager: Purging layout sheets.");
            _screenStack.Clear();
        }
    }
}
