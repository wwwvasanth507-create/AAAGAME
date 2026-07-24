package com.antigravity.voidodyssey.core;

/**
 * Handles the actual OpenGL ES 3.0 graphics rendering logic.
 */
@kotlin.Metadata(mv = {1, 9, 0}, k = 1, xi = 48, d1 = {"\u0000\u0086\u0001\n\u0002\u0018\u0002\n\u0002\u0018\u0002\n\u0000\n\u0002\u0018\u0002\n\u0000\n\u0002\u0018\u0002\n\u0002\b\u0002\n\u0002\u0010\b\n\u0002\b\u0002\n\u0002\u0018\u0002\n\u0002\b\b\n\u0002\u0010\u0007\n\u0002\b\r\n\u0002\u0010\u0014\n\u0002\b\u0003\n\u0002\u0018\u0002\n\u0002\b\u0005\n\u0002\u0010 \n\u0002\u0018\u0002\n\u0000\n\u0002\u0018\u0002\n\u0000\n\u0002\u0018\u0002\n\u0002\b\u0005\n\u0002\u0018\u0002\n\u0002\b\u0005\n\u0002\u0010\u0002\n\u0002\b\u0010\n\u0002\u0010\u000e\n\u0000\n\u0002\u0018\u0002\n\u0002\b\u0002\n\u0002\u0018\u0002\n\u0002\b\u0005\n\u0002\u0018\u0002\n\u0002\b\u0005\u0018\u00002\u00020\u0001B\u0015\u0012\u0006\u0010\u0002\u001a\u00020\u0003\u0012\u0006\u0010\u0004\u001a\u00020\u0005\u00a2\u0006\u0002\u0010\u0006J \u0010<\u001a\u00020=2\u0006\u0010>\u001a\u00020\b2\u0006\u0010?\u001a\u00020\b2\u0006\u0010@\u001a\u00020\bH\u0002J\b\u0010A\u001a\u00020\u000bH\u0002J\b\u0010B\u001a\u00020\u000bH\u0002J\b\u0010C\u001a\u00020\u000bH\u0002J\b\u0010D\u001a\u00020=H\u0002JP\u0010E\u001a\u00020=2\u0006\u0010F\u001a\u00020\u00142\u0006\u0010G\u001a\u00020\u00142\u0006\u0010H\u001a\u00020\u00142\u0006\u0010I\u001a\u00020\u00142\u0006\u0010J\u001a\u00020\u00142\u0006\u0010K\u001a\u00020\u000b2\u0006\u0010>\u001a\u00020\b2\u0006\u0010?\u001a\u00020\b2\u0006\u0010@\u001a\u00020\bH\u0002J\u001e\u0010L\u001a\u00020\u000b2\u0006\u0010M\u001a\u00020N2\f\u0010O\u001a\b\u0012\u0004\u0012\u00020\u000b0PH\u0002J\u0012\u0010Q\u001a\u00020=2\b\u0010R\u001a\u0004\u0018\u00010SH\u0016J\"\u0010T\u001a\u00020=2\b\u0010R\u001a\u0004\u0018\u00010S2\u0006\u0010U\u001a\u00020\b2\u0006\u0010V\u001a\u00020\bH\u0016J\u001c\u0010W\u001a\u00020=2\b\u0010R\u001a\u0004\u0018\u00010S2\b\u0010X\u001a\u0004\u0018\u00010YH\u0016J*\u0010Z\u001a\u00020=2\u0006\u0010[\u001a\u0002012\f\u0010\\\u001a\b\u0012\u0004\u0012\u00020-0,2\f\u0010]\u001a\b\u0012\u0004\u0012\u00020/0,R\u000e\u0010\u0007\u001a\u00020\bX\u0082\u000e\u00a2\u0006\u0002\n\u0000R\u000e\u0010\t\u001a\u00020\bX\u0082\u000e\u00a2\u0006\u0002\n\u0000R\u001e\u0010\f\u001a\u00020\u000b2\u0006\u0010\n\u001a\u00020\u000b@BX\u0086.\u00a2\u0006\b\n\u0000\u001a\u0004\b\r\u0010\u000eR\u000e\u0010\u000f\u001a\u00020\bX\u0082\u000e\u00a2\u0006\u0002\n\u0000R\u001e\u0010\u0010\u001a\u00020\u000b2\u0006\u0010\n\u001a\u00020\u000b@BX\u0086.\u00a2\u0006\b\n\u0000\u001a\u0004\b\u0011\u0010\u000eR\u000e\u0010\u0012\u001a\u00020\bX\u0082\u000e\u00a2\u0006\u0002\n\u0000R\u001a\u0010\u0013\u001a\u00020\u0014X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u0015\u0010\u0016\"\u0004\b\u0017\u0010\u0018R\u001a\u0010\u0019\u001a\u00020\u0014X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u001a\u0010\u0016\"\u0004\b\u001b\u0010\u0018R\u001a\u0010\u001c\u001a\u00020\u0014X\u0086\u000e\u00a2\u0006\u000e\n\u0000\u001a\u0004\b\u001d\u0010\u0016\"\u0004\b\u001e\u0010\u0018R\u000e\u0010\u0002\u001a\u00020\u0003X\u0082\u0004\u00a2\u0006\u0002\n\u0000R\u001e\u0010\u001f\u001a\u00020\u000b2\u0006\u0010\n\u001a\u00020\u000b@BX\u0086.\u00a2\u0006\b\n\u0000\u001a\u0004\b \u0010\u000eR\u000e\u0010!\u001a\u00020\"X\u0082\u0004\u00a2\u0006\u0002\n\u0000R\u000e\u0010#\u001a\u00020\"X\u0082\u0004\u00a2\u0006\u0002\n\u0000R\u000e\u0010$\u001a\u00020\"X\u0082\u0004\u00a2\u0006\u0002\n\u0000R\u000e\u0010%\u001a\u00020\"X\u0082\u0004\u00a2\u0006\u0002\n\u0000R\u001e\u0010\'\u001a\u00020&2\u0006\u0010\n\u001a\u00020&@BX\u0086.\u00a2\u0006\b\n\u0000\u001a\u0004\b(\u0010)R\u000e\u0010*\u001a\u00020\"X\u0082\u0004\u00a2\u0006\u0002\n\u0000R\u000e\u0010\u0004\u001a\u00020\u0005X\u0082\u0004\u00a2\u0006\u0002\n\u0000R\u0014\u0010+\u001a\b\u0012\u0004\u0012\u00020-0,X\u0082\u000e\u00a2\u0006\u0002\n\u0000R\u0014\u0010.\u001a\b\u0012\u0004\u0012\u00020/0,X\u0082\u000e\u00a2\u0006\u0002\n\u0000R\u0010\u00100\u001a\u0004\u0018\u000101X\u0082\u000e\u00a2\u0006\u0002\n\u0000R\u000e\u00102\u001a\u00020\bX\u0082\u000e\u00a2\u0006\u0002\n\u0000R\u000e\u00103\u001a\u00020\bX\u0082\u000e\u00a2\u0006\u0002\n\u0000R\u001e\u00104\u001a\u00020\u000b2\u0006\u0010\n\u001a\u00020\u000b@BX\u0086.\u00a2\u0006\b\n\u0000\u001a\u0004\b5\u0010\u000eR\u000e\u00106\u001a\u00020\bX\u0082\u000e\u00a2\u0006\u0002\n\u0000R\u001e\u00108\u001a\u0002072\u0006\u0010\n\u001a\u000207@BX\u0086.\u00a2\u0006\b\n\u0000\u001a\u0004\b9\u0010:R\u000e\u0010;\u001a\u00020\"X\u0082\u0004\u00a2\u0006\u0002\n\u0000\u00a8\u0006^"}, d2 = {"Lcom/antigravity/voidodyssey/core/GLES30Renderer;", "Landroid/opengl/GLSurfaceView$Renderer;", "context", "Landroid/content/Context;", "qualityManager", "Lcom/antigravity/voidodyssey/core/QualityManager;", "(Landroid/content/Context;Lcom/antigravity/voidodyssey/core/QualityManager;)V", "asteroidAlbedo", "", "asteroidMR", "<set-?>", "Lcom/antigravity/voidodyssey/core/Mesh;", "asteroidMesh", "getAsteroidMesh", "()Lcom/antigravity/voidodyssey/core/Mesh;", "asteroidNormal", "backgroundQuad", "getBackgroundQuad", "bgTexture", "cameraX", "", "getCameraX", "()F", "setCameraX", "(F)V", "cameraY", "getCameraY", "setCameraY", "cameraZ", "getCameraZ", "setCameraZ", "laserMesh", "getLaserMesh", "lightColor", "", "lightDir", "modelMatrix", "normalMatrix", "Lcom/antigravity/voidodyssey/core/Shader;", "pbrShader", "getPbrShader", "()Lcom/antigravity/voidodyssey/core/Shader;", "projectionMatrix", "renderAsteroids", "", "Lcom/antigravity/voidodyssey/game/Asteroid;", "renderLasers", "Lcom/antigravity/voidodyssey/game/Laser;", "renderPlayerShip", "Lcom/antigravity/voidodyssey/game/PlayerShip;", "shipAlbedo", "shipMR", "shipMesh", "getShipMesh", "shipNormal", "Lcom/antigravity/voidodyssey/core/TextureManager;", "textureManager", "getTextureManager", "()Lcom/antigravity/voidodyssey/core/TextureManager;", "viewMatrix", "bindPbrTextures", "", "albedo", "normal", "mr", "createAsteroidVertices", "createLaserVertices", "createSpaceshipVertices", "drawBackground", "drawEntity", "x", "y", "z", "rotY", "scale", "mesh", "loadMeshOrFallback", "path", "", "fallbackGen", "Lkotlin/Function0;", "onDrawFrame", "gl", "Ljavax/microedition/khronos/opengles/GL10;", "onSurfaceChanged", "width", "height", "onSurfaceCreated", "config", "Ljavax/microedition/khronos/egl/EGLConfig;", "updateRenderState", "player", "asteroids", "lasers", "app_debug"})
public final class GLES30Renderer implements android.opengl.GLSurfaceView.Renderer {
    @org.jetbrains.annotations.NotNull()
    private final android.content.Context context = null;
    @org.jetbrains.annotations.NotNull()
    private final com.antigravity.voidodyssey.core.QualityManager qualityManager = null;
    private com.antigravity.voidodyssey.core.TextureManager textureManager;
    private com.antigravity.voidodyssey.core.Shader pbrShader;
    private com.antigravity.voidodyssey.core.Mesh shipMesh;
    private com.antigravity.voidodyssey.core.Mesh asteroidMesh;
    private com.antigravity.voidodyssey.core.Mesh laserMesh;
    private com.antigravity.voidodyssey.core.Mesh backgroundQuad;
    private int shipAlbedo = 0;
    private int shipNormal = 0;
    private int shipMR = 0;
    private int asteroidAlbedo = 0;
    private int asteroidNormal = 0;
    private int asteroidMR = 0;
    private int bgTexture = 0;
    @org.jetbrains.annotations.NotNull()
    private final float[] viewMatrix = null;
    @org.jetbrains.annotations.NotNull()
    private final float[] projectionMatrix = null;
    @org.jetbrains.annotations.NotNull()
    private final float[] modelMatrix = null;
    @org.jetbrains.annotations.NotNull()
    private final float[] normalMatrix = null;
    private float cameraX = 0.0F;
    private float cameraY = 0.0F;
    private float cameraZ = 20.0F;
    @org.jetbrains.annotations.NotNull()
    private final float[] lightDir = {-0.5F, -1.0F, -0.5F};
    @org.jetbrains.annotations.NotNull()
    private final float[] lightColor = {1.5F, 1.4F, 1.2F};
    @kotlin.jvm.Volatile()
    @org.jetbrains.annotations.Nullable()
    private volatile com.antigravity.voidodyssey.game.PlayerShip renderPlayerShip;
    @kotlin.jvm.Volatile()
    @org.jetbrains.annotations.NotNull()
    private volatile java.util.List<com.antigravity.voidodyssey.game.Asteroid> renderAsteroids;
    @kotlin.jvm.Volatile()
    @org.jetbrains.annotations.NotNull()
    private volatile java.util.List<com.antigravity.voidodyssey.game.Laser> renderLasers;
    
