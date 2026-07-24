package com.antigravity.voidodyssey.game;

/**
 * Thread-safe fixed-timestep game update loop that triggers physics, collisions, and state changes.
 */
@kotlin.Metadata(mv = {1, 9, 0}, k = 1, xi = 48, d1 = {"\u0000d\n\u0002\u0018\u0002\n\u0002\u0018\u0002\n\u0000\n\u0002\u0018\u0002\n\u0000\n\u0002\u0018\u0002\n\u0000\n\u0002\u0018\u0002\n\u0002\b\u0002\n\u0002\u0010\b\n\u0002\b\u0005\n\u0002\u0010 \n\u0002\u0018\u0002\n\u0002\b\u0003\n\u0002\u0018\u0002\n\u0000\n\u0002\u0018\u0002\n\u0000\n\u0002\u0018\u0002\n\u0002\b\u0005\n\u0002\u0018\u0002\n\u0002\b\u0003\n\u0002\u0018\u0002\n\u0002\b\u0004\n\u0002\u0010\u0002\n\u0002\b\t\n\u0002\u0010\u0007\n\u0000\u0018\u00002\u00020\u0001B\u001d\u0012\u0006\u0010\u0002\u001a\u00020\u0003\u0012\u0006\u0010\u0004\u001a\u00020\u0005\u0012\u0006\u0010\u0006\u001a\u00020\u0007\u00a2\u0006\u0002\u0010\bJ\b\u0010\'\u001a\u00020(H\u0002J\u0006\u0010)\u001a\u00020(J\b\u0010*\u001a\u00020(H\u0002J\b\u0010+\u001a\u00020(H\u0002J\b\u0010,\u001a\u00020(H\u0016J\u0006\u0010-\u001a\u00020(J\u0006\u0010.\u001a\u00020(J\u0006\u0010/\u001a\u00020(J\u0010\u00100\u001a\u00020(2\u0006\u00101\u001a\u000202H\u0002R\u001a\u0010\t\u001a\u00020\nX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u000b\u0010\f\"\u0004\b\r\u0010\u000eR\u0017\u0010\u000f\u001a\b\u0012\u0004\u0012\u00020\u00110\u0010\u00a2\u0006\b\n\u0000\u001a\u0004\b\u0012\u0010\u0013R\u000e\u0010\u0002\u001a\u00020\u0003X\u0082\u0004\u00a2\u0006\u0002\n\u0000R\u000e\u0010\u0014\u001a\u00020\u0015X\u0082\u0004\u00a2\u0006\u0002\n\u0000R\u0010\u0010\u0016\u001a\u0004\u0018\u00010\u0017X\u0082\u000e\u00a2\u0006\u0002\n\u0000R\u000e\u0010\u0006\u001a\u00020\u0007X\u0082\u0004\u00a2\u0006\u0002\n\u0000R\u0017\u0010\u0018\u001a\b\u0012\u0004\u0012\u00020\u00190\u0010\u00a2\u0006\b\n\u0000\u001a\u0004\b\u001a\u0010\u0013R\u001a\u0010\u001b\u001a\u00020\nX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u001c\u0010\f\"\u0004\b\u001d\u0010\u000eR\u0011\u0010\u001e\u001a\u00020\u001f\u00a2\u0006\b\n\u0000\u001a\u0004\b \u0010!R\u000e\u0010\u0004\u001a\u00020\u0005X\u0082\u0004\u00a2\u0006\u0002\n\u0000R\u000e\u0010\"\u001a\u00020#X\u0082\u0004\u00a2\u0006\u0002\n\u0000R\u001a\u0010$\u001a\u00020\nX\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b%\u0010\f\"\u0004\b&\u0010\u000e\u00a8\u00063"}, d2 = {"Lcom/antigravity/voidodyssey/game/GameLoop;", "Ljava/lang/Runnable;", "context", "Landroid/content/Context;", "renderer", "Lcom/antigravity/voidodyssey/core/GLES30Renderer;", "inputManager", "Lcom/antigravity/voidodyssey/game/InputManager;", "(Landroid/content/Context;Lcom/antigravity/voidodyssey/core/GLES30Renderer;Lcom/antigravity/voidodyssey/game/InputManager;)V", "activeSlotId", "", "getActiveSlotId", "()I", "setActiveSlotId", "(I)V", "asteroids", "", "Lcom/antigravity/voidodyssey/game/Asteroid;", "getAsteroids", "()Ljava/util/List;", "db", "Lcom/antigravity/voidodyssey/db/GameDatabase;", "gameThread", "Ljava/lang/Thread;", "lasers", "Lcom/antigravity/voidodyssey/game/Laser;", "getLasers", "oreMined", "getOreMined", "setOreMined", "playerShip", "Lcom/antigravity/voidodyssey/game/PlayerShip;", "getPlayerShip", "()Lcom/antigravity/voidodyssey/game/PlayerShip;", "running", "Ljava/util/concurrent/atomic/AtomicBoolean;", "score", "getScore", "setScore", "checkCollisions", "", "loadSavedProfile", "resetWorld", "respawnMissingAsteroids", "run", "saveProfile", "start", "stop", "update", "dt", "", "app_release"})
public final class GameLoop implements java.lang.Runnable {
    @org.jetbrains.annotations.NotNull()
    private final android.content.Context context = null;
    @org.jetbrains.annotations.NotNull()
    private final com.antigravity.voidodyssey.core.GLES30Renderer renderer = null;
    @org.jetbrains.annotations.NotNull()
    private final com.antigravity.voidodyssey.game.InputManager inputManager = null;
    @org.jetbrains.annotations.NotNull()
    private final com.antigravity.voidodyssey.db.GameDatabase db = null;
    @org.jetbrains.annotations.NotNull()
    private final com.antigravity.voidodyssey.game.PlayerShip playerShip = null;
    @org.jetbrains.annotations.NotNull()
    private final java.util.List<com.antigravity.voidodyssey.game.Laser> lasers = null;
    @org.jetbrains.annotations.NotNull()
    private final java.util.List<com.antigravity.voidodyssey.game.Asteroid> asteroids = null;
    @org.jetbrains.annotations.NotNull()
    private final java.util.concurrent.atomic.AtomicBoolean running = null;
    @org.jetbrains.annotations.Nullable()
    private java.lang.Thread gameThread;
    private int score = 0;
    private int oreMined = 0;
    private int activeSlotId = 1;
    
