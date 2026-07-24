using System;
using System.Diagnostics;

namespace HeroOfEternia.Core
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Critical
    }

    /// <summary>
    /// Thread-safe logger. Supports Info, Warning, Error, and Critical logs.
    /// Strips debug output in Release builds for performance and security.
    /// </summary>
    public static class Logger
    {
        private static readonly object _lock = new object();

        [Conditional("DEBUG")]
        public static void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        [Conditional("DEBUG")]
        public static void Warning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        public static void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        public static void Critical(string message)
        {
            Log(LogLevel.Critical, message);
            // In a production build, Critical errors would trigger telemetry / crash dumps reporting hooks
        }

        private static void Log(LogLevel level, string message)
        {
            lock (_lock)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string logMessage = $"[{timestamp}] [{level.ToString().ToUpper()}] {message}";

                switch (level)
                {
                    case LogLevel.Info:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    case LogLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LogLevel.Critical:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        break;
                }

                Console.WriteLine(logMessage);
                Console.ResetColor();

                // If running inside Godot editor, redirect to Godot engine prints
                // GD.PrintT(logMessage);
            }
        }
    }
}
