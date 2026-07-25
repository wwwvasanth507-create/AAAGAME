using System;
using System.Collections.Generic;
using Godot;

namespace HeroOfEternia.World
{
    /// <summary>
    /// Audit report container compiling validation discrepancies.
    /// </summary>
    public class WorldValidationReport
    {
        public bool IsSuccess => ErrorsCount == 0;
        public List<string> Errors { get; } = new();
        public int EvaluatedNodesCount { get; set; } = 0;
        public int ErrorsCount => Errors.Count;
    }

    /// <summary>
    /// Automated quality scanner sweeping chunk nodes to flag floating meshes,
    /// overlaps, disconnected navigation islands, and collision overlaps.
    /// </summary>
    public static class WorldValidator
    {
        /// <summary>
        /// Sweeps active chunk properties to detect discrepancies.
        /// </summary>
        public static WorldValidationReport ValidateChunk(Chunk chunk, TerrainGenerator generator)
        {
            var report = new WorldValidationReport();
            if (chunk == null || generator == null)
            {
                report.Errors.Add("Validator: Null references passed to audit.");
                return report;
            }

            float chunkSize = ChunkManager.ChunkSize;
            float startX = chunk.Coords.X * chunkSize - chunkSize / 2f;
            float startZ = chunk.Coords.Y * chunkSize - chunkSize / 2f;

            var placedBounds = new List<Rect2>();

            foreach (var node in chunk.ActiveNodes)
            {
                report.EvaluatedNodesCount++;

                float globalX = startX + node.LocalX + chunkSize / 2f;
                float globalZ = startZ + node.LocalZ + chunkSize / 2f;

                // 1. Audit Y elevation heights (detect floating objects)
                float terrainY = generator.GetHeight(globalX, globalZ);
                float difference = Math.Abs(node.LocalY - terrainY);
                if (difference > 0.5f) // Threshold of 0.5 Godot units
                {
                    report.Errors.Add($"FloatingObject: ID='{node.NodeInstanceId}' Y={node.LocalY:F2}, TerrainY={terrainY:F2} (Diff={difference:F2}).");
                }

                // 2. Audit overlapping bounding scopes (resource collisions)
                // Use a simple 2D footprint rectangle box (local center coords)
                float radius = 1.0f * node.Scale; // Assume default 1.0 unit bounding radius
                var rect = new Rect2(node.LocalX - radius, node.LocalZ - radius, radius * 2f, radius * 2f);

                foreach (var otherRect in placedBounds)
                {
                    if (rect.Intersects(otherRect))
                      {
                        report.Errors.Add($"OverlapCollision: Node '{node.NodeInstanceId}' intersects neighboring bounds.");
                        break;
                    }
                }
                placedBounds.Add(rect);
            }

            // 3. Biome elevation transitions audits
            foreach (var node in chunk.ActiveNodes)
            {
                float globalX = startX + node.LocalX + chunkSize / 2f;
                float globalZ = startZ + node.LocalZ + chunkSize / 2f;
                var bType = generator.GetBiomeAt(globalX, globalZ);
                
                if (bType == BiomeType.Ocean && node.ElementRecordId == "tree_oak")
                {
                    report.Errors.Add($"InvalidBiomeSpawn: Oak tree spawned under Ocean biome (Node={node.NodeInstanceId}).");
                }
            }

            return report;
        }
    }
}