    public GameLoop(@org.jetbrains.annotations.NotNull()
    android.content.Context context, @org.jetbrains.annotations.NotNull()
    com.antigravity.voidodyssey.core.GLES30Renderer renderer, @org.jetbrains.annotations.NotNull()
    com.antigravity.voidodyssey.game.InputManager inputManager) {
        super();
    }
    
    @org.jetbrains.annotations.NotNull()
    public final com.antigravity.voidodyssey.game.PlayerShip getPlayerShip() {
        return null;
    }
    
    @org.jetbrains.annotations.NotNull()
    public final java.util.List<com.antigravity.voidodyssey.game.Laser> getLasers() {
        return null;
    }
    
    @org.jetbrains.annotations.NotNull()
    public final java.util.List<com.antigravity.voidodyssey.game.Asteroid> getAsteroids() {
        return null;
    }
    
    public final int getScore() {
        return 0;
    }
    
    public final void setScore(int p0) {
    }
    
    public final int getOreMined() {
        return 0;
    }
    
    public final void setOreMined(int p0) {
    }
    
    public final int getActiveSlotId() {
        return 0;
    }
    
    public final void setActiveSlotId(int p0) {
    }
    
    private final void resetWorld() {
    }
    
    public final void start() {
    }
    
    public final void stop() {
    }
    
    @java.lang.Override()
    public void run() {
    }
    
    private final void update(float dt) {
    }
    
    private final void checkCollisions() {
    }
    
    private final void respawnMissingAsteroids() {
    }
    
    public final void loadSavedProfile() {
    }
    
    public final void saveProfile() {
    }
}