using Godot;

namespace HeroOfEternia.Player
{
    /// <summary>
    /// PlayerMovement handles all velocity calculations for the CharacterBody3D.
    /// Called by player states — never by external systems.
    ///
    /// Physics parameters:
    ///   Gravity      — Godot project gravity (configurable in project settings)
    ///   Friction     — deceleration on ground
    ///   Air Control  — reduced lateral acceleration in the air
    /// </summary>
    public class PlayerMovement
    {
        // ---------------------------------------------------------------
        // PHYSICS CONSTANTS
        // ---------------------------------------------------------------
        private const float Friction         = 12f;  // Ground deceleration
        private const float AirControl       = 0.35f; // 0=no air control, 1=full
        private const float MaxStepHeight    = 0.35f; // Snapped step detection
        private const float SlopeLimit       = 45f;  // degrees

        private readonly float _gravity;

        public PlayerMovement()
        {
            _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
        }

        // ---------------------------------------------------------------
        // GROUND MOVEMENT
        // ---------------------------------------------------------------

        /// <summary>Moves the player along the ground at the given speed.</summary>
        public void ApplyGroundMovement(PlayerRoot player, float delta, float speed)
        {
            var cam    = player.Camera;
            var input  = player.Input.Current.MoveAxis;

            // Compute world-space movement direction from camera orientation
            Vector3 forward = cam != null ? cam.GetForwardDirection() : -Vector3.Forward;
            Vector3 right   = cam != null ? cam.GetRightDirection()   :  Vector3.Right;
            Vector3 dir     = (forward * -input.Y + right * input.X).Normalized();

            // Rotate player model to face movement direction
            if (dir.LengthSquared() > 0.01f)
            {
                float targetYaw = Mathf.Atan2(dir.X, dir.Z);
                float currentYaw = player.GlobalRotation.Y;
                float smoothYaw = Mathf.LerpAngle(currentYaw, targetYaw, delta * 12f);
                player.GlobalRotation = new Vector3(0, smoothYaw, 0);
            }

            // Horizontal velocity — accelerate toward target
            Vector3 targetVelocity = dir * speed;
            Vector3 vel            = player.Velocity;

            vel.X = Mathf.MoveToward(vel.X, targetVelocity.X, speed * Friction * delta);
            vel.Z = Mathf.MoveToward(vel.Z, targetVelocity.Z, speed * Friction * delta);

            // Apply gravity
            if (!player.IsOnFloor())
            {
                vel.Y -= _gravity * delta;
            }
            else
            {
                vel.Y = -0.1f; // Keep snapped to floor
            }

            player.Velocity = vel;
            player.MoveAndSlide();
        }

        // ---------------------------------------------------------------
        // AIR MOVEMENT
        // ---------------------------------------------------------------

        /// <summary>Applies gravity and reduced lateral control while airborne.</summary>
        public void ApplyAirMovement(PlayerRoot player, float delta)
        {
            var cam   = player.Camera;
            var input = player.Input.Current.MoveAxis;

            Vector3 forward = cam != null ? cam.GetForwardDirection() : -Vector3.Forward;
            Vector3 right   = cam != null ? cam.GetRightDirection()   :  Vector3.Right;
            Vector3 dir     = (forward * -input.Y + right * input.X).Normalized();

            Vector3 vel = player.Velocity;

            // Reduced horizontal control in air
            if (dir.LengthSquared() > 0.01f)
            {
                float airSpeed = player.Data.RunSpeed * AirControl;
                vel.X = Mathf.MoveToward(vel.X, dir.X * airSpeed, airSpeed * delta * 4f);
                vel.Z = Mathf.MoveToward(vel.Z, dir.Z * airSpeed, airSpeed * delta * 4f);
            }

            // Apply gravity
            vel.Y -= _gravity * delta;

            player.Velocity = vel;
            player.MoveAndSlide();
        }

        // ---------------------------------------------------------------
        // JUMP
        // ---------------------------------------------------------------

        /// <summary>Applies an upward impulse for the jump.</summary>
        public void ApplyJump(PlayerRoot player)
        {
            var vel  = player.Velocity;
            vel.Y    = player.Data.JumpVelocity;
            player.Velocity = vel;
        }

        // ---------------------------------------------------------------
        // SURFACE DETECTION
        // ---------------------------------------------------------------

        /// <summary>Returns the surface material below the player's feet.</summary>
        public SurfaceType DetectSurface(PlayerRoot player)
        {
            // Raycast downward from ground check point
            var spaceState = player.GetWorld3D().DirectSpaceState;
            var origin     = player.GlobalPosition + Vector3.Up * 0.1f;
            var query      = PhysicsRayQueryParameters3D.Create(origin, origin - Vector3.Up * 0.6f);
            var result     = spaceState.IntersectRay(query);

            if (result.Count == 0) return SurfaceType.Stone;

            // Future: read a surface material tag from the collider's metadata
            var collider = result["collider"].AsGodotObject();
            if (collider is Node3D node)
            {
                if (node.HasMeta("surface")) return ParseSurface(node.GetMeta("surface").AsString());
            }

            return SurfaceType.Stone; // Default
        }

        private static SurfaceType ParseSurface(string tag) => tag.ToLower() switch
        {
            "grass"  => SurfaceType.Grass,
            "wood"   => SurfaceType.Wood,
            "sand"   => SurfaceType.Sand,
            "snow"   => SurfaceType.Snow,
            "water"  => SurfaceType.Water,
            "mud"    => SurfaceType.Mud,
            _        => SurfaceType.Stone,
        };
    }

    /// <summary>Surface material tags used for footstep audio selection.</summary>
    public enum SurfaceType
    {
        Stone,
        Grass,
        Wood,
        Sand,
        Snow,
        Water,
        Mud,
    }
}
