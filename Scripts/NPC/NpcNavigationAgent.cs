using System;
using HeroOfEternia.Core;
using HeroOfEternia.World;

namespace HeroOfEternia.NPC
{
    /// <summary>
    /// Tracks the navigation state for a single NPC.
    /// Uses the static NavigationFoundation.IsWalkable(height, slopeDeg) API.
    /// Fully headless-safe — no live Godot NavigationAgent3D dependency.
    /// </summary>
    public class NpcNavigationAgent
    {
        public string NpcId { get; private set; }

        private float _currentX;
        private float _currentY;
        private float _currentZ;

        private float _destX;
        private float _destY;
        private float _destZ;

        private bool _hasDestination = false;
        private float _stepSize = 1.0f;      // metres per update tick
        private float _arrivalThreshold = 1.5f;

        // Optional flat terrain height for walkability check (default 0 = flat ground)
        private float _assumedHeight = 0f;
        private float _assumedSlope  = 0f;

        public bool HasDestination => _hasDestination;
        public bool HasReached => _hasDestination && DistanceToDestination() <= _arrivalThreshold;

        public NpcNavigationAgent(string npcId,
                                  float startX, float startY, float startZ,
                                  float stepSize = 1.0f)
        {
            NpcId     = npcId;
            _currentX = startX;
            _currentY = startY;
            _currentZ = startZ;
            _stepSize = stepSize;
        }

        /// <summary>
        /// Sets a new world-space destination. Returns false if destination is not walkable
        /// based on the currently assumed terrain height and slope.
        /// </summary>
        public bool SetDestination(float x, float y, float z,
                                   float terrainHeight = 0f, float slopeDeg = 0f)
        {
            if (!NavigationFoundation.IsWalkable(terrainHeight, slopeDeg))
            {
                Logger.Info($"NpcNavAgent[{NpcId}]: destination ({x:F1},{z:F1}) is not walkable — ignored.");
                return false;
            }
            _destX = x;
            _destY = y;
            _destZ = z;
            _assumedHeight = terrainHeight;
            _assumedSlope  = slopeDeg;
            _hasDestination = true;
            return true;
        }

        /// <summary>
        /// Advances the NPC one step towards the destination (called per tick).
        /// Validates each next cell via NavigationFoundation before moving.
        /// </summary>
        public bool AdvanceStep(float nextCellHeight = 0f, float nextCellSlope = 0f)
        {
            if (!_hasDestination || HasReached) return false;

            float dx = _destX - _currentX;
            float dz = _destZ - _currentZ;
            float dist = MathF.Sqrt(dx * dx + dz * dz);

            if (dist < 0.001f) return false;

            float nx = _currentX + (dx / dist) * _stepSize;
            float nz = _currentZ + (dz / dist) * _stepSize;

            // Validate next cell walkability before moving
            if (!NavigationFoundation.IsWalkable(nextCellHeight, nextCellSlope))
            {
                Logger.Info($"NpcNavAgent[{NpcId}]: step blocked at ({nx:F1},{nz:F1}).");
                return false;
            }

            _currentX = nx;
            _currentZ = nz;
            return true;
        }

        public void ClearDestination()
        {
            _hasDestination = false;
        }

        public (float x, float y, float z) GetPosition() => (_currentX, _currentY, _currentZ);

        public float DistanceToDestination()
        {
            float dx = _destX - _currentX;
            float dz = _destZ - _currentZ;
            return MathF.Sqrt(dx * dx + dz * dz);
        }

        /// <summary>
        /// Returns position as a serializable float array for Save V6.
        /// </summary>
        public float[] GetPositionSnapshot() => new[] { _currentX, _currentY, _currentZ };

        /// <summary>
        /// Restores position from a Save V6 snapshot.
        /// </summary>
        public void RestorePosition(float[] snapshot)
        {
            if (snapshot == null || snapshot.Length < 3) return;
            _currentX = snapshot[0];
            _currentY = snapshot[1];
            _currentZ = snapshot[2];
        }
    }
}
