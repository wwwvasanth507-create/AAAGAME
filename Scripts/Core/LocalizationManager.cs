using System;
using System.Collections.Generic;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// Governs translation assets and language configurations.
    /// </summary>
    public class LocalizationManager
    {
        private readonly Dictionary<string, string> _translations = new Dictionary<string, string>();
        public string ActiveLanguage { get; private set; } = "en";

        public void Initialize(string languageCode)
        {
            ActiveLanguage = languageCode;
            Logger.Info($"LocalizationManager: Initializing language dictionaries for '{ActiveLanguage}'");
            
            // Mock localization entries
            _translations["MENU_START"] = "Launch Operations";
            _translations["MENU_SETTINGS"] = "System Settings";
            _translations["MENU_EXIT"] = "Exit Terminal";
        }

        public string GetText(string key)
        {
            if (_translations.TryGetValue(key, out string? value))
            {
                return value;
            }
            Logger.Warning($"LocalizationManager: Translation key '{key}' not found.");
            return key;
        }
    }
}
