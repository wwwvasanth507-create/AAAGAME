using System;

namespace HeroOfEternia.Core
{
    public enum GameState
    {
        None,
        Boot,
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    /// <summary>
    /// GameManager manages the main lifecycle state machine of Hero of Eternia.
    /// Coordinating system initializations and level transitions.
    /// </summary>
    public class GameManager
    {
        public GameState CurrentState { get; private set; } = GameState.None;

        public event Action<GameState>? OnGameStateChanged;

        public void Initialize()
        {
            Logger.Info("GameManager: Initializing project managers...");
            TransitionTo(GameState.Boot);
        }

        public void TransitionTo(GameState newState)
        {
            if (CurrentState == newState) return;

            Logger.Info($"GameManager: State transition from {CurrentState} to {newState}");
            CurrentState = newState;

            OnGameStateChanged?.Invoke(CurrentState);

            switch (CurrentState)
            {
                case GameState.Boot:
                    HandleBoot();
                    break;
                case GameState.MainMenu:
                    HandleMainMenu();
                    break;
                case GameState.Playing:
                    HandlePlaying();
                    break;
                case GameState.Paused:
                    HandlePaused();
                    break;
                case GameState.GameOver:
                    HandleGameOver();
                    break;
            }
        }

        private void HandleBoot()
        {
            Logger.Info("GameManager: Executing boot sequence systems check...");
            // Transitions automatically to MainMenu after boot completes
            TransitionTo(GameState.MainMenu);
        }

        private void HandleMainMenu()
        {
            Logger.Info("GameManager: Main menu screen loaded. Waiting for user start.");
        }

        private void HandlePlaying()
        {
            Logger.Info("GameManager: Gameplay thread active. Core logic executing.");
        }

        private void HandlePaused()
        {
            Logger.Info("GameManager: Gameplay thread paused. Loops frozen.");
        }

        private void HandleGameOver()
        {
            Logger.Info("GameManager: Defeat triggers active. Save database state finalized.");
        }
    }
}
