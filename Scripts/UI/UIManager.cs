using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.UI
{
    /// <summary>
    /// Central UI manager responsible for screen lifecycle, navigation stack,
    /// modal dialogs, layer management, focus management, transition animations,
    /// input routing, UI state persistence, and future plugin support.
    /// Registered in ServiceLocator as IInitializable.
    /// </summary>
    public class UIManager : IInitializable
    {
        // ---------------------------------------------------------------
        // Constants
        // ---------------------------------------------------------------
        private const int MaxScreenStackDepth = 20;
        private const float DefaultTransitionDuration = 0.25f;
        private const string UIPrefsKey = "ui_preferences";

        // ---------------------------------------------------------------
        // Layer definitions (rendered in order)
        // ---------------------------------------------------------------
        public enum UILayer
        {
            Background = 0,
            Game = 1,
            HUD = 2,
            Screens = 3,
            Popups = 4,
            Modals = 5,
            Notifications = 6,
            Tooltips = 7,
            Debug = 8,
            Overlay = 9
        }

        // ---------------------------------------------------------------
        // Screen state
        // ---------------------------------------------------------------
        private readonly Stack<UIScreen> _screenStack = new Stack<UIScreen>();
        private readonly Dictionary<string, UIScreen> _registeredScreens = new Dictionary<string, UIScreen>();
        private readonly Dictionary<UILayer, CanvasLayer> _layers = new Dictionary<UILayer, CanvasLayer>();
        private readonly List<UIModal> _activeModals = new List<UIModal>();
        private readonly Queue<UINotification> _notificationQueue = new Queue<UINotification>();
        private readonly List<IUIPlugin> _plugins = new List<IUIPlugin>();

        private UIScreen _currentScreen;
        private bool _isTransitioning;
        private bool _initialized;
        private Node _uiRoot;
        private Window _gameWindow;

        // ---------------------------------------------------------------
        // Events
        // ---------------------------------------------------------------
        public event Action<string> OnScreenOpened;
        public event Action<string> OnScreenClosed;
        public event Action<string> OnModalOpened;
        public event Action<string> OnModalClosed;
        public event Action<UILayer> OnLayerVisibilityChanged;
        public event Action OnTransitionStarted;
        public event Action OnTransitionCompleted;
        public event Action<UIPreferences> OnPreferencesChanged;

        // ---------------------------------------------------------------
        // Properties
        // ---------------------------------------------------------------
        public UIScreen CurrentScreen => _currentScreen;
        public int ScreenStackDepth => _screenStack.Count;
        public bool IsTransitioning => _isTransitioning;
        public UIPreferences Preferences { get; private set; } = new UIPreferences();
        public Node UIRoot => _uiRoot;

        // ---------------------------------------------------------------
        // Initialization
        // ---------------------------------------------------------------
        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("UIManager: Initializing UI framework...");

            // Create UI root
            _gameWindow = Engine.GetMainLoop() as Window ?? new Window();
            _uiRoot = new CanvasLayer { Layer = 0, Name = "UIRoot" };
            _gameWindow.AddChild(_uiRoot);

            // Create all layers
            foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
            {
                var canvasLayer = new CanvasLayer
                {
                    Layer = (int)layer * 10,
                    Name = $"Layer_{layer}"
                };
                _uiRoot.AddChild(canvasLayer);
                _layers[layer] = canvasLayer;
            }

            // Load saved preferences
            LoadPreferences();

            Logger.Info("UIManager: UI framework initialized successfully.");
        }

        public void Shutdown()
        {
            Logger.Info("UIManager: Shutting down UI framework...");

            SavePreferences();
            ClearAll();
            _plugins.Clear();

            if (_uiRoot != null && IsInstanceValid(_uiRoot))
            {
                _uiRoot.QueueFree();
            }

            _initialized = false;
            Logger.Info("UIManager: Shutdown complete.");
        }

        // ---------------------------------------------------------------
        // Screen Registration & Lifecycle
        // ---------------------------------------------------------------
        public void RegisterScreen(string screenId, UIScreen screen)
        {
            if (_registeredScreens.ContainsKey(screenId))
            {
                Logger.Warning($"UIManager: Screen '{screenId}' already registered. Overwriting.");
            }
            _registeredScreens[screenId] = screen;
            Logger.Info($"UIManager: Screen '{screenId}' registered.");
        }

        public void UnregisterScreen(string screenId)
        {
            if (_registeredScreens.Remove(screenId))
            {
                Logger.Info($"UIManager: Screen '{screenId}' unregistered.");
            }
        }

        public T GetScreen<T>(string screenId) where T : UIScreen
        {
            if (_registeredScreens.TryGetValue(screenId, out var screen))
                return screen as T;
            return null;
        }

        public void OpenScreen(string screenId, object args = null, bool animated = true)
        {
            if (_isTransitioning)
            {
                Logger.Warning("UIManager: Cannot open screen during transition.");
                return;
            }

            if (!_registeredScreens.TryGetValue(screenId, out var screen))
            {
                Logger.Error($"UIManager: Screen '{screenId}' not registered.");
                return;
            }

            if (_screenStack.Count >= MaxScreenStackDepth)
            {
                Logger.Error($"UIManager: Screen stack overflow (max {MaxScreenStackDepth}).");
                return;
            }

            _isTransitioning = true;
            OnTransitionStarted?.Invoke();

            // Hide current screen
            if (_currentScreen != null)
            {
                _currentScreen.OnDeactivate();
                if (animated)
                    AnimateScreenOut(_currentScreen, () => { });
                else
                    _currentScreen.Visible = false;
            }

            // Push to stack
            _screenStack.Push(screen);
            _currentScreen = screen;

            // Add to screen layer
            var screenLayer = _layers[UILayer.Screens];
            if (screen.Parent != screenLayer)
            {
                screenLayer.AddChild(screen);
            }

            // Show new screen
            screen.Visible = true;
            screen.OnActivate(args);

            if (animated)
                AnimateScreenIn(screen, () => { });
            else
                screen.Modulate = Colors.White;

            _isTransitioning = false;
            OnTransitionCompleted?.Invoke();
            OnScreenOpened?.Invoke(screenId);

            // Notify plugins
            foreach (var plugin in _plugins)
                plugin.OnScreenOpened(screenId);

            Logger.Info($"UIManager: Screen '{screenId}' opened.");
        }

        public void CloseScreen(bool animated = true)
        {
            if (_isTransitioning)
            {
                Logger.Warning("UIManager: Cannot close screen during transition.");
                return;
            }

            if (_screenStack.Count == 0)
            {
                Logger.Warning("UIManager: No screens to close.");
                return;
            }

            var closingScreen = _screenStack.Pop();
            _isTransitioning = true;
            OnTransitionStarted?.Invoke();

            string screenId = GetScreenId(closingScreen);

            closingScreen.OnDeactivate();
            if (animated)
                AnimateScreenOut(closingScreen, () =>
                {
                    closingScreen.Visible = false;
                    if (closingScreen.Parent == _layers[UILayer.Screens])
                        _layers[UILayer.Screens].RemoveChild(closingScreen);
                });
            else
            {
                closingScreen.Visible = false;
                if (closingScreen.Parent == _layers[UILayer.Screens])
                    _layers[UILayer.Screens].RemoveChild(closingScreen);
            }

            // Restore previous screen
            if (_screenStack.Count > 0)
            {
                _currentScreen = _screenStack.Peek();
                _currentScreen.Visible = true;
                _currentScreen.OnActivate(null);
                if (animated)
                    AnimateScreenIn(_currentScreen, () => { });
            }
            else
            {
                _currentScreen = null;
            }

            _isTransitioning = false;
            OnTransitionCompleted?.Invoke();
            OnScreenClosed?.Invoke(screenId);

            foreach (var plugin in _plugins)
                plugin.OnScreenClosed(screenId);

            Logger.Info($"UIManager: Screen '{screenId}' closed.");
        }

        public void CloseToRoot(bool animated = true)
        {
            while (_screenStack.Count > 1)
            {
                CloseScreen(animated);
            }
        }

        public void ClearAll()
        {
            _screenStack.Clear();
            _activeModals.Clear();
            _notificationQueue.Clear();
            _currentScreen = null;

            foreach (var layer in _layers.Values)
            {
                foreach (Node child in layer.GetChildren())
                {
                    layer.RemoveChild(child);
                    child.QueueFree();
                }
            }

            Logger.Info("UIManager: All UI cleared.");
        }

        // ---------------------------------------------------------------
        // Modal Dialogs
        // ---------------------------------------------------------------
        public void ShowModal(UIModal modal, bool animated = true)
        {
            if (_activeModals.Contains(modal))
            {
                Logger.Warning("UIManager: Modal already active.");
                return;
            }

            _activeModals.Add(modal);
            var modalLayer = _layers[UILayer.Modals];
            modalLayer.AddChild(modal);
            modal.OnOpen();

            if (animated)
                AnimateModalIn(modal);

            OnModalOpened?.Invoke(modal.Name);
            Logger.Info($"UIManager: Modal '{modal.Name}' shown.");
        }

        public void CloseModal(UIModal modal, bool animated = true)
        {
            if (!_activeModals.Remove(modal))
            {
                Logger.Warning("UIManager: Modal not active.");
                return;
            }

            modal.OnClose();
            if (animated)
                AnimateModalOut(modal, () =>
                {
                    if (modal.Parent == _layers[UILayer.Modals])
                        _layers[UILayer.Modals].RemoveChild(modal);
                });
            else
            {
                if (modal.Parent == _layers[UILayer.Modals])
                    _layers[UILayer.Modals].RemoveChild(modal);
            }

            OnModalClosed?.Invoke(modal.Name);
            Logger.Info($"UIManager: Modal '{modal.Name}' closed.");
        }

        public void CloseTopModal(bool animated = true)
        {
            if (_activeModals.Count > 0)
                CloseModal(_activeModals[_activeModals.Count - 1], animated);
        }

        // ---------------------------------------------------------------
        // Layer Management
        // ---------------------------------------------------------------
        public void SetLayerVisible(UILayer layer, bool visible)
        {
            if (_layers.TryGetValue(layer, out var canvasLayer))
            {
                canvasLayer.Visible = visible;
                OnLayerVisibilityChanged?.Invoke(layer);
            }
        }

        public bool IsLayerVisible(UILayer layer)
        {
            return _layers.TryGetValue(layer, out var canvasLayer) && canvasLayer.Visible;
        }

        public CanvasLayer GetLayer(UILayer layer)
        {
            return _layers.TryGetValue(layer, out var canvasLayer) ? canvasLayer : null;
        }

        // ---------------------------------------------------------------
        // Focus Management
        // ---------------------------------------------------------------
        public void SetFocus(Control control)
        {
            if (control != null)
                control.GrabFocus();
        }

        public void ClearFocus()
        {
            if (_gameWindow != null)
            {
                var focusOwner = _gameWindow.GuiGetFocusOwner();
                if (focusOwner != null)
                    focusOwner.ReleaseFocus();
            }
        }

        // ---------------------------------------------------------------
        // Transition Animations
        // ---------------------------------------------------------------
        private void AnimateScreenIn(UIScreen screen, Action onComplete)
        {
            if (screen == null || !IsInstanceValid(screen))
            {
                onComplete?.Invoke();
                return;
            }

            screen.Modulate = new Color(1, 1, 1, 0);
            screen.Scale = new Vector2(0.95f, 0.95f);

            var tween = screen.CreateTween();
            tween.SetParallel(true);
            tween.TweenProperty(screen, "modulate", Colors.White, DefaultTransitionDuration)
                 .SetEase(Tween.EaseType.Out)
                 .SetTrans(Tween.TransitionType.Quad);
            tween.TweenProperty(screen, "scale", Vector2.One, DefaultTransitionDuration)
                 .SetEase(Tween.EaseType.Out)
                 .SetTrans(Tween.TransitionType.Back);
            tween.Finished += () => onComplete?.Invoke();
        }

        private void AnimateScreenOut(UIScreen screen, Action onComplete)
        {
            if (screen == null || !IsInstanceValid(screen))
            {
                onComplete?.Invoke();
                return;
            }

            var tween = screen.CreateTween();
            tween.SetParallel(true);
            tween.TweenProperty(screen, "modulate", new Color(1, 1, 1, 0), DefaultTransitionDuration * 0.75f)
                 .SetEase(Tween.EaseType.In)
                 .SetTrans(Tween.TransitionType.Quad);
            tween.TweenProperty(screen, "scale", new Vector2(1.05f, 1.05f), DefaultTransitionDuration * 0.75f)
                 .SetEase(Tween.EaseType.In)
                 .SetTrans(Tween.TransitionType.Quad);
            tween.Finished += () => onComplete?.Invoke();
        }

        private void AnimateModalIn(UIModal modal)
        {
            if (modal == null || !IsInstanceValid(modal)) return;

            modal.Modulate = new Color(1, 1, 1, 0);
            var tween = modal.CreateTween();
            tween.TweenProperty(modal, "modulate", Colors.White, DefaultTransitionDuration)
                 .SetEase(Tween.EaseType.Out)
                 .SetTrans(Tween.TransitionType.Quad);
        }

        private void AnimateModalOut(UIModal modal, Action onComplete)
        {
            if (modal == null || !IsInstanceValid(modal))
            {
                onComplete?.Invoke();
                return;
            }

            var tween = modal.CreateTween();
            tween.TweenProperty(modal, "modulate", new Color(1, 1, 1, 0), DefaultTransitionDuration * 0.75f)
                 .SetEase(Tween.EaseType.In)
                 .SetTrans(Tween.TransitionType.Quad);
            tween.Finished += () => onComplete?.Invoke();
        }

        // ---------------------------------------------------------------
        // Input Routing
        // ---------------------------------------------------------------
        public bool HandleBackButton()
        {
            // Close top modal first
            if (_activeModals.Count > 0)
            {
                CloseTopModal();
                return true;
            }

            // Close current screen if not root
            if (_screenStack.Count > 1)
            {
                CloseScreen();
                return true;
            }

            return false;
        }

        // ---------------------------------------------------------------
        // Plugin Support
        // ---------------------------------------------------------------
        public void RegisterPlugin(IUIPlugin plugin)
        {
            if (!_plugins.Contains(plugin))
            {
                _plugins.Add(plugin);
                plugin.OnRegistered(this);
                Logger.Info($"UIManager: Plugin '{plugin.GetType().Name}' registered.");
            }
        }

        public void UnregisterPlugin(IUIPlugin plugin)
        {
            if (_plugins.Remove(plugin))
            {
                plugin.OnUnregistered();
                Logger.Info($"UIManager: Plugin '{plugin.GetType().Name}' unregistered.");
            }
        }

        // ---------------------------------------------------------------
        // Preferences Persistence
        // ---------------------------------------------------------------
        public void SavePreferences()
        {
            try
            {
                string json = JSON.Stringify(new Dictionary<string, Variant>
                {
                    ["hud_enabled"] = Preferences.HUDEnabled,
                    ["ui_scale"] = Preferences.UIScale,
                    ["text_size"] = Preferences.TextSize,
                    ["high_contrast"] = Preferences.HighContrast,
                    ["reduced_motion"] = Preferences.ReducedMotion,
                    ["last_screen"] = Preferences.LastOpenedScreen ?? "",
                    ["notifications_enabled"] = Preferences.NotificationsEnabled,
                    ["accessibility_font"] = Preferences.AccessibilityFont,
                    ["color_blind_mode"] = Preferences.ColorBlindMode,
                    ["subtitle_enabled"] = Preferences.SubtitleEnabled,
                    ["haptic_feedback"] = Preferences.HapticFeedback,
                    ["show_fps"] = Preferences.ShowFPS
                });

                var settings = ServiceLocator.Get<SettingsManager>();
                settings?.SetSetting(UIPrefsKey, json);
                Logger.Info("UIManager: Preferences saved.");
            }
            catch (Exception ex)
            {
                Logger.Error($"UIManager: Failed to save preferences: {ex.Message}");
            }
        }

        private void LoadPreferences()
        {
            try
            {
                var settings = ServiceLocator.Get<SettingsManager>();
                string json = settings?.GetSetting(UIPrefsKey) as string;
                if (string.IsNullOrEmpty(json)) return;

                var dict = JSON.ParseString(json) as Godot.Collections.Dictionary;
                if (dict == null) return;

                if (dict.ContainsKey("hud_enabled"))
                    Preferences.HUDEnabled = (bool)dict["hud_enabled"];
                if (dict.ContainsKey("ui_scale"))
                    Preferences.UIScale = (float)(double)dict["ui_scale"];
                if (dict.ContainsKey("text_size"))
                    Preferences.TextSize = (float)(double)dict["text_size"];
                if (dict.ContainsKey("high_contrast"))
                    Preferences.HighContrast = (bool)dict["high_contrast"];
                if (dict.ContainsKey("reduced_motion"))
                    Preferences.ReducedMotion = (bool)dict["reduced_motion"];
                if (dict.ContainsKey("last_screen"))
                    Preferences.LastOpenedScreen = (string)dict["last_screen"];
                if (dict.ContainsKey("notifications_enabled"))
                    Preferences.NotificationsEnabled = (bool)dict["notifications_enabled"];
                if (dict.ContainsKey("accessibility_font"))
                    Preferences.AccessibilityFont = (string)dict["accessibility_font"];
                if (dict.ContainsKey("color_blind_mode"))
                    Preferences.ColorBlindMode = (string)dict["color_blind_mode"];
                if (dict.ContainsKey("subtitle_enabled"))
                    Preferences.SubtitleEnabled = (bool)dict["subtitle_enabled"];
                if (dict.ContainsKey("haptic_feedback"))
                    Preferences.HapticFeedback = (bool)dict["haptic_feedback"];
                if (dict.ContainsKey("show_fps"))
                    Preferences.ShowFPS = (bool)dict["show_fps"];

                Logger.Info("UIManager: Preferences loaded.");
            }
            catch (Exception ex)
            {
                Logger.Warning($"UIManager: Failed to load preferences: {ex.Message}");
            }
        }

        public void ApplyPreferences()
        {
            OnPreferencesChanged?.Invoke(Preferences);
            Logger.Info("UIManager: Preferences applied.");
        }

        // ---------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------
        private string GetScreenId(UIScreen screen)
        {
            foreach (var kvp in _registeredScreens)
            {
                if (kvp.Value == screen)
                    return kvp.Key;
            }
            return screen.Name;
        }

        private bool IsInstanceValid(Node node)
        {
            return node != null && Godot.Object.IsInstanceValid(node);
        }
    }

    // ---------------------------------------------------------------
    // UI Preferences Data Model
    // ---------------------------------------------------------------
    public class UIPreferences
    {
        public bool HUDEnabled { get; set; } = true;
        public float UIScale { get; set; } = 1.0f;
        public float TextSize { get; set; } = 1.0f;
        public bool HighContrast { get; set; } = false;
        public bool ReducedMotion { get; set; } = false;
        public string LastOpenedScreen { get; set; } = null;
        public bool NotificationsEnabled { get; set; } = true;
        public string AccessibilityFont { get; set; } = "default";
        public string ColorBlindMode { get; set; } = "none";
        public bool SubtitleEnabled { get; set; } = true;
        public bool HapticFeedback { get; set; } = true;
        public bool ShowFPS { get; set; } = false;
    }

    // ---------------------------------------------------------------
    // UI Plugin Interface
    // ---------------------------------------------------------------
    public interface IUIPlugin
    {
        void OnRegistered(UIManager manager);
        void OnUnregistered();
        void OnScreenOpened(string screenId);
        void OnScreenClosed(string screenId);
        void OnUpdate(float delta);
    }

    // ---------------------------------------------------------------
    // Base UIScreen class
    // ---------------------------------------------------------------
    public abstract partial class UIScreen : Control
    {
        public bool IsActive { get; private set; }
        public bool LazyLoad { get; set; } = false;
        public bool IsLazyLoaded { get; set; } = false;

        public virtual void OnActivate(object args)
        {
            IsActive = true;
            Visible = true;

            if (LazyLoad && !IsLazyLoaded)
            {
                OnLazyLoad();
                IsLazyLoaded = true;
            }
        }

        public virtual void OnDeactivate()
        {
            IsActive = false;
        }

        protected virtual void OnLazyLoad()
        {
            // Override in derived screens for lazy initialization
        }

        public virtual void OnBackPressed()
        {
            var uiManager = ServiceLocator.Get<UIManager>();
            uiManager?.CloseScreen();
        }

        public virtual void OnScreenResized(Vector2 newSize)
        {
            // Override for responsive layout
        }
    }

    // ---------------------------------------------------------------
    // Base UIModal class
    // ---------------------------------------------------------------
    public abstract partial class UIModal : Control
    {
        public bool IsOpen { get; private set; }
        public bool BlockInput { get; set; } = true;

        public virtual void OnOpen()
        {
            IsOpen = true;
            Visible = true;
        }

        public virtual void OnClose()
        {
            IsOpen = false;
            Visible = false;
        }

        public virtual void OnBackPressed()
        {
            var uiManager = ServiceLocator.Get<UIManager>();
            uiManager?.CloseModal(this);
        }
    }

    // ---------------------------------------------------------------
    // UINotification data model
    // ---------------------------------------------------------------
    public class UINotification
    {
        public string Id { get; set; }
        public string TitleKey { get; set; }
        public string MessageKey { get; set; }
        public string Icon { get; set; }
        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
        public float Duration { get; set; } = 3.0f;
        public bool Persistent { get; set; } = false;
        public Action OnClick { get; set; }
        public Action OnDismiss { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    public enum NotificationPriority
    {
        Low = 0,
        Normal = 1,
        High = 2,
        Critical = 3
    }
}