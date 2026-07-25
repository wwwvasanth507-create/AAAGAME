using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Combat
{
    /// <summary>
    /// Data definition for a projectile type.
    /// Loaded from weapons_config.json or projectiles_config.json.
    /// </summary>
    public class ProjectileData
    {
        public string         UniqueId      { get; set; } = "";
        public float          Speed         { get; set; } = 20f;    // m/s
        public float          Gravity       { get; set; } = 0f;     // m/s² downward
        public float          Lifetime      { get; set; } = 5f;     // seconds
        public bool           Piercing      { get; set; } = false;
        public int            BounceCount   { get; set; } = 0;
        public bool           IsHoming      { get; set; } = false;
        public float          HomingStrength{ get; set; } = 5f;     // turn rate deg/s
        public DamageInstance Damage        { get; set; } = new();
        public string         VfxHookKey    { get; set; } = "vfx_arrow";
        public string         AudioHookKey  { get; set; } = "sfx_arrow_fly";
        public string         ImpactVfxKey  { get; set; } = "vfx_arrow_impact";
    }

    /// <summary>
    /// Runtime state for a single in-flight projectile.
    /// </summary>
    public class ActiveProjectile
    {
        public string         Id            { get; set; } = "";
        public ProjectileData Data          { get; set; } = null!;
        public float          PosX          { get; set; }
        public float          PosY          { get; set; }
        public float          PosZ          { get; set; }
        public float          VelX          { get; set; }
        public float          VelY          { get; set; }
        public float          VelZ          { get; set; }
        public float          LifetimeLeft  { get; set; }
        public int            BouncesLeft   { get; set; }
        public bool           IsExpired     { get; set; } = false;
        public string         ShooterFaction{ get; set; } = "player";
    }

    /// <summary>
    /// Generic projectile simulation system.
    /// Fully headless-safe — position/velocity updated in data space.
    /// Impact events fired through the OnImpact callback.
    /// </summary>
    public class ProjectileSystem
    {
        private readonly Dictionary<string, ActiveProjectile> _active = new();
        private readonly Random _rng = new();
        private int _nextId = 0;

        /// <summary>Fired when a projectile hits a target. (projectile, hitResult)</summary>
        public event Action<ActiveProjectile, HitResult>? OnImpact;

        /// <summary>Fired when a projectile expires without hitting anything.</summary>
        public event Action<ActiveProjectile>? OnExpired;

        // ─────────────────────── Fire ───────────────────────

        /// <summary>
        /// Fires a new projectile.
        /// dirX/dirY/dirZ must be a normalised direction vector.
        /// </summary>
        public ActiveProjectile Fire(ProjectileData data,
                                     float originX, float originY, float originZ,
                                     float dirX,    float dirY,    float dirZ,
                                     string shooterFaction = "player")
        {
            string id = $"proj_{_nextId++}";
            float speed = data.Speed;

            var proj = new ActiveProjectile
            {
                Id             = id,
                Data           = data,
                PosX           = originX, PosY = originY, PosZ = originZ,
                VelX           = dirX * speed,
                VelY           = dirY * speed,
                VelZ           = dirZ * speed,
                LifetimeLeft   = data.Lifetime,
                BouncesLeft    = data.BounceCount,
                ShooterFaction = shooterFaction
            };

            _active[id] = proj;
            Logger.Info($"ProjectileSystem: fired '{data.UniqueId}' (id:{id}) dir=({dirX:F2},{dirY:F2},{dirZ:F2})");
            return proj;
        }

        // ─────────────────────── Update Loop ───────────────────────

        /// <summary>
        /// Advances all projectiles for one frame.
        /// Pass a snapshot of targets for per-frame hit detection.
        /// </summary>
        public void UpdateAll(double delta, IEnumerable<CombatTarget>? targets = null)
        {
            var toRemove = new List<string>();
            float dt = (float)delta;

            foreach (var kv in _active)
            {
                var p = kv.Value;
                if (p.IsExpired) { toRemove.Add(kv.Key); continue; }

                // Gravity
                p.VelY -= p.Data.Gravity * dt;

                // Move
                p.PosX += p.VelX * dt;
                p.PosY += p.VelY * dt;
                p.PosZ += p.VelZ * dt;

                // Homing — rotate velocity toward target (stub: keeps direction for now)
                // Full homing requires a lock-on target ID — hook reserved for Prompt 20+

                // Lifetime
                p.LifetimeLeft -= dt;
                if (p.LifetimeLeft <= 0f)
                {
                    p.IsExpired = true;
                    OnExpired?.Invoke(p);
                    toRemove.Add(kv.Key);
                    continue;
                }

                // Hit detection (if targets provided)
                if (targets != null)
                {
                    var hit = HitDetection.CheckProjectile(p.PosX, p.PosY, p.PosZ,
                                                           targets, p.ShooterFaction);
                    if (hit.DidHit)
                    {
                        OnImpact?.Invoke(p, hit);
                        if (!p.Data.Piercing)
                        {
                            p.IsExpired = true;
                            toRemove.Add(kv.Key);
                        }
                    }
                }
            }

            foreach (var id in toRemove) _active.Remove(id);
        }

        // ─────────────────────── Queries ───────────────────────

        public int ActiveCount => _active.Count;

        public ActiveProjectile? Get(string id) =>
            _active.TryGetValue(id, out var p) ? p : null;

        public void ClearAll()
        {
            _active.Clear();
            Logger.Info("ProjectileSystem: all projectiles cleared.");
        }
    }
}
