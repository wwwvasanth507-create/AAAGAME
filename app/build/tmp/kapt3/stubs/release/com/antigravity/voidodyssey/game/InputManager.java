package com.antigravity.voidodyssey.game;

/**
 * Manages touch gestures, virtual joysticks, throttle sliders, and fire button states.
 */
@kotlin.Metadata(mv = {1, 9, 0}, k = 1, xi = 48, d1 = {"\u00002\n\u0002\u0018\u0002\n\u0002\u0010\u0000\n\u0002\b\u0002\n\u0002\u0010\u000b\n\u0002\b\u0007\n\u0002\u0010\u0007\n\u0002\b\u000e\n\u0002\u0010\b\n\u0002\b\u0006\n\u0002\u0018\u0002\n\u0000\n\u0002\u0010\u0002\n\u0002\b\u0003\u0018\u00002\u00020\u0001B\u0005\u00a2\u0006\u0002\u0010\u0002J\u000e\u0010 \u001a\u00020\u00042\u0006\u0010!\u001a\u00020\"J\u0016\u0010#\u001a\u00020$2\u0006\u0010%\u001a\u00020\u001b2\u0006\u0010&\u001a\u00020\u001bR\u001a\u0010\u0003\u001a\u00020\u0004X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u0003\u0010\u0005\"\u0004\b\u0006\u0010\u0007R\u001a\u0010\b\u001a\u00020\u0004X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\t\u0010\u0005\"\u0004\b\n\u0010\u0007R\u001a\u0010\u000b\u001a\u00020\fX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\r\u0010\u000e\"\u0004\b\u000f\u0010\u0010R\u001a\u0010\u0011\u001a\u00020\fX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u0012\u0010\u000e\"\u0004\b\u0013\u0010\u0010R\u001a\u0010\u0014\u001a\u00020\fX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u0015\u0010\u000e\"\u0004\b\u0016\u0010\u0010R\u001a\u0010\u0017\u001a\u00020\fX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u0018\u0010\u000e\"\u0004\b\u0019\u0010\u0010R\u000e\u0010\u001a\u001a\u00020\u001bX\u0082\u000e\u00a2\u0006\u0002\n\u0000R\u000e\u0010\u001c\u001a\u00020\u001bX\u0082\u000e\u00a2\u0006\u0002\n\u0000R\u001a\u0010\u001d\u001a\u00020\fX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u001e\u0010\u000e\"\u0004\b\u001f\u0010\u0010\u00a8\u0006\'"}, d2 = {"Lcom/antigravity/voidodyssey/game/InputManager;", "", "()V", "isFiring", "", "()Z", "setFiring", "(Z)V", "joystickActive", "getJoystickActive", "setJoystickActive", "joystickStartX", "", "getJoystickStartX", "()F", "setJoystickStartX", "(F)V", "joystickStartY", "getJoystickStartY", "setJoystickStartY", "joystickX", "getJoystickX", "setJoystickX", "joystickY", "getJoystickY", "setJoystickY", "screenHeight", "", "screenWidth", "throttle", "getThrottle", "setThrottle", "handleTouchEvent", "event", "Landroid/view/MotionEvent;", "updateScreenSize", "", "width", "height", "app_release"})
public final class InputManager {
    private boolean joystickActive = false;
    private float joystickStartX = 0.0F;
    private float joystickStartY = 0.0F;
    private float joystickX = 0.0F;
    private float joystickY = 0.0F;
    private float throttle = 0.0F;
    private boolean isFiring = false;
    private int screenWidth = 0;
    private int screenHeight = 0;
    
    public InputManager() {
        super();
    }
    
    public final boolean getJoystickActive() {
        return false;
    }
    
    public final void setJoystickActive(boolean p0) {
    }
    
    public final float getJoystickStartX() {
        return 0.0F;
    }
    
    public final void setJoystickStartX(float p0) {
    }
    
    public final float getJoystickStartY() {
        return 0.0F;
    }
    
    public final void setJoystickStartY(float p0) {
    }
    
    public final float getJoystickX() {
        return 0.0F;
    }
    
    public final void setJoystickX(float p0) {
    }
    
    public final float getJoystickY() {
        return 0.0F;
    }
    
    public final void setJoystickY(float p0) {
    }
    
    public final float getThrottle() {
        return 0.0F;
    }
    
    public final void setThrottle(float p0) {
    }
    
    public final boolean isFiring() {
        return false;
    }
    
    public final void setFiring(boolean p0) {
    }
    
    public final void updateScreenSize(int width, int height) {
    }
    
    public final boolean handleTouchEvent(@org.jetbrains.annotations.NotNull()
    android.view.MotionEvent event) {
        return false;
    }
}