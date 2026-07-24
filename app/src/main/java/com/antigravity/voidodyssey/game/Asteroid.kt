package com.antigravity.voidodyssey.game

/**
 * Represents an asteroid entity that can be shot and mined.
 */
class Asteroid {
    var x = 0f
    var y = 0f
    var z = 0f

    var rotY = 0f
    var rotSpeed = 0f

    var scale = 1f
    var radius = 1f
    var active = false
    var health = 10f

    fun spawn(startX: Float, startY: Float, startZ: Float, size: Float) {
        x = startX
        y = startY
        z = startZ
        scale = size
        radius = size * 0.8f // Simple bounding sphere radius
        active = true
        health = size * 10f
        rotY = (Math.random() * 360).toFloat()
        rotSpeed = (20 + Math.random() * 40).toFloat()
    }

    fun update(dt: Float) {
        if (!active) return
        rotY += rotSpeed * dt
        if (rotY >= 360f) rotY -= 360f
    }

    fun takeDamage(damage: Float): Boolean {
        if (!active) return false
        health -= damage
        if (health <= 0f) {
            active = false
            return true // Destroyed
        }
        return false
    }
}
