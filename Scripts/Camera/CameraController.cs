using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.Camera
{
    /// <summary>Identifies which camera mode is currently active.</summary>
    public enum CameraMode
    {
        ThirdPerson,
        FirstPerson,   // Framework stub — full implementation in a future phase
        PhotoMode,
        FreeCam,       // Developer only
    }

    /// <summary>
    /// CameraController provides smooth third-person follow with:
    ///  - Spring-arm collision avoidance
    ///  - Configurable zoom / distance
    ///  - Camera shake (trauma system)
    ///  - Soft lock-on orbit
    ///  - Dynamic FOV scaling during sprint
    ///  - Input from InputHandler (no direct hardware polling)
    /// </summary>
    public partial class CameraController : Node3D
    {
        // ---------------------------------------------------------------
        // EXPORTS — tunable in Godot inspector
        // ---------------------------------------------------------------
        [Export] public NodePath  TargetPath           = new NodePath("../Player");
        [Export] public float     Distance             = 5.0f;
        [Export] public float     MinDistance          = 1.5f;
        [Export] public float     MaxDistance          = 12.0f;
        [Export] public float     Height               = 1.8f;
        [Export] public float     PitchMin             = -40.0f; // degrees
        [Export] public float     PitchMax             = 60.0f;
        [Export] public float     RotationSensitivity  = 0.4f;
        [Export] public float     ZoomSpeed            = 3.0f;
        [Export] public float     FollowSmoothness     = 8.0f;   // higher = tighter follow
        [Export] public float     RotationSmoothness   = 10.0f;
        [Export] public bool      InvertY              = false;
        [Export] public float     BaseFov              = 75.0f;
        [Export] public float     SprintFovBoost       = 8.0f;
        [Export] public CameraMode Mode                = CameraMode.ThirdPerson;

        // ---------------------------------------------------------------
        // PRIVATE STATE
        // ---------------------------------------------------------------
        private Node3D?    _target;
        private Camera3D   _camera     = null!;
        private SpringArm3D _springArm = null!;

        private float _yaw   = 0f;   // horizontal angle (degrees)
        private float _pitch = -15f; // vertical angle (degrees)

        private float _currentDistance;
        private float _targetDistance;

        // Shake (trauma system — squared decay)
        private float _trauma     = 0f;
        private float _shakeTime  = 0f;

        // FOV
        private float _currentFov;

        // Lock-on target (set by external system)
        private Node3D? _lockTarget;
        private bool    _isLocked = false;

        // FreeCam
        private Vector3 _freeCamVelocity = Vector3.Zero;

        // Input reference
        private Input.InputHandler? _inputHandler;

        // ---------------------------------------------------------------
        // LIFECYCLE
        // ---------------------------------------------------------------

        public override void _Ready()
        {
            _currentDistance = Distance;
            _targetDistance  = Distance;
            _currentFov      = BaseFov;

            // Find target node
            if (!TargetPath.IsEmpty)
                _target = GetNodeOrNull<Node3D>(TargetPath);

            // Find InputHandler in scene
            _inputHandler = GetTree().Root.FindChild("InputHandler", true, false)
                           as Input.InputHandler;

            // Build spring arm
            _springArm = new SpringArm3D
            {
                SpringLength = Distance,
                CollisionMask = 1, // collide with world geometry
                Margin = 0.1f
            };
            AddChild(_springArm);

            // Build camera
            _camera = new Camera3D { Fov = BaseFov };
            _springArm.AddChild(_camera);

            Logger.Info($"CameraController: Ready. Mode={Mode}, Distance={Distance}");
        }

        public override void _Process(double delta)
        {
            float dt = (float)delta;

            if (_target == null) return;

            switch (Mode)
            {
                case CameraMode.ThirdPerson: UpdateThirdPerson(dt); break;
                case CameraMode.FreeCam:     UpdateFreeCam(dt);     break;
                case CameraMode.PhotoMode:   UpdateFreeCam(dt);     break; // same as free
                // FirstPerson stub: implement in Phase 7
            }

            UpdateShake(dt);
            UpdateFov(dt);
        }

        // ---------------------------------------------------------------
        // THIRD PERSON
        // ---------------------------------------------------------------

        private void UpdateThirdPerson(float dt)
        {
            var frame = _inputHandler?.Current;

            // --- Camera rotation input ---
            if (_isLocked && _lockTarget != null)
            {
                // Orbit smoothly toward lock target
                Vector3 toTarget = (_lockTarget.GlobalPosition - GlobalPosition).Normalized();
                float   targetYaw = Mathf.RadToDeg(Mathf.Atan2(toTarget.X, toTarget.Z));
                _yaw = Mathf.LerpAngle(_yaw, targetYaw, dt * RotationSmoothness);
            }
            else if (frame != null)
            {
                float lookX = frame.LookDelta.X;
                float lookY = frame.LookDelta.Y * (InvertY ? -1f : 1f);
                _yaw   -= lookX * RotationSensitivity;
                _pitch  = Mathf.Clamp(_pitch - lookY * RotationSensitivity, PitchMin, PitchMax);
            }

            // Reset button
            if (frame != null && frame.CameraReset)
            {
                _yaw   = _target?.GlobalRotationDegrees.Y ?? 0f;
                _pitch = -15f;
            }

            // --- Zoom ---
            if (frame != null)
            {
                _targetDistance = Mathf.Clamp(
                    _targetDistance - frame.CameraZoom * ZoomSpeed * 0.016f,
                    MinDistance, MaxDistance);
            }
            _currentDistance = Mathf.Lerp(_currentDistance, _targetDistance, dt * 6f);
            _springArm.SpringLength = _currentDistance;

            // --- Follow position ---
            Vector3 targetPos = _target!.GlobalPosition + Vector3.Up * Height;
            GlobalPosition = GlobalPosition.Lerp(targetPos, dt * FollowSmoothness);

            // --- Apply rotation ---
            GlobalRotationDegrees = new Vector3(_pitch, _yaw, 0f);
        }

        // ---------------------------------------------------------------
        // FREE CAM (developer / photo mode)
        // ---------------------------------------------------------------

        private void UpdateFreeCam(float dt)
        {
            float speed = 8f;
            var move    = _inputHandler?.Current.MoveAxis ?? Vector2.Zero;
            var look    = _inputHandler?.Current.LookDelta ?? Vector2.Zero;

            _yaw   -= look.X * RotationSensitivity;
            _pitch  = Mathf.Clamp(_pitch - look.Y * RotationSensitivity, -89f, 89f);
            GlobalRotationDegrees = new Vector3(_pitch, _yaw, 0f);

            Vector3 forward = -GlobalTransform.Basis.Z;
            Vector3 right   =  GlobalTransform.Basis.X;
            Vector3 dir     = (forward * -move.Y + right * move.X).Normalized();
            GlobalPosition += dir * speed * dt;
        }

        // ---------------------------------------------------------------
        // CAMERA SHAKE (trauma system)
        // ---------------------------------------------------------------

        /// <summary>Add trauma (0..1). Shake intensity = trauma². Call for hits, landings, explosions.</summary>
        public void AddTrauma(float amount)
        {
            _trauma = Mathf.Min(_trauma + amount, 1.0f);
        }

        private void UpdateShake(float dt)
        {
            if (_trauma <= 0f) return;

            float shake = _trauma * _trauma;
            _shakeTime += dt * 40f;

            float offX = Mathf.Sin(_shakeTime * 1.3f) * shake * 0.8f;
            float offY = Mathf.Cos(_shakeTime * 1.7f) * shake * 0.8f;
            _springArm.Rotation = new Vector3(
                Mathf.DegToRad(offY),
                Mathf.DegToRad(offX),
                0f);

            _trauma = Mathf.Max(0f, _trauma - dt * 1.5f);
        }

        // ---------------------------------------------------------------
        // DYNAMIC FOV
        // ---------------------------------------------------------------

        private void UpdateFov(float dt)
        {
            bool isSprinting = _inputHandler?.Current.Sprint ?? false;
            float targetFov  = isSprinting ? BaseFov + SprintFovBoost : BaseFov;
            _currentFov = Mathf.Lerp(_currentFov, targetFov, dt * 5f);
            _camera.Fov = _currentFov;
        }

        // ---------------------------------------------------------------
        // PUBLIC API
        // ---------------------------------------------------------------

        public void SetMode(CameraMode mode)
        {
            Mode = mode;
            Logger.Info($"CameraController: Mode switched to {mode}.");
        }

        public void SetLockTarget(Node3D? target)
        {
            _lockTarget = target;
            _isLocked   = target != null;
        }

        public void SetSensitivity(float sens) =>
            RotationSensitivity = Mathf.Clamp(sens, 0.05f, 2.0f);

        public void SetInvertY(bool invert) => InvertY = invert;

        /// <summary>Returns the world direction the camera is looking (for movement orientation).</summary>
        public Vector3 GetForwardDirection()
        {
            var basis = _springArm.GlobalTransform.Basis;
            return -new Vector3(basis.Z.X, 0, basis.Z.Z).Normalized();
        }

        public Vector3 GetRightDirection()
        {
            var basis = _springArm.GlobalTransform.Basis;
            return new Vector3(basis.X.X, 0, basis.X.Z).Normalized();
        }
    }
}
