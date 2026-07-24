package com.antigravity.voidodyssey.core;

/**
 * Handles vertex buffer allocations (VAO/VBO/EBO) and renders 3D geometries.
 * Includes an optimized loader for Wavefront OBJ files.
 */
@kotlin.Metadata(mv = {1, 9, 0}, k = 1, xi = 48, d1 = {"\u0000(\n\u0002\u0018\u0002\n\u0002\u0010\u0000\n\u0000\n\u0002\u0010\u0014\n\u0000\n\u0002\u0010\u0017\n\u0002\b\u0002\n\u0002\u0010\b\n\u0002\b\u0007\n\u0002\u0010\u0002\n\u0002\b\u0004\u0018\u0000 \u00132\u00020\u0001:\u0001\u0013B\u0015\u0012\u0006\u0010\u0002\u001a\u00020\u0003\u0012\u0006\u0010\u0004\u001a\u00020\u0005\u00a2\u0006\u0002\u0010\u0006J\u0006\u0010\u000f\u001a\u00020\u0010J\u0006\u0010\u0011\u001a\u00020\u0010J\b\u0010\u0012\u001a\u00020\u0010H\u0002R\u000e\u0010\u0007\u001a\u00020\bX\u0082\u000e\u00a2\u0006\u0002\n\u0000R\u0011\u0010\u0004\u001a\u00020\u0005\u00a2\u0006\b\n\u0000\u001a\u0004\b\t\u0010\nR\u000e\u0010\u000b\u001a\u00020\bX\u0082\u000e\u00a2\u0006\u0002\n\u0000R\u000e\u0010\f\u001a\u00020\bX\u0082\u000e\u00a2\u0006\u0002\n\u0000R\u0011\u0010\u0002\u001a\u00020\u0003\u00a2\u0006\b\n\u0000\u001a\u0004\b\r\u0010\u000e\u00a8\u0006\u0014"}, d2 = {"Lcom/antigravity/voidodyssey/core/Mesh;", "", "vertices", "", "indices", "", "([F[S)V", "eboId", "", "getIndices", "()[S", "vaoId", "vboId", "getVertices", "()[F", "draw", "", "release", "setupMesh", "Companion", "app_debug"})
public final class Mesh {
    @org.jetbrains.annotations.NotNull()
    private final float[] vertices = null;
    @org.jetbrains.annotations.NotNull()
    private final short[] indices = null;
    private int vaoId = 0;
    private int vboId = 0;
    private int eboId = 0;
    @org.jetbrains.annotations.NotNull()
    public static final com.antigravity.voidodyssey.core.Mesh.Companion Companion = null;
    
    public Mesh(@org.jetbrains.annotations.NotNull()
    float[] vertices, @org.jetbrains.annotations.NotNull()
    short[] indices) {
        super();
    }
    
    @org.jetbrains.annotations.NotNull()
    public final float[] getVertices() {
        return null;
    }
    
    @org.jetbrains.annotations.NotNull()
    public final short[] getIndices() {
        return null;
    }
    
    private final void setupMesh() {
    }
    
    public final void draw() {
    }
    
    public final void release() {
    }
    
    @kotlin.Metadata(mv = {1, 9, 0}, k = 1, xi = 48, d1 = {"\u0000 \n\u0002\u0018\u0002\n\u0002\u0010\u0000\n\u0002\b\u0002\n\u0002\u0018\u0002\n\u0002\b\u0002\n\u0002\u0018\u0002\n\u0000\n\u0002\u0010\u000e\n\u0000\b\u0086\u0003\u0018\u00002\u00020\u0001B\u0007\b\u0002\u00a2\u0006\u0002\u0010\u0002J\u0006\u0010\u0003\u001a\u00020\u0004J\u0016\u0010\u0005\u001a\u00020\u00042\u0006\u0010\u0006\u001a\u00020\u00072\u0006\u0010\b\u001a\u00020\t\u00a8\u0006\n"}, d2 = {"Lcom/antigravity/voidodyssey/core/Mesh$Companion;", "", "()V", "createQuad", "Lcom/antigravity/voidodyssey/core/Mesh;", "loadFromObj", "context", "Landroid/content/Context;", "path", "", "app_debug"})
    public static final class Companion {
        
        private Companion() {
            super();
        }
        
        /**
         * Parses an .obj file, computes normal tangents, and outputs a Mesh.
         * OBJ format parsing supports v, vt, vn, f.
         */
        @org.jetbrains.annotations.NotNull()
        public final com.antigravity.voidodyssey.core.Mesh loadFromObj(@org.jetbrains.annotations.NotNull()
        android.content.Context context, @org.jetbrains.annotations.NotNull()
        java.lang.String path) {
            return null;
        }
        
        /**
         * Helper to create a simple quad (e.g., for space background nebula, billboard particles, or UI textures).
         */
        @org.jetbrains.annotations.NotNull()
        public final com.antigravity.voidodyssey.core.Mesh createQuad() {
            return null;
        }
    }
}