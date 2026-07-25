using System;
using Godot;

namespace HeroOfEternia.World
{
    /// <summary>
    /// Headless-safe data-driven navigation mesh utility.
    /// Determines cell-by-cell walkability based on terrain slope calculations and water elevations,
    /// avoiding expensive runtime graphical scene-tree NavMesh baking.
    /// </summary>
    public static class NavigationFoundation
    {
        public const float MaxWalkableSlopeDegrees = 30.0f;
        public const float WaterHeightThreshold = -2.5f;

        /// <summary>
        /// Evaluates if a single height and slope configuration is walkable by standard units.
        /// </summary>
        public static bool IsWalkable(float height, float slopeAngleDegrees)
        {
            if (height < WaterHeightThreshold) return false; // Water boundary restriction
            if (slopeAngleDegrees > MaxWalkableSlopeDegrees) return false; // Slope restriction
            return true;
        }

        /// <summary>
        /// Generates a boolean walkability matrix grid for a specific 2D chunk coordinate region.
        /// Resolution maps cells density (e.g. 16x16 grid).
        /// </summary>
        public static bool[,] GenerateNavigationGrid(TerrainGenerator generator, Vector2I chunkCoords, int resolution = 16)
        {
            bool[,] grid = new bool[resolution, resolution];
            float chunkSize = ChunkManager.ChunkSize;
            float cellSize = chunkSize / resolution;

            float startX = chunkCoords.X * chunkSize - chunkSize / 2f;
            float startZ = chunkCoords.Y * chunkSize - chunkSize / 2f;

            for (int x = 0; x < resolution; x++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    float cellCenterX = startX + x * cellSize + cellSize / 2f;
                    float cellCenterZ = startZ + z * cellSize + cellSize / 2f;

                    // Fetch center height
                    float centerHeight = generator.GetHeight(cellCenterX, cellCenterZ);

                    // Fetch neighbor heights to calculate slope angle
                    float northHeight = generator.GetHeight(cellCenterX, cellCenterZ - 1f);
                    float eastHeight = generator.GetHeight(cellCenterX + 1f, cellCenterZ);

                    // Slope derivatives
                    float dzdx = eastHeight - centerHeight;
                    float dzdy = northHeight - centerHeight;

                    // Slope angle in degrees
                    float slopeAngleDegrees = (float)(Math.Atan(Math.Sqrt(dzdx * dzdx + dzdy * dzdy)) * (180.0 / Math.PI));

                    grid[x, z] = IsWalkable(centerHeight, slopeAngleDegrees);
                }
            }

            return grid;
        }
    }
}
