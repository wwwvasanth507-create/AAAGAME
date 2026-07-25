using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.Combat
{
    /// <summary>
    /// Per-attacker cooldown tracker entry.
    /// </summary>
    internal class CooldownEntry
    {
        public string WeaponId  { get; set; } = "";
        public double Remaining { get; set; } = 0.0;
    }

    /// <summary>
    /// Central combat orchestration service.
    /// Registered in ServiceLocator as "CombatManager".
    ///
    /// Owns: TargetingSystem, StatusEffectSystem, ProjectileSystem, WeaponDatabase.
    /// All results are broadcast as CombatEvent records through the EventBus.
    /// No direct coupling to enemy AI, quests, or rendering.
    /// </summary>
    public class CombatManager
    {
        // ─────────────────────── Sub-systems ───────────────────────
        public TargetingSystem    Targeting       { get; } = new();
        public StatusEffectSystem StatusEffects   { get; } = new();
        public ProjectileSystem   Projectiles     { get; } = new();
        public WeaponDatabase     Weapons         { get; } = new();

        // ─────────────────────── Internal state ───────────────────────
        // attackerId → list of cooldown entries
        private readonly Dictionary<string, List<CooldownEntry>> _cooldowns = new();

        // Per-entity resistance profiles (set by entity registration)
        private readonly Dictionary<string, ResistanceProfile> _resistances = new();

        // Per-entity current health / max health (lightweight data mirrors)
        private readonly Dictionary<string, (float current, float max)> _health = new();

        private readonly Random _rng = new();

        // ─────────────────────── Events ───────────────────────
        public event Action<CombatEvent>? OnCombatEvent;

        public CombatManager()
        {
            // Wire projectile impacts → damage pipeline
            Projectiles.OnImpact += (proj, hit) =>
            {
                var dmg = new DamageInstance
                {
                    AttackerId     = proj.Data.Damage.AttackerId,
                    TargetId       = hit.TargetId,
                    BaseDamage     = proj.Data.Damage.BaseDamage,
                    Type           = proj.Data.Damage.Type,
                    CritChance     = proj.Data.Damage.CritChance,
                    CritMultiplier = proj.Data.Damage.CritMultiplier,
                    SourceWeaponId = proj.Data.Damage.SourceWeaponId
                };
                ApplyDamageToTarget(dmg);
                FireEvent(new CombatEvent
                {
                    Type      = CombatEventType.ProjectileImpact,
                    ActorId   = dmg.AttackerId,
                    TargetId  = hit.TargetId,
                    Value     = dmg.BaseDamage,
                    MetaTag   = proj.Data.UniqueId
                });
            };
        }

        // ─────────────────────── Entity Registration ───────────────────────

        public void RegisterEntity(string entityId, float maxHealth,
                                   ResistanceProfile? resistances = null)
        {
            _health[entityId]      = (maxHealth, maxHealth);
            _resistances[entityId] = resistances ?? ResistanceProfile.Default();
            Logger.Info($"CombatManager: registered entity '{entityId}' (HP:{maxHealth})");
        }

        public void UnregisterEntity(string entityId)
        {
            _health.Remove(entityId);
            _resistances.Remove(entityId);
            _cooldowns.Remove(entityId);
            StatusEffects.RemoveAll(entityId);
        }

        public float GetHealth(string entityId) =>
            _health.TryGetValue(entityId, out var h) ? h.current : 0f;

        public bool IsAlive(string entityId) => GetHealth(entityId) > 0f;

        // ─────────────────────── Attack Execution ───────────────────────

        /// <summary>
        /// Executes a melee attack:
        /// 1. Cooldown check
        /// 2. Build hit volume from weapon range
        /// 3. Hit detection
        /// 4. Damage processing
        /// 5. Status effect application
        /// 6. CombatEvent broadcast
        /// </summary>
        public List<HitResult> ExecuteAttack(string attackerId, string weaponId,
                                              float attackerX, float attackerY, float attackerZ,
                                              bool isHeavy = false)
        {
            var weapon = Weapons.GetOrDefault(weaponId);
            if (!CheckCooldown(attackerId, weaponId, isHeavy ? weapon.AttackSpeed * 1.8f : weapon.AttackSpeed))
            {
                Logger.Info($"CombatManager: '{attackerId}' attack blocked by cooldown.");
                return new List<HitResult>();
            }

            StartCooldown(attackerId, weaponId, 1f / weapon.AttackSpeed);

            FireEvent(new CombatEvent
            {
                Type    = CombatEventType.AttackStarted,
                ActorId = attackerId,
                MetaTag = weaponId
            });

            // Build attack volume
            var volume = HitVolume.MakeSphere(attackerId,
                attackerX, attackerY, attackerZ,
                weapon.Range, "player");  // "player" = friendly faction, won't hit self

            // Gather valid targets
            var candidates = GatherTargets(attackerId);
            var hits       = HitDetection.CheckMelee(volume, candidates);

            foreach (var hit in hits)
            {
                var dmg = new DamageInstance
                {
                    AttackerId     = attackerId,
                    TargetId       = hit.TargetId,
                    BaseDamage     = weapon.BaseDamage * (isHeavy ? 1.6f : 1f),
                    Type           = weapon.DamageType,
                    CritChance     = 0.05f + weapon.CritChanceBonus,
                    CritMultiplier = 1.5f + weapon.CritMultiplierBonus,
                    SourceWeaponId = weaponId
                };

                ApplyDamageToTarget(dmg);

                FireEvent(new CombatEvent
                {
                    Type     = CombatEventType.HitLanded,
                    ActorId  = attackerId,
                    TargetId = hit.TargetId,
                    Value    = dmg.BaseDamage,
                    MetaTag  = weaponId
                });
            }

            if (hits.Count == 0)
            {
                FireEvent(new CombatEvent
                {
                    Type    = CombatEventType.HitMissed,
                    ActorId = attackerId,
                    MetaTag = weaponId
                });
            }

            return hits;
        }

        /// <summary>
        /// Fires a projectile from a ranged weapon.
        /// </summary>
        public ActiveProjectile FireProjectile(string shooterId, string weaponId,
                                               float ox, float oy, float oz,
                                               float dx, float dy, float dz)
        {
            var weapon = Weapons.GetOrDefault(weaponId);
            var projData = new ProjectileData
            {
                UniqueId     = $"{weaponId}_proj",
                Speed        = weapon.Range * 3f,   // scale speed from range
                Lifetime     = 5f,
                Damage       = new DamageInstance
                {
                    AttackerId     = shooterId,
                    BaseDamage     = weapon.BaseDamage,
                    Type           = weapon.DamageType,
                    CritChance     = 0.05f + weapon.CritChanceBonus,
                    CritMultiplier = 1.5f + weapon.CritMultiplierBonus,
                    SourceWeaponId = weaponId
                },
                VfxHookKey   = weapon.VfxHookKey,
                ImpactVfxKey = weapon.VfxHookKey + "_impact"
            };

            var proj = Projectiles.Fire(projData, ox, oy, oz, dx, dy, dz);
            FireEvent(new CombatEvent
            {
                Type    = CombatEventType.ProjectileFired,
                ActorId = shooterId,
                MetaTag = weaponId
            });
            return proj;
        }

        // ─────────────────────── Update ───────────────────────

        /// <summary>
        /// Call every frame. Ticks: cooldowns, status effects, projectiles.
        /// </summary>
        public void Update(double delta, IEnumerable<CombatTarget>? targetSnapshot = null)
        {
            // Tick cooldowns
            foreach (var entries in _cooldowns.Values)
                foreach (var e in entries)
                    e.Remaining = Math.Max(0, e.Remaining - delta);

            // Tick status effects → collect tick damages
            var ticks = StatusEffects.Tick(delta);
            foreach (var (entityId, damage, dmgType) in ticks)
            {
                var dmg = new DamageInstance
                {
                    AttackerId = "StatusEffect",
                    TargetId   = entityId,
                    BaseDamage = damage,
                    Type       = dmgType
                };
                ApplyDamageToTarget(dmg);
            }

            // Tick projectiles
            Projectiles.UpdateAll(delta, targetSnapshot);
        }

        // ─────────────────────── Apply Damage ───────────────────────

        private void ApplyDamageToTarget(DamageInstance dmg)
        {
            if (!_health.TryGetValue(dmg.TargetId, out var hp)) return;
            var res = _resistances.TryGetValue(dmg.TargetId, out var r)
                ? r : ResistanceProfile.Default();

            float processed = DamageSystem.ProcessDamage(dmg, res, _rng);
            float newHp     = DamageSystem.ApplyToHealth(hp.current, hp.max, processed);
            _health[dmg.TargetId] = (newHp, hp.max);

            FireEvent(new CombatEvent
            {
                Type     = processed < 0 ? CombatEventType.DamageReceived : CombatEventType.DamageDealt,
                ActorId  = dmg.AttackerId,
                TargetId = dmg.TargetId,
                Value    = processed,
                MetaTag  = dmg.IsCritical ? "critical" : ""
            });

            if (dmg.IsCritical)
                FireEvent(new CombatEvent { Type = CombatEventType.CriticalHit, ActorId = dmg.AttackerId, TargetId = dmg.TargetId, Value = processed });

            if (DamageSystem.IsLethal(hp.current, processed))
            {
                FireEvent(new CombatEvent { Type = CombatEventType.EntityDied, TargetId = dmg.TargetId, ActorId = dmg.AttackerId });
                Logger.Info($"CombatManager: '{dmg.TargetId}' killed by '{dmg.AttackerId}'");
            }
        }

        // ─────────────────────── Cooldowns ───────────────────────

        private bool CheckCooldown(string attackerId, string weaponId, float attackSpeed)
        {
            if (!_cooldowns.TryGetValue(attackerId, out var list)) return true;
            foreach (var e in list)
                if (e.WeaponId == weaponId && e.Remaining > 0) return false;
            return true;
        }

        private void StartCooldown(string attackerId, string weaponId, float durationSeconds)
        {
            if (!_cooldowns.TryGetValue(attackerId, out var list))
            {
                list = new List<CooldownEntry>();
                _cooldowns[attackerId] = list;
            }
            foreach (var e in list)
            {
                if (e.WeaponId == weaponId) { e.Remaining = durationSeconds; return; }
            }
            list.Add(new CooldownEntry { WeaponId = weaponId, Remaining = durationSeconds });
        }

        // ─────────────────────── Helpers ───────────────────────

        private List<CombatTarget> GatherTargets(string attackerId)
        {
            // Returns all registered CombatTargets from TargetingSystem
            var list = new List<CombatTarget>();
            foreach (var kv in Targeting.All)
                if (kv.Key != attackerId) list.Add(kv.Value);
            return list;
        }

        private void FireEvent(CombatEvent evt)
        {
            evt.Timestamp = Environment.TickCount64 / 1000.0;
            OnCombatEvent?.Invoke(evt);
        }
    }
}
