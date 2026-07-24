package com.antigravity.voidodyssey

import android.annotation.SuppressLint
import android.opengl.GLSurfaceView
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.MotionEvent
import android.view.View
import android.widget.Button
import android.widget.LinearLayout
import android.widget.ProgressBar
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import com.antigravity.voidodyssey.core.GLES30Renderer
import com.antigravity.voidodyssey.core.QualityManager
import com.antigravity.voidodyssey.core.QualityPreset
import com.antigravity.voidodyssey.db.GameDatabase
import com.antigravity.voidodyssey.db.SaveSlot
import com.antigravity.voidodyssey.game.GameLoop
import com.antigravity.voidodyssey.game.InputManager

class MainActivity : AppCompatActivity() {

    private lateinit var glSurfaceView: GLSurfaceView
    private lateinit var renderer: GLES30Renderer
    private lateinit var qualityManager: QualityManager
    private lateinit var gameLoop: GameLoop
    private val inputManager = InputManager()

    // UI overlays
    private lateinit var mainMenuOverlay: LinearLayout
    private lateinit var settingsOverlay: LinearLayout
    private lateinit var saveSlotsOverlay: LinearLayout
    private lateinit var hudOverlay: View

    // HUD items
    private lateinit var pbHull: ProgressBar
    private lateinit var pbShield: ProgressBar
    private lateinit var tvScore: TextView
    private lateinit var tvOre: TextView

    // HUD status timer
    private val mainHandler = Handler(Looper.getMainLooper())
    private val uiUpdateRunnable = object : Runnable {
        override fun run() {
            updateHUD()
            mainHandler.postDelayed(this, 100) // Update at 10Hz (100ms)
        }
    }

    @SuppressLint("ClickableViewAccessibility")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setFullscreenMode()
        setContentView(R.layout.activity_main)

        // Initialize Managers
        qualityManager = QualityManager(this)
        renderer = GLES30Renderer(this, qualityManager)

        // Bind Views
        glSurfaceView = findViewById(R.id.gl_surface_view)
        mainMenuOverlay = findViewById(R.id.main_menu_overlay)
        settingsOverlay = findViewById(R.id.settings_overlay)
        saveSlotsOverlay = findViewById(R.id.save_slots_overlay)
        hudOverlay = findViewById(R.id.hud_overlay)

        pbHull = findViewById(R.id.pb_hull)
        pbShield = findViewById(R.id.pb_shield)
        tvScore = findViewById(R.id.tv_score)
        tvOre = findViewById(R.id.tv_ore)

        // Initialize GLSurfaceView
        glSurfaceView.setEGLContextClientVersion(3)
        glSurfaceView.setRenderer(renderer)
        // Render only when there are visual state changes to save CPU/battery
        glSurfaceView.renderMode = GLSurfaceView.RENDERMODE_CONTINUOUSLY

        // Initialize Game Loop
        gameLoop = GameLoop(this, renderer, inputManager)

        // Bind touch inputs
        glSurfaceView.setOnTouchListener { _, event ->
            inputManager.handleTouchEvent(event)
        }

        // Setup UI layouts size changes in inputManager
        glSurfaceView.post {
            inputManager.updateScreenSize(glSurfaceView.width, glSurfaceView.height)
        }

