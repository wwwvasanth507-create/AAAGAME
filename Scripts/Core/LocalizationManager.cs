using System;
using System.Collections.Generic;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// Governs translation assets and language configurations.
    /// Implements IInitializable — ServiceLocator calls Initialize() automatically on first Get.
    /// Default language is "en" (English).
    /// </summary>
    public class LocalizationManager : IInitializable
    {
        private readonly Dictionary<string, string> _translations = new Dictionary<string, string>();
        public string ActiveLanguage { get; private set; } = "en";

        /// <summary>IInitializable contract — defaults to English.</summary>
        public void Initialize() => Initialize("en");

        public void Initialize(string languageCode)
        {
            ActiveLanguage = languageCode;
            Logger.Info($"LocalizationManager: Initializing language dictionaries for '{ActiveLanguage}'");

            // Base English entries — future phases will load these from JSON files
            _translations["MENU_START"]    = "New Adventure";
            _translations["MENU_CONTINUE"] = "Continue Journey";
            _translations["MENU_SETTINGS"] = "Settings";
            _translations["MENU_EXIT"]     = "Exit";
            _translations["SETTINGS_AUDIO"]       = "Audio";
            _translations["SETTINGS_GRAPHICS"]    = "Graphics";
            _translations["SETTINGS_CONTROLS"]    = "Controls";
            _translations["SETTINGS_LANGUAGE"]    = "Language";
            _translations["SETTINGS_ACCESSIBILITY"] = "Accessibility";
            _translations["SAVE_SLOT_EMPTY"]  = "Empty Slot";
            _translations["SAVE_SLOT_LOADING"] = "Loading save...";
            _translations["ERROR_SAVE_CORRUPT"] = "Save data corrupted. Backup restored.";
        }

        public string GetText(string key)
        {
            if (_translations.TryGetValue(key, out string? value))
            {
                return value;
            }
            Logger.Warning($"LocalizationManager: Translation key '{key}' not found for language '{ActiveLanguage}'.");
            return key; // Graceful fallback — return the key itself
        }

        /// <summary>Hot-swaps language at runtime and re-populates the dictionary.</summary>
        public void ChangeLanguage(string languageCode)
        {
            _translations.Clear();
            Initialize(languageCode);
        }
    }
}
