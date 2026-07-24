package com.antigravity.voidodyssey.game

/**
 * Represents the Player's Spaceship entity.
 */
class PlayerShip {
    var x = 0f
    var y = 0f
    var z = 0f

    var rotY = 0f
    var vx = 0f
    var vy = 0f
    var vz = 0f

    var hull = 100f
    val maxHull = 100f
    var shield = 50f
    val maxShield = 50f

    var currentSpeed = 0f
    var targetSpeed = 0f
    private val acceleration = 8f
    private val maxSpeed = 15f

    var fireCooldown = 0f
    private val fireRate = 0.2f // Fire once every 200ms

    fun update(dt: Float) {
        // Handle thrusters/speed interpolation
        currentSpeed += (targetSpeed - currentSpeed) * acceleration * dt
        if (currentSpeed > maxSpeed) currentSpeed = maxSpeed
        if (currentSpeed < -maxSpeed / 2f) currentSpeed = -maxSpeed / 2f

        // Move ship in the direction it's facing (rotY is rotation around Y-axis)
        val angleRad = Math.toRadians(rotY.toDouble())
        vx = (-Math.sin(angleRad) * currentSpeed).toFloat()
        vz = (-Math.cos(angleRad) * currentSpeed).toFloat()

        x += vx * dt
        y += vy * dt
        z += vz * dt

        // Boundary checks (keep player in a sphere sector of 150m)
        val distSq = x*x + y*y + z*z
        if (distSq > 150f * 150f) {
            // Bounce/Stop ship
            x -= vx * dt
            y -= vy * dt
            z -= vz * dt
            currentSpeed = -currentSpeed * 0.5f
        }

        // Shield recharge
        if (shield < maxShield) {
            shield += 2f * dt // Recharge shield at 2 units per second
            if (shield > maxShield) shield = maxShield
        }

        if (fireCooldown > 0f) {
            fireCooldown -= dt
        }
    }

    fun steer(horizontal: Float, vertical: Float, dt: Float) {
        // Yaw (turn left/right)
        rotY += horizontal * 100f * dt

        // Pitch / Elevation (move up/down)
        vy += vertical * 5f * dt
        // Clamp vertical speed
        if (vy > 5f) vy = 5f
        if (vy < -5f) vy = -5f

        // Apply friction to vertical speed so ship returns to plane
        if (vertical == 0f) {
            vy += -vy * 4f * dt
        }
    }

    fun fireLaser(pool: List<Laser>): Boolean {
        if (fireCooldown <= 0f) {
            // Find an inactive laser in the pool
            val laser = pool.find { !it.active }
            if (laser != null) {
                val angleRad = Math.toRadians(rotY.toDouble())
                // Project laser slightly in front of the ship
                val startX = x - (Math.sin(angleRad) * 1.5f).toFloat()
                val startY = y
                val startZ = z - (Math.cos(angleRad) * 1.5f).toFloat()

                // Laser speed = ship speed + forward velocity
                val laserSpeed = 25f
                val lvx = (-Math.sin(angleRad) * laserSpeed).toFloat()
                val lvz = (-Math.cos(angleRad) * laserSpeed).toFloat()

                laser.fire(startX, startY, startZ, lvx, vy, lvz)
                fireCooldown = fireRate
                return true
            }
        }
        return false
    }

    fun takeDamage(damage: Float) {
        if (shield > 0f) {
            shield -= damage
            if (shield < 0f) {
                hull += shield // Subtract excess damage from hull
                shield = 0f
            }
        } else {
            hull -= damage
            if (hull < 0f) hull = 0f
        }
    }
}