        setupButtons()
    }

    private fun setFullscreenMode() {
        window.decorView.systemUiVisibility = (
            View.SYSTEM_UI_FLAG_FULLSCREEN or
            View.SYSTEM_UI_FLAG_HIDE_NAVIGATION or
            View.SYSTEM_UI_FLAG_IMMERSIVE_STICKY or
            View.SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN or
            View.SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION or
            View.SYSTEM_UI_FLAG_LAYOUT_STABLE
        )
    }

    private fun setupButtons() {
        // Main Menu Buttons
        findViewById<Button>(R.id.btn_start_game).setOnClickListener {
            showHUD()
            gameLoop.start()
        }

        findViewById<Button>(R.id.btn_open_settings).setOnClickListener {
            showSettings()
        }

        findViewById<Button>(R.id.btn_open_saves).setOnClickListener {
            showSaveSlots()
        }

        // Settings Buttons
        findViewById<Button>(R.id.btn_preset_low).setOnClickListener { changeQualityPreset(QualityPreset.LOW) }
        findViewById<Button>(R.id.btn_preset_med).setOnClickListener { changeQualityPreset(QualityPreset.MEDIUM) }
        findViewById<Button>(R.id.btn_preset_high).setOnClickListener { changeQualityPreset(QualityPreset.HIGH) }
        findViewById<Button>(R.id.btn_preset_ultra).setOnClickListener { changeQualityPreset(QualityPreset.ULTRA) }

        findViewById<Button>(R.id.btn_settings_back).setOnClickListener { showMainMenu() }

        // Save Slots Buttons
        val slot1 = findViewById<Button>(R.id.btn_slot_1)
        val slot2 = findViewById<Button>(R.id.btn_slot_2)
        val slot3 = findViewById<Button>(R.id.btn_slot_3)

        slot1.setOnClickListener { selectSaveSlot(1) }
        slot2.setOnClickListener { selectSaveSlot(2) }
        slot3.setOnClickListener { selectSaveSlot(3) }

        findViewById<Button>(R.id.btn_saves_back).setOnClickListener { showMainMenu() }

        // HUD Buttons
        findViewById<Button>(R.id.btn_hud_menu).setOnClickListener {
            gameLoop.stop()
            showMainMenu()
        }

        // Initialize display of optimal quality presets description
        updateQualityDetailsLabel()
        loadSaveSlotLabels(slot1, slot2, slot3)
    }

    private fun loadSaveSlotLabels(slot1: Button, slot2: Button, slot3: Button) {
        Thread {
            val db = GameDatabase.getDatabase(this)
            val slots = db.gameDao().getAllSaveSlots()
            
            runOnUiThread {
                slots.find { it.slotId == 1 }?.let { slot1.text = "SLOT 1: ${it.label}" }
                slots.find { it.slotId == 2 }?.let { slot2.text = "SLOT 2: ${it.label}" }
                slots.find { it.slotId == 3 }?.let { slot3.text = "SLOT 3: ${it.label}" }
            }
        }.start()
    }

    private fun selectSaveSlot(slotId: Int) {
        gameLoop.activeSlotId = slotId
        gameLoop.loadSavedProfile()
        
        // Write active slot metdata to Database
        Thread {
            val db = GameDatabase.getDatabase(this)
            db.gameDao().insertSaveSlot(
                SaveSlot(slotId, "Sol Sector (Saved " + System.currentTimeMillis() / 1000 + ")", System.currentTimeMillis())
            )
            val slotButton1 = findViewById<Button>(R.id.btn_slot_1)
            val slotButton2 = findViewById<Button>(R.id.btn_slot_2)
            val slotButton3 = findViewById<Button>(R.id.btn_slot_3)
            loadSaveSlotLabels(slotButton1, slotButton2, slotButton3)
        }.start()

        showHUD()
        gameLoop.start()
    }

    private fun changeQualityPreset(preset: QualityPreset) {
        qualityManager.applyPreset(preset)
        updateQualityDetailsLabel()
        // Trigger surface size change to recalibrate aspect resolution scales
        glSurfaceView.post {
            renderer.onSurfaceChanged(null, glSurfaceView.width, glSurfaceView.height)
        }
    }

    private fun updateQualityDetailsLabel() {
        val s = qualityManager.currentSettings
        val details = "Active Preset: ${s.preset.name}\nResolution Scale: ${s.resolutionScale}x\nMax Particles: ${s.maxParticles}\nShadow Map Size: ${s.shadowMapSize}px\nAnisotropic Filtering: ${if (s.enableAnisotropicFiltering) "${s.anisotropicLevel}x" else "Off"}"
        findViewById<TextView>(R.id.tv_quality_details).text = details
    }

    private fun updateHUD() {
        pbHull.progress = gameLoop.playerShip.hull.toInt().coerceIn(0, 100)
        pbShield.progress = gameLoop.playerShip.shield.toInt().coerceIn(0, 50)
        tvScore.text = String.format("SCORE: %04d", gameLoop.score)
        tvOre.text = "ORE: ${gameLoop.oreMined}"
    }

    // --- UI State Management ---
    private fun showMainMenu() {
        setFullscreenMode()
        mainMenuOverlay.visibility = View.VISIBLE
        settingsOverlay.visibility = View.GONE
        saveSlotsOverlay.visibility = View.GONE
        hudOverlay.visibility = View.GONE
        mainHandler.removeCallbacks(uiUpdateRunnable)
    }

    private fun showSettings() {
        mainMenuOverlay.visibility = View.GONE
        settingsOverlay.visibility = View.VISIBLE
        saveSlotsOverlay.visibility = View.GONE
        hudOverlay.visibility = View.GONE
    }

    private fun showSaveSlots() {
        mainMenuOverlay.visibility = View.GONE
        settingsOverlay.visibility = View.GONE
        saveSlotsOverlay.visibility = View.VISIBLE
        hudOverlay.visibility = View.GONE
    }

    private fun showHUD() {
        setFullscreenMode()
        mainMenuOverlay.visibility = View.GONE
        settingsOverlay.visibility = View.GONE
        saveSlotsOverlay.visibility = View.GONE
        hudOverlay.visibility = View.VISIBLE
        mainHandler.post(uiUpdateRunnable)
    }

    // --- Lifecycle bindings ---
    override fun onResume() {
        super.onResume()
        glSurfaceView.onResume()
        setFullscreenMode()
        if (hudOverlay.visibility == View.VISIBLE) {
            gameLoop.start()
            mainHandler.post(uiUpdateRunnable)
        }
    }

    override fun onPause() {
        super.onPause()
        glSurfaceView.onPause()
        gameLoop.stop()
        mainHandler.removeCallbacks(uiUpdateRunnable)
    }
}
