using System;
using Godot;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// Governs scene loads and loading screens animations.
    /// Supports asynchronous load triggers using Godot's ResourceLoader.
    /// </summary>
    public partial class SceneManager : Node, IInitializable
    {
        public string CurrentSceneName { get; private set; } = "Boot";

        public event Action<string, float>? OnLoadProgress;
        public event Action<string>? OnSceneLoaded;

        private bool _isLoading = false;
        private string? _targetScenePath = null;

        public void Initialize()
        {
            // Attach to the root scene tree dynamically so _Process ticks run.
            var tree = Engine.GetMainLoop() as SceneTree;
            if (tree != null)
            {
                tree.Root.CallDeferred(Node.MethodName.AddChild, this);
                Logger.Info("SceneManager: Node successfully initialized and attached to root.");
            }
            else
            {
                Logger.Warning("SceneManager: Failed to locate active SceneTree. Deferred attach failed.");
            }
        }

        public override void _Process(double delta)
        {
            if (!_isLoading || string.IsNullOrEmpty(_targetScenePath)) return;

            var progressArray = new Godot.Collections.Array();
            var status = ResourceLoader.LoadThreadedGetStatus(_targetScenePath, progressArray);

            switch (status)
            {
                case ResourceLoader.ThreadLoadStatus.InProgress:
                    float progress = progressArray.Count > 0 ? progressArray[0].AsSingle() : 0.0f;
                    OnLoadProgress?.Invoke(_targetScenePath, progress);
                    break;

                case ResourceLoader.ThreadLoadStatus.Loaded:
                    OnLoadProgress?.Invoke(_targetScenePath, 1.0f);
                    var packedScene = ResourceLoader.LoadThreadedGet(_targetScenePath) as PackedScene;
                    
                    if (packedScene != null)
                    {
                        var error = GetTree().ChangeSceneToPacked(packedScene);
                        if (error == Error.Ok)
                        {
                            CurrentSceneName = _targetScenePath;
                            Logger.Info($"SceneManager: Loaded and transitioned to scene '{_targetScenePath}' successfully.");
                        }
                        else
                        {
                            Logger.Error($"SceneManager: Failed to transition scene tree to packed scene. Error={error}");
                        }
                    }
                    else
                    {
                        Logger.Error($"SceneManager: Loaded resource '{_targetScenePath}' could not be cast to PackedScene.");
                    }

                    _isLoading = false;
                    string loadedPath = _targetScenePath;
                    _targetScenePath = null;

                    OnSceneLoaded?.Invoke(loadedPath);

                    // Perform Garbage Collection sweeps for performance optimization on Android
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    break;

                case ResourceLoader.ThreadLoadStatus.Failed:
                case ResourceLoader.ThreadLoadStatus.InvalidResource:
                    Logger.Error($"SceneManager: Threaded load failed for '{_targetScenePath}' with status: {status}");
                    _isLoading = false;
                    _targetScenePath = null;
                    break;
            }
        }

        /// <summary>
        /// Loads a target scene asynchronously, displaying the loading scene as an intermediate layer if specified.
        /// </summary>
        public void LoadScene(string sceneName, bool useLoadingScreen = true)
        {
            string targetPath = ResolveScenePath(sceneName);
            Logger.Info($"SceneManager: Initiating load for scene '{sceneName}' resolved to path: {targetPath}");

            if (useLoadingScreen && sceneName != "Loading")
            {
                CurrentSceneName = "Loading";
                GetTree().ChangeSceneToFile("res://Scenes/Loading.tscn");
            }

            _targetScenePath = targetPath;
            _isLoading = true;

            var error = ResourceLoader.LoadThreadedRequest(targetPath);
            if (error != Error.Ok)
            {
                Logger.Error($"SceneManager: LoadThreadedRequest failed for '{targetPath}'. Error={error}");
                _isLoading = false;
                _targetScenePath = null;
            }
        }

        private string ResolveScenePath(string name)
        {
            if (name.Contains("://")) return name;

            switch (name.ToLowerInvariant())
            {
                case "boot": return "res://Scenes/Boot.tscn";
                case "credits": return "res://Scenes/Credits.tscn";
                case "loading": return "res://Scenes/Loading.tscn";
                case "mainmenu": return "res://Scenes/MainMenu.tscn";
                case "player": return "res://Scenes/Player.tscn";
                case "settings": return "res://Scenes/Settings.tscn";
                case "splash": return "res://Scenes/Splash.tscn";
                case "testenvironment": return "res://Scenes/TestEnvironment.tscn";
                case "gameworld": return "res://Scenes/GameWorld.tscn";
                case "hud": return "res://Scenes/HUD.tscn";
                default: return $"res://Scenes/{name}.tscn";
            }
        }
    }
}
