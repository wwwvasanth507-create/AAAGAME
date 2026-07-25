using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.World
{
    public enum ChunkState
    {
        Unloaded,
        Loading,
        Loaded
    }

    /// <summary>
    /// Represets a single spawned item instance inside a chunk (Tree, Ore Vein, etc.).
    /// Coordinates are relative to the chunk center.
    /// </summary>
    public class SpawnedNode
    {
        public string NodeInstanceId { get; set; } = "";
        public string ElementRecordId { get; set; } = "";
        
        // Relative Local coordinate offsets
        public float LocalX { get; set; }
        public float LocalY { get; set; }
        public float LocalZ { get; set; }
        
        public float Scale { get; set; } = 1.0f;
        public float RotationY { get; set; } = 0.0f;
    }

    /// <summary>
    /// Structural model for a 2D world chunk container.
    /// Tracks static nodes and checks mined histories to support state recovery.
    /// </summary>
    public class Chunk
    {
        public Vector2I Coords { get; set; }
        public ChunkState State { get; set; } = ChunkState.Unloaded;
        
        // Spawned nodes active within chunk boundaries
        public List<SpawnedNode> ActiveNodes { get; set; } = new();
        
        // Tracks instance IDs of nodes that were modified/deleted (mined ores, chopped trees)
        public HashSet<string> ModifiedNodeIds { get; set; } = new();

        public Chunk(Vector2I coords)
        {
            Coords = coords;
        }

        /// <summary>
        /// Unique string key lookup format for saving.
        /// </summary>
        public string Key => $"{Coords.X}_{Coords.Y}";

        /// <summary>
        /// Clears memory arrays.
        /// </summary>
        public void Clear()
        {
            ActiveNodes.Clear();
            ModifiedNodeIds.Clear();
            State = ChunkState.Unloaded;
        }
    }
}
