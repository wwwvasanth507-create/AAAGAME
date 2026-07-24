package com.antigravity.voidodyssey.core

import android.content.Context
import android.opengl.GLES30
import java.io.BufferedReader
import java.io.InputStreamReader
import java.nio.ByteBuffer
import java.nio.ByteOrder
import java.nio.FloatBuffer
import java.nio.ShortBuffer

/**
 * Handles vertex buffer allocations (VAO/VBO/EBO) and renders 3D geometries.
 * Includes an optimized loader for Wavefront OBJ files.
 */
class Mesh(
    val vertices: FloatArray,
    val indices: ShortArray
) {
    private var vaoId: Int = 0
    private var vboId: Int = 0
    private var eboId: Int = 0

    init {
        setupMesh()
    }

    private fun setupMesh() {
        val vao = IntArray(1)
        val vbo = IntArray(1)
        val ebo = IntArray(1)

        GLES30.glGenVertexArrays(1, vao, 0)
        GLES30.glGenBuffers(1, vbo, 0)
        GLES30.glGenBuffers(1, ebo, 0)

        vaoId = vao[0]
        vboId = vbo[0]
        eboId = ebo[0]

        // Create buffers
        val vertexBuffer = ByteBuffer.allocateDirect(vertices.size * 4).run {
            order(ByteOrder.nativeOrder())
            asFloatBuffer().apply {
                put(vertices)
                position(0)
            }
        }

        val indexBuffer = ByteBuffer.allocateDirect(indices.size * 2).run {
            order(ByteOrder.nativeOrder())
            asShortBuffer().apply {
                put(indices)
                position(0)
            }
        }

        GLES30.glBindVertexArray(vaoId)

        // Upload vertex buffer
        GLES30.glBindBuffer(GLES30.GL_ARRAY_BUFFER, vboId)
        GLES30.glBufferData(
            GLES30.GL_ARRAY_BUFFER,
            vertices.size * 4,
            vertexBuffer,
            GLES30.GL_STATIC_DRAW
        )

        // Upload index buffer
        GLES30.glBindBuffer(GLES30.GL_ELEMENT_ARRAY_BUFFER, eboId)
        GLES30.glBufferData(
            GLES30.GL_ELEMENT_ARRAY_BUFFER,
            indices.size * 2,
            indexBuffer,
            GLES30.GL_STATIC_DRAW
        )

        // Vertex layout attributes:
        // Position: 3 floats, TexCoord: 2 floats, Normal: 3 floats, Tangent: 3 floats
        // Stride: 11 floats * 4 bytes = 44 bytes
        val stride = 11 * 4

        // 0: Position
        GLES30.glEnableVertexAttribArray(0)
        GLES30.glVertexAttribPointer(0, 3, GLES30.GL_FLOAT, false, stride, 0)

        // 1: TexCoord
        GLES30.glEnableVertexAttribArray(1)
        GLES30.glVertexAttribPointer(1, 2, GLES30.GL_FLOAT, false, stride, 3 * 4)

        // 2: Normal
        GLES30.glEnableVertexAttribArray(2)
        GLES30.glVertexAttribPointer(2, 3, GLES30.GL_FLOAT, false, stride, (3 + 2) * 4)

        // 3: Tangent
        GLES30.glEnableVertexAttribArray(3)
        GLES30.glVertexAttribPointer(3, 3, GLES30.GL_FLOAT, false, stride, (3 + 2 + 3) * 4)

        GLES30.glBindVertexArray(0)
    }

    fun draw() {
        GLES30.glBindVertexArray(vaoId)
        GLES30.glDrawElements(GLES30.GL_TRIANGLES, indices.size, GLES30.GL_UNSIGNED_SHORT, 0)
        GLES30.glBindVertexArray(0)
    }

    fun release() {
        val vao = intArrayOf(vaoId)
        val vbo = intArrayOf(vboId)
        val ebo = intArrayOf(eboId)
        GLES30.glDeleteVertexArrays(1, vao, 0)
        GLES30.glDeleteBuffers(1, vbo, 0)
        GLES30.glDeleteBuffers(1, ebo, 0)
    }

    companion object {
        /**
         * Parses an .obj file, computes normal tangents, and outputs a Mesh.
         * OBJ format parsing supports v, vt, vn, f.
         */
        fun loadFromObj(context: Context, path: String): Mesh {
            val positions = ArrayList<FloatArray>()
            val texCoords = ArrayList<FloatArray>()
            val normals = ArrayList<FloatArray>()

            // Face definition: [positionIndex, uvIndex, normalIndex]
            val faceIndices = ArrayList<IntArray>()

            context.assets.open(path).use { stream ->
                BufferedReader(InputStreamReader(stream)).use { reader ->
                    var line = reader.readLine()
                    while (line != null) {
                        line = line.trim()
                        if (line.startsWith("v ")) {
                            val parts = line.split("\\s+".toRegex())
                            positions.add(floatArrayOf(parts[1].toFloat(), parts[2].toFloat(), parts[3].toFloat()))
                        } else if (line.startsWith("vt ")) {
                            val parts = line.split("\\s+".toRegex())
                            texCoords.add(floatArrayOf(parts[1].toFloat(), parts[2].toFloat()))
                        } else if (line.startsWith("vn ")) {
                            val parts = line.split("\\s+".toRegex())
                            normals.add(floatArrayOf(parts[1].toFloat(), parts[2].toFloat(), parts[3].toFloat()))
                        } else if (line.startsWith("f ")) {
                            val parts = line.split("\\s+".toRegex()).drop(1)
                            // Support triangulating polygons during OBJ parse if face has > 3 vertices
                            val tempFaces = ArrayList<IntArray>()
                            for (part in parts) {
                                val subParts = part.split("/")
                                val vIdx = subParts[0].toInt() - 1
                                val vtIdx = if (subParts.size > 1 && subParts[1].isNotEmpty()) subParts[1].toInt() - 1 else -1
                                val vnIdx = if (subParts.size > 2 && subParts[2].isNotEmpty()) subParts[2].toInt() - 1 else -1
                                tempFaces.add(intArrayOf(vIdx, vtIdx, vnIdx))
                            }
                            // Triangulate fan pattern
                            for (i in 1 until tempFaces.size - 1) {
                                faceIndices.add(tempFaces[0])
                                faceIndices.add(tempFaces[i])
                                faceIndices.add(tempFaces[i + 1])
                            }
                        }
                        line = reader.readLine()
                    }
                }
            }

            // Create flat vertices list: Stride 11 (Pos:3, UV:2, Normal:3, Tangent:3)
            val vertexMap = HashMap<String, Short>()
            val uniqueVertices = ArrayList<FloatArray>()
            val indices = ArrayList<Short>()

            for (face in faceIndices) {
                val key = "${face[0]}/${face[1]}/${face[2]}"
                var index = vertexMap[key]
                if (index == null) {
                    val pos = positions[face[0]]
                    val uv = if (face[1] != -1 && face[1] < texCoords.size) texCoords[face[1]] else floatArrayOf(0f, 0f)
                    val norm = if (face[2] != -1 && face[2] < normals.size) normals[face[2]] else floatArrayOf(0f, 1f, 0f)
                    // Tangent initially filled with 0s, calculated afterwards
                    val vData = floatArrayOf(
                        pos[0], pos[1], pos[2],
                        uv[0], uv[1],
                        norm[0], norm[1], norm[2],
                        0f, 0f, 0f
                    )
                    index = uniqueVertices.size.toShort()
                    uniqueVertices.add(vData)
                    vertexMap[key] = index
                }
                indices.add(index)
            }

            // Compute Tangents for normal mapping
            // Tangents are computed per triangle face
            for (i in 0 until indices.size step 3) {
                val idx0 = indices[i].toInt()
                val idx1 = indices[i + 1].toInt()
                val idx2 = indices[i + 2].toInt()

                val v0 = uniqueVertices[idx0]
                val v1 = uniqueVertices[idx1]
                val v2 = uniqueVertices[idx2]

                // Positions
                val edge1X = v1[0] - v0[0]
                val edge1Y = v1[1] - v0[1]
                val edge1Z = v1[2] - v0[2]
                val edge2X = v2[0] - v0[0]
                val edge2Y = v2[1] - v0[1]
                val edge2Z = v2[2] - v0[2]

                // UVs
                val deltaUV1X = v1[3] - v0[3]
                val deltaUV1Y = v1[4] - v0[4]
                val deltaUV2X = v2[3] - v0[3]
                val deltaUV2Y = v2[4] - v0[4]

                val r = 1.0f / (deltaUV1X * deltaUV2Y - deltaUV2X * deltaUV1Y).let { if (it == 0f) 0.0001f else it }
                val tangentX = r * (deltaUV2Y * edge1X - deltaUV1Y * edge2X)
                val tangentY = r * (deltaUV2Y * edge1Y - deltaUV1Y * edge2Y)
                val tangentZ = r * (deltaUV2Y * edge1Z - deltaUV1Y * edge2Z)

                // Accumulate tangents on all 3 vertices
                v0[8] += tangentX; v0[9] += tangentY; v0[10] += tangentZ
                v1[8] += tangentX; v1[9] += tangentY; v1[10] += tangentZ
                v2[8] += tangentX; v2[9] += tangentY; v2[10] += tangentZ
            }

            // Gram-Schmidt orthogonalize tangents
            val flatVertices = FloatArray(uniqueVertices.size * 11)
            var offset = 0
            for (v in uniqueVertices) {
                // Normalize tangent
                val tx = v[8]; val ty = v[9]; val tz = v[10]
                val nx = v[5]; val ny = v[6]; val nz = v[7]
                
                // dot(T, N)
                val dot = tx * nx + ty * ny + tz * nz
                var ox = tx - dot * nx
                var oy = ty - dot * ny
                var oz = tz - dot * nz
                val len = Math.sqrt((ox * ox + oy * oy + oz * oz).toDouble()).toFloat()
                if (len > 0f) {
                    ox /= len; oy /= len; oz /= len
                }

                v[8] = ox; v[9] = oy; v[10] = oz

                System.arraycopy(v, 0, flatVertices, offset, 11)
                offset += 11
            }

            return Mesh(flatVertices, indices.toShortArray())
        }

        /**
         * Helper to create a simple quad (e.g., for space background nebula, billboard particles, or UI textures).
         */
        fun createQuad(): Mesh {
            val vertices = floatArrayOf(
                // Position      // UV      // Normal     // Tangent
                -1f,  1f, 0f,    0f, 0f,    0f, 0f, 1f,   1f, 0f, 0f,
                -1f, -1f, 0f,    0f, 1f,    0f, 0f, 1f,   1f, 0f, 0f,
                 1f, -1f, 0f,    1f, 1f,    0f, 0f, 1f,   1f, 0f, 0f,
                 1f,  1f, 0f,    1f, 0f,    0f, 0f, 1f,   1f, 0f, 0f
            )
            val indices = shortArrayOf(0, 1, 2, 0, 2, 3)
            return Mesh(vertices, indices)
        }
    }
}
