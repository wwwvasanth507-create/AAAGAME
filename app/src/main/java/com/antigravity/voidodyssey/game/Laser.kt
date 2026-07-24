package com.antigravity.voidodyssey.game

/**
 * Represents a laser projectile in the game.
 * Uses pooling to optimize memory and GC.
 */
class Laser {
    var x = 0f
    var y = 0f
    var z = 0f

    var vx = 0f
    var vy = 0f
    var vz = 0f

    var active = false
    var distanceTraveled = 0f
    private val maxDistance = 40f

    fun fire(startX: Float, startY: Float, startZ: Float, velX: Float, velY: Float, velZ: Float) {
        x = startX
        y = startY
        z = startZ
        vx = velX
        vy = velY
        vz = velZ
        active = true
        distanceTraveled = 0f
    }

    fun update(dt: Float) {
        if (!active) return

        val dx = vx * dt
        val dy = vy * dt
        val dz = vz * dt

        x += dx
        y += dy
        z += dz

        distanceTraveled += Math.sqrt((dx * dx + dy * dy + dz * dz).toDouble()).toFloat()
        if (distanceTraveled >= maxDistance) {
            active = false
        }
    }
}