    public GLES30Renderer(@org.jetbrains.annotations.NotNull()
    android.content.Context context, @org.jetbrains.annotations.NotNull()
    com.antigravity.voidodyssey.core.QualityManager qualityManager) {
        super();
    }
    
    @org.jetbrains.annotations.NotNull()
    public final com.antigravity.voidodyssey.core.TextureManager getTextureManager() {
        return null;
    }
    
    @org.jetbrains.annotations.NotNull()
    public final com.antigravity.voidodyssey.core.Shader getPbrShader() {
        return null;
    }
    
    @org.jetbrains.annotations.NotNull()
    public final com.antigravity.voidodyssey.core.Mesh getShipMesh() {
        return null;
    }
    
    @org.jetbrains.annotations.NotNull()
    public final com.antigravity.voidodyssey.core.Mesh getAsteroidMesh() {
        return null;
    }
    
    @org.jetbrains.annotations.NotNull()
    public final com.antigravity.voidodyssey.core.Mesh getLaserMesh() {
        return null;
    }
    
    @org.jetbrains.annotations.NotNull()
    public final com.antigravity.voidodyssey.core.Mesh getBackgroundQuad() {
        return null;
    }
    
    public final float getCameraX() {
        return 0.0F;
    }
    
    public final void setCameraX(float p0) {
    }
    
