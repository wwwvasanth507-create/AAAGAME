using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.World.Content
{
    public enum DecorationType
    {
        Tree,
        Rock,
        Flower,
        Grass,
        FallenLog,
        SmallRuins,
        Statue,
        Sign,
        Campfire,
        RoadDetail
    }

    public class DecorationSpawn
    {
        public DecorationType Type { get; set; } = DecorationType.Tree;
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; } = Vector3.One;
        public float RotationY { get; set; } = 0.0f;
    }

    /// <summary>
    /// Procedural decoration generator placing trees, rocks, flora, signs, and props
    /// with non-repetitive seeded distribution.
    /// </summary>
    public class WorldDecorationSystem
    {
        public List<DecorationSpawn> GenerateChunkDecorations(int chunkX, int chunkZ, int seed, float vegetationDensity = 1.0f)
        {
            var spawns = new List<DecorationSpawn>();
            var random = new Random(HashCode.Combine(seed, chunkX, chunkZ));

            int count = (int)(20 * Math.Clamp(vegetationDensity, 0.1f, 3.0f));
            for (int i = 0; i < count; i++)
            {
                float x = (float)(random.NextDouble() * 64.0 - 32.0) + (chunkX * 64);
                float z = (float)(random.NextDouble() * 64.0 - 32.0) + (chunkZ * 64);
                DecorationType type = (DecorationType)random.Next(0, 10);

                spawns.Add(new DecorationSpawn
                {
                    Type = type,
                    Position = new Vector3(x, 0, z),
                    RotationY = (float)(random.NextDouble() * Math.PI * 2),
                    Scale = Vector3.One * (float)(0.8 + random.NextDouble() * 0.4)
                });
            }
            return spawns;
        }
    }
}
