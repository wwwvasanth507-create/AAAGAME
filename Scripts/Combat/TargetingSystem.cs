using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Combat
{
    /// <summary>
    /// Lightweight data record describing a targetable entity.
    /// Passed by value — no Godot node dependency.
    /// </summary>
    public class CombatTarget
    {
        public string   TargetId  { get; set; } = "";
        public string   FactionId { get; set; } = "neutral";
        public float    WorldX    { get; set; }
        public float    WorldY    { get; set; }
        public float    WorldZ    { get; set; }
        public bool     IsAlive   { get; set; } = true;
        public int      Priority  { get; set; } = 0;      // Higher = preferred
        public float    ColliderRadius { get; set; } = 0.5f; // For LoS box test
    }

    /// <summary>
    /// Modular targeting framework.
    /// Headless-safe — all geometry checks use data-side math only.
    /// </summary>
    public class TargetingSystem
    {
        private readonly Dictionary<string, CombatTarget> _registeredTargets = new();

        private string? _softLockId = null;
        private string? _hardLockId = null;
        private TargetMode _mode = TargetMode.Free;

        public TargetMode Mode => _mode;
        public string? CurrentTargetId => _hardLockId ?? _softLockId;

        // ─────────────────────── Target Registry ───────────────────────

        public void RegisterTarget(CombatTarget target)  => _registeredTargets[target.TargetId] = target;
        public void UnregisterTarget(string targetId)     => _registeredTargets.Remove(targetId);
        public void UpdateTarget(CombatTarget target)     => _registeredTargets[target.TargetId] = target;

        public CombatTarget? GetTarget(string id) =>
            _registeredTargets.TryGetValue(id, out var t) ? t : null;

        // ─────────────────────── Mode Selection ───────────────────────

        public void SetMode(TargetMode mode)
        {
            _mode = mode;
            Logger.Info($"TargetingSystem: mode → {mode}");
            if (mode == TargetMode.Free) { _softLockId = null; _hardLockId = null; }
        }

        // ─────────────────────── Target Selection ───────────────────────

        /// <summary>
        /// Finds the nearest alive target to the given position within maxRange.
        /// Optionally excludes a faction (e.g. "player" faction).
        /// </summary>
        public CombatTarget? FindNearest(float ox, float oy, float oz,
                                         float maxRange, string excludeFaction = "")
        {
            CombatTarget? best = null;
            float bestDist = float.MaxValue;

            foreach (var t in _registeredTargets.Values)
            {
                if (!t.IsAlive) continue;
                if (!string.IsNullOrEmpty(excludeFaction) && t.FactionId == excludeFaction) continue;

                float d = Distance(ox, oy, oz, t.WorldX, t.WorldY, t.WorldZ);
                if (d <= maxRange && d < bestDist)
                {
                    bestDist = d;
                    best = t;
                }
            }
            return best;
        }

        /// <summary>
        /// Soft-locks to the nearest valid target and returns it.
        /// </summary>
        public CombatTarget? SoftLock(float ox, float oy, float oz,
                                       float maxRange, string excludeFaction = "")
        {
            var t = FindNearest(ox, oy, oz, maxRange, excludeFaction);
            _softLockId = t?.TargetId;
            _mode = t != null ? TargetMode.SoftLock : TargetMode.Free;
            return t;
        }

        /// <summary>
        /// Hard-locks to the specified target ID.
        /// </summary>
        public bool HardLock(string targetId)
        {
            if (!_registeredTargets.TryGetValue(targetId, out var t) || !t.IsAlive)
            {
                Logger.Info($"TargetingSystem: HardLock failed — '{targetId}' not found or dead.");
                return false;
            }
            _hardLockId = targetId;
            _mode = TargetMode.HardLock;
            Logger.Info($"TargetingSystem: hard-locked → '{targetId}'");
            return true;
        }

        public void ClearLock()
        {
            _softLockId = null;
            _hardLockId = null;
            _mode = TargetMode.Free;
        }

        /// <summary>
        /// Switches to the next target by priority (cyclic).
        /// </summary>
        public CombatTarget? SwitchTarget(float ox, float oy, float oz,
                                           float maxRange, string excludeFaction = "")
        {
            var candidates = new List<CombatTarget>();
            foreach (var t in _registeredTargets.Values)
            {
                if (!t.IsAlive) continue;
                if (!string.IsNullOrEmpty(excludeFaction) && t.FactionId == excludeFaction) continue;
                if (Distance(ox, oy, oz, t.WorldX, t.WorldY, t.WorldZ) <= maxRange)
                    candidates.Add(t);
            }
            if (candidates.Count == 0) { ClearLock(); return null; }
            candidates.Sort((a, b) => b.Priority.CompareTo(a.Priority));
            int idx = candidates.FindIndex(c => c.TargetId == CurrentTargetId);
            var next = candidates[(idx + 1) % candidates.Count];
            HardLock(next.TargetId);
            return next;
        }

        // ─────────────────────── Validation ───────────────────────

        /// <summary>
        /// Validates that a target is within range.
        /// </summary>
        public bool IsInRange(string targetId, float ox, float oy, float oz, float maxRange)
        {
            if (!_registeredTargets.TryGetValue(targetId, out var t)) return false;
            return Distance(ox, oy, oz, t.WorldX, t.WorldY, t.WorldZ) <= maxRange;
        }

        /// <summary>
        /// Simple LoS check using a forward-vector dot product against target direction.
        /// Full ray-cast requires scene graph — this approximation is headless-safe.
        /// </summary>
        public bool HasLineOfSight(float ox, float oy, float oz,
                                   float fwdX, float fwdZ,
                                   string targetId, float fovDegrees = 120f)
        {
            if (!_registeredTargets.TryGetValue(targetId, out var t)) return false;
            float dx = t.WorldX - ox;
            float dz = t.WorldZ - oz;
            float len = MathF.Sqrt(dx * dx + dz * dz);
            if (len < 0.001f) return true;
            float dot = (dx / len) * fwdX + (dz / len) * fwdZ;
            float fovCos = MathF.Cos(fovDegrees * MathF.PI / 180f / 2f);
            return dot >= fovCos;
        }

        // ─────────────────────── Helpers ───────────────────────

        private static float Distance(float ax, float ay, float az,
                                       float bx, float by, float bz)
        {
            float dx = bx - ax, dy = by - ay, dz = bz - az;
            return MathF.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public int RegisteredCount => _registeredTargets.Count;

        public IReadOnlyDictionary<string, CombatTarget> All => _registeredTargets;
    }
}
