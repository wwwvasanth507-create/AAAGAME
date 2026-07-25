using System;
using Godot;

namespace HeroOfEternia.Graphics
{
    public enum ParticleType
    {
        Dust,
        Smoke,
        Fire,
        Magic,
        WaterSplash,
        RainSplash,
        Snow,
        Leaves,
        Sand,
        Spark,
        Explosion,
        Healing,
        Buff,
        Debuff,
        Environmental,
        Custom
    }

    public enum VFXPriority
    {
        Low = 0,
        Medium = 10,
        High = 20,
        Critical = 30
    }

    /// <summary>
    /// Data-driven configuration model for particle effect presets.
    /// </summary>
    public class ParticleEffectConfig
    {
        public string EffectId { get; set; } = string.Empty;
        public ParticleType Type { get; set; } = ParticleType.Custom;
        public float LifetimeSeconds { get; set; } = 2.0f;
        public int EmissionAmount { get; set; } = 20;
        public Color BaseColor { get; set; } = Colors.White;
        public float StartScale { get; set; } = 1.0f;
        public float EndScale { get; set; } = 0.0f;
        public Vector3 InitialVelocity { get; set; } = Vector3.Zero;
        public VFXPriority Priority { get; set; } = VFXPriority.Medium;
        public bool Loop { get; set; } = false;
        public float MaxDistanceLOD { get; set; } = 50.0f;
    }
}
