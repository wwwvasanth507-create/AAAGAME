using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Godot;

namespace HeroOfEternia.Input
{
    /// <summary>
    /// All named game input actions. Every action maps to a string key used by Godot's InputMap.
    /// New actions can be added here without changing any consuming system.
    /// </summary>
    public static class InputActions
    {
        // Movement
        public const string MoveForward  = "move_forward";
        public const string MoveBack     = "move_back";
        public const string MoveLeft     = "move_left";
        public const string MoveRight    = "move_right";
        public const string Sprint       = "sprint";
        public const string Walk         = "walk";
        public const string Jump         = "jump";
        public const string Roll         = "roll";
        public const string Crouch       = "crouch";

        // Combat (stubs — implemented in a future phase)
        public const string Attack       = "attack";
        public const string HeavyAttack  = "heavy_attack";
        public const string Block        = "block";
        public const string Skill1       = "skill_1";
        public const string Skill2       = "skill_2";
        public const string Skill3       = "skill_3";
        public const string Skill4       = "skill_4";

        // Interaction
        public const string Interact     = "interact";
        public const string LockTarget   = "lock_target";

        // UI / Menus
        public const string OpenInventory = "open_inventory";
        public const string OpenMap      = "open_map";
        public const string OpenQuests   = "open_quests";
        public const string OpenSettings = "open_settings";
        public const string Pause        = "pause";

        // Camera
        public const string CameraRotateLeft  = "camera_rotate_left";
        public const string CameraRotateRight = "camera_rotate_right";
        public const string CameraZoomIn      = "camera_zoom_in";
        public const string CameraZoomOut     = "camera_zoom_out";
        public const string CameraReset       = "camera_reset";
    }

    /// <summary>
    /// Serializable binding record: maps an action name to a key/button scancode.
    /// Supports keyboard, mouse buttons, and gamepad buttons.
    /// </summary>
    public class ActionBinding
    {
        public string ActionName { get; set; } = "";
        public int    KeyScancode   { get; set; } = 0; // Key enum int value
        public int    MouseButton   { get; set; } = -1; // -1 = not bound
        public int    GamepadButton { get; set; } = -1; // -1 = not bound
    }

    /// <summary>
    /// InputActionMap registers all default Godot InputMap actions and supports
    /// runtime rebinding with local disk persistence.
    /// </summary>
    public static class InputActionMap
    {
        private static readonly string _bindingsPath =
            Path.Combine(OS.GetUserDataDir(), "input_bindings.json");

        // ---------------------------------------------------------------
        // DEFAULT BINDINGS
        // ---------------------------------------------------------------
        private static readonly List<ActionBinding> _defaults = new()
        {
            new() { ActionName = InputActions.MoveForward,  KeyScancode = (int)Key.W },
            new() { ActionName = InputActions.MoveBack,     KeyScancode = (int)Key.S },
            new() { ActionName = InputActions.MoveLeft,     KeyScancode = (int)Key.A },
            new() { ActionName = InputActions.MoveRight,    KeyScancode = (int)Key.D },
            new() { ActionName = InputActions.Sprint,       KeyScancode = (int)Key.Shift },
            new() { ActionName = InputActions.Walk,         KeyScancode = (int)Key.Ctrl },
            new() { ActionName = InputActions.Jump,         KeyScancode = (int)Key.Space },
            new() { ActionName = InputActions.Roll,         KeyScancode = (int)Key.Q },
            new() { ActionName = InputActions.Crouch,       KeyScancode = (int)Key.C },
            new() { ActionName = InputActions.Attack,       MouseButton = (int)MouseButton.Left },
            new() { ActionName = InputActions.HeavyAttack,  MouseButton = (int)MouseButton.Right },
            new() { ActionName = InputActions.Block,        KeyScancode = (int)Key.F },
            new() { ActionName = InputActions.Skill1,       KeyScancode = (int)Key.Key1 },
            new() { ActionName = InputActions.Skill2,       KeyScancode = (int)Key.Key2 },
            new() { ActionName = InputActions.Skill3,       KeyScancode = (int)Key.Key3 },
            new() { ActionName = InputActions.Skill4,       KeyScancode = (int)Key.Key4 },
            new() { ActionName = InputActions.Interact,     KeyScancode = (int)Key.E },
            new() { ActionName = InputActions.LockTarget,   KeyScancode = (int)Key.R },
            new() { ActionName = InputActions.OpenInventory,KeyScancode = (int)Key.I },
            new() { ActionName = InputActions.OpenMap,      KeyScancode = (int)Key.M },
            new() { ActionName = InputActions.OpenQuests,   KeyScancode = (int)Key.J },
            new() { ActionName = InputActions.OpenSettings, KeyScancode = (int)Key.Escape },
            new() { ActionName = InputActions.Pause,        KeyScancode = (int)Key.P },
            new() { ActionName = InputActions.CameraRotateLeft,  KeyScancode = (int)Key.Left },
            new() { ActionName = InputActions.CameraRotateRight, KeyScancode = (int)Key.Right },
            new() { ActionName = InputActions.CameraZoomIn,      KeyScancode = (int)Key.Equal },
            new() { ActionName = InputActions.CameraZoomOut,     KeyScancode = (int)Key.Minus },
            new() { ActionName = InputActions.CameraReset,       KeyScancode = (int)Key.V },
        };

