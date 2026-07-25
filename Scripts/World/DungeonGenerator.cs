using Godot;
using System;
using System.Collections.Generic;

namespace HeroOfEternia.World
{
    public struct DungeonRoom
    {
        public int X;
        public int Z;
        public int Width;
        public int Depth;

        public DungeonRoom(int x, int z, int w, int d)
        {
            X = x;
            Z = z;
            Width = w;
            Depth = d;
        }

        public Vector3 Center => new Vector3(X + Width / 2f, 0, Z + Depth / 2f);
    }

    public enum TileType
    {
        Wall = 0,
        Floor = 1,
        Door = 2,
        Trap = 3,
        StairsDown = 4
    }

    public class DungeonGenerator
    {
        public int Width { get; private set; }
        public int Depth { get; private set; }
        public TileType[,] Grid { get; private set; }
        public List<DungeonRoom> Rooms { get; private set; } = new();

        public DungeonGenerator(int width = 64, int depth = 64)
        {
            Width = width;
            Depth = depth;
            Grid = new TileType[Width, Depth];
        }

        public void GenerateDungeon(int seed, int roomCount = 8, int minRoomSize = 6, int maxRoomSize = 14)
        {
            var rng = new Random(seed);
            Rooms.Clear();
            Array.Clear(Grid, 0, Grid.Length);

            // BSP Partitioning for room placement
            for (int i = 0; i < roomCount; i++)
            {
                int w = rng.Next(minRoomSize, maxRoomSize + 1);
                int d = rng.Next(minRoomSize, maxRoomSize + 1);
                int x = rng.Next(2, Width - w - 2);
                int z = rng.Next(2, Depth - d - 2);

                var newRoom = new DungeonRoom(x, z, w, d);
                bool overlaps = false;

                foreach (var r in Rooms)
                {
                    if (x < r.X + r.Width + 2 && x + w + 2 > r.X &&
                        z < r.Z + r.Depth + 2 && z + d + 2 > r.Z)
                    {
                        overlaps = true;
                        break;
                    }
                }

                if (!overlaps)
                {
                    Rooms.Add(newRoom);
                    CarveRoom(newRoom);
                }
            }

            // Connect adjacent rooms with corridors
            for (int i = 0; i < Rooms.Count - 1; i++)
            {
                CarveCorridor(Rooms[i].Center, Rooms[i + 1].Center);
            }

            // Add stairs down in last room
            if (Rooms.Count > 0)
            {
                var lastRoom = Rooms[^1];
                Grid[(int)lastRoom.Center.X, (int)lastRoom.Center.Z] = TileType.StairsDown;
            }

            GD.Print($"[DungeonGenerator] Generated dungeon floor with {Rooms.Count} rooms (Seed: {seed}).");
        }

        private void CarveRoom(DungeonRoom r)
        {
            for (int x = r.X; x < r.X + r.Width; x++)
            {
                for (int z = r.Z; z < r.Z + r.Depth; z++)
                {
                    Grid[x, z] = TileType.Floor;
                }
            }
        }

        private void CarveCorridor(Vector3 start, Vector3 end)
        {
            int x = (int)start.X;
            int z = (int)start.Z;
            int targetX = (int)end.X;
            int targetZ = (int)end.Z;

            while (x != targetX)
            {
                Grid[x, z] = TileType.Floor;
                x += Math.Sign(targetX - x);
            }

            while (z != targetZ)
            {
                Grid[x, z] = TileType.Floor;
                z += Math.Sign(targetZ - z);
            }
        }
    }
}
