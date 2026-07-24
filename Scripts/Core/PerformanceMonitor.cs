using System;
using Godot;

namespace HeroOfEternia.Core
{
    /// <summary>
    /// Developer performance overlay tracking FPS, memory, draw calls, and CPU time.
    /// Only visible when developer debug mode is active.
    /// </summary>
    public partial class PerformanceMonitor : Label
    {
        private float _updateIntervalSeconds = 0.5f;
        private float _timeAccumulator = 0.0f;
        private bool _devModeActive = false;

        public override void _Ready()
        {
            // Position the overlay in the top-left screen corner
            Position = new Vector2(10, 10);
            
            // Set basic UI style
            LabelSettings = new LabelSettings
            {
                FontSize = 14,
                FontColor = new Color(0.0f, 1.0f, 0.0f, 0.85f), // Neon Green
                OutlineSize = 2,
                OutlineColor = new Color(0, 0, 0, 1)
            };

            // Read debug settings to toggle visibility
            CheckVisibility();
        }

        public override void _Process(double delta)
        {
            if (!_devModeActive) return;

            _timeAccumulator += (float)delta;
            if (_timeAccumulator >= _updateIntervalSeconds)
            {
                _timeAccumulator = 0.0f;
                UpdateMonitorStats();
            }
        }

        public void CheckVisibility()
        {
            try
            {
                var settings = ServiceLocator.Get<SettingsManager>();
                _devModeActive = settings.DebugModeEnabled;
                Visible = _devModeActive;
                Logger.Info($"PerformanceMonitor: Developer Overlay state updated. Visible={_devModeActive}");
            }
            catch (Exception)
            {
                // ServiceLocator might not have initialized yet; check again next frame
                Visible = false;
            }
        }

        private void UpdateMonitorStats()
        {
            double fps = Performance.GetMonitor(Performance.Monitor.TimeFps);
            double processTime = Performance.GetMonitor(Performance.Monitor.TimeProcess) * 1000.0; // milliseconds
            double drawCalls = Performance.GetMonitor(Performance.Monitor.RenderTotalDrawCallsInFrame);
            
            // Convert bytes to MB
            double staticMem = Performance.GetMonitor(Performance.Monitor.MemoryStatic) / (1024.0 * 1024.0);

            string batteryStr = "N/A (Android API Removed)";

            Text = $"--- ETERNIA DEV MONITOR ---\n" +
                   $"FPS: {fps:0.0}\n" +
                   $"Frame Time: {processTime:0.00} ms\n" +
                   $"Static Memory: {staticMem:0.0} MB\n" +
                   $"Draw Calls: {drawCalls}\n" +
                   $"Battery Limit: {batteryStr}";
        }
    }
}
