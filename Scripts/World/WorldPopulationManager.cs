using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.World
{
    public enum LandmarkType
    {
        Village,
        EnemyCamp,
        Ruins,
        Shrine,
        TreasureLocation,
        BossArena,
        WatchTower
    }

    /// <summary>
    /// Represets a generated structural placement node reference.
    /// Data-only reference; does not load active graphical node meshes.
    /// </summary>
    public class PopulatedLandmark
    {
        public string UniqueId { get; set; } = "";
        public LandmarkType Type { get; set; }
        public float GlobalX { get; set; }
        public float GlobalZ { get; set; }
        public float ElevationY { get; set; }
        public string Description { get; set; } = "";
    }

    /// <summary>
    /// Evaluates terrain flats and distributes villages, temples, and camps deterministically.
    /// </summary>
    public class WorldPopulationManager
    {
        private readonly List<PopulatedLandmark> _landmarks = new();
        public ulong Seed { get; private set; }

        public WorldPopulationManager(ulong seed)
        {
            Seed = seed;
        }

        /// <summary>
        /// Populates landmark nodes list deterministically.
        /// </summary>
        public void GenerateLandmarks(TerrainGenerator generator, int radiusChunks = 10)
        {
            _landmarks.Clear();
            
            var rng = new RandomNumberGenerator();
            rng.Seed = Seed;

            float chunkSize = ChunkManager.ChunkSize;
            float worldHalfSize = radiusChunks * chunkSize;

            // Generate 1 boss arena at center
            _landmarks.Add(new PopulatedLandmark
            {
                UniqueId = "landmark_arena_central",
                Type = LandmarkType.BossArena,
                GlobalX = 0f,
                GlobalZ = 0f,
                ElevationY = generator.GetHeight(0f, 0f),
                Description = "Ancient Arena of Eternia"
            });

            // Distribute 8 landmarks around coordinates
            for (int i = 0; i < 8; i++)
            {
                float rx = rng.RandfRange(-worldHalfSize, worldHalfSize);
                float rz = rng.RandfRange(-worldHalfSize, worldHalfSize);
                float y = generator.GetHeight(rx, rz);

                LandmarkType lType = (LandmarkType)rng.RandiRange(0, Enum.GetNames(typeof(LandmarkType)).Length - 1);
                
                // Keep villages on flat plateaus or plains
                if (lType == LandmarkType.Village && (y < 2.0f || y > 15f))
                {
                    // Relocate to a flatter height
                    y = 5.0f; 
                }

                _landmarks.Add(new PopulatedLandmark
                {
                    UniqueId = $"landmark_{lType.ToString().ToLower()}_{i}",
                    Type = lType,
                    GlobalX = rx,
                    GlobalZ = rz,
                    ElevationY = y,
                    Description = $"Procedural {lType} ruins structures."
                });
            }
        }

        public List<PopulatedLandmark> GetAllLandmarks()
        {
            return new List<PopulatedLandmark>(_landmarks);
        }

        public PopulatedLandmark? GetLandmark(string uniqueId)
        {
            return _landmarks.Find(l => l.UniqueId == uniqueId);
        }
    }
}
