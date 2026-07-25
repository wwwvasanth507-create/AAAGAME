using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.World
{
    /// <summary>
    /// Core streaming service. Handles thread-safe chunk loading/unloading
    /// based on player distance heuristics. Emits delegate hooks for object pools.
    /// </summary>
    public class ChunkManager : IInitializable
    {
        public const float ChunkSize = 32.0f; // Dimension of single square chunk in Godot units
        public const int StreamingDistance = 2; // Loading radius in chunks

        private readonly ConcurrentDictionary<string, Chunk> _chunks = new();
        private readonly HashSet<string> _loadingQueue = new();
        private readonly object _queueLock = new();

        public ulong ActiveSeed { get; set; } = 12345u;

        // Dispatch triggers when chunks change state. Integrated with visual object pools.
        public event Action<Chunk>? OnChunkLoaded;
        public event Action<Chunk>? OnChunkUnloaded;

        private Vector2I _lastPlayerChunk = new(-999, -999);

        public void Initialize()
        {
            _chunks.Clear();
            _loadingQueue.Clear();
            _lastPlayerChunk = new(-999, -999);
            Logger.Info($"ChunkManager: Initialized with World Seed: {WorldSeed.ToShareString(ActiveSeed)}");
        }

        /// <summary>
        /// Scans player coordinates and handles loading/unloading boundaries.
        /// Call periodically from game tick or player coordinate switches.
        /// </summary>
        public void UpdatePlayerPosition(Vector3 playerPos)
        {
            int playerChunkX = (int)Math.Floor(playerPos.X / ChunkSize);
            int playerChunkZ = (int)Math.Floor(playerPos.Z / ChunkSize);
            Vector2I currentPlayerChunk = new Vector2I(playerChunkX, playerChunkZ);

            if (currentPlayerChunk == _lastPlayerChunk) return;

            _lastPlayerChunk = currentPlayerChunk;
            EvaluateChunkStreaming(currentPlayerChunk);
        }

        private void EvaluateChunkStreaming(Vector2I playerChunk)
        {
            var targetKeys = new HashSet<string>();

            // 1. Identify chunks inside load boundaries
            for (int x = -StreamingDistance; x <= StreamingDistance; x++)
            {
                for (int z = -StreamingDistance; z <= StreamingDistance; z++)
                {
                    Vector2I chunkCoords = playerChunk + new Vector2I(x, z);
                    string key = $"{chunkCoords.X}_{chunkCoords.Y}";
                    targetKeys.Add(key);

                    if (!_chunks.ContainsKey(key))
                    {
                        lock (_queueLock)
                        {
                            if (!_loadingQueue.Contains(key))
                            {
                                _loadingQueue.Add(key);
                                Task.Run(() => GenerateChunkAsync(chunkCoords, key));
                            }
                        }
                    }
                }
            }

            // 2. Identify chunks outside unload boundaries (Distance + 1 buffer to prevent loading thrash)
            var toUnload = new List<string>();
            foreach (var key in _chunks.Keys)
            {
                if (!targetKeys.Contains(key))
                {
                    // Split key back to coords
                    var parts = key.Split('_');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int cx) && int.TryParse(parts[1], out int cy))
                    {
                        int dx = Math.Abs(cx - playerChunk.X);
                        int dy = Math.Abs(cy - playerChunk.Y);
                        if (dx > StreamingDistance + 1 || dy > StreamingDistance + 1)
                        {
                            toUnload.Add(key);
                        }
                    }
                }
            }

            // Unload chunks outside boundaries
            foreach (var key in toUnload)
            {
                if (_chunks.TryRemove(key, out var chunk))
                {
                    chunk.Clear();
                    OnChunkUnloaded?.Invoke(chunk);
                }
            }
        }

        private void GenerateChunkAsync(Vector2I coords, string key)
        {
            try
            {
                var chunk = new Chunk(coords);
                chunk.State = ChunkState.Loading;

                // Deterministic seed formulation for this specific chunk
                ulong chunkSeed = ActiveSeed ^ (ulong)coords.X ^ ((ulong)coords.Y << 32);
                
                var rng = new RandomNumberGenerator();
                rng.Seed = chunkSeed;

                // Deterministic generation of 3D entities inside chunk
                int treeCount = rng.RandiRange(2, 6);
                for (int i = 0; i < treeCount; i++)
                {
                    string instanceId = $"{key}_tree_{i}";
                    
                    // Verify if this node has already been mined/modified in active save
                    if (chunk.ModifiedNodeIds.Contains(instanceId)) continue;

                    chunk.ActiveNodes.Add(new SpawnedNode
                    {
                        NodeInstanceId = instanceId,
                        ElementRecordId = "tree_oak",
                        LocalX = rng.RandfRange(-ChunkSize / 2f, ChunkSize / 2f),
                        LocalY = 0f,
                        LocalZ = rng.RandfRange(-ChunkSize / 2f, ChunkSize / 2f),
                        Scale = rng.RandfRange(0.8f, 1.4f),
                        RotationY = rng.RandfRange(0f, Mathf.Tau)
                    });
                }

                int rockCount = rng.RandiRange(1, 3);
                for (int i = 0; i < rockCount; i++)
                {
                    string instanceId = $"{key}_rock_{i}";
                    if (chunk.ModifiedNodeIds.Contains(instanceId)) continue;

                    chunk.ActiveNodes.Add(new SpawnedNode
                    {
                        NodeInstanceId = instanceId,
                        ElementRecordId = "rock_granite",
                        LocalX = rng.RandfRange(-ChunkSize / 2f, ChunkSize / 2f),
                        LocalY = 0f,
                        LocalZ = rng.RandfRange(-ChunkSize / 2f, ChunkSize / 2f),
                        Scale = rng.RandfRange(0.7f, 1.2f),
                        RotationY = rng.RandfRange(0f, Mathf.Tau)
                    });
                }

                chunk.State = ChunkState.Loaded;
                _chunks[key] = chunk;

                // Safe dispatch back to main thread or listener hooks
                OnChunkLoaded?.Invoke(chunk);
            }
            catch (Exception ex)
            {
                Logger.Error($"ChunkManager: Async generation failure for chunk {coords}: {ex.Message}");
            }
            finally
            {
                lock (_queueLock)
                {
                    _loadingQueue.Remove(key);
                }
            }
        }

        /// <summary>
        /// Retrieves loaded chunk reference by coordinate key. Returns null if not loaded.
        /// </summary>
        public Chunk? GetChunk(Vector2I coords)
        {
            string key = $"{coords.X}_{coords.Y}";
            return _chunks.TryGetValue(key, out var chunk) ? chunk : null;
        }

        /// <summary>
        /// Returns a snapshot dictionary of all loaded chunks (useful for saving).
        /// </summary>
        public Dictionary<string, Chunk> GetLoadedChunks()
        {
            return new Dictionary<string, Chunk>(_chunks);
        }

        /// <summary>
        /// Marks a node in a chunk as modified (mined/collected) so it doesn't respawn.
        /// </summary>
        public void ModifyNode(Vector2I chunkCoords, string nodeInstanceId)
        {
            string key = $"{chunkCoords.X}_{chunkCoords.Y}";
            if (_chunks.TryGetValue(key, out var chunk))
            {
                chunk.ModifiedNodeIds.Add(nodeInstanceId);
                chunk.ActiveNodes.RemoveAll(n => n.NodeInstanceId == nodeInstanceId);
            }
        }
    }
}
