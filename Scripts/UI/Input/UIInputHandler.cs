using System;
using System.Collections.Generic;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.UI.Input
{
    /// <summary>
    /// UI input handler supporting touch, mouse, keyboard, gamepad (future),
    /// gesture hooks, long press, double tap, drag & drop, pinch hook,
    /// and input rebinding framework.
    /// </summary>
    public partial class UIInputHandler : Node
    {
        // ---------------------------------------------------------------
        // Constants
        // ---------------------------------------------------------------
        private const float LongPressDuration = 0.5f;
        private const float DoubleTapMaxInterval = 0.3f;
        private const float DragThreshold = 10f;

        // ---------------------------------------------------------------
        // Input Mode
        // ---------------------------------------------------------------
        public enum InputMode
        {
            Touch,
            Mouse,
            Keyboard,
            Gamepad
        }

        // ---------------------------------------------------------------
        // Events
        // ---------------------------------------------------------------
        public event Action<Vector2> OnTap;
        public event Action<Vector2> OnLongPress;
        public event Action<Vector2> OnDoubleTap;
        public event Action<Vector2> OnDragStart;
        public event Action<Vector2, Vector2> OnDragUpdate;
        public event Action<Vector2> OnDragEnd;
        public event Action<float> OnPinch;
        public event Action<Vector2> OnSwipe;
        public event Action OnBackButton;
        public event Action OnPauseButton;
        public event Action<string> OnRebindRequested;

        // ---------------------------------------------------------------
        // State
        // ---------------------------------------------------------------
        private InputMode _currentMode = InputMode.Touch;
        private bool _isDragging;
        private Vector2 _dragStartPos;
        private Vector2 _lastTouchPos;
        private float _touchStartTime;
        private float _lastTapTime;
        private Vector2 _lastTapPos;
        private float _pinchStartDistance;
        private bool _isPinching;
        private int _activeTouchId = -1;
        private int _secondaryTouchId = -1;

        private readonly Dictionary<string, InputAction> _actions = new Dictionary<string, InputAction>();
        private readonly List<IGestureHandler> _gestureHandlers = new List<IGestureHandler>();

        // ---------------------------------------------------------------
        // Properties
        // ---------------------------------------------------------------
        public InputMode CurrentMode => _currentMode;
        public bool IsDragging => _isDragging;
        public Vector2 LastTouchPosition => _lastTouchPos;

        // ---------------------------------------------------------------
        // Lifecycle
        // ---------------------------------------------------------------
        public override void _Ready()
        {
            Name = "UIInputHandler";
            RegisterDefaultActions();
            GD.Print("UIInputHandler: Initialized.");
        }

        public override void _Input(Godot.InputEvent @event)
        {
            DetectInputMode(@event);
            ProcessTouchInput(@event);
            ProcessMouseInput(@event);
            ProcessKeyboardInput(@event);
        }

        // ---------------------------------------------------------------
        // Input Mode Detection
        // ---------------------------------------------------------------
        private void DetectInputMode(Godot.InputEvent @event)
        {
            if (@event is Godot.InputEventScreenTouch || @event is Godot.InputEventScreenDrag)
                _currentMode = InputMode.Touch;
            else if (@event is Godot.InputEventMouseButton || @event is Godot.InputEventMouseMotion)
            {
                if (_currentMode != InputMode.Keyboard && _currentMode != InputMode.Gamepad)
                    _currentMode = InputMode.Mouse;
            }
            else if (@event is Godot.InputEventKey)
                _currentMode = InputMode.Keyboard;
            else if (@event is Godot.InputEventJoypadButton || @event is Godot.InputEventJoypadMotion)
                _currentMode = InputMode.Gamepad;
        }

        // ---------------------------------------------------------------
        // Touch Input Processing
        // ---------------------------------------------------------------
        private void ProcessTouchInput(Godot.InputEvent @event)
        {
            if (@event is Godot.InputEventScreenTouch touch)
            {
                if (touch.Pressed)
                {
                    // First touch
                    if (_activeTouchId == -1)
                    {
                        _activeTouchId = touch.Index;
                        _touchStartTime = Time.GetTicksMsec() / 1000f;
                        _lastTouchPos = touch.Position;
                        _dragStartPos = touch.Position;
                        _isDragging = false;

                        // Check for double tap
                        float now = Time.GetTicksMsec() / 1000f;
                        if (now - _lastTapTime < DoubleTapMaxInterval &&
                            _lastTapPos.DistanceTo(touch.Position) < 50f)
                        {
                            OnDoubleTap?.Invoke(touch.Position);
                            _lastTapTime = 0;
                        }
                        else
                        {
                            _lastTapTime = now;
                            _lastTapPos = touch.Position;
                        }
                    }
                    // Second touch (pinch start)
                    else if (_secondaryTouchId == -1)
                    {
                        _secondaryTouchId = touch.Index;
                        _isPinching = true;
                        _pinchStartDistance = GetTouchDistance();
                    }
                }
                else
                {
                    if (touch.Index == _activeTouchId)
                    {
                        float touchDuration = (Time.GetTicksMsec() / 1000f) - _touchStartTime;

                        if (!_isDragging)
                        {
                            if (touchDuration >= LongPressDuration)
                                OnLongPress?.Invoke(touch.Position);
                            else
                                OnTap?.Invoke(touch.Position);
                        }
                        else
                        {
                            OnDragEnd?.Invoke(touch.Position);
                        }

                        _activeTouchId = -1;
                        _isDragging = false;
                        _isPinching = false;
                    }
                    else if (touch.Index == _secondaryTouchId)
                    {
                        _secondaryTouchId = -1;
                        _isPinching = false;
                    }
                }
            }
            else if (@event is Godot.InputEventScreenDrag drag)
            {
                if (drag.Index == _activeTouchId)
                {
                    float dragDistance = drag.Position.DistanceTo(_dragStartPos);

                    if (!_isDragging && dragDistance > DragThreshold)
                    {
                        _isDragging = true;
                        OnDragStart?.Invoke(_dragStartPos);
                    }

                    if (_isDragging)
                    {
                        OnDragUpdate?.Invoke(_dragStartPos, drag.Position);
                    }

                    _lastTouchPos = drag.Position;
                }

                // Pinch update
                if (_isPinching && _activeTouchId != -1 && _secondaryTouchId != -1)
                {
                    float currentDistance = GetTouchDistance();
                    float scale = _pinchStartDistance > 0 ? currentDistance / _pinchStartDistance : 1;
                    OnPinch?.Invoke(scale);
                }
            }
        }

        // ---------------------------------------------------------------
        // Mouse Input Processing
        // ---------------------------------------------------------------
        private void ProcessMouseInput(Godot.InputEvent @event)
        {
            if (@event is Godot.InputEventMouseButton mouseBtn)
            {
                if (mouseBtn.ButtonIndex == MouseButton.Left)
                {
                    if (mouseBtn.Pressed)
                    {
                        _touchStartTime = Time.GetTicksMsec() / 1000f;
                        _lastTouchPos = mouseBtn.Position;
                        _dragStartPos = mouseBtn.Position;
                        _isDragging = false;
                    }
                    else
                    {
                        float touchDuration = (Time.GetTicksMsec() / 1000f) - _touchStartTime;

                        if (!_isDragging)
                        {
                            if (touchDuration >= LongPressDuration)
                                OnLongPress?.Invoke(mouseBtn.Position);
                            else
                                OnTap?.Invoke(mouseBtn.Position);
                        }
                        else
                        {
                            OnDragEnd?.Invoke(mouseBtn.Position);
                        }

                        _isDragging = false;
                    }
                }
            }
            else if (@event is Godot.InputEventMouseMotion mouseMotion)
            {
                if (mouseMotion.ButtonMask.HasFlag(MouseButtonMask.Left))
                {
                    float dragDistance = mouseMotion.Position.DistanceTo(_dragStartPos);

                    if (!_isDragging && dragDistance > DragThreshold)
                    {
                        _isDragging = true;
                        OnDragStart?.Invoke(_dragStartPos);
                    }

                    if (_isDragging)
                    {
                        OnDragUpdate?.Invoke(_dragStartPos, mouseMotion.Position);
                    }

                    _lastTouchPos = mouseMotion.Position;
                }
            }
        }

        // ---------------------------------------------------------------
        // Keyboard Input Processing
        // ---------------------------------------------------------------
        private void ProcessKeyboardInput(Godot.InputEvent @event)
        {
            if (@event is Godot.InputEventKey key)
            {
                if (key.Pressed && !key.Echo)
                {
                    switch (key.Keycode)
                    {
                        case Key.Escape:
                            OnBackButton?.Invoke();
                            break;
                        case Key.P:
                            OnPauseButton?.Invoke();
                            break;
                    }

                    // Check custom actions
                    foreach (var kvp in _actions)
                    {
                        if (kvp.Value.Key == key.Keycode)
                        {
                            kvp.Value.OnTriggered?.Invoke();
                        }
                    }
                }
            }
        }

        // ---------------------------------------------------------------
        // Gesture Handlers
        // ---------------------------------------------------------------
        public void RegisterGestureHandler(IGestureHandler handler)
        {
            if (!_gestureHandlers.Contains(handler))
            {
                _gestureHandlers.Add(handler);
                handler.OnRegistered(this);
            }
        }

        public void UnregisterGestureHandler(IGestureHandler handler)
        {
            if (_gestureHandlers.Remove(handler))
                handler.OnUnregistered();
        }

        // ---------------------------------------------------------------
        // Input Action System (Rebinding Framework)
        // ---------------------------------------------------------------
        public void RegisterAction(string actionName, Key defaultKey, Action onTriggered)
        {
            _actions[actionName] = new InputAction
            {
                Name = actionName,
                Key = defaultKey,
                OnTriggered = onTriggered
            };
        }

        public void RebindAction(string actionName, Key newKey)
        {
            if (_actions.TryGetValue(actionName, out var action))
            {
                action.Key = newKey;
                Logger.Info($"UIInputHandler: Rebound '{actionName}' to {newKey}.");
                OnRebindRequested?.Invoke(actionName);
            }
        }

        public Key GetActionKey(string actionName)
        {
            return _actions.TryGetValue(actionName, out var action) ? action.Key : Key.None;
        }

        public IReadOnlyDictionary<string, InputAction> GetActions()
        {
            return _actions;
        }

        private void RegisterDefaultActions()
        {
            RegisterAction("ui_accept", Key.Enter, () => { });
            RegisterAction("ui_cancel", Key.Escape, () => OnBackButton?.Invoke());
            RegisterAction("ui_pause", Key.P, () => OnPauseButton?.Invoke());
            RegisterAction("ui_inventory", Key.I, () => { });
            RegisterAction("ui_character", Key.C, () => { });
            RegisterAction("ui_journal", Key.J, () => { });
            RegisterAction("ui_map", Key.M, () => { });
            RegisterAction("ui_abilities", Key.K, () => { });
        }

        // ---------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------
        private float GetTouchDistance()
        {
            // Simplified: in a real implementation, track both touch positions
            return 100f;
        }

        // ---------------------------------------------------------------
        // Public API
        // ---------------------------------------------------------------
        public bool IsTouchDevice()
        {
            return OS.HasFeature("mobile") || OS.HasFeature("android") || OS.HasFeature("ios");
        }

        public void Vibrate(float durationMs = 50)
        {
            if (OS.HasFeature("android") || OS.HasFeature("ios"))
            {
                // Input.vibrate_handheld(durationMs); // Godot 4 API
            }
        }
    }

    // ---------------------------------------------------------------
    // Input Action Data Model
    // ---------------------------------------------------------------
    public class InputAction
    {
        public string Name { get; set; }
        public Key Key { get; set; }
        public Action OnTriggered { get; set; }
    }

    // ---------------------------------------------------------------
    // Gesture Handler Interface
    // ---------------------------------------------------------------
    public interface IGestureHandler
    {
        void OnRegistered(UIInputHandler handler);
        void OnUnregistered();
        bool HandleTap(Vector2 position);
        bool HandleLongPress(Vector2 position);
        bool HandleDoubleTap(Vector2 position);
        bool HandleDragStart(Vector2 position);
        bool HandleDragUpdate(Vector2 start, Vector2 current);
        bool HandleDragEnd(Vector2 position);
        bool HandlePinch(float scale);
        bool HandleSwipe(Vector2 direction);
    }
}