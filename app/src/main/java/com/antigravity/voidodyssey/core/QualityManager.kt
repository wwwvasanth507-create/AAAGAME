package com.antigravity.voidodyssey.core

import android.app.ActivityManager
import android.content.Context
import android.os.Build
import android.util.Log

enum class QualityPreset {
    LOW,
    MEDIUM,
    HIGH,
    ULTRA
}

data class RenderSettings(
    val preset: QualityPreset,
    val resolutionScale: Float,
    val maxParticles: Int,
    val shadowMapSize: Int,
    val enableAnisotropicFiltering: Boolean,
    val anisotropicLevel: Int,
    val targetFps: Int
)

/**
 * Automatically detects device capabilities and configures scalable rendering settings.
 */
class QualityManager(private val context: Context) {

    var currentSettings: RenderSettings = detectOptimalSettings()
        private set

    fun applyPreset(preset: QualityPreset) {
        currentSettings = getSettingsForPreset(preset)
        Log.i("QualityManager", "Graphics quality preset set to: $preset")
    }

    private fun detectOptimalSettings(): RenderSettings {
        val cores = Runtime.getRuntime().availableProcessors()
        val totalRamMb = getSystemTotalRamMb()

        Log.i("QualityManager", "System stats: CPU Cores = $cores, Total RAM = ${totalRamMb}MB")

        val preset = when {
            totalRamMb >= 8000 && cores >= 8 -> QualityPreset.ULTRA
            totalRamMb >= 4000 && cores >= 6 -> QualityPreset.HIGH
            totalRamMb >= 3000 && cores >= 4 -> QualityPreset.MEDIUM
            else -> QualityPreset.LOW
        }

        Log.i("QualityManager", "Auto-detected graphics preset: $preset")
        return getSettingsForPreset(preset)
    }

    private fun getSettingsForPreset(preset: QualityPreset): RenderSettings {
        return when (preset) {
            QualityPreset.LOW -> RenderSettings(
                preset = QualityPreset.LOW,
                resolutionScale = 0.5f,
                maxParticles = 50,
                shadowMapSize = 0,
                enableAnisotropicFiltering = false,
                anisotropicLevel = 0,
                targetFps = 30
            )
            QualityPreset.MEDIUM -> RenderSettings(
                preset = QualityPreset.MEDIUM,
                resolutionScale = 0.75f,
                maxParticles = 150,
                shadowMapSize = 512,
                enableAnisotropicFiltering = false,
                anisotropicLevel = 0,
                targetFps = 60
            )
            QualityPreset.HIGH -> RenderSettings(
                preset = QualityPreset.HIGH,
                resolutionScale = 1.0f,
                maxParticles = 300,
                shadowMapSize = 1024,
                enableAnisotropicFiltering = true,
                anisotropicLevel = 4,
                targetFps = 60
            )
            QualityPreset.ULTRA -> RenderSettings(
                preset = QualityPreset.ULTRA,
                resolutionScale = 1.0f,
                maxParticles = 600,
                shadowMapSize = 2048,
                enableAnisotropicFiltering = true,
                anisotropicLevel = 16,
                targetFps = 60
            )
        }
    }

    private fun getSystemTotalRamMb(): Long {
        return try {
            val actManager = context.getSystemService(Context.ACTIVITY_SERVICE) as ActivityManager
            val memInfo = ActivityManager.MemoryInfo()
            actManager.getMemoryInfo(memInfo)
            memInfo.totalMem / (1024 * 1024)
        } catch (e: Exception) {
            2048 // Fallback to 2GB minimum
        }
    }
}
