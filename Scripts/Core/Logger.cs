using System;
using System.Diagnostics;
using Godot;

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
    /// Thread-safe logger. Supports Info, Warning, Error, and Critical levels.
    /// Strips Info/Warning in Release builds via conditional compilation.
    /// Routes to Godot's Output panel when running inside the Godot engine.
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
        }

        private static void Log(LogLevel level, string message)
        {
            lock (_lock)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string logMessage = $"[{timestamp}] [{level.ToString().ToUpper()}] {message}";

                // Route to Godot Output panel when available; fall back to Console in headless mode.
                try
                {
                    switch (level)
                    {
                        case LogLevel.Info:
                            GD.Print(logMessage);
                            break;
                        case LogLevel.Warning:
                            GD.PushWarning(logMessage);
                            break;
                        case LogLevel.Error:
                        case LogLevel.Critical:
                            GD.PushError(logMessage);
                            break;
                    }
                }
                catch
                {
                    // Godot engine not initialised (pure unit test context) — use Console fallback.
                    Console.ForegroundColor = level switch
                    {
                        LogLevel.Warning  => ConsoleColor.Yellow,
                        LogLevel.Error    => ConsoleColor.Red,
                        LogLevel.Critical => ConsoleColor.DarkRed,
                        _                 => ConsoleColor.White
                    };
                    Console.WriteLine(logMessage);
                    Console.ResetColor();
                }
            }
        }
    }
}
