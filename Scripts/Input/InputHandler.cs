using Godot;

namespace HeroOfEternia.Input
{
    /// <summary>
    /// Unified frame-snapshot of all player inputs, sourced from any device type
    /// (keyboard, mouse, gamepad, or virtual touch joystick).
    /// Systems read from InputFrame — they never poll devices directly.
    /// </summary>
    public class InputFrame
    {
        /// <summary>Normalised movement direction in the XZ plane (-1..1 each axis).</summary>
        public Vector2 MoveAxis    { get; set; } = Vector2.Zero;
        /// <summary>Camera look delta (mouse delta or right-stick input).</summary>
        public Vector2 LookDelta   { get; set; } = Vector2.Zero;

        public bool Sprint     { get; set; }
        public bool Walk       { get; set; }
        public bool Jump       { get; set; }
        public bool Roll       { get; set; }
        public bool Crouch     { get; set; }
        public bool Attack     { get; set; }
        public bool HeavyAttack{ get; set; }
        public bool Block      { get; set; }
        public bool Skill1     { get; set; }
        public bool Skill2     { get; set; }
        public bool Skill3     { get; set; }
        public bool Skill4     { get; set; }
        public bool Interact   { get; set; }
        public bool LockTarget { get; set; }
        public bool OpenInventory { get; set; }
        public bool OpenMap    { get; set; }
        public bool OpenQuests { get; set; }
        public bool OpenSettings { get; set; }
        public bool Pause      { get; set; }
        public bool CameraReset{ get; set; }
        public float CameraZoom { get; set; } // -1 zoom out .. +1 zoom in

        /// <summary>True when input is from a touch device this frame.</summary>
        public bool IsTouchDevice { get; set; }
    }

    /// <summary>
    /// InputHandler polls all hardware sources every frame and produces a single
    /// InputFrame that every game system reads. Injected touch input from
    /// TouchControls.cs overrides move/look axes on Android.
    /// </summary>
    public partial class InputHandler : Node
    {
        // Injected by TouchControls when running on mobile
        public Vector2 TouchMoveAxis  { get; set; } = Vector2.Zero;
        public Vector2 TouchLookDelta { get; set; } = Vector2.Zero;
        public bool    TouchJumped    { get; set; }
        public bool    TouchRolled    { get; set; }
        public bool    TouchAttacked  { get; set; }
        public bool    TouchSkill1    { get; set; }
        public bool    TouchSkill2    { get; set; }
        public bool    TouchInteract  { get; set; }

        private bool _isTouchPlatform;
        private float _mouseSensitivity = 0.25f;

        /// <summary>Current fully-resolved input state for this frame.</summary>
        public InputFrame Current { get; private set; } = new InputFrame();

        public override void _Ready()
        {
            _isTouchPlatform = OS.GetName() == "Android" || OS.GetName() == "iOS";
            Core.Logger.Info($"InputHandler: Platform detected as '{OS.GetName()}'. Touch={_isTouchPlatform}");
        }

        public override void _Process(double delta)
        {
            BuildFrame();
        }

        public override void _Input(InputEvent ev)
        {
            // Mouse look (editor / PC mode)
            if (!_isTouchPlatform && ev is InputEventMouseMotion mm)
            {
                Current.LookDelta = mm.Relative * _mouseSensitivity;
            }
        }

        public void SetMouseSensitivity(float sensitivity)
        {
            _mouseSensitivity = Mathf.Clamp(sensitivity, 0.05f, 2.0f);
        }

        // -------------------------------------------------------------------
        // PRIVATE
        // -------------------------------------------------------------------