    public final float getCameraY() {
        return 0.0F;
    }
    
    public final void setCameraY(float p0) {
    }
    
    public final float getCameraZ() {
        return 0.0F;
    }
    
    public final void setCameraZ(float p0) {
    }
    
    public final void updateRenderState(@org.jetbrains.annotations.NotNull()
    com.antigravity.voidodyssey.game.PlayerShip player, @org.jetbrains.annotations.NotNull()
    java.util.List<com.antigravity.voidodyssey.game.Asteroid> asteroids, @org.jetbrains.annotations.NotNull()
    java.util.List<com.antigravity.voidodyssey.game.Laser> lasers) {
    }
    
    @java.lang.Override()
    public void onSurfaceCreated(@org.jetbrains.annotations.Nullable()
    javax.microedition.khronos.opengles.GL10 gl, @org.jetbrains.annotations.Nullable()
    javax.microedition.khronos.egl.EGLConfig config) {
    }
    
    @java.lang.Override()
    public void onSurfaceChanged(@org.jetbrains.annotations.Nullable()
    javax.microedition.khronos.opengles.GL10 gl, int width, int height) {
    }
    
    @java.lang.Override()
    public void onDrawFrame(@org.jetbrains.annotations.Nullable()
    javax.microedition.khronos.opengles.GL10 gl) {
    }
    
    private final void drawBackground() {
    }
    
    private final void drawEntity(float x, float y, float z, float rotY, float scale, com.antigravity.voidodyssey.core.Mesh mesh, int albedo, int normal, int mr) {
    }
    
    private final void bindPbrTextures(int albedo, int normal, int mr) {
    }
    
    private final com.antigravity.voidodyssey.core.Mesh loadMeshOrFallback(java.lang.String path, kotlin.jvm.functions.Function0<com.antigravity.voidodyssey.core.Mesh> fallbackGen) {
        return null;
    }
    
    private final com.antigravity.voidodyssey.core.Mesh createSpaceshipVertices() {
        return null;
    }
    
    private final com.antigravity.voidodyssey.core.Mesh createAsteroidVertices() {
        return null;
    }
    
    private final com.antigravity.voidodyssey.core.Mesh createLaserVertices() {
        return null;
    }
}