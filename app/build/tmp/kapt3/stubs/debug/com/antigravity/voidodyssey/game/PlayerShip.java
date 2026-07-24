package com.antigravity.voidodyssey.game;

/**
 * Represents the Player's Spaceship entity.
 */
@kotlin.Metadata(mv = {1, 9, 0}, k = 1, xi = 48, d1 = {"\u0000,\n\u0002\u0018\u0002\n\u0002\u0010\u0000\n\u0002\b\u0002\n\u0002\u0010\u0007\n\u0002\b-\n\u0002\u0010\u000b\n\u0000\n\u0002\u0010 \n\u0002\u0018\u0002\n\u0000\n\u0002\u0010\u0002\n\u0002\b\u0007\u0018\u00002\u00020\u0001B\u0005\u00a2\u0006\u0002\u0010\u0002J\u0014\u00101\u001a\u0002022\f\u00103\u001a\b\u0012\u0004\u0012\u00020504J\u001e\u00106\u001a\u0002072\u0006\u00108\u001a\u00020\u00042\u0006\u00109\u001a\u00020\u00042\u0006\u0010:\u001a\u00020\u0004J\u000e\u0010;\u001a\u0002072\u0006\u0010<\u001a\u00020\u0004J\u000e\u0010=\u001a\u0002072\u0006\u0010:\u001a\u00020\u0004R\u000e\u0010\u0003\u001a\u00020\u0004X\u0082D\u00a2\u0006\u0002\n\u0000R\u001a\u0010\u0005\u001a\u00020\u0004X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u0006\u0010\u0007\"\u0004\b\b\u0010\tR\u001a\u0010\n\u001a\u00020\u0004X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u000b\u0010\u0007\"\u0004\b\f\u0010\tR\u000e\u0010\r\u001a\u00020\u0004X\u0082D\u00a2\u0006\u0002\n\u0000R\u001a\u0010\u000e\u001a\u00020\u0004X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u000f\u0010\u0007\"\u0004\b\u0010\u0010\tR\u0014\u0010\u0011\u001a\u00020\u0004X\u0086D\u00a2\u0006\b\n\u0000\u001a\u0004\b\u0012\u0010\u0007R\u0014\u0010\u0013\u001a\u00020\u0004X\u0086D\u00a2\u0006\b\n\u0000\u001a\u0004\b\u0014\u0010\u0007R\u000e\u0010\u0015\u001a\u00020\u0004X\u0082D\u00a2\u0006\u0002\n\u0000R\u001a\u0010\u0016\u001a\u00020\u0004X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u0017\u0010\u0007\"\u0004\b\u0018\u0010\tR\u001a\u0010\u0019\u001a\u00020\u0004X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u001a\u0010\u0007\"\u0004\b\u001b\u0010\tR\u001a\u0010\u001c\u001a\u00020\u0004X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u001d\u0010\u0007\"\u0004\b\u001e\u0010\tR\u001a\u0010\u001f\u001a\u00020\u0004X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b \u0010\u0007\"\u0004\b!\u0010\tR\u001a\u0010\"\u001a\u00020\u0004X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b#\u0010\u0007\"\u0004\b$\u0010\tR\u001a\u0010%\u001a\u00020\u0004X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b&\u0010\u0007\"\u0004\b\'\u0010\tR\u001a\u0010(\u001a\u00020\u0004X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b)\u0010\u0007\"\u0004\b*\u0010\tR\u001a\u0010+\u001a\u00020\u0004X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b,\u0010\u0007\"\u0004\b-\u0010\tR\u001a\u0010.\u001a\u00020\u0004X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b/\u0010\u0007\"\u0004\b0\u0010\t\u00a8\u0006>"}, d2 = {"Lcom/antigravity/voidodyssey/game/PlayerShip;", "", "()V", "acceleration", "", "currentSpeed", "getCurrentSpeed", "()F", "setCurrentSpeed", "(F)V", "fireCooldown", "getFireCooldown", "setFireCooldown", "fireRate", "hull", "getHull", "setHull", "maxHull", "getMaxHull", "maxShield", "getMaxShield", "maxSpeed", "rotY", "getRotY", "setRotY", "shield", "getShield", "setShield", "targetSpeed", "getTargetSpeed", "setTargetSpeed", "vx", "getVx", "setVx", "vy", "getVy", "setVy", "vz", "getVz", "setVz", "x", "getX", "setX", "y", "getY", "setY", "z", "getZ", "setZ", "fireLaser", "", "pool", "", "Lcom/antigravity/voidodyssey/game/Laser;", "steer", "", "horizontal", "vertical", "dt", "takeDamage", "damage", "update", "app_debug"})
public final class PlayerShip {
    private float x = 0.0F;
    private float y = 0.0F;
    private float z = 0.0F;
    private float rotY = 0.0F;
    private float vx = 0.0F;
    private float vy = 0.0F;
    private float vz = 0.0F;
    private float hull = 100.0F;
    private final float maxHull = 100.0F;
    private float shield = 50.0F;
    private final float maxShield = 50.0F;
    private float currentSpeed = 0.0F;
    private float targetSpeed = 0.0F;
    private final float acceleration = 8.0F;
    private final float maxSpeed = 15.0F;
    private float fireCooldown = 0.0F;
    private final float fireRate = 0.2F;
    
    public PlayerShip() {
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
    
    public final float getRotY() {
        return 0.0F;
    }
    
    public final void setRotY(float p0) {
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
    
    public final float getHull() {
        return 0.0F;
    }
    
    public final void setHull(float p0) {
    }
    
    public final float getMaxHull() {
        return 0.0F;
    }
    
    public final float getShield() {
        return 0.0F;
    }
    
    public final void setShield(float p0) {
    }
    
    public final float getMaxShield() {
        return 0.0F;
    }
    
    public final float getCurrentSpeed() {
        return 0.0F;
    }
    
    public final void setCurrentSpeed(float p0) {
    }
    
    public final float getTargetSpeed() {
        return 0.0F;
    }
    
    public final void setTargetSpeed(float p0) {
    }
    
    public final float getFireCooldown() {
        return 0.0F;
    }
    
    public final void setFireCooldown(float p0) {
    }
    
    public final void update(float dt) {
    }
    
    public final void steer(float horizontal, float vertical, float dt) {
    }
    
    public final boolean fireLaser(@org.jetbrains.annotations.NotNull()
    java.util.List<com.antigravity.voidodyssey.game.Laser> pool) {
        return false;
    }
    
    public final void takeDamage(float damage) {
    }
}