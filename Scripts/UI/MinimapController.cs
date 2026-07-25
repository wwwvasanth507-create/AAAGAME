using Godot;
using System;
using System.Collections.Generic;
using HeroOfEternia.Core;

namespace HeroOfEternia.UI
{
    public partial class MinimapController : Control, IInitializable
    {
        private static MinimapController? _instance;
        public static MinimapController Instance => _instance ??= new MinimapController();

        public float MapScale { get; set; } = 2.0f;
        public Vector2 PlayerWorldPos { get; private set; }
        private readonly List<Vector2> _enemyPositions = new();
        private readonly HashSet<Vector2I> _exploredTiles = new();

        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            GD.Print("[MinimapController] Initialized.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
            _enemyPositions.Clear();
            _exploredTiles.Clear();
        }

        public void UpdatePlayerPosition(Vector3 pos)
        {
            PlayerWorldPos = new Vector2(pos.X, pos.Z);
            RevealFogOfWar(pos);
            QueueRedraw();
        }

        public void UpdateEnemyPositions(List<Vector3> enemies)
        {
            _enemyPositions.Clear();
            foreach (var e in enemies)
            {
                _enemyPositions.Add(new Vector2(e.X, e.Z));
            }
            QueueRedraw();
        }

        private void RevealFogOfWar(Vector3 playerPos)
        {
            int centerTileX = (int)MathF.Floor(playerPos.X / 4f);
            int centerTileZ = (int)MathF.Floor(playerPos.Z / 4f);
            int visionRadius = 5;

            for (int dx = -visionRadius; dx <= visionRadius; dx++)
            {
                for (int dz = -visionRadius; dz <= visionRadius; dz++)
                {
                    if (dx * dx + dz * dz <= visionRadius * visionRadius)
                    {
                        _exploredTiles.Add(new Vector2I(centerTileX + dx, centerTileZ + dz));
                    }
                }
            }
        }

        public bool IsTileExplored(int tileX, int tileZ)
        {
            return _exploredTiles.Contains(new Vector2I(tileX, tileZ));
        }

        public override void _Draw()
        {
            // Minimap background circle
            DrawCircle(Vector2.Zero, 60f, new Color(0, 0, 0, 0.6f));

            // Enemies (red dots)
            foreach (var e in _enemyPositions)
            {
                Vector2 rel = (e - PlayerWorldPos) * MapScale;
                if (rel.Length() <= 55f)
                {
                    DrawCircle(rel, 3f, Colors.Red);
                }
            }

            // Player marker (green arrow/dot in center)
            DrawCircle(Vector2.Zero, 4f, Colors.Green);
        }
    }
}
