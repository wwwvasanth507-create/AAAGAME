package com.antigravity.voidodyssey.core;

/**
 * Helper class to compile and manage OpenGL ES 3.0 shader programs.
 */
@kotlin.Metadata(mv = {1, 9, 0}, k = 1, xi = 48, d1 = {"\u0000>\n\u0002\u0018\u0002\n\u0002\u0010\u0000\n\u0000\n\u0002\u0018\u0002\n\u0000\n\u0002\u0010\u000e\n\u0002\b\u0003\n\u0002\u0010\b\n\u0002\b\u0004\n\u0002\u0018\u0002\n\u0002\b\b\n\u0002\u0010\u0002\n\u0000\n\u0002\u0010\u0007\n\u0002\b\u0003\n\u0002\u0010\u0014\n\u0002\b\b\u0018\u00002\u00020\u0001B\u001d\u0012\u0006\u0010\u0002\u001a\u00020\u0003\u0012\u0006\u0010\u0004\u001a\u00020\u0005\u0012\u0006\u0010\u0006\u001a\u00020\u0005\u00a2\u0006\u0002\u0010\u0007J\u0018\u0010\u000f\u001a\u00020\t2\u0006\u0010\u0010\u001a\u00020\t2\u0006\u0010\u0011\u001a\u00020\u0005H\u0002J\u0010\u0010\u0012\u001a\u00020\t2\u0006\u0010\u0013\u001a\u00020\u0005H\u0002J\u0010\u0010\u0014\u001a\u00020\u00052\u0006\u0010\u0015\u001a\u00020\u0005H\u0002J\u0016\u0010\u0016\u001a\u00020\u00172\u0006\u0010\u0013\u001a\u00020\u00052\u0006\u0010\u0018\u001a\u00020\u0019J\u0016\u0010\u001a\u001a\u00020\u00172\u0006\u0010\u0013\u001a\u00020\u00052\u0006\u0010\u0018\u001a\u00020\tJ\u0016\u0010\u001b\u001a\u00020\u00172\u0006\u0010\u0013\u001a\u00020\u00052\u0006\u0010\u001c\u001a\u00020\u001dJ\u0016\u0010\u001e\u001a\u00020\u00172\u0006\u0010\u0013\u001a\u00020\u00052\u0006\u0010\u001c\u001a\u00020\u001dJ&\u0010\u001f\u001a\u00020\u00172\u0006\u0010\u0013\u001a\u00020\u00052\u0006\u0010 \u001a\u00020\u00192\u0006\u0010!\u001a\u00020\u00192\u0006\u0010\"\u001a\u00020\u0019J\u0016\u0010\u001f\u001a\u00020\u00172\u0006\u0010\u0013\u001a\u00020\u00052\u0006\u0010#\u001a\u00020\u001dJ\u0006\u0010$\u001a\u00020\u0017R\u000e\u0010\u0002\u001a\u00020\u0003X\u0082\u0004\u00a2\u0006\u0002\n\u0000R\u001e\u0010\n\u001a\u00020\t2\u0006\u0010\b\u001a\u00020\t@BX\u0086\u000e\u00a2\u0006\b\n\u0000\u001a\u0004\b\u000b\u0010\fR\u001a\u0010\r\u001a\u000e\u0012\u0004\u0012\u00020\u0005\u0012\u0004\u0012\u00020\t0\u000eX\u0082\u0004\u00a2\u0006\u0002\n\u0000\u00a8\u0006%"}, d2 = {"Lcom/antigravity/voidodyssey/core/Shader;", "", "context", "Landroid/content/Context;", "vertPath", "", "fragPath", "(Landroid/content/Context;Ljava/lang/String;Ljava/lang/String;)V", "<set-?>", "", "programId", "getProgramId", "()I", "uniformLocations", "Ljava/util/HashMap;", "compileShader", "type", "code", "getUniformLocation", "name", "loadShaderFromAssets", "path", "setFloat", "", "value", "", "setInt", "setMat3", "matrix", "", "setMat4", "setVec3", "x", "y", "z", "values", "use", "app_debug"})
public final class Shader {
    @org.jetbrains.annotations.NotNull()
    private final android.content.Context context = null;
    private int programId = 0;
    @org.jetbrains.annotations.NotNull()
    private final java.util.HashMap<java.lang.String, java.lang.Integer> uniformLocations = null;
    
    public Shader(@org.jetbrains.annotations.NotNull()
    android.content.Context context, @org.jetbrains.annotations.NotNull()
    java.lang.String vertPath, @org.jetbrains.annotations.NotNull()
    java.lang.String fragPath) {
        super();
    }
    
    public final int getProgramId() {
        return 0;
    }
    
    public final void use() {
    }
    
    private final int getUniformLocation(java.lang.String name) {
        return 0;
    }
    
    public final void setInt(@org.jetbrains.annotations.NotNull()
    java.lang.String name, int value) {
    }
    
    public final void setFloat(@org.jetbrains.annotations.NotNull()
    java.lang.String name, float value) {
    }
    
    public final void setVec3(@org.jetbrains.annotations.NotNull()
    java.lang.String name, float x, float y, float z) {
    }
    
    public final void setVec3(@org.jetbrains.annotations.NotNull()
    java.lang.String name, @org.jetbrains.annotations.NotNull()
    float[] values) {
    }
    
    public final void setMat4(@org.jetbrains.annotations.NotNull()
    java.lang.String name, @org.jetbrains.annotations.NotNull()
    float[] matrix) {
    }
    
    public final void setMat3(@org.jetbrains.annotations.NotNull()
    java.lang.String name, @org.jetbrains.annotations.NotNull()
    float[] matrix) {
    }
    
    private final int compileShader(int type, java.lang.String code) {
        return 0;
    }
    
    private final java.lang.String loadShaderFromAssets(java.lang.String path) {
        return null;
    }
}