        /// <summary>Registers all actions into Godot's InputMap from disk or defaults.</summary>
        public static void Initialize()
        {
            var bindings = LoadBindings();
            foreach (var b in bindings)
            {
                RegisterBinding(b);
            }
            Core.Logger.Info($"InputActionMap: Registered {bindings.Count} action bindings.");
        }

        /// <summary>Rebinds a single action and saves all bindings to disk.</summary>
        public static void Rebind(string actionName, Key newKey)
        {
            if (!InputMap.HasAction(actionName))
            {
                Core.Logger.Warning($"InputActionMap: Action '{actionName}' not found in InputMap.");
                return;
            }
            InputMap.ActionEraseEvents(actionName);

            var ev = new InputEventKey { Keycode = newKey };
            InputMap.ActionAddEvent(actionName, ev);

            SaveBindings();
            Core.Logger.Info($"InputActionMap: Rebound '{actionName}' to key {newKey}.");
        }

        /// <summary>Resets all bindings to factory defaults.</summary>
        public static void ResetToDefaults()
        {
            foreach (var b in _defaults)
            {
                RegisterBinding(b);
            }
            SaveBindings();
            Core.Logger.Info("InputActionMap: All bindings reset to factory defaults.");
        }

        // ---------------------------------------------------------------
        // Private helpers
        // ---------------------------------------------------------------

        private static void RegisterBinding(ActionBinding b)
        {
            if (!InputMap.HasAction(b.ActionName))
            {
                InputMap.AddAction(b.ActionName);
            }
            else
            {
                InputMap.ActionEraseEvents(b.ActionName);
            }

            if (b.KeyScancode > 0)
            {
                var ev = new InputEventKey { Keycode = (Key)b.KeyScancode };
                InputMap.ActionAddEvent(b.ActionName, ev);
            }
            if (b.MouseButton >= 0)
            {
                var ev = new InputEventMouseButton { ButtonIndex = (MouseButton)b.MouseButton };
                InputMap.ActionAddEvent(b.ActionName, ev);
            }
            if (b.GamepadButton >= 0)
            {
                var ev = new InputEventJoypadButton { ButtonIndex = (JoyButton)b.GamepadButton };
                InputMap.ActionAddEvent(b.ActionName, ev);
            }
        }

        private static List<ActionBinding> LoadBindings()
        {
            if (File.Exists(_bindingsPath))
            {
                try
                {
                    string json = File.ReadAllText(_bindingsPath);
                    var loaded = JsonSerializer.Deserialize<List<ActionBinding>>(json);
                    if (loaded != null && loaded.Count > 0)
                    {
                        Core.Logger.Info("InputActionMap: Loaded custom bindings from disk.");
                        return loaded;
                    }
                }
                catch (Exception ex)
                {
                    Core.Logger.Error($"InputActionMap: Failed to load bindings: {ex.Message}");
                }
            }
            return _defaults;
        }

        private static void SaveBindings()
        {
            try
            {
                var current = new List<ActionBinding>();
                foreach (var b in _defaults)
                {
                    current.Add(new ActionBinding
                    {
                        ActionName    = b.ActionName,
                        KeyScancode   = b.KeyScancode,
                        MouseButton   = b.MouseButton,
                        GamepadButton = b.GamepadButton
                    });
                }
                string json = JsonSerializer.Serialize(current,
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_bindingsPath, json);
            }
            catch (Exception ex)
            {
                Core.Logger.Error($"InputActionMap: Failed to save bindings: {ex.Message}");
            }
        }
    }
}
