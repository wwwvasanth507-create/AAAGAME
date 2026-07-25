using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.UI.Layout
{
    /// <summary>
    /// Responsive mobile layout system supporting phones, small tablets, large tablets,
    /// different aspect ratios, landscape mode, safe areas, dynamic DPI scaling,
    /// and foldable device hooks. Applies adaptive scaling to all UI elements.
    /// </summary>
    public partial class ResponsiveLayout : Node
    {
        // ---------------------------------------------------------------
        // Device Categories
        // ---------------------------------------------------------------
        public enum DeviceCategory
        {
            Phone,
            SmallTablet,
            LargeTablet,
            Desktop
        }

        // ---------------------------------------------------------------
        // Constants
        // ---------------------------------------------------------------
        private const float PhoneMaxWidth = 480f;
        private const float SmallTabletMaxWidth = 768f;
        private const float LargeTabletMaxWidth = 1024f;
        private const float ReferenceWidth = 1920f;
        private const float ReferenceHeight = 1080f;
        private const float MinUIScale = 0.5f;
        private const float MaxUIScale = 2.0f;

        // ---------------------------------------------------------------
        // Events
        // ---------------------------------------------------------------
        public event Action<DeviceCategory> OnDeviceCategoryChanged;
        public event Action<Vector2> OnScreenSizeChanged;
        public event Action<float> OnScaleChanged;
        public event Action<SafeAreaMargins> OnSafeAreaChanged;
        public event Action<bool> OnOrientationChanged;

        // ---------------------------------------------------------------
        // State
        // ---------------------------------------------------------------
        private DeviceCategory _currentCategory;
        private float _currentScale = 1.0f;
        private Vector2 _screenSize;
        private bool _isLandscape;
        private SafeAreaMargins _safeArea;
        private bool _isFoldable;
        private bool _isFolded = true;
        private float _foldHingeOffset;

        private readonly List<IResponsiveElement> _elements = new List<IResponsiveElement>();
        private readonly Dictionary<DeviceCategory, LayoutPreset> _presets = new Dictionary<DeviceCategory, LayoutPreset>();

        // ---------------------------------------------------------------
        // Properties
        // ---------------------------------------------------------------
        public DeviceCategory CurrentCategory => _currentCategory;
        public float CurrentScale => _currentScale;
        public Vector2 ScreenSize => _screenSize;
        public bool IsLandscape => _isLandscape;
        public SafeAreaMargins SafeArea => _safeArea;
        public bool IsFoldable => _isFoldable;
        public bool IsFolded => _isFolded;

        // ---------------------------------------------------------------
        // Lifecycle
        // ---------------------------------------------------------------
        public override void _Ready()
        {
            Name = "ResponsiveLayout";
            InitializePresets();
            DetectDevice();
            UpdateLayout();

            GetViewport().SizeChanged += OnViewportSizeChanged;
            GD.Print("ResponsiveLayout: Initialized.");
        }

        public override void _ExitTree()
        {
            GetViewport().SizeChanged -= OnViewportSizeChanged;
        }

        // ---------------------------------------------------------------
        // Initialization
        // ---------------------------------------------------------------
        private void InitializePresets()
        {
            _presets[DeviceCategory.Phone] = new LayoutPreset
            {
                UIScale = 1.4f,
                GridColumns = 2,
                ListItemHeight = 55,
                ButtonHeight = 50,
                Padding = 10,
                FontScale = 1.1f,
                ShowSidebar = false,
                UseBottomNav = true
            };

            _presets[DeviceCategory.SmallTablet] = new LayoutPreset
            {
                UIScale = 1.2f,
                GridColumns = 3,
                ListItemHeight = 50,
                ButtonHeight = 45,
                Padding = 15,
                FontScale = 1.0f,
                ShowSidebar = false,
                UseBottomNav = true
            };

            _presets[DeviceCategory.LargeTablet] = new LayoutPreset
            {
                UIScale = 1.0f,
                GridColumns = 4,
                ListItemHeight = 45,
                ButtonHeight = 40,
                Padding = 20,
                FontScale = 1.0f,
                ShowSidebar = true,
                UseBottomNav = false
            };

            _presets[DeviceCategory.Desktop] = new LayoutPreset
            {
                UIScale = 1.0f,
                GridColumns = 6,
                ListItemHeight = 40,
                ButtonHeight = 35,
                Padding = 25,
                FontScale = 1.0f,
                ShowSidebar = true,
                UseBottomNav = false
            };
        }

        private void DetectDevice()
        {
            _screenSize = GetViewport().GetVisibleRect().Size;
            _isLandscape = _screenSize.X > _screenSize.Y;

            // Detect foldable
            _isFoldable = OS.HasFeature("android") && 
                          (ProjectSettings.GetSetting("display/window/handheld/foldable_detection", false).AsBool());

            // Calculate DPI-based scale
            float dpi = DisplayServer.ScreenGetDpi();
            float baseDPI = 160f;
            float dpiScale = Mathf.Clamp(dpi / baseDPI, 0.8f, 1.5f);

            // Determine category by width
            float width = _isLandscape ? _screenSize.Y : _screenSize.X;
            float dpWidth = width / dpiScale;

            if (dpWidth <= PhoneMaxWidth)
                _currentCategory = DeviceCategory.Phone;
            else if (dpWidth <= SmallTabletMaxWidth)
                _currentCategory = DeviceCategory.SmallTablet;
            else if (dpWidth <= LargeTabletMaxWidth)
                _currentCategory = DeviceCategory.LargeTablet;
            else
                _currentCategory = DeviceCategory.Desktop;

            // Calculate safe areas
            CalculateSafeArea();

            Logger.Info($"ResponsiveLayout: Detected {_currentCategory} (DP: {dpWidth:F0}, DPI: {dpi:F0}, Landscape: {_isLandscape})");
        }

        private void CalculateSafeArea()
        {
            _safeArea = new SafeAreaMargins();

            if (OS.HasFeature("mobile") || OS.HasFeature("android"))
            {
                // Approximate safe areas for common devices
                float dpi = DisplayServer.ScreenGetDpi();
                float statusBarHeight = 24f * (dpi / 160f);
                float navBarHeight = 48f * (dpi / 160f);

                _safeArea.Top = _isLandscape ? 0 : statusBarHeight;
                _safeArea.Bottom = _isLandscape ? 0 : navBarHeight;
                _safeArea.Left = _isLandscape ? statusBarHeight : 0;
                _safeArea.Right = _isLandscape ? navBarHeight : 0;
            }
            else
            {
                _safeArea.Top = 0;
                _safeArea.Bottom = 0;
                _safeArea.Left = 0;
                _safeArea.Right = 0;
            }
        }

        // ---------------------------------------------------------------
        // Layout Update
        // ---------------------------------------------------------------
        private void UpdateLayout()
        {
            if (!_presets.TryGetValue(_currentCategory, out var preset))
                return;

            float dpi = DisplayServer.ScreenGetDpi();
            float baseDPI = 160f;
            float dpiScale = Mathf.Clamp(dpi / baseDPI, 0.8f, 1.5f);

            // Calculate final UI scale
            _currentScale = Mathf.Clamp(preset.UIScale * dpiScale, MinUIScale, MaxUIScale);

            // Update all registered elements
            var responsiveInfo = new ResponsiveInfo
            {
                Category = _currentCategory,
                Scale = _currentScale,
                FontScale = preset.FontScale,
                GridColumns = preset.GridColumns,
                Padding = preset.Padding * _currentScale,
                ButtonHeight = preset.ButtonHeight * _currentScale,
                ListItemHeight = preset.ListItemHeight * _currentScale,
                ShowSidebar = preset.ShowSidebar,
                UseBottomNav = preset.UseBottomNav,
                IsLandscape = _isLandscape,
                ScreenSize = _screenSize,
                SafeArea = _safeArea
            };

            foreach (var element in _elements)
            {
                element.OnLayoutChanged(responsiveInfo);
            }

            OnScaleChanged?.Invoke(_currentScale);
        }

        private void OnViewportSizeChanged()
        {
            Vector2 newSize = GetViewport().GetVisibleRect().Size;
            bool wasLandscape = _isLandscape;

            _screenSize = newSize;
            _isLandscape = newSize.X > newSize.Y;

            DetectDevice();
            UpdateLayout();

            OnScreenSizeChanged?.Invoke(newSize);

            if (_isLandscape != wasLandscape)
                OnOrientationChanged?.Invoke(_isLandscape);
        }

        // ---------------------------------------------------------------
        // Element Registration
        // ---------------------------------------------------------------
        public void RegisterElement(IResponsiveElement element)
        {
            if (!_elements.Contains(element))
            {
                _elements.Add(element);
                // Immediately apply current layout
                UpdateLayout();
            }
        }

        public void UnregisterElement(IResponsiveElement element)
        {
            _elements.Remove(element);
        }

        // ---------------------------------------------------------------
        // Safe Area Adjustment
        // ---------------------------------------------------------------
        public Vector2 ApplySafeArea(Vector2 position, Vector2 size)
        {
            return new Vector2(
                Mathf.Max(position.X, _safeArea.Left),
                Mathf.Max(position.Y, _safeArea.Top)
            );
        }

        // ---------------------------------------------------------------
        // Foldable Support
        // ---------------------------------------------------------------
        public void SetFoldState(bool folded)
        {
            if (!_isFoldable || _isFolded == folded) return;

            _isFolded = folded;
            OnViewportSizeChanged();
            Logger.Info($"ResponsiveLayout: Fold state changed to {(folded ? "folded" : "unfolded")}");
        }

        // ---------------------------------------------------------------
        // Public API
        // ---------------------------------------------------------------
        public LayoutPreset GetPreset(DeviceCategory category)
        {
            return _presets.TryGetValue(category, out var preset) ? preset : _presets[DeviceCategory.Desktop];
        }

        public float GetDP()
        {
            float dpi = DisplayServer.ScreenGetDpi();
            return dpi > 0 ? _screenSize.X / (dpi / 160f) : _screenSize.X;
        }

        public bool IsMobile()
        {
            return _currentCategory == DeviceCategory.Phone || 
                   _currentCategory == DeviceCategory.SmallTablet || 
                   _currentCategory == DeviceCategory.LargeTablet;
        }
    }

    // ---------------------------------------------------------------
    // Safe Area Margins
    // ---------------------------------------------------------------
    public struct SafeAreaMargins
    {
        public float Top { get; set; }
        public float Bottom { get; set; }
        public float Left { get; set; }
        public float Right { get; set; }
    }

    // ---------------------------------------------------------------
    // Layout Preset
    // ---------------------------------------------------------------
    public class LayoutPreset
    {
        public float UIScale { get; set; } = 1.0f;
        public int GridColumns { get; set; } = 4;
        public float ListItemHeight { get; set; } = 45;
        public float ButtonHeight { get; set; } = 40;
        public float Padding { get; set; } = 20;
        public float FontScale { get; set; } = 1.0f;
        public bool ShowSidebar { get; set; } = true;
        public bool UseBottomNav { get; set; } = false;
    }

    // ---------------------------------------------------------------
    // Responsive Info passed to elements
    // ---------------------------------------------------------------
    public struct ResponsiveInfo
    {
        public ResponsiveLayout.DeviceCategory Category;
        public float Scale;
        public float FontScale;
        public int GridColumns;
        public float Padding;
        public float ButtonHeight;
        public float ListItemHeight;
        public bool ShowSidebar;
        public bool UseBottomNav;
        public bool IsLandscape;
        public Vector2 ScreenSize;
        public SafeAreaMargins SafeArea;
    }

    // ---------------------------------------------------------------
    // Responsive Element Interface
    // ---------------------------------------------------------------
    public interface IResponsiveElement
    {
        void OnLayoutChanged(ResponsiveInfo info);
    }
}