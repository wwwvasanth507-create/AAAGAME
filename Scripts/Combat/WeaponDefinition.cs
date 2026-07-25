using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using HeroOfEternia.Core;

namespace HeroOfEternia.Combat
{
    /// <summary>
    /// Complete data definition for a weapon.
    /// Every field is JSON-serializable and loaded from weapons_config.json.
    /// </summary>
    public class WeaponData
    {
        public string     UniqueId           { get; set; } = "";
        public string     DisplayName        { get; set; } = "";
        public WeaponType Type               { get; set; } = WeaponType.Sword;
        public float      AttackSpeed        { get; set; } = 1.0f;   // attacks per second
        public float      Range              { get; set; } = 2.0f;   // metres
        public float      BaseDamage         { get; set; } = 10f;
        public DamageType DamageType         { get; set; } = DamageType.Physical;
        public string     AnimationProfileKey { get; set; } = "anim_sword";
        public string     AudioHookKey       { get; set; } = "sfx_sword_swing";
        public string     VfxHookKey         { get; set; } = "vfx_slash";
        public int        DurabilityMax      { get; set; } = 100;      // Hook — durability not implemented
        public float      CritChanceBonus    { get; set; } = 0f;
        public float      CritMultiplierBonus{ get; set; } = 0f;
        public Dictionary<string, float> StatModifiers { get; set; } = new(); // "Strength" → delta
        public string     UpgradeDataHook    { get; set; } = "";       // Future upgrade reference
        public bool       IsDualWieldable    { get; set; } = false;
        public bool       IsProjectile       { get; set; } = false;    // Bow, Crossbow, Wand
    }

    /// <summary>
    /// In-memory weapon registry. Loads from weapons_config.json via ConfigManager.
    /// Provides O(1) lookup by UniqueId.
    /// </summary>
    public class WeaponDatabase
    {
        private readonly Dictionary<string, WeaponData> _weapons = new();

        public WeaponDatabase() { RegisterDefaults(); }

        // ─────────────────────── Registration ───────────────────────

        public void Register(WeaponData weapon)
        {
            _weapons[weapon.UniqueId] = weapon;
        }

        /// <summary>
        /// Loads weapons from JSON array string (from ConfigManager).
        /// Merges into existing registry — does not clear defaults.
        /// </summary>
        public void LoadFromJson(string json)
        {
            try
            {
                var list = JsonSerializer.Deserialize<List<WeaponData>>(json);
                if (list == null) return;
                foreach (var w in list) Register(w);
                Logger.Info($"WeaponDatabase: loaded {list.Count} weapons from JSON.");
            }
            catch (Exception ex)
            {
                Logger.Error($"WeaponDatabase: JSON load failed: {ex.Message}");
            }
        }

        // ─────────────────────── Queries ───────────────────────

        public WeaponData? Get(string uniqueId) =>
            _weapons.TryGetValue(uniqueId, out var w) ? w : null;

        public WeaponData GetOrDefault(string uniqueId) =>
            _weapons.TryGetValue(uniqueId, out var w) ? w : _weapons["wpn_unarmed"];

        public IReadOnlyDictionary<string, WeaponData> All => _weapons;

        // ─────────────────────── Built-in defaults ───────────────────────

