package com.antigravity.voidodyssey.game

import android.content.Context
import android.os.SystemClock
import android.util.Log
import com.antigravity.voidodyssey.core.GLES30Renderer
import com.antigravity.voidodyssey.db.GameDatabase
import com.antigravity.voidodyssey.db.PlayerProfile
import java.util.concurrent.atomic.AtomicBoolean

/**
 * Thread-safe fixed-timestep game update loop that triggers physics, collisions, and state changes.
 */
class GameLoop(
    private val context: Context,
    private val renderer: GLES30Renderer,
    private val inputManager: InputManager
) : Runnable {

    private val db = GameDatabase.getDatabase(context)

    val playerShip = PlayerShip()
    
    // Object Pools to eliminate GC allocation stutters
    val lasers = List(20) { Laser() }
    val asteroids = List(15) { Asteroid() }

    private val running = AtomicBoolean(false)
    private var gameThread: Thread? = null

    // Game stats & save management
    var score = 0
    var oreMined = 0
    var activeSlotId = 1

    init {
        resetWorld()
    }

    private fun resetWorld() {
        playerShip.x = 0f
        playerShip.y = 0f
        playerShip.z = 0f
        playerShip.currentSpeed = 0f
        playerShip.targetSpeed = 0f
        playerShip.hull = 100f
        playerShip.shield = 50f

        // Reset object pools
        lasers.forEach { it.active = false }
        asteroids.forEach { it.active = false }

        // Spawn asteroids at random coordinates in a sphere
        for (ast in asteroids) {
            val rx = (-30 + Math.random() * 60).toFloat()
            val ry = (-10 + Math.random() * 20).toFloat()
            val rz = (-40 - Math.random() * 30).toFloat() // Spawn in front
            val size = (1f + Math.random() * 2.5f).toFloat()
            ast.spawn(rx, ry, rz, size)
        }
    }

    fun start() {
        if (running.compareAndSet(false, true)) {
            loadSavedProfile()
            gameThread = Thread(this, "GameLoopThread").apply { start() }
            Log.i("GameLoop", "Game loop started.")
        }
    }

    fun stop() {
        if (running.compareAndSet(true, false)) {
            try {
                saveProfile()
                gameThread?.join(1000)
                Log.i("GameLoop", "Game loop stopped.")
            } catch (e: InterruptedException) {
                Log.e("GameLoop", "Game loop thread interrupted during join: ${e.message}")
            }
        }
    }

    override fun run() {
        var lastTime = SystemClock.elapsedRealtimeNanos()
        val targetDelta = 16666666L // ~60 FPS in nanoseconds (16.67ms)

        while (running.get()) {
            val now = SystemClock.elapsedRealtimeNanos()
            var elapsed = now - lastTime
            
            if (elapsed >= targetDelta) {
                lastTime = now
                val dt = elapsed / 1_000_000_000f // Convert nanoseconds to float seconds
                
                // Limit frame time spike to prevent game physics breaking
                val clampedDt = Math.min(dt, 0.1f)

                update(clampedDt)

                // Push thread-safe snapshot to renderer
                renderer.updateRenderState(playerShip, asteroids, lasers)
            } else {
                // Yield thread to conserve battery and CPU heating
                val sleepMs = (targetDelta - elapsed) / 1_000_000L
                if (sleepMs > 0) {
                    try {
                        Thread.sleep(sleepMs)
                    } catch (e: InterruptedException) {
                        // Suppress
                    }
                }
            }
        }
    }

    private fun update(dt: Float) {
        // 1. Process steering touch controls
        playerShip.steer(inputManager.joystickX, -inputManager.joystickY, dt)
        playerShip.targetSpeed = inputManager.throttle * 15f // Map throttle to speed

        // 2. Process fire request
        if (inputManager.isFiring) {
            playerShip.fireLaser(lasers)
        }

        // 3. Update entities
        playerShip.update(dt)
        lasers.forEach { it.update(dt) }
        asteroids.forEach { it.update(dt) }

        // 4. Resolve collisions
        checkCollisions()

        // 5. Respawn asteroids if they wander/destroy
        respawnMissingAsteroids()
    }

    private fun checkCollisions() {
        // Laser vs Asteroid collisions
        for (laser in lasers) {
            if (!laser.active) continue

            for (ast in asteroids) {
                if (!ast.active) continue

                val dx = laser.x - ast.x
                val dy = laser.y - ast.y
                val dz = laser.z - ast.z
                val distSq = dx*dx + dy*dy + dz*dz

                // Radius check: asteroid radius + laser radius (0.2)
                val collisionDist = ast.radius + 0.2f
                if (distSq < collisionDist * collisionDist) {
                    laser.active = false
                    val destroyed = ast.takeDamage(5f)
                    if (destroyed) {
                        score += 10
                        oreMined += (ast.scale * 2).toInt()
                    }
                    break
                }
            }
        }

        // Ship vs Asteroid collisions
        for (ast in asteroids) {
            if (!ast.active) continue

            val dx = playerShip.x - ast.x
            val dy = playerShip.y - ast.y
            val dz = playerShip.z - ast.z
            val distSq = dx*dx + dy*dy + dz*dz

            val collisionDist = ast.radius + 1.2f // ship size offset
            if (distSq < collisionDist * collisionDist) {
                // Collided! Ship takes damage, asteroid bounces/deactivates
                playerShip.takeDamage(ast.scale * 15f)
                ast.active = false
                Log.i("GameLoop", "Player ship hit asteroid! Shield: ${playerShip.shield}, Hull: ${playerShip.hull}")
            }
        }
    }

    private fun respawnMissingAsteroids() {
        for (ast in asteroids) {
            if (!ast.active) {
                // Spawn a new asteroid ahead of the player
                val rx = playerShip.x + (-25 + Math.random() * 50).toFloat()
                val ry = playerShip.y + (-8 + Math.random() * 16).toFloat()
                val rz = playerShip.z - (30 + Math.random() * 30).toFloat()
                val size = (1f + Math.random() * 2f).toFloat()
                ast.spawn(rx, ry, rz, size)
            }
        }
    }

    fun loadSavedProfile() {
        Thread {
            try {
                val profile = db.gameDao().getPlayerProfile(activeSlotId)
                if (profile != null) {
                    playerShip.x = 0f
                    playerShip.y = 0f
                    playerShip.z = 0f
                    playerShip.hull = profile.hull
                    playerShip.shield = profile.shield
                    Log.i("GameLoop", "Loaded save game profile for Slot $activeSlotId.")
                } else {
                    Log.i("GameLoop", "No profile found for Slot $activeSlotId. Setting defaults.")
                }
            } catch (e: Exception) {
                Log.e("GameLoop", "Error reading DB profile: ${e.message}")
            }
        }.start()
    }

    fun saveProfile() {
        Thread {
            try {
                val profile = PlayerProfile(
                    slotId = activeSlotId,
                    playerName = "Rogue Pilot",
                    level = 1,
                    xp = score,
                    credits = oreMined * 15L,
                    currentSystem = "Sol Sector 4",
                    shipType = "Fighter Class A",
                    hull = playerShip.hull,
                    shield = playerShip.shield,
                    playTime = 0,
                    worldSeed = 1337420L
                )
                db.gameDao().insertPlayerProfile(profile)
                Log.i("GameLoop", "Profile automatically saved to Slot $activeSlotId.")
            } catch (e: Exception) {
                Log.e("GameLoop", "Error writing DB profile: ${e.message}")
            }
        }.start()
    }
}
