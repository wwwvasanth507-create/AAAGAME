using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Combat
{
    public record ArenaBoundary
    {
        public Vector3 Center { get; init; } = Vector3.Zero;
        public float Radius { get; init; } = 30f; // Cylinder radius
        public float Height { get; init; } = 20f;
    }

    public record ArenaSafeZone
    {
        public Vector3 Center { get; init; } = Vector3.Zero;
        public float Radius { get; init; } = 5f;
    }

    public record ArenaHazardZone
    {
        public string HazardId { get; init; } = string.Empty;
        public Vector3 Center { get; init; } = Vector3.Zero;
        public float Radius { get; init; } = 4f;
        public float DamagePerSecond { get; init; } = 15f;
        public string Element { get; init; } = "fire";
    }

    public record ArenaCameraZone
    {
        public Vector3 Center { get; init; } = Vector3.Zero;
        public float LookAtHeight { get; init; } = 2f;
        public float CameraDistance { get; init; } = 15f;
        public float CameraAngleDegrees { get; init; } = 45f;
    }

    public record ArenaDefinition
    {
        public string ArenaId { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public ArenaBoundary Boundary { get; init; } = new();
        public Vector3 EntryPoint { get; init; } = Vector3.Zero;
        public Vector3 ExitPoint { get; init; } = Vector3.Zero;
        public List<Vector3> BossSpawnLocations { get; init; } = new();
        public List<Vector3> MinionSpawnLocations { get; init; } = new();
        public List<ArenaSafeZone> SafeZones { get; init; } = new();
        public List<ArenaHazardZone> Hazards { get; init; } = new();
        public ArenaCameraZone CameraSettings { get; init; } = new();
        public bool LockGatesOnStart { get; init; } = true;
        public bool ResetBossOnPlayerExit { get; init; } = true;
        public int MaxSupportedPlayers { get; init; } = 4; // Future multiplayer hook
    }

    public class ArenaInstance
    {
        public ArenaDefinition Definition { get; }
        private bool _gatesLocked = false;
        private readonly List<ArenaHazardZone> _activeHazards = new();

        public bool GatesLocked => _gatesLocked;
        public IReadOnlyList<ArenaHazardZone> ActiveHazards => _activeHazards;

        public ArenaInstance(ArenaDefinition definition)
        {
            Definition = definition;
            _activeHazards.AddRange(definition.Hazards);
        }

        public void LockGates()
        {
            if (Definition.LockGatesOnStart)
            {
                _gatesLocked = true;
            }
        }

        public void UnlockGates()
        {
            _gatesLocked = false;
        }

        public bool IsWithinBoundaries(Vector3 position)
        {
            // Simple cylindrical boundary collision calculation
            Vector3 diff = position - Definition.Boundary.Center;
            float distHorizontalSq = diff.X * diff.X + diff.Z * diff.Z;
            float maxRadiusSq = Definition.Boundary.Radius * Definition.Boundary.Radius;

            if (distHorizontalSq > maxRadiusSq) return false;
            if (position.Y < Definition.Boundary.Center.Y || position.Y > Definition.Boundary.Center.Y + Definition.Boundary.Height) return false;

            return true;
        }

        public bool IsInSafeZone(Vector3 position)
        {
            foreach (var zone in Definition.SafeZones)
            {
                float distSq = position.DistanceSquaredTo(zone.Center);
                if (distSq <= zone.Radius * zone.Radius) return true;
            }
            return false;
        }

        public ArenaHazardZone? GetActiveHazardCollision(Vector3 position)
        {
            if (IsInSafeZone(position)) return null;

            foreach (var hazard in _activeHazards)
            {
                float distSq = position.DistanceSquaredTo(hazard.Center);
                if (distSq <= hazard.Radius * hazard.Radius)
                {
                    return hazard;
                }
            }
            return null;
        }

        public void Reset()
        {
            UnlockGates();
            _activeHazards.Clear();
            _activeHazards.AddRange(Definition.Hazards);
        }
    }
}
