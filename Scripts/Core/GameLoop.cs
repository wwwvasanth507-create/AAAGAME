using System;
using Godot;
using HeroOfEternia.Core;
using HeroOfEternia.Enemies;
using HeroOfEternia.Player;

namespace HeroOfEternia.Core
{
    // ----------------------------------------------------------------
    // Game loop events
    // ----------------------------------------------------------------
    public record PlayerDiedEvent();
    public record GameOverEvent(int WavesCompleted, int EnemiesKilled, int TotalXp);
    public record PlayerLeveledUpEvent(int NewLevel, int TotalXp);
    public record XpGainedEvent(int Amount, int Total, int ToNextLevel);
    public record GamePausedEvent(bool IsPaused);

    /// <summary>
    /// GameLoop manages the top-level runtime gameplay session.
    ///
    /// Responsibilities:
    ///   - Session timer (tracks total play time)
    ///   - Wave progression — listens to EnemySpawner events
    ///   - Player death → GameOver state
    ///   - XP + levelling pipeline
    ///   - Pause / resume through GameManager
    ///   - Autosave on wave completion
    /// </summary>
    public partial class GameLoop : Node
    {
        // ----------------------------------------------------------------
        // Configuration
        // ----------------------------------------------------------------
        [Export] public int   BaseXpToLevel    { get; set; } = 100;
        [Export] public float XpScaleFactor    { get; set; } = 1.5f;   // per level
        [Export] public bool  AutosaveOnWave   { get; set; } = true;

        // ----------------------------------------------------------------
        // Runtime state
        // ----------------------------------------------------------------
        public int   PlayerLevel       { get; private set; } = 1;
        public int   PlayerXp          { get; private set; } = 0;
        public int   EnemiesKilled     { get; private set; } = 0;
        public int   WavesCompleted    { get; private set; } = 0;
        public float SessionTimeSec    { get; private set; } = 0f;
        public bool  IsPaused          { get; private set; } = false;
        public bool  IsGameOver        { get; private set; } = false;

        private int XpToNextLevel => (int)(BaseXpToLevel * MathF.Pow(XpScaleFactor, PlayerLevel - 1));

        // ----------------------------------------------------------------
        // Lifecycle
        // ----------------------------------------------------------------
        public override void _Ready()
        {
            // Subscribe to world events
            EventBus.Subscribe<EnemyDiedEvent>(OnEnemyDied);
            EventBus.Subscribe<WaveCompleteEvent>(OnWaveComplete);
            EventBus.Subscribe<AllWavesCompleteEvent>(OnAllWavesComplete);
            EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);

            Logger.Info("GameLoop: Session started.");
        }

        public override void _ExitTree()
        {
            EventBus.Unsubscribe<EnemyDiedEvent>(OnEnemyDied);
            EventBus.Unsubscribe<WaveCompleteEvent>(OnWaveComplete);
            EventBus.Unsubscribe<AllWavesCompleteEvent>(OnAllWavesComplete);
            EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
        }

        public override void _Process(double delta)
        {
            if (IsPaused || IsGameOver) return;
            SessionTimeSec += (float)delta;
        }

        // ----------------------------------------------------------------
        // Pause / resume
        // ----------------------------------------------------------------
        public void Pause()
        {
            if (IsPaused || IsGameOver) return;
            IsPaused = true;
            GetTree().Paused = true;
            EventBus.Publish(new GamePausedEvent(true));
            Logger.Info("GameLoop: Paused.");
        }

        public void Resume()
        {
            if (!IsPaused) return;
            IsPaused = false;
            GetTree().Paused = false;
            EventBus.Publish(new GamePausedEvent(false));
            Logger.Info("GameLoop: Resumed.");
        }

        // ----------------------------------------------------------------
        // XP & Levelling
        // ----------------------------------------------------------------
        public void AwardXp(int amount)
        {
            if (IsGameOver) return;
            PlayerXp += amount;

            int toNext = XpToNextLevel;
            EventBus.Publish(new XpGainedEvent(amount, PlayerXp, toNext));

            // Level up loop
            while (PlayerXp >= XpToNextLevel)
            {
                PlayerXp  -= XpToNextLevel;
                PlayerLevel++;
                Logger.Info($"GameLoop: Level up! Level={PlayerLevel} XP={PlayerXp}/{XpToNextLevel}");
                EventBus.Publish(new PlayerLeveledUpEvent(PlayerLevel, PlayerXp));
            }
        }

        // ----------------------------------------------------------------
        // Event handlers
        // ----------------------------------------------------------------
        private void OnEnemyDied(EnemyDiedEvent e)
        {
            EnemiesKilled++;
            Logger.Info($"GameLoop: Enemy '{e.DisplayName}' killed. Total={EnemiesKilled}");
            AwardXp(e.XpReward);
        }

        private void OnWaveComplete(WaveCompleteEvent e)
        {
            WavesCompleted++;
            Logger.Info($"GameLoop: Wave {e.WaveNumber} complete. Waves done={WavesCompleted}");

            if (AutosaveOnWave)
            {
                Logger.Info("GameLoop: Autosave triggered on wave completion.");
                TriggerAutosave();
            }
        }

        private void OnAllWavesComplete(AllWavesCompleteEvent e)
        {
            Logger.Info($"GameLoop: All {e.TotalWaves} waves cleared! Victory.");
            TriggerAutosave();
            // Could transition to victory screen — handled by SceneManager in future
        }

        private void OnPlayerDied(PlayerDiedEvent _)
        {
            if (IsGameOver) return;
            IsGameOver = true;
            Logger.Info($"GameLoop: Player died. Waves={WavesCompleted} Kills={EnemiesKilled} XP={PlayerXp}");
            EventBus.Publish(new GameOverEvent(WavesCompleted, EnemiesKilled, PlayerXp));
            // Transition to GameOver scene (future Prompt scope)
        }

        // ----------------------------------------------------------------
        // Autosave helper — writes session state to SaveManager slot 0
        // ----------------------------------------------------------------
        private void TriggerAutosave()
        {
            try
            {
                // Resolve SaveManager from service locator
                var sm = ServiceLocator.Get<SaveManager>();
                // Update the active profile with current session stats
                sm.UpdateSessionStats(PlayerLevel, PlayerXp, EnemiesKilled, WavesCompleted);
                sm.Save(0);
                Logger.Info("GameLoop: Autosave complete → slot 0.");
            }
            catch (Exception ex)
            {
                Logger.Error($"GameLoop: Autosave failed: {ex.Message}");
            }
        }

        // ----------------------------------------------------------------
        // State query helpers
        // ----------------------------------------------------------------
        public string GetSessionTimeFormatted()
        {
            int h = (int)(SessionTimeSec / 3600f);
            int m = (int)((SessionTimeSec % 3600f) / 60f);
            int s = (int)(SessionTimeSec % 60f);
            return $"{h:D2}:{m:D2}:{s:D2}";
        }

        public override string ToString() =>
            $"GameLoop[Lv={PlayerLevel} XP={PlayerXp}/{XpToNextLevel} " +
            $"Kills={EnemiesKilled} Waves={WavesCompleted} Time={GetSessionTimeFormatted()}]";
    }
}
