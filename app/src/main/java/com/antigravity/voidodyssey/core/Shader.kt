package com.antigravity.voidodyssey.core

import android.content.Context
import android.opengl.GLES30
import android.util.Log
import java.io.BufferedReader
import java.io.InputStreamReader

/**
 * Helper class to compile and manage OpenGL ES 3.0 shader programs.
 */
class Shader(private val context: Context, vertPath: String, fragPath: String) {

    var programId: Int = 0
        private set

    private val uniformLocations = HashMap<String, Int>()

    init {
        val vertCode = loadShaderFromAssets(vertPath)
        val fragCode = loadShaderFromAssets(fragPath)

        val vertShader = compileShader(GLES30.GL_VERTEX_SHADER, vertCode)
        val fragShader = compileShader(GLES30.GL_FRAGMENT_SHADER, fragCode)

        programId = GLES30.glCreateProgram()
        if (programId == 0) {
            throw RuntimeException("Failed to create shader program")
        }

        GLES30.glAttachShader(programId, vertShader)
        GLES30.glAttachShader(programId, fragShader)
        GLES30.glLinkProgram(programId)

        val linkStatus = IntArray(1)
        GLES30.glGetProgramiv(programId, GLES30.GL_LINK_STATUS, linkStatus, 0)
        if (linkStatus[0] == 0) {
            val log = GLES30.glGetProgramInfoLog(programId)
            GLES30.glDeleteProgram(programId)
            throw RuntimeException("Failed to link shader program: $log")
        }

        // Clean up individual shaders after linking
        GLES30.glDeleteShader(vertShader)
        GLES30.glDeleteShader(fragShader)
    }

    fun use() {
        GLES30.glUseProgram(programId)
    }

    private fun getUniformLocation(name: String): Int {
        return uniformLocations.getOrPut(name) {
            val loc = GLES30.glGetUniformLocation(programId, name)
            if (loc == -1) {
                Log.w("Shader", "Uniform '$name' not found in shader program")
            }
            loc
        }
    }

    fun setInt(name: String, value: Int) {
        val loc = getUniformLocation(name)
        if (loc != -1) GLES30.glUniform1i(loc, value)
    }

    fun setFloat(name: String, value: Float) {
        val loc = getUniformLocation(name)
        if (loc != -1) GLES30.glUniform1f(loc, value)
    }

    fun setVec3(name: String, x: Float, y: Float, z: Float) {
        val loc = getUniformLocation(name)
        if (loc != -1) GLES30.glUniform3f(loc, x, y, z)
    }

    fun setVec3(name: String, values: FloatArray) {
        val loc = getUniformLocation(name)
        if (loc != -1) GLES30.glUniform3fv(loc, 1, values, 0)
    }

    fun setMat4(name: String, matrix: FloatArray) {
        val loc = getUniformLocation(name)
        if (loc != -1) GLES30.glUniformMatrix4fv(loc, 1, false, matrix, 0)
    }

    fun setMat3(name: String, matrix: FloatArray) {
        val loc = getUniformLocation(name)
        if (loc != -1) GLES30.glUniformMatrix3fv(loc, 1, false, matrix, 0)
    }

    private fun compileShader(type: Int, code: String): Int {
        val shaderId = GLES30.glCreateShader(type)
        if (shaderId == 0) {
            throw RuntimeException("Failed to create shader type: $type")
        }

        GLES30.glShaderSource(shaderId, code)
        GLES30.glCompileShader(shaderId)

        val compileStatus = IntArray(1)
        GLES30.glGetShaderiv(shaderId, GLES30.GL_COMPILE_STATUS, compileStatus, 0)
        if (compileStatus[0] == 0) {
            val log = GLES30.glGetShaderInfoLog(shaderId)
            GLES30.glDeleteShader(shaderId)
            throw RuntimeException("Failed to compile shader ($type): $log")
        }

        return shaderId
    }

    private fun loadShaderFromAssets(path: String): String {
        val sb = StringBuilder()
        context.assets.open(path).use { stream ->
            BufferedReader(InputStreamReader(stream)).use { reader ->
                var line = reader.readLine()
                while (line != null) {
                    sb.append(line).append("\n")
                    line = reader.readLine()
                }
            }
        }
        return sb.toString()
    }
}
