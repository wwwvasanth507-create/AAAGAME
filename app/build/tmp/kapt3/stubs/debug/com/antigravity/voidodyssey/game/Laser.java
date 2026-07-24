package com.antigravity.voidodyssey.game;

/**
 * Represents a laser projectile in the game.
 * Uses pooling to optimize memory and GC.
 */
@kotlin.Metadata(mv = {1, 9, 0}, k = 1, xi = 48, d1 = {"\u0000$\n\u0002\u0018\u0002\n\u0002\u0010\u0000\n\u0002\b\u0002\n\u0002\u0010\u000b\n\u0002\b\u0005\n\u0002\u0010\u0007\n\u0002\b\u0018\n\u0002\u0010\u0002\n\u0002\b\t\u0018\u00002\u00020\u0001B\u0005\u00a2\u0006\u0002\u0010\u0002J6\u0010\"\u001a\u00020#2\u0006\u0010$\u001a\u00020\n2\u0006\u0010%\u001a\u00020\n2\u0006\u0010&\u001a\u00020\n2\u0006\u0010\'\u001a\u00020\n2\u0006\u0010(\u001a\u00020\n2\u0006\u0010)\u001a\u00020\nJ\u000e\u0010*\u001a\u00020#2\u0006\u0010+\u001a\u00020\nR\u001a\u0010\u0003\u001a\u00020\u0004X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u0005\u0010\u0006\"\u0004\b\u0007\u0010\bR\u001a\u0010\t\u001a\u00020\nX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u000b\u0010\f\"\u0004\b\r\u0010\u000eR\u000e\u0010\u000f\u001a\u00020\nX\u0082D\u00a2\u0006\u0002\n\u0000R\u001a\u0010\u0010\u001a\u00020\nX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u0011\u0010\f\"\u0004\b\u0012\u0010\u000eR\u001a\u0010\u0013\u001a\u00020\nX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u0014\u0010\f\"\u0004\b\u0015\u0010\u000eR\u001a\u0010\u0016\u001a\u00020\nX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u0017\u0010\f\"\u0004\b\u0018\u0010\u000eR\u001a\u0010\u0019\u001a\u00020\nX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u001a\u0010\f\"\u0004\b\u001b\u0010\u000eR\u001a\u0010\u001c\u001a\u00020\nX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u001d\u0010\f\"\u0004\b\u001e\u0010\u000eR\u001a\u0010\u001f\u001a\u00020\nX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b \u0010\f\"\u0004\b!\u0010\u000e\u00a8\u0006,"}, d2 = {"Lcom/antigravity/voidodyssey/game/Laser;", "", "()V", "active", "", "getActive", "()Z", "setActive", "(Z)V", "distanceTraveled", "", "getDistanceTraveled", "()F", "setDistanceTraveled", "(F)V", "maxDistance", "vx", "getVx", "setVx", "vy", "getVy", "setVy", "vz", "getVz", "setVz", "x", "getX", "setX", "y", "getY", "setY", "z", "getZ", "setZ", "fire", "", "startX", "startY", "startZ", "velX", "velY", "velZ", "update", "dt", "app_debug"})
public final class Laser {
    private float x = 0.0F;
    private float y = 0.0F;
    private float z = 0.0F;
    private float vx = 0.0F;
    private float vy = 0.0F;
    private float vz = 0.0F;
    private boolean active = false;
    private float distanceTraveled = 0.0F;
    private final float maxDistance = 40.0F;
    
    public Laser() {
        super();
    }
    
    public final float getX() {
        return 0.0F;
    }
    
    public final void setX(float p0) {
    }
    
    public final float getY() {
        return 0.0F;
    }
    
    public final void setY(float p0) {
    }
    
    public final float getZ() {
        return 0.0F;
    }
    
    public final void setZ(float p0) {
    }
    
    public final float getVx() {
        return 0.0F;
    }
    
    public final void setVx(float p0) {
    }
    
    public final float getVy() {
        return 0.0F;
    }
    
    public final void setVy(float p0) {
    }
    
    public final float getVz() {
        return 0.0F;
    }
    
    public final void setVz(float p0) {
    }
    
    public final boolean getActive() {
        return false;
    }
    
    public final void setActive(boolean p0) {
    }
    
    public final float getDistanceTraveled() {
        return 0.0F;
    }
    
    public final void setDistanceTraveled(float p0) {
    }
    
    public final void fire(float startX, float startY, float startZ, float velX, float velY, float velZ) {
    }
    
    public final void update(float dt) {
    }
}