        private void BuildFrame()
        {
            bool isTouch = _isTouchPlatform;

            // --- Movement axis ---
            Vector2 kbMove = Vector2.Zero;
            if (!isTouch)
            {
                if (Godot.Input.IsActionPressed(InputActions.MoveForward)) kbMove.Y -= 1;
                if (Godot.Input.IsActionPressed(InputActions.MoveBack))    kbMove.Y += 1;
                if (Godot.Input.IsActionPressed(InputActions.MoveLeft))    kbMove.X -= 1;
                if (Godot.Input.IsActionPressed(InputActions.MoveRight))   kbMove.X += 1;
                kbMove = kbMove.LimitLength(1.0f); // Prevent diagonal speed boost
            }

            Vector2 gamepadMove = Godot.Input.GetVector(
                InputActions.MoveLeft, InputActions.MoveRight,
                InputActions.MoveForward, InputActions.MoveBack);

            // Touch overrides all when on mobile
            Vector2 finalMove = isTouch
                ? TouchMoveAxis
                : (gamepadMove.LengthSquared() > 0.01f ? gamepadMove : kbMove);

            // --- Camera look ---
            Vector2 gamepadLook = Godot.Input.GetVector(
                InputActions.CameraRotateLeft, InputActions.CameraRotateRight,
                "camera_rotate_up", "camera_rotate_down");

            Vector2 finalLook = isTouch
                ? TouchLookDelta
                : (gamepadLook.LengthSquared() > 0.01f ? gamepadLook * 2.0f : Current.LookDelta);

            // --- Camera zoom ---
            float zoom = 0f;
            zoom += Godot.Input.GetActionStrength(InputActions.CameraZoomIn);
            zoom -= Godot.Input.GetActionStrength(InputActions.CameraZoomOut);

            // --- Build final frame ---
            Current = new InputFrame
            {
                MoveAxis     = finalMove,
                LookDelta    = finalLook,
                IsTouchDevice = isTouch,

                Sprint       = Godot.Input.IsActionPressed(InputActions.Sprint),
                Walk         = Godot.Input.IsActionPressed(InputActions.Walk),
                Jump         = Godot.Input.IsActionJustPressed(InputActions.Jump) || (isTouch && TouchJumped),
                Roll         = Godot.Input.IsActionJustPressed(InputActions.Roll) || (isTouch && TouchRolled),
                Crouch       = Godot.Input.IsActionPressed(InputActions.Crouch),
                Attack       = Godot.Input.IsActionJustPressed(InputActions.Attack) || (isTouch && TouchAttacked),
                HeavyAttack  = Godot.Input.IsActionJustPressed(InputActions.HeavyAttack),
                Block        = Godot.Input.IsActionPressed(InputActions.Block),
                Skill1       = Godot.Input.IsActionJustPressed(InputActions.Skill1) || (isTouch && TouchSkill1),
                Skill2       = Godot.Input.IsActionJustPressed(InputActions.Skill2) || (isTouch && TouchSkill2),
                Skill3       = Godot.Input.IsActionJustPressed(InputActions.Skill3),
                Skill4       = Godot.Input.IsActionJustPressed(InputActions.Skill4),
                Interact     = Godot.Input.IsActionJustPressed(InputActions.Interact) || (isTouch && TouchInteract),
                LockTarget   = Godot.Input.IsActionJustPressed(InputActions.LockTarget),
                OpenInventory = Godot.Input.IsActionJustPressed(InputActions.OpenInventory),
                OpenMap      = Godot.Input.IsActionJustPressed(InputActions.OpenMap),
                OpenQuests   = Godot.Input.IsActionJustPressed(InputActions.OpenQuests),
                OpenSettings = Godot.Input.IsActionJustPressed(InputActions.OpenSettings),
                Pause        = Godot.Input.IsActionJustPressed(InputActions.Pause),
                CameraReset  = Godot.Input.IsActionJustPressed(InputActions.CameraReset),
                CameraZoom   = zoom,
            };

            // Clear one-shot touch flags after consuming them
            TouchJumped   = false;
            TouchRolled   = false;
            TouchAttacked = false;
            TouchSkill1   = false;
            TouchSkill2   = false;
            TouchInteract = false;
            TouchLookDelta = Vector2.Zero;
        }
    }
}
