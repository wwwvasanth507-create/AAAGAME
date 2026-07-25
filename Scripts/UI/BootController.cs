using Godot;
using System.Linq;
using HeroOfEternia.Core;

namespace HeroOfEternia.UI
{
    /// <summary>
    /// BootController replaces the bare TestRunner node on Boot.tscn.
    /// - If '--run-tests' is passed on the command line, it delegates to TestRunner.
    /// - Otherwise it initialises all ServiceLocator services and transitions to MainMenu.
    /// </summary>
    public partial class BootController : Control
    {
        // Service instances — registered globally via ServiceLocator
        private PerformanceManager _performanceManager = null!;
        private SettingsManager    _settingsManager    = null!;
        private LocalizationManager _localizationManager = null!;
        private GameManager        _gameManager        = null!;
        private AudioManager       _audioManager       = null!;
        private SceneManager       _sceneManager       = null!;
        private ResourceManager    _resourceManager    = null!;
        private UIManager          _uiManager          = null!;

        public override void _Ready()
        {
            string[] args = OS.GetCmdlineArgs();

            // Headless test mode — hand control to TestRunner logic
            if (args.Contains("--run-tests"))
            {
                GD.Print("BootController: Test mode detected. Delegating to TestRunner.");
                var runner = new TestRunner();
                AddChild(runner);
                runner._Ready();
                return;
            }

            GD.Print("BootController: Starting full boot sequence...");
            InitialiseServices();
            TransitionToMainMenu();
        }

        // ----------------------------------------------------------------
        // Service initialisation
        // ----------------------------------------------------------------
        private void InitialiseServices()
        {
            ServiceLocator.Clear();

            string userDir = OS.GetUserDataDir();

            _performanceManager  = new PerformanceManager();
            _settingsManager     = new SettingsManager(userDir);
            _localizationManager = new LocalizationManager();
            _gameManager         = new GameManager();
            _audioManager        = new AudioManager();
            _sceneManager        = new SceneManager();
            _resourceManager     = new ResourceManager();
            _uiManager           = new UIManager();

            ServiceLocator.Register(_performanceManager);
            ServiceLocator.Register(_settingsManager);
            ServiceLocator.Register(_localizationManager);
            ServiceLocator.Register(_gameManager);
            ServiceLocator.Register(_audioManager);
            ServiceLocator.Register(_sceneManager);
            ServiceLocator.Register(_resourceManager);
            ServiceLocator.Register(_uiManager);

            // Trigger lazy-init on all registered services
            ServiceLocator.Get<PerformanceManager>();
            ServiceLocator.Get<SettingsManager>();
            ServiceLocator.Get<LocalizationManager>();
            ServiceLocator.Get<GameManager>();
            ServiceLocator.Get<AudioManager>();
            ServiceLocator.Get<SceneManager>();
            ServiceLocator.Get<ResourceManager>();
            ServiceLocator.Get<UIManager>();

            GD.Print("BootController: All services initialised successfully.");
        }

        // ----------------------------------------------------------------
        // Scene transition — Boot → MainMenu
        // ----------------------------------------------------------------
        private void TransitionToMainMenu()
        {
            GD.Print("BootController: Transitioning to MainMenu.");
            // Use direct scene change (no loading screen for the first transition)
            GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
        }
    }
}
