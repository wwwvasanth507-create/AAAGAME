using Godot;

namespace HeroOfEternia.Input
{
    /// <summary>
    /// TouchControls manages the virtual joystick, action buttons, and gesture
    /// recognition for mobile play. Writes into InputHandler on each frame.
    ///
    /// Layout:
    ///   Left half  — Dynamic joystick (spawns at touch-down point)
    ///   Right half — Action buttons (Jump, Attack, Skill1, Skill2, Interact, Roll)
    ///
    /// Supports left-handed mode (mirror layout), tablet scaling, and opacity tuning.
    /// </summary>
    public partial class TouchControls : CanvasLayer
    {
        // ---------------------------------------------------------------
        // EXPORTED SETTINGS
        // ---------------------------------------------------------------
        [Export] public float JoystickRadius   = 80f;
        [Export] public float ButtonSize       = 90f;
        [Export] public float JoystickOpacity  = 0.55f;
        [Export] public bool  LeftHandedMode   = false;

        // ---------------------------------------------------------------
        // NODE REFERENCES (assigned in _Ready via code-created nodes)
        // ---------------------------------------------------------------
        private TextureRect _joystickBase    = null!;
        private TextureRect _joystickKnob    = null!;
        private Button      _btnJump         = null!;
        private Button      _btnAttack       = null!;
        private Button      _btnSkill1       = null!;
        private Button      _btnSkill2       = null!;
        private Button      _btnInteract     = null!;
        private Button      _btnRoll         = null!;

        private InputHandler _inputHandler   = null!;

        // ---------------------------------------------------------------
        // JOYSTICK STATE
        // ---------------------------------------------------------------
        private bool   _joystickActive    = false;
        private int    _joystickFingerId  = -1;
        private Vector2 _joystickOrigin   = Vector2.Zero;
        private Vector2 _joystickDelta    = Vector2.Zero;

        // ---------------------------------------------------------------
        // GESTURE STATE
        // ---------------------------------------------------------------
        private float  _lastTapTime       = 0f;
        private const float DoubleTapMaxGap = 0.3f;   // seconds
        private Vector2 _swipeStart       = Vector2.Zero;
        private const float SwipeMinDist  = 80f;
        private bool   _longPressActive   = false;
        private float  _pressTimer        = 0f;
        private const float LongPressTime = 0.6f;

        // ---------------------------------------------------------------
        // LIFECYCLE
        // ---------------------------------------------------------------

        public override void _Ready()
        {
            // Fetch InputHandler from parent scene
            _inputHandler = GetTree().Root.FindChild("InputHandler", true, false) as InputHandler
                           ?? GetParent().GetNodeOrNull<InputHandler>("InputHandler")!;

            Layer = 10; // Draw above gameplay
            BuildUI();
            AdaptToScreen();
            Core.Logger.Info($"TouchControls: Initialised. LeftHanded={LeftHandedMode}, Radius={JoystickRadius}");
        }

        public override void _Process(double delta)
        {
            // Long-press detection on joystick touch
            if (_joystickActive)
            {
                _pressTimer += (float)delta;
                if (_pressTimer >= LongPressTime && !_longPressActive)
                {
                    _longPressActive = true;
                    OnLongPress(_joystickOrigin);
                }
            }

            // Drive InputHandler move axis from joystick
            if (_inputHandler != null)
            {
                _inputHandler.TouchMoveAxis = _joystickDelta;
            }
        }

        public override void _Input(InputEvent ev)
        {
            if (ev is InputEventScreenTouch touch)
            {
                HandleTouch(touch);
            }
            else if (ev is InputEventScreenDrag drag)
            {
                HandleDrag(drag);
            }
        }

        // ---------------------------------------------------------------
        // TOUCH HANDLING
        // ---------------------------------------------------------------

        private void HandleTouch(InputEventScreenTouch touch)
        {
            bool isLeftSide = IsLeftSide(touch.Position);

            if (touch.Pressed)
            {
                if (isLeftSide && !_joystickActive)
                {
                    // Dynamic joystick — spawn at touch point
                    _joystickActive   = true;
                    _joystickFingerId = touch.Index;
                    _joystickOrigin   = touch.Position;
                    _pressTimer       = 0f;
                    _longPressActive  = false;

                    _joystickBase.Position = touch.Position - new Vector2(JoystickRadius, JoystickRadius);
                    _joystickBase.Visible  = true;
                    _joystickKnob.Visible  = true;
                }
                else if (!isLeftSide)
                {
                    // Gesture: double-tap detection on right side
                    float now = Time.GetTicksMsec() / 1000.0f;
                    if (now - _lastTapTime < DoubleTapMaxGap)
                    {
                        OnDoubleTap(touch.Position);
                    }
                    _lastTapTime = now;
                    _swipeStart  = touch.Position;
                }
            }
            else // Released
            {
                if (touch.Index == _joystickFingerId)
                {
                    // Check swipe on release
                    float swipeDist = _swipeStart.DistanceTo(touch.Position);
                    if (swipeDist >= SwipeMinDist)
                    {
                        OnSwipe((_swipeStart - touch.Position).Normalized());
                    }

                    ResetJoystick();
                }
            }
        }

