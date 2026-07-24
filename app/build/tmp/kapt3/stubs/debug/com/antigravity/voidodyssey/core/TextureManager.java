package com.antigravity.voidodyssey.core;

/**
 * Manages texture loading, binding, caching, and provides fallback textures.
 */
@kotlin.Metadata(mv = {1, 9, 0}, k = 1, xi = 48, d1 = {"\u00006\n\u0002\u0018\u0002\n\u0002\u0010\u0000\n\u0000\n\u0002\u0018\u0002\n\u0002\b\u0002\n\u0002\u0018\u0002\n\u0002\u0010\u000e\n\u0002\u0010\b\n\u0002\b\b\n\u0002\u0010\u0015\n\u0000\n\u0002\u0010\u0002\n\u0002\b\u0002\n\u0002\u0018\u0002\n\u0000\u0018\u00002\u00020\u0001B\r\u0012\u0006\u0010\u0002\u001a\u00020\u0003\u00a2\u0006\u0002\u0010\u0004J&\u0010\t\u001a\u00020\b2\u0006\u0010\n\u001a\u00020\b2\u0006\u0010\u000b\u001a\u00020\b2\u0006\u0010\f\u001a\u00020\b2\u0006\u0010\r\u001a\u00020\bJ\u001a\u0010\u000e\u001a\u00020\b2\u0006\u0010\u000f\u001a\u00020\u00072\n\b\u0002\u0010\u0010\u001a\u0004\u0018\u00010\u0011J\u0006\u0010\u0012\u001a\u00020\u0013J\u0010\u0010\u0014\u001a\u00020\b2\u0006\u0010\u0015\u001a\u00020\u0016H\u0002R\u000e\u0010\u0002\u001a\u00020\u0003X\u0082\u0004\u00a2\u0006\u0002\n\u0000R\u001a\u0010\u0005\u001a\u000e\u0012\u0004\u0012\u00020\u0007\u0012\u0004\u0012\u00020\b0\u0006X\u0082\u0004\u00a2\u0006\u0002\n\u0000\u00a8\u0006\u0017"}, d2 = {"Lcom/antigravity/voidodyssey/core/TextureManager;", "", "context", "Landroid/content/Context;", "(Landroid/content/Context;)V", "textures", "Ljava/util/HashMap;", "", "", "createSolidColorTexture", "r", "g", "b", "a", "loadTexture", "path", "fallbackColor", "", "releaseAll", "", "uploadBitmapToGPU", "bitmap", "Landroid/graphics/Bitmap;", "app_debug"})
public final class TextureManager {
    @org.jetbrains.annotations.NotNull()
    private final android.content.Context context = null;
    @org.jetbrains.annotations.NotNull()
    private final java.util.HashMap<java.lang.String, java.lang.Integer> textures = null;
    
    public TextureManager(@org.jetbrains.annotations.NotNull()
    android.content.Context context) {
        super();
    }
    
    /**
     * Loads a texture from the assets folder. Falls back to a default solid texture if not found.
     */
    public final int loadTexture(@org.jetbrains.annotations.NotNull()
    java.lang.String path, @org.jetbrains.annotations.Nullable()
    int[] fallbackColor) {
        return 0;
    }
    
    /**
     * Creates a simple 1x1 solid color texture for fallbacks.
     */
    public final int createSolidColorTexture(int r, int g, int b, int a) {
        return 0;
    }
    
    private final int uploadBitmapToGPU(android.graphics.Bitmap bitmap) {
        return 0;
    }
    
    public final void releaseAll() {
    }
}