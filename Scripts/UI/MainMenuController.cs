using Godot;
using HeroOfEternia.Core;

namespace HeroOfEternia.UI
{
    /// <summary>
    /// MainMenuController drives the wired Main Menu scene.
    /// Handles Play / Settings / Quit button presses and delegates
    /// scene navigation to SceneManager.
    /// </summary>
    public partial class MainMenuController : Control
    {
        // Node references — matched by name from MainMenu.tscn
        [Export] public Button? PlayButton     { get; set; }
        [Export] public Button? SettingsButton { get; set; }
        [Export] public Button? QuitButton     { get; set; }
        [Export] public Label?  TitleLabel     { get; set; }
        [Export] public Label?  VersionLabel   { get; set; }

        public override void _Ready()
        {
            // Wire button signals
            if (PlayButton     != null) PlayButton.Pressed     += OnPlayPressed;
            if (SettingsButton != null) SettingsButton.Pressed += OnSettingsPressed;
            if (QuitButton     != null) QuitButton.Pressed     += OnQuitPressed;

            if (TitleLabel   != null) TitleLabel.Text   = "HERO OF ETERNIA";
            if (VersionLabel != null) VersionLabel.Text = "v0.11.0 — Prototype";

            GD.Print("MainMenuController: Main menu ready.");
        }

        // ----------------------------------------------------------------
        // Button handlers
        // ----------------------------------------------------------------
        private void OnPlayPressed()
        {
            GD.Print("MainMenuController: Play pressed — loading GameWorld...");
            GetTree().ChangeSceneToFile("res://Scenes/GameWorld.tscn");
        }

        private void OnSettingsPressed()
        {
            GD.Print("MainMenuController: Settings pressed — loading Settings...");
            GetTree().ChangeSceneToFile("res://Scenes/Settings.tscn");
        }

        private void OnQuitPressed()
        {
            GD.Print("MainMenuController: Quit pressed — shutting down.");
            GetTree().Quit();
        }
    }
}
