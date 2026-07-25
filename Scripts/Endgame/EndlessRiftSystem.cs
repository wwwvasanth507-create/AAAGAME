using Godot;
using System;
using HeroOfEternia.Core;

namespace HeroOfEternia.Endgame
{
    public class EndlessRiftSystem : IInitializable
    {
        private static EndlessRiftSystem? _instance;
        public static EndlessRiftSystem Instance => _instance ??= new EndlessRiftSystem();

        public int CurrentRiftFloor { get; private set; } = 1;
        public int HighestRiftFloor { get; private set; } = 1;
        public float EnemyDamageMultiplier => 1.0f + (CurrentRiftFloor - 1) * 0.15f;
        public float EnemyHealthMultiplier => 1.0f + (CurrentRiftFloor - 1) * 0.25f;

        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;
            GD.Print("[EndlessRiftSystem] Initialized.");
        }

        public void Shutdown()
        {
            _isInitialized = false;
        }

        public void StartRiftFloor(int floor)
        {
            CurrentRiftFloor = floor;
            EventBus.Publish(CurrentRiftFloor);
            GD.Print($"[EndlessRiftSystem] Entering Endless Rift Floor {CurrentRiftFloor}");
        }

        public void ClearCurrentFloor()
        {
            EventBus.Publish(CurrentRiftFloor);
            if (CurrentRiftFloor > HighestRiftFloor)
            {
                HighestRiftFloor = CurrentRiftFloor;
                EventBus.Publish(HighestRiftFloor);
            }
            CurrentRiftFloor++;
            GD.Print($"[EndlessRiftSystem] Cleared Floor {CurrentRiftFloor - 1}! Next: Floor {CurrentRiftFloor}");
        }
    }
}
