package com.antigravity.voidodyssey.core

import android.content.Context
import android.graphics.Bitmap
import android.graphics.BitmapFactory
import android.opengl.GLES30
import android.opengl.GLUtils
import android.util.Log
import java.io.IOException
import java.nio.ByteBuffer
import java.nio.ByteOrder

/**
 * Manages texture loading, binding, caching, and provides fallback textures.
 */
class TextureManager(private val context: Context) {

    private val textures = HashMap<String, Int>()

    /**
     * Loads a texture from the assets folder. Falls back to a default solid texture if not found.
     */
    fun loadTexture(path: String, fallbackColor: IntArray? = null): Int {
        return textures.getOrPut(path) {
            try {
                val bitmap = context.assets.open(path).use { stream ->
                    BitmapFactory.decodeStream(stream)
                } ?: throw IOException("Failed to decode bitmap stream")

                val textureId = uploadBitmapToGPU(bitmap)
                bitmap.recycle()
                textureId
            } catch (e: Exception) {
                Log.e("TextureManager", "Error loading texture '$path': ${e.message}. Using fallback.")
                fallbackColor?.let { createSolidColorTexture(it[0], it[1], it[2], it[3]) }
                    ?: createSolidColorTexture(255, 255, 255, 255) // White default
            }
        }
    }

    /**
     * Creates a simple 1x1 solid color texture for fallbacks.
     */
    fun createSolidColorTexture(r: Int, g: Int, b: Int, a: Int): Int {
        val textureIds = IntArray(1)
        GLES30.glGenTextures(1, textureIds, 0)
        val textureId = textureIds[0]

        GLES30.glBindTexture(GLES30.GL_TEXTURE_2D, textureId)

        val buffer = ByteBuffer.allocateDirect(4).run {
            order(ByteOrder.nativeOrder())
            put(r.toByte())
            put(g.toByte())
            put(b.toByte())
            put(a.toByte())
            position(0)
        }

        GLES30.glTexImage2D(
            GLES30.GL_TEXTURE_2D, 0, GLES30.GL_RGBA, 1, 1, 0,
            GLES30.GL_RGBA, GLES30.GL_UNSIGNED_BYTE, buffer
        )

        GLES30.glTexParameteri(GLES30.GL_TEXTURE_2D, GLES30.GL_TEXTURE_MIN_FILTER, GLES30.GL_NEAREST)
        GLES30.glTexParameteri(GLES30.GL_TEXTURE_2D, GLES30.GL_TEXTURE_MAG_FILTER, GLES30.GL_NEAREST)
        GLES30.glBindTexture(GLES30.GL_TEXTURE_2D, 0)

        return textureId
    }

    private fun uploadBitmapToGPU(bitmap: Bitmap): Int {
        val textureIds = IntArray(1)
        GLES30.glGenTextures(1, textureIds, 0)
        val textureId = textureIds[0]

        GLES30.glBindTexture(GLES30.GL_TEXTURE_2D, textureId)

        // Set filtering parameters
        GLES30.glTexParameteri(GLES30.GL_TEXTURE_2D, GLES30.GL_TEXTURE_MIN_FILTER, GLES30.GL_LINEAR_MIPMAP_LINEAR)
        GLES30.glTexParameteri(GLES30.GL_TEXTURE_2D, GLES30.GL_TEXTURE_MAG_FILTER, GLES30.GL_LINEAR)
        GLES30.glTexParameteri(GLES30.GL_TEXTURE_2D, GLES30.GL_TEXTURE_WRAP_S, GLES30.GL_REPEAT)
        GLES30.glTexParameteri(GLES30.GL_TEXTURE_2D, GLES30.GL_TEXTURE_WRAP_T, GLES30.GL_REPEAT)

        // Load the bitmap into the bound texture
        GLUtils.texImage2D(GLES30.GL_TEXTURE_2D, 0, bitmap, 0)

        // Generate mipmaps for performance optimization and reducing aliases
        GLES30.glGenerateMipmap(GLES30.GL_TEXTURE_2D)

        GLES30.glBindTexture(GLES30.GL_TEXTURE_2D, 0)

        return textureId
    }

    fun releaseAll() {
        val ids = textures.values.toIntArray()
        if (ids.isNotEmpty()) {
            GLES30.glDeleteTextures(ids.size, ids, 0)
        }
        textures.clear()
    }
}
