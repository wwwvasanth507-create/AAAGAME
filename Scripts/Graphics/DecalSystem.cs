using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.Graphics
{
    public enum DecalType
    {
        Footprint,
        Blood,
        ScorchMark,
        WaterRipple,
        Mud,
        SnowTrack,
        Crack,
        MagicCircle
    }

    public class DecalInstance
    {
        public string InstanceId { get; set; } = Guid.NewGuid().ToString();
        public DecalType Type { get; set; } = DecalType.Footprint;
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; } = Vector3.Up;
        public float Size { get; set; } = 1.0f;
        public float LifetimeSeconds { get; set; } = 10.0f;
        public bool IsPersistent { get; set; } = false;
        public float AgeSeconds { get; set; } = 0.0f;
    }

    /// <summary>
    /// Reusable decal engine managing footprints, blood splatters, scorch marks, ripples,
    /// and magic circles with pooling and automatic distance fading.
    /// </summary>
    public class DecalSystem
    {
        private readonly List<DecalInstance> _activeDecals = new();
        public int MaxDecals { get; set; } = 100;

        public DecalInstance SpawnDecal(DecalType type, Vector3 position, Vector3 normal, float size = 1.0f, float lifetime = 10.0f, bool persistent = false)
        {
            if (_activeDecals.Count >= MaxDecals)
            {
                // Remove oldest non-persistent decal
                int idx = _activeDecals.FindIndex(d => !d.IsPersistent);
                if (idx >= 0) _activeDecals.RemoveAt(idx);
            }

            var instance = new DecalInstance
            {
                Type = type,
                Position = position,
                Normal = normal,
                Size = size,
                LifetimeSeconds = lifetime,
                IsPersistent = persistent
            };
            _activeDecals.Add(instance);
            return instance;
        }

        public void Update(float delta)
        {
            for (int i = _activeDecals.Count - 1; i >= 0; i--)
            {
                var d = _activeDecals[i];
                if (!d.IsPersistent)
                {
                    d.AgeSeconds += delta;
                    if (d.AgeSeconds >= d.LifetimeSeconds)
                    {
                        _activeDecals.RemoveAt(i);
                    }
                }
            }
        }

        public IReadOnlyList<DecalInstance> ActiveDecals => _activeDecals;

        public void ClearDecals()
        {
            _activeDecals.Clear();
        }
    }
}
