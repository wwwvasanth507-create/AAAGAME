package com.antigravity.voidodyssey

import com.antigravity.voidodyssey.game.Asteroid
import com.antigravity.voidodyssey.game.Laser
import com.antigravity.voidodyssey.game.PlayerShip
import org.junit.Assert.*
import org.junit.Test

class GameLogicUnitTest {

    @Test
    fun testPlayerShipInitialPosition() {
        val ship = PlayerShip()
        assertEquals(0f, ship.x, 0.001f)
        assertEquals(0f, ship.y, 0.001f)
        assertEquals(0f, ship.z, 0.001f)
        assertEquals(100f, ship.hull, 0.001f)
        assertEquals(50f, ship.shield, 0.001f)
    }

    @Test
    fun testPlayerShipMovementAndSteering() {
        val ship = PlayerShip()
        ship.targetSpeed = 10f
        
        // Update physics by 1 second (ship should accelerate towards 10m/s)
        ship.update(1.0f)
        assertTrue(ship.currentSpeed > 0f)
        assertTrue(ship.z < 0f) // Facing forward initially (rotY = 0) moves in negative Z

        // Yaw ship to the right (rotY = 90 degrees)
        ship.steer(0.9f, 0f, 1.0f) // Yaw 90 degrees/s
        assertEquals(90f, ship.rotY, 0.01f)

        // Reset speed and update: moving at rotY=90 should move ship in negative X direction
        ship.currentSpeed = 10f
        val oldX = ship.x
        ship.update(0.1f)
        assertTrue(ship.x < oldX)
    }

    @Test
    fun testLaserFiringCooldown() {
        val ship = PlayerShip()
        val laserPool = List(5) { Laser() }

        // Initial fire should succeed
        val fired1 = ship.fireLaser(laserPool)
        assertTrue(fired1)
        assertTrue(laserPool[0].active)
        assertTrue(ship.fireCooldown > 0f)

        // Immediate fire should fail due to cooldown
        val fired2 = ship.fireLaser(laserPool)
        assertFalse(fired2)

        // Progress cooldown and try again
        ship.update(0.3f) // Cooldown is 0.2s, so 0.3s clears it
        val fired3 = ship.fireLaser(laserPool)
        assertTrue(fired3)
        assertTrue(laserPool[1].active)
    }

    @Test
    fun testShipDamageResolution() {
        val ship = PlayerShip()
        ship.shield = 30f
        ship.hull = 100f

        // Damage within shield capacity
        ship.takeDamage(10f)
        assertEquals(20f, ship.shield, 0.001f)
        assertEquals(100f, ship.hull, 0.001f)

        // Damage exceeding shield capacity (should hit hull)
        ship.takeDamage(35f)
        assertEquals(0f, ship.shield, 0.001f)
        assertEquals(85f, ship.hull, 0.001f)
    }

    @Test
    fun testSphereCollisionIntersection() {
        val asteroid = Asteroid()
        asteroid.spawn(0f, 0f, -10f, 2f) // Radius is size * 0.8 = 1.6f
        
        val laser = Laser()
        laser.fire(0f, 0f, -9.5f, 0f, 0f, -20f) // Active, very close to asteroid center

        val dx = laser.x - asteroid.x
        val dy = laser.y - asteroid.y
        val dz = laser.z - asteroid.z
        val distSq = dx*dx + dy*dy + dz*dz

        val collisionDist = asteroid.radius + 0.2f // asteroid radius + laser radius offset
        assertTrue(distSq < collisionDist * collisionDist) // Collision matches
    }
}
