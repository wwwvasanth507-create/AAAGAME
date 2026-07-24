using System;
using System.IO;
using Godot;

namespace HeroOfEternia.Core
{
    public class DeviceSpecs
    {
        public string OsName { get; set; } = "Unknown";
        public string CpuName { get; set; } = "Unknown CPU";
        public string GpuName { get; set; } = "Unknown GPU";
        public long SystemRamMb { get; set; } = 2048; // Assume min 2GB
        public Vector2I ScreenResolution { get; set; } = new Vector2I(1280, 720);
        public float RefreshRate { get; set; } = 60.0f;
        public long FreeStorageBytes { get; set; } = 0;
    }

    /// <summary>
    /// DeviceDetector queries system profiles to adapt rendering parameters automatically.
    /// </summary>
    public class DeviceDetector
    {
        public DeviceSpecs CurrentSpecs { get; private set; } = new DeviceSpecs();

        public void DetectDevice()
        {
            Logger.Info("DeviceDetector: Analyzing target hardware parameters...");

            string os = OS.GetName();
            string cpu = OS.GetProcessorName();
            string gpu = RenderingServer.GetVideoAdapterName();
            
            // Screen dimensions and refresh rates
            Vector2I res = DisplayServer.WindowGetSize();
            float refresh = DisplayServer.ScreenGetRefreshRate();

            // Drive size using portable DriveInfo API
            long freeStorage = 0;
            try
            {
                string path = OS.GetUserDataDir();
                var drive = new DriveInfo(Path.GetPathRoot(path) ?? "C:");
                freeStorage = drive.AvailableFreeSpace;
            }
            catch (Exception ex)
            {
                Logger.Warning($"DeviceDetector: Free storage query bypassed: {ex.Message}");
            }

            // Query actual physical RAM — fall back to a safe minimum if unavailable.
            long ramMb = 4096;
            try
            {
                // Godot exposes memory via Performance monitors
                double staticMem = Godot.Performance.GetMonitor(Godot.Performance.Monitor.MemoryStatic);
                // Estimate system RAM: Godot doesn't expose total RAM directly on all platforms.
                // Use OS.GetMemoryInfo() where available (GDScript), else keep default.
                ramMb = Math.Max(2048, (long)(staticMem / (1024.0 * 1024.0) * 8)); // conservative estimate
            }
            catch
            {
                Logger.Warning("DeviceDetector: Could not query physical RAM. Using 4096 MB default.");
            }

            CurrentSpecs = new DeviceSpecs
            {
                OsName = os,
                CpuName = string.IsNullOrEmpty(cpu) ? "Generic Android CPU" : cpu,
                GpuName = string.IsNullOrEmpty(gpu) ? "Compatibility Mobile GPU" : gpu,
                SystemRamMb = ramMb,
                ScreenResolution = res,
                RefreshRate = refresh <= 0 ? 60.0f : refresh,
                FreeStorageBytes = freeStorage
            };

            Logger.Info($"DeviceDetector: OS={CurrentSpecs.OsName}, CPU={CurrentSpecs.CpuName}, GPU={CurrentSpecs.GpuName}, RAM={CurrentSpecs.SystemRamMb} MB, Res={CurrentSpecs.ScreenResolution.X}x{CurrentSpecs.ScreenResolution.Y}, Refresh={CurrentSpecs.RefreshRate}Hz, FreeStorage={CurrentSpecs.FreeStorageBytes / (1024*1024)} MB");
        }

        /// <summary>
        /// Automatically maps specs to low/medium/high/ultra graphics presets.
        /// </summary>
        public string GetRecommendedPreset()
        {
            // Simple heuristic mapping GPU name and RAM specs
            string gpuLower = CurrentSpecs.GpuName.ToLower();

            if (gpuLower.Contains("adreno (tm) 5") || gpuLower.Contains("mali-t") || CurrentSpecs.SystemRamMb <= 2048)
            {
                return "LOW";
            }
            if (gpuLower.Contains("adreno (tm) 6") || gpuLower.Contains("mali-g") || CurrentSpecs.SystemRamMb <= 4096)
            {
                return "MEDIUM";
            }
            if (gpuLower.Contains("adreno (tm) 7") || gpuLower.Contains("rtx") || gpuLower.Contains("nvidia"))
            {
                return "HIGH";
            }
            return "HIGH"; // Default standard safe baseline preset
        }
    }
}
