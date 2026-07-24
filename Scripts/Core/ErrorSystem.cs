using System;
using System.IO;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// ErrorSystem captures unhandled application exceptions, fatal errors, 
    /// asset misses, and writes diagnostic reports to local crash logs.
    /// </summary>
    public static class ErrorSystem
    {
        private static string? _logFilePath;

        public static void Initialize(string logDir)
        {
            try
            {
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                _logFilePath = Path.Combine(logDir, "crash_log.txt");

                // Bind to AppDomain unhandled exceptions
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                Logger.Info("ErrorSystem: Global exception listener initialized.");
            }
            catch (Exception ex)
            {
                Logger.Error($"ErrorSystem: Failed to initialize: {ex.Message}");
            }
        }

        public static void ReportFatalError(string context, Exception ex)
        {
            string errorMessage = $"FATAL ERROR in [{context}]: {ex.Message}\nStackTrace:\n{ex.StackTrace}";
            Logger.Critical(errorMessage);
            WriteCrashLog(errorMessage);
            
            // In a GUI build, we would display a pop-up warning dialog card to the user before quitting
        }

        public static void ReportAssetMiss(string assetPath)
        {
            string warnMessage = $"RESOURCE ERROR: Critical asset was not found: '{assetPath}'";
            Logger.Error(warnMessage);
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string msg = "UNHANDLED SYSTEM EXCEPTION:\n";
            if (e.ExceptionObject is Exception ex)
            {
                msg += $"{ex.Message}\nStackTrace:\n{ex.StackTrace}";
            }
            else
            {
                msg += e.ExceptionObject?.ToString() ?? "Unknown exception payload.";
            }

            Logger.Critical(msg);
            WriteCrashLog(msg);
        }

        private static void WriteCrashLog(string message)
        {
            if (string.IsNullOrEmpty(_logFilePath)) return;

            try
            {
                string entry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}] {message}\n" +
                               $"----------------------------------------\n";
                File.AppendAllText(_logFilePath, entry);
            }
            catch (Exception)
            {
                // Can't write to disk; skip to avoid recursion loops
            }
        }
    }
}
