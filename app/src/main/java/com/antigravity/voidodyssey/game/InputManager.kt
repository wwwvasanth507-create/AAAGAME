package com.antigravity.voidodyssey.game

import android.view.MotionEvent

/**
 * Manages touch gestures, virtual joysticks, throttle sliders, and fire button states.
 */
class InputManager {

    // Virtual Joystick properties
    var joystickActive = false
    var joystickStartX = 0f
    var joystickStartY = 0f
    var joystickX = 0f // Normalized delta X [-1, 1]
    var joystickY = 0f // Normalized delta Y [-1, 1]

    // Throttle / Acceleration slider
    var throttle = 0f // [0, 1] range

    // Fire button
    var isFiring = false

    // Screen dimensions to check hitboxes
    private var screenWidth = 0
    private var screenHeight = 0

    fun updateScreenSize(width: Int, height: Int) {
        screenWidth = width
        screenHeight = height
    }

    fun handleTouchEvent(event: MotionEvent): Boolean {
        val action = event.actionMasked
        val pointerCount = event.pointerCount

        // We can support multi-touch for steering while firing
        isFiring = false

        for (i in 0 until pointerCount) {
            val pointerId = event.getPointerId(i)
            val x = event.getX(i)
            val y = event.getY(i)

            // Let's divide screen layout:
            // Left 40% of screen: virtual joystick
            // Right 30% of screen: throttle slider
            // Far Right 30% bottom: fire button
            val isLeftQuarter = x < screenWidth * 0.4f
            val isRightMiddle = x >= screenWidth * 0.4f && x < screenWidth * 0.75f
            val isFarRight = x >= screenWidth * 0.75f

            when (action) {
                MotionEvent.ACTION_DOWN, MotionEvent.ACTION_POINTER_DOWN -> {
                    if (isLeftQuarter) {
                        joystickActive = true
                        joystickStartX = x
                        joystickStartY = y
                        joystickX = 0f
                        joystickY = 0f
                    } else if (isFarRight) {
                        isFiring = true
                    }
                }
                MotionEvent.ACTION_MOVE -> {
                    if (joystickActive && isLeftQuarter) {
                        val dx = x - joystickStartX
                        val dy = y - joystickStartY
                        val maxDistance = 150f // Pixels limit for full tilt
                        
                        val distance = Math.sqrt((dx * dx + dy * dy).toDouble()).toFloat()
                        if (distance > 0f) {
                            val clampDistance = Math.min(distance, maxDistance)
                            joystickX = (dx / distance) * (clampDistance / maxDistance)
                            joystickY = (dy / distance) * (clampDistance / maxDistance)
                        }
                    }

                    if (isRightMiddle) {
                        // Throttle slider height-based input [0, 1]
                        val relativeY = 1.0f - (y / screenHeight.toFloat()).coerceIn(0.0f, 1.0f)
                        throttle = relativeY
                    }
                }
                MotionEvent.ACTION_UP, MotionEvent.ACTION_POINTER_UP, MotionEvent.ACTION_CANCEL -> {
                    if (isLeftQuarter) {
                        joystickActive = false
                        joystickX = 0f
                        joystickY = 0f
                    }
                }
            }
        }
        return true
    }
}