        private void RegisterDefaults()
        {
            Register(new WeaponData { UniqueId = "wpn_unarmed",    DisplayName = "Fists",          Type = WeaponType.Unarmed,    BaseDamage = 3f,   AttackSpeed = 2.0f, Range = 1.5f,  DamageType = DamageType.Physical });
            Register(new WeaponData { UniqueId = "wpn_sword",      DisplayName = "Iron Sword",     Type = WeaponType.Sword,      BaseDamage = 15f,  AttackSpeed = 1.4f, Range = 2.0f,  DamageType = DamageType.Physical, AnimationProfileKey = "anim_sword",     AudioHookKey = "sfx_sword_swing",  VfxHookKey = "vfx_slash"     });
            Register(new WeaponData { UniqueId = "wpn_greatsword", DisplayName = "Great Sword",    Type = WeaponType.GreatSword, BaseDamage = 28f,  AttackSpeed = 0.7f, Range = 2.8f,  DamageType = DamageType.Physical, AnimationProfileKey = "anim_greatsword",AudioHookKey = "sfx_greatsword",   VfxHookKey = "vfx_heavy_slash" });
            Register(new WeaponData { UniqueId = "wpn_axe",        DisplayName = "Hand Axe",      Type = WeaponType.Axe,        BaseDamage = 18f,  AttackSpeed = 1.1f, Range = 1.9f,  DamageType = DamageType.Physical, AnimationProfileKey = "anim_axe",       AudioHookKey = "sfx_axe_swing",    VfxHookKey = "vfx_slash"     });
            Register(new WeaponData { UniqueId = "wpn_hammer",     DisplayName = "War Hammer",     Type = WeaponType.Hammer,     BaseDamage = 25f,  AttackSpeed = 0.8f, Range = 2.2f,  DamageType = DamageType.Physical, AnimationProfileKey = "anim_hammer",    AudioHookKey = "sfx_hammer_swing", VfxHookKey = "vfx_blunt"     });
            Register(new WeaponData { UniqueId = "wpn_dagger",     DisplayName = "Steel Dagger",   Type = WeaponType.Dagger,     BaseDamage = 9f,   AttackSpeed = 2.2f, Range = 1.2f,  DamageType = DamageType.Physical, AnimationProfileKey = "anim_dagger",    AudioHookKey = "sfx_dagger",       VfxHookKey = "vfx_pierce",   CritChanceBonus = 0.08f });
            Register(new WeaponData { UniqueId = "wpn_spear",      DisplayName = "Iron Spear",     Type = WeaponType.Spear,      BaseDamage = 20f,  AttackSpeed = 1.0f, Range = 3.5f,  DamageType = DamageType.Physical, AnimationProfileKey = "anim_spear",     AudioHookKey = "sfx_spear",        VfxHookKey = "vfx_pierce"    });
            Register(new WeaponData { UniqueId = "wpn_bow",        DisplayName = "Wooden Bow",     Type = WeaponType.Bow,        BaseDamage = 12f,  AttackSpeed = 0.9f, Range = 30f,   DamageType = DamageType.Physical, AnimationProfileKey = "anim_bow",       AudioHookKey = "sfx_bowshot",      VfxHookKey = "vfx_arrow",    IsProjectile = true });
            Register(new WeaponData { UniqueId = "wpn_crossbow",   DisplayName = "Crossbow",       Type = WeaponType.Crossbow,   BaseDamage = 18f,  AttackSpeed = 0.6f, Range = 35f,   DamageType = DamageType.Physical, AnimationProfileKey = "anim_crossbow",  AudioHookKey = "sfx_crossbow",     VfxHookKey = "vfx_bolt",     IsProjectile = true });
            Register(new WeaponData { UniqueId = "wpn_staff",      DisplayName = "Wooden Staff",   Type = WeaponType.Staff,      BaseDamage = 8f,   AttackSpeed = 1.0f, Range = 20f,   DamageType = DamageType.Fire,     AnimationProfileKey = "anim_staff",     AudioHookKey = "sfx_magic_cast",   VfxHookKey = "vfx_fireball", IsProjectile = true });
            Register(new WeaponData { UniqueId = "wpn_wand",       DisplayName = "Magic Wand",     Type = WeaponType.Wand,       BaseDamage = 6f,   AttackSpeed = 1.8f, Range = 18f,   DamageType = DamageType.Lightning,AnimationProfileKey = "anim_wand",      AudioHookKey = "sfx_wand_cast",    VfxHookKey = "vfx_bolt",     IsProjectile = true });
            Register(new WeaponData { UniqueId = "wpn_shield",     DisplayName = "Wooden Shield",  Type = WeaponType.Shield,     BaseDamage = 5f,   AttackSpeed = 0.8f, Range = 1.2f,  DamageType = DamageType.Physical, AnimationProfileKey = "anim_shield",    AudioHookKey = "sfx_shield_bash",  VfxHookKey = "vfx_bash"      });
        }
    }
}
