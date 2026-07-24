package com.antigravity.voidodyssey.game;

/**
 * Represents an asteroid entity that can be shot and mined.
 */
@kotlin.Metadata(mv = {1, 9, 0}, k = 1, xi = 48, d1 = {"\u0000$\n\u0002\u0018\u0002\n\u0002\u0010\u0000\n\u0002\b\u0002\n\u0002\u0010\u000b\n\u0002\b\u0005\n\u0002\u0010\u0007\n\u0002\b\u001a\n\u0002\u0010\u0002\n\u0002\b\t\u0018\u00002\u00020\u0001B\u0005\u00a2\u0006\u0002\u0010\u0002J&\u0010$\u001a\u00020%2\u0006\u0010&\u001a\u00020\n2\u0006\u0010\'\u001a\u00020\n2\u0006\u0010(\u001a\u00020\n2\u0006\u0010)\u001a\u00020\nJ\u000e\u0010*\u001a\u00020\u00042\u0006\u0010+\u001a\u00020\nJ\u000e\u0010,\u001a\u00020%2\u0006\u0010-\u001a\u00020\nR\u001a\u0010\u0003\u001a\u00020\u0004X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u0005\u0010\u0006\"\u0004\b\u0007\u0010\bR\u001a\u0010\t\u001a\u00020\nX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u000b\u0010\f\"\u0004\b\r\u0010\u000eR\u001a\u0010\u000f\u001a\u00020\nX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u0010\u0010\f\"\u0004\b\u0011\u0010\u000eR\u001a\u0010\u0012\u001a\u00020\nX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u0013\u0010\f\"\u0004\b\u0014\u0010\u000eR\u001a\u0010\u0015\u001a\u00020\nX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u0016\u0010\f\"\u0004\b\u0017\u0010\u000eR\u001a\u0010\u0018\u001a\u00020\nX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u0019\u0010\f\"\u0004\b\u001a\u0010\u000eR\u001a\u0010\u001b\u001a\u00020\nX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u001c\u0010\f\"\u0004\b\u001d\u0010\u000eR\u001a\u0010\u001e\u001a\u00020\nX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u001f\u0010\f\"\u0004\b \u0010\u000eR\u001a\u0010!\u001a\u00020\nX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\"\u0010\f\"\u0004\b#\u0010\u000e\u00a8\u0006."}, d2 = {"Lcom/antigravity/voidodyssey/game/Asteroid;", "", "()V", "active", "", "getActive", "()Z", "setActive", "(Z)V", "health", "", "getHealth", "()F", "setHealth", "(F)V", "radius", "getRadius", "setRadius", "rotSpeed", "getRotSpeed", "setRotSpeed", "rotY", "getRotY", "setRotY", "scale", "getScale", "setScale", "x", "getX", "setX", "y", "getY", "setY", "z", "getZ", "setZ", "spawn", "", "startX", "startY", "startZ", "size", "takeDamage", "damage", "update", "dt", "app_debug"})
public final class Asteroid {
    private float x = 0.0F;
    private float y = 0.0F;
    private float z = 0.0F;
    private float rotY = 0.0F;
    private float rotSpeed = 0.0F;
    private float scale = 1.0F;
    private float radius = 1.0F;
    private boolean active = false;
    private float health = 10.0F;
    
    public Asteroid() {
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
    
    public final float getRotSpeed() {
        return 0.0F;
    }
    
    public final void setRotSpeed(float p0) {
    }
    
    public final float getScale() {
        return 0.0F;
    }
    
    public final void setScale(float p0) {
    }
    
    public final float getRadius() {
        return 0.0F;
    }
    
    public final void setRadius(float p0) {
    }
    
    public final boolean getActive() {
        return false;
    }
    
    public final void setActive(boolean p0) {
    }
    
    public final float getHealth() {
        return 0.0F;
    }
    
    public final void setHealth(float p0) {
    }
    
    public final void spawn(float startX, float startY, float startZ, float size) {
    }
    
    public final void update(float dt) {
    }
    
    public final boolean takeDamage(float damage) {
        return false;
    }
}