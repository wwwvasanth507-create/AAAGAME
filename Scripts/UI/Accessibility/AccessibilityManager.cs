using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.UI.Accessibility
{
    /// <summary>
    /// Accessibility manager providing adjustable text size, high contrast mode,
    /// color-blind friendly hooks, subtitle framework, UI scaling, reduced motion mode,
    /// screen reader labels, haptic feedback toggle, and future voice navigation hooks.
    /// Registered in ServiceLocator as IInitializable.
    /// </summary>
    public class AccessibilityManager : IInitializable
    {
        // ---------------------------------------------------------------
        // Accessibility Settings
        // ---------------------------------------------------------------
        public float TextScale { get; set; } = 1.0f;
        public bool HighContrast { get; set; } = false;
        public ColorBlindMode ColorBlindMode { get; set; } = ColorBlindMode.None;
        public bool SubtitleEnabled { get; set; } = true;
        public float SubtitleSize { get; set; } = 1.0f;
        public float UIScale { get; set; } = 1.0f;
        public bool ReducedMotion { get; set; } = false;
        public bool ScreenReaderEnabled { get; set; } = false;
        public bool HapticFeedback { get; set; } = true;
        public bool VoiceNavigation { get; set; } = false;

        // ---------------------------------------------------------------
        // Color Filters for Color Blindness
        // ---------------------------------------------------------------
        private static readonly Color ProtanopiaFilter = new Color(0.567f, 0.433f, 0);
        private static readonly Color DeuteranopiaFilter = new Color(0.625f, 0.375f, 0);
        private static readonly Color TritanopiaFilter = new Color(0.95f, 0.433f, 0.475f);

        // ---------------------------------------------------------------
        // Events
        // ---------------------------------------------------------------
        public event Action<AccessibilitySettings> OnSettingsChanged;
        public event Action<float> OnTextScaleChanged;
        public event Action<bool> OnHighContrastChanged;
        public event Action<ColorBlindMode> OnColorBlindModeChanged;
        public event Action<bool> OnSubtitleChanged;
        public event Action<bool> OnReducedMotionChanged;

        // ---------------------------------------------------------------
        // State
        // ---------------------------------------------------------------
        private bool _initialized;
        private readonly List<IAccessibleElement> _elements = new List<IAccessibleElement>();
        private WorldEnvironment _worldEnvironment;
        private CanvasLayer _subtitleLayer;
        private Label _subtitleLabel;

        // ---------------------------------------------------------------
        // Initialization
        // ---------------------------------------------------------------
        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            Logger.Info("AccessibilityManager: Initializing...");

            // Create subtitle layer
            var window = (Engine.GetMainLoop() as SceneTree)?.Root ?? new Window();
            _subtitleLayer = new CanvasLayer
            {
                Layer = 70, // Above notifications
                Name = "SubtitleLayer"
            };
            window.AddChild(_subtitleLayer);

            _subtitleLabel = new Label
            {
                Text = "",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Position = new Vector2I(200, 800),
                Size = new Vector2I(1520, 200),
                AutowrapMode = TextServer.AutowrapMode.Word,
                Visible = false
            };
            _subtitleLayer.AddChild(_subtitleLabel);

            // Load saved settings
            LoadSettings();

            Logger.Info("AccessibilityManager: Initialized successfully.");
        }

        public void Shutdown()
        {
            SaveSettings();

            if (_subtitleLayer != null && GodotObject.IsInstanceValid(_subtitleLayer))
            {
                _subtitleLayer.QueueFree();
            }

            _initialized = false;
        }

        // ---------------------------------------------------------------
        // Settings Persistence
        // ---------------------------------------------------------------
        public void SaveSettings()
        {
            try
            {
                var payload = new Dictionary<string, object>
                {
                    ["text_scale"] = TextScale,
                    ["high_contrast"] = HighContrast,
                    ["color_blind_mode"] = (int)ColorBlindMode,
                    ["subtitle"] = SubtitleEnabled,
                    ["subtitle_size"] = SubtitleSize,
                    ["ui_scale"] = UIScale,
                    ["reduced_motion"] = ReducedMotion,
                    ["screen_reader"] = ScreenReaderEnabled,
                    ["haptic"] = HapticFeedback,
                    ["voice_nav"] = VoiceNavigation
                };
                string json = System.Text.Json.JsonSerializer.Serialize(payload);
                string path = System.IO.Path.Combine(Godot.OS.GetUserDataDir(), "accessibility_settings.json");
                System.IO.File.WriteAllText(path, json);
                Logger.Info("AccessibilityManager: Settings saved.");
            }
            catch (Exception ex)
            {
                Logger.Error($"AccessibilityManager: Failed to save settings: {ex.Message}");
            }
        }

        private void LoadSettings()
        {
            try
            {
                string path = System.IO.Path.Combine(Godot.OS.GetUserDataDir(), "accessibility_settings.json");
                if (!System.IO.File.Exists(path)) return;
                string json = System.IO.File.ReadAllText(path);
                var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, System.Text.Json.JsonElement>>(json);
                if (dict == null) return;

                if (dict.TryGetValue("text_scale", out var ts)) TextScale = ts.GetSingle();
                if (dict.TryGetValue("high_contrast", out var hc)) HighContrast = hc.GetBoolean();
                if (dict.TryGetValue("color_blind_mode", out var cb)) ColorBlindMode = (ColorBlindMode)cb.GetInt32();
                if (dict.TryGetValue("subtitle", out var sub)) SubtitleEnabled = sub.GetBoolean();
                if (dict.TryGetValue("subtitle_size", out var ss)) SubtitleSize = ss.GetSingle();
                if (dict.TryGetValue("ui_scale", out var us)) UIScale = us.GetSingle();
                if (dict.TryGetValue("reduced_motion", out var rm)) ReducedMotion = rm.GetBoolean();
                if (dict.TryGetValue("screen_reader", out var sr)) ScreenReaderEnabled = sr.GetBoolean();
                if (dict.TryGetValue("haptic", out var hap)) HapticFeedback = hap.GetBoolean();
                if (dict.TryGetValue("voice_nav", out var vn)) VoiceNavigation = vn.GetBoolean();

                Logger.Info("AccessibilityManager: Settings loaded.");
            }
            catch (Exception ex)
            {
                Logger.Warning($"AccessibilityManager: Failed to load settings: {ex.Message}");
            }
        }

        public void ApplySettings()
        {
            OnSettingsChanged?.Invoke(GetSettings());
            Logger.Info("AccessibilityManager: Settings applied.");
        }

        public AccessibilitySettings GetSettings()
        {
            return new AccessibilitySettings
            {
                TextScale = TextScale,
                HighContrast = HighContrast,
                ColorBlindMode = ColorBlindMode,
                SubtitleEnabled = SubtitleEnabled,
                SubtitleSize = SubtitleSize,
                UIScale = UIScale,
                ReducedMotion = ReducedMotion,
                ScreenReaderEnabled = ScreenReaderEnabled,
                HapticFeedback = HapticFeedback,
                VoiceNavigation = VoiceNavigation
            };
        }

        // ---------------------------------------------------------------
        // Text Scale
        // ---------------------------------------------------------------
        public void SetTextScale(float scale)
        {
            TextScale = Mathf.Clamp(scale, 0.5f, 2.0f);
            OnTextScaleChanged?.Invoke(TextScale);
            UpdateElements();
            Logger.Info($"AccessibilityManager: Text scale set to {TextScale:F2}");
        }

        // ---------------------------------------------------------------
        // High Contrast
        // ---------------------------------------------------------------
        public void SetHighContrast(bool enabled)
        {
            HighContrast = enabled;
            OnHighContrastChanged?.Invoke(enabled);
            UpdateElements();
            Logger.Info($"AccessibilityManager: High contrast {(enabled ? "enabled" : "disabled")}");
        }

        // ---------------------------------------------------------------
        // Color Blind Mode
        // ---------------------------------------------------------------
        public void SetColorBlindMode(ColorBlindMode mode)
        {
            ColorBlindMode = mode;
            ApplyColorBlindFilter();
            OnColorBlindModeChanged?.Invoke(mode);
            Logger.Info($"AccessibilityManager: Color blind mode set to {mode}");
        }

        private void ApplyColorBlindFilter()
        {
            if (_worldEnvironment == null)
            {
                // Find or create WorldEnvironment
                _worldEnvironment = GetTree()?.GetNodeOrNull<WorldEnvironment>("WorldEnvironment");
                if (_worldEnvironment == null)
                {
                    _worldEnvironment = new WorldEnvironment();
                    _worldEnvironment.Name = "AccessibilityWorldEnvironment";
                }
            }

            // Note: Full color blind filter implementation requires a custom shader
            // This provides the framework hook for future implementation
            switch (ColorBlindMode)
            {
                case ColorBlindMode.Protanopia:
                    // Apply protanopia filter
                    break;
                case ColorBlindMode.Deuteranopia:
                    // Apply deuteranopia filter
                    break;
                case ColorBlindMode.Tritanopia:
                    // Apply tritanopia filter
                    break;
                default:
                    // Remove filter
                    break;
            }
        }

        // ---------------------------------------------------------------
        // Subtitle System
        // ---------------------------------------------------------------
        public void ShowSubtitle(string text, float duration = 3.0f)
        {
            if (!SubtitleEnabled || _subtitleLabel == null) return;

            _subtitleLabel.Text = text;
            _subtitleLabel.Visible = true;
            _subtitleLabel.Scale = new Vector2(SubtitleSize, SubtitleSize);
            _subtitleLabel.Modulate = HighContrast ? new Color(1, 1, 0) : Colors.White;

            // Auto-hide after duration
            var tween = _subtitleLabel.CreateTween();
            tween.SetParallel(false);
            tween.TweenInterval(duration);
            tween.TweenProperty(_subtitleLabel, "modulate", new Color(1, 1, 1, 0), 0.5f);
            tween.Finished += () =>
            {
                if (_subtitleLabel != null)
                    _subtitleLabel.Visible = false;
            };
        }

        public void HideSubtitle()
        {
            if (_subtitleLabel != null)
            {
                _subtitleLabel.Visible = false;
                _subtitleLabel.Text = "";
            }
        }

        public void SetSubtitleSize(float size)
        {
            SubtitleSize = Mathf.Clamp(size, 0.5f, 2.0f);
            if (_subtitleLabel != null)
                _subtitleLabel.Scale = new Vector2(SubtitleSize, SubtitleSize);
        }

        // ---------------------------------------------------------------
        // Reduced Motion
        // ---------------------------------------------------------------
        public void SetReducedMotion(bool enabled)
        {
            ReducedMotion = enabled;
            OnReducedMotionChanged?.Invoke(enabled);
            Logger.Info($"AccessibilityManager: Reduced motion {(enabled ? "enabled" : "disabled")}");
        }

        public float GetTransitionDuration(float normalDuration)
        {
            return ReducedMotion ? normalDuration * 0.3f : normalDuration;
        }

        // ---------------------------------------------------------------
        // Screen Reader
        // ---------------------------------------------------------------
        public void AnnounceScreenReader(string text)
        {
            if (!ScreenReaderEnabled) return;

            Logger.Info($"ScreenReader: {text}");
            // Future: Integrate with platform screen reader API
            // Android: AccessibilityEvent using Java interop
        }

        public void SetScreenReaderLabel(Control control, string label)
        {
            if (control == null) return;

            control.TooltipText = label;
        }

        // ---------------------------------------------------------------
        // Haptic Feedback
        // ---------------------------------------------------------------
        public void TriggerHaptic(HapticType type)
        {
            if (!HapticFeedback) return;

            float duration = type switch
            {
                HapticType.Light => 20f,
                HapticType.Medium => 50f,
                HapticType.Heavy => 100f,
                _ => 50f
            };

            if (OS.HasFeature("android") || OS.HasFeature("ios"))
            {
                // Input.vibrate_handheld(duration); // Godot 4 API
            }
        }

        // ---------------------------------------------------------------
        // Element Management
        // ---------------------------------------------------------------
        public void RegisterAccessibleElement(IAccessibleElement element)
        {
            if (!_elements.Contains(element))
            {
                _elements.Add(element);
                element.OnAccessibilityChanged(GetSettings());
            }
        }

        public void UnregisterAccessibleElement(IAccessibleElement element)
        {
            _elements.Remove(element);
        }

        private void UpdateElements()
        {
            var settings = GetSettings();
            foreach (var element in _elements)
                element.OnAccessibilityChanged(settings);
        }

        // ---------------------------------------------------------------
        // Voice Navigation (Future)
        // ---------------------------------------------------------------
        public void EnableVoiceNavigation(bool enable)
        {
            VoiceNavigation = enable;
            Logger.Info($"AccessibilityManager: Voice navigation {(enable ? "enabled" : "disabled")}");
        }

        public void ProcessVoiceCommand(string command)
        {
            if (!VoiceNavigation) return;
            // Future: Integrate with speech recognition
            Logger.Info($"AccessibilityManager: Voice command received: {command}");
        }

        // ---------------------------------------------------------------
        // Utility
        // ---------------------------------------------------------------
        private Window? GetTree()
        {
            return (Engine.GetMainLoop() as SceneTree)?.Root;
        }
    }

    // ---------------------------------------------------------------
    // Color Blind Mode Enum
    // ---------------------------------------------------------------
    public enum ColorBlindMode
    {
        None = 0,
        Protanopia = 1,
        Deuteranopia = 2,
        Tritanopia = 3
    }

    // ---------------------------------------------------------------
    // Haptic Type Enum
    // ---------------------------------------------------------------
    public enum HapticType
    {
        Light,
        Medium,
        Heavy
    }

    // ---------------------------------------------------------------
    // Accessibility Settings Data Model
    // ---------------------------------------------------------------
    public struct AccessibilitySettings
    {
        public float TextScale;
        public bool HighContrast;
        public ColorBlindMode ColorBlindMode;
        public bool SubtitleEnabled;
        public float SubtitleSize;
        public float UIScale;
        public bool ReducedMotion;
        public bool ScreenReaderEnabled;
        public bool HapticFeedback;
        public bool VoiceNavigation;
    }

    // ---------------------------------------------------------------
    // Accessible Element Interface
    // ---------------------------------------------------------------
    public interface IAccessibleElement
    {
        void OnAccessibilityChanged(AccessibilitySettings settings);
    }
}