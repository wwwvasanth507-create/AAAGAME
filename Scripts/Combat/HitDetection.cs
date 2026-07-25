using System;
using System.Collections.Generic;

namespace HeroOfEternia.Combat
{
    /// <summary>
    /// Data-side bounding volume for hit detection.
    /// No Godot Area3D dependency — fully headless-safe.
    /// </summary>
    public class HitVolume
    {
        public string          OwnerId      { get; set; } = "";
        public HitVolumeShape  Shape        { get; set; } = HitVolumeShape.Sphere;

        // AABB fields
        public float MinX { get; set; } public float MinY { get; set; } public float MinZ { get; set; }
        public float MaxX { get; set; } public float MaxY { get; set; } public float MaxZ { get; set; }

        // Sphere fields
        public float CenterX { get; set; } public float CenterY { get; set; } public float CenterZ { get; set; }
        public float Radius  { get; set; } = 0.5f;

        // Layer & faction
        public int  LayerMask     { get; set; } = 0b_0000_0001;  // bit flags
        public bool FriendlyFire  { get; set; } = false;
        public string FactionId   { get; set; } = "enemy";

        // Factory helpers
        public static HitVolume MakeSphere(string ownerId, float cx, float cy, float cz,
                                           float radius, string factionId = "enemy")
            => new HitVolume { OwnerId = ownerId, Shape = HitVolumeShape.Sphere,
                               CenterX = cx, CenterY = cy, CenterZ = cz,
                               Radius = radius, FactionId = factionId };

        public static HitVolume MakeAABB(string ownerId,
                                         float minX, float minY, float minZ,
                                         float maxX, float maxY, float maxZ,
                                         string factionId = "enemy")
            => new HitVolume { OwnerId = ownerId, Shape = HitVolumeShape.AABB,
                               MinX = minX, MinY = minY, MinZ = minZ,
                               MaxX = maxX, MaxY = maxY, MaxZ = maxZ, FactionId = factionId };
    }

    /// <summary>
    /// Static headless hit detection service.
    /// Supports melee sweeps, projectile point checks, and area-of-effect radius checks.
    /// </summary>
    public static class HitDetection
    {
        // ─────────────────────── Melee ───────────────────────

        /// <summary>
        /// Checks a melee attack volume against a list of candidate targets.
        /// Returns all targets whose bounding spheres overlap with the attack volume.
        /// </summary>
        public static List<HitResult> CheckMelee(HitVolume attackVolume,
                                                  IEnumerable<CombatTarget> candidates)
        {
            var results = new List<HitResult>();
            foreach (var t in candidates)
            {
                if (!t.IsAlive) continue;
                if (!attackVolume.FriendlyFire && t.FactionId == attackVolume.FactionId) continue;

                bool hit = attackVolume.Shape == HitVolumeShape.Sphere
                    ? SphereSphereOverlap(attackVolume, t)
                    : AABBSphereOverlap(attackVolume, t);

                if (hit)
                {
                    results.Add(new HitResult
                    {
                        DidHit   = true,
                        TargetId = t.TargetId,
                        Distance = Distance(attackVolume.CenterX, attackVolume.CenterY, attackVolume.CenterZ,
                                            t.WorldX, t.WorldY, t.WorldZ),
                        HitPointX = t.WorldX,
                        HitPointY = t.WorldY,
                        HitPointZ = t.WorldZ
                    });
                }
            }
            return results;
        }

        // ─────────────────────── Projectile ───────────────────────

        /// <summary>
        /// Checks if a projectile point is within any target's collider radius.
        /// </summary>
        public static HitResult CheckProjectile(float px, float py, float pz,
                                                 IEnumerable<CombatTarget> candidates,
                                                 string shooterFaction = "player")
        {
            foreach (var t in candidates)
            {
                if (!t.IsAlive) continue;
                if (t.FactionId == shooterFaction) continue;

                float d = Distance(px, py, pz, t.WorldX, t.WorldY, t.WorldZ);
                if (d <= t.ColliderRadius)
                {
                    return new HitResult
                    {
                        DidHit = true, TargetId = t.TargetId, Distance = d,
                        HitPointX = t.WorldX, HitPointY = t.WorldY, HitPointZ = t.WorldZ
                    };
                }
            }
            return new HitResult { DidHit = false };
        }

        // ─────────────────────── Area Effect ───────────────────────

        /// <summary>
        /// Checks all targets within an area-of-effect radius.
        /// </summary>
        public static List<HitResult> CheckAreaEffect(float cx, float cy, float cz,
                                                       float radius,
                                                       IEnumerable<CombatTarget> candidates,
                                                       bool friendlyFire = false,
                                                       string excludeFaction = "player")
        {
            var results = new List<HitResult>();
            foreach (var t in candidates)
            {
                if (!t.IsAlive) continue;
                if (!friendlyFire && t.FactionId == excludeFaction) continue;

                float d = Distance(cx, cy, cz, t.WorldX, t.WorldY, t.WorldZ);
                if (d <= radius)
                {
                    results.Add(new HitResult
                    {
                        DidHit = true, TargetId = t.TargetId, Distance = d,
                        HitPointX = t.WorldX, HitPointY = t.WorldY, HitPointZ = t.WorldZ
                    });
                }
            }
            return results;
        }

        // ─────────────────────── Geometry Helpers ───────────────────────

        private static bool SphereSphereOverlap(HitVolume v, CombatTarget t)
        {
            float d = Distance(v.CenterX, v.CenterY, v.CenterZ, t.WorldX, t.WorldY, t.WorldZ);
            return d <= (v.Radius + t.ColliderRadius);
        }

        private static bool AABBSphereOverlap(HitVolume v, CombatTarget t)
        {
            float cx = Math.Clamp(t.WorldX, v.MinX, v.MaxX);
            float cy = Math.Clamp(t.WorldY, v.MinY, v.MaxY);
            float cz = Math.Clamp(t.WorldZ, v.MinZ, v.MaxZ);
            float d  = Distance(cx, cy, cz, t.WorldX, t.WorldY, t.WorldZ);
            return d <= t.ColliderRadius;
        }

        private static float Distance(float ax, float ay, float az,
                                       float bx, float by, float bz)
        {
            float dx = bx - ax, dy = by - ay, dz = bz - az;
            return MathF.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }
}