        private void HandleDrag(InputEventScreenDrag drag)
        {
            if (!_joystickActive || drag.Index != _joystickFingerId) return;

            Vector2 delta = drag.Position - _joystickOrigin;
            float   dist  = delta.Length();

            // Clamp knob within radius
            Vector2 clamped = dist <= JoystickRadius
                ? delta
                : delta.Normalized() * JoystickRadius;

            _joystickKnob.Position = _joystickBase.Position + new Vector2(JoystickRadius, JoystickRadius)
                                     + clamped - new Vector2(JoystickRadius * 0.5f, JoystickRadius * 0.5f);

            // Expose normalised axis
            _joystickDelta = clamped / JoystickRadius;

            // Deadzone from SettingsManager
            float deadzone = 0.15f;
            if (_joystickDelta.Length() < deadzone)
                _joystickDelta = Vector2.Zero;
        }

        // ---------------------------------------------------------------
        // GESTURE CALLBACKS
        // ---------------------------------------------------------------

        private void OnDoubleTap(Vector2 pos)
        {
            Core.Logger.Info("TouchControls: Double-tap detected.");
            if (_inputHandler != null) _inputHandler.TouchRolled = true;
        }

        private void OnLongPress(Vector2 pos)
        {
            Core.Logger.Info("TouchControls: Long-press detected.");
            if (_inputHandler != null) _inputHandler.TouchInteract = true;
        }

        private void OnSwipe(Vector2 direction)
        {
            Core.Logger.Info($"TouchControls: Swipe detected direction={direction}.");
            // Upward swipe = jump
            if (direction.Y > 0.6f && _inputHandler != null)
                _inputHandler.TouchJumped = true;
        }

        // ---------------------------------------------------------------
        // HELPERS
        // ---------------------------------------------------------------

        private bool IsLeftSide(Vector2 pos)
        {
            float halfW = GetViewport().GetVisibleRect().Size.X * 0.5f;
            bool physLeft = pos.X < halfW;
            return LeftHandedMode ? !physLeft : physLeft;
        }

        private void ResetJoystick()
        {
            _joystickActive   = false;
            _joystickFingerId = -1;
            _joystickDelta    = Vector2.Zero;
            _joystickBase.Visible = false;
            _joystickKnob.Visible = false;
            _longPressActive  = false;
            _pressTimer       = 0f;
        }

        // ---------------------------------------------------------------
        // PROCEDURAL UI CONSTRUCTION
        // ---------------------------------------------------------------

        private void BuildUI()
        {
            // -- Joystick base ring --
            _joystickBase = new TextureRect
            {
                Size          = new Vector2(JoystickRadius * 2, JoystickRadius * 2),
                Modulate      = new Color(1, 1, 1, JoystickOpacity),
                Visible       = false,
                MouseFilter   = Control.MouseFilterEnum.Ignore,
            };
            AddChild(_joystickBase);

            // -- Joystick knob --
            _joystickKnob = new TextureRect
            {
                Size          = new Vector2(JoystickRadius * 0.6f, JoystickRadius * 0.6f),
                Modulate      = new Color(1, 1, 1, JoystickOpacity + 0.2f),
                Visible       = false,
                MouseFilter   = Control.MouseFilterEnum.Ignore,
            };
            AddChild(_joystickKnob);

            // -- Action buttons --
            var viewSize = GetViewport()?.GetVisibleRect().Size ?? new Vector2(1280, 720);
            float margin  = 20f;
            float rightX  = LeftHandedMode ? margin : viewSize.X - ButtonSize - margin;
            float bottomY = viewSize.Y - ButtonSize - margin;

            _btnJump     = CreateButton("JUMP",     new Vector2(rightX,              bottomY - ButtonSize * 2));
            _btnAttack   = CreateButton("ATK",      new Vector2(rightX + ButtonSize, bottomY - ButtonSize));
            _btnSkill1   = CreateButton("SK1",      new Vector2(rightX + ButtonSize, bottomY - ButtonSize * 3));
            _btnSkill2   = CreateButton("SK2",      new Vector2(rightX,              bottomY - ButtonSize * 3));
            _btnInteract = CreateButton("USE",      new Vector2(rightX - ButtonSize, bottomY - ButtonSize));
            _btnRoll     = CreateButton("ROLL",     new Vector2(rightX,              bottomY));

            _btnJump    .Pressed += () => { if (_inputHandler != null) _inputHandler.TouchJumped   = true; };
            _btnAttack  .Pressed += () => { if (_inputHandler != null) _inputHandler.TouchAttacked  = true; };
            _btnSkill1  .Pressed += () => { if (_inputHandler != null) _inputHandler.TouchSkill1    = true; };
            _btnSkill2  .Pressed += () => { if (_inputHandler != null) _inputHandler.TouchSkill2    = true; };
            _btnInteract.Pressed += () => { if (_inputHandler != null) _inputHandler.TouchInteract  = true; };
            _btnRoll    .Pressed += () => { if (_inputHandler != null) _inputHandler.TouchRolled    = true; };
        }

        private Button CreateButton(string label, Vector2 pos)
        {
            var btn = new Button
            {
                Text     = label,
                Size     = new Vector2(ButtonSize, ButtonSize),
                Position = pos,
            };
            AddChild(btn);
            return btn;
        }

        private void AdaptToScreen()
        {
            var viewSize = GetViewport()?.GetVisibleRect().Size ?? new Vector2(1280, 720);
            // Scale buttons for tablets (screen width > 900 dp)
            if (viewSize.X > 900)
            {
                ButtonSize     *= 1.25f;
                JoystickRadius *= 1.25f;
            }
        }
    }
}
