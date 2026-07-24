using System;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// Governs scene loads and loading screens animations.
    /// Supports asynchronous load triggers.
    /// </summary>
    public class SceneManager
    {
        public string CurrentSceneName { get; private set; } = "Boot";

        public event Action<string, float>? OnLoadProgress;
        public event Action<string>? OnSceneLoaded;

        public void LoadScene(string sceneName, bool useLoadingScreen = true)
        {
            Logger.Info($"SceneManager: Initiating load for scene '{sceneName}'");

            if (useLoadingScreen)
            {
                // Push loading transition scene first
                CurrentSceneName = "Loading";
                OnLoadProgress?.Invoke(sceneName, 0.0f);
            }

            // Simulate asset loads (Mocked for Phase 2 project init)
            // Real implementation will query Godot's ResourceLoader.LoadThreadedRequest
            SimulateAsyncLoad(sceneName);
        }

        private void SimulateAsyncLoad(string sceneName)
        {
            Logger.Info($"SceneManager: Simulating resource package load for '{sceneName}'...");
            OnLoadProgress?.Invoke(sceneName, 0.5f);
            OnLoadProgress?.Invoke(sceneName, 1.0f);
            
            CurrentSceneName = sceneName;
            OnSceneLoaded?.Invoke(CurrentSceneName);
            Logger.Info($"SceneManager: Loaded scene '{sceneName}' successfully.");
        }
    }
}
