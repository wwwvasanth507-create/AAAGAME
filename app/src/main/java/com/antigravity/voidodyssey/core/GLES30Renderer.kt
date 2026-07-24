package com.antigravity.voidodyssey.core

import android.content.Context
import android.opengl.GLES30
import android.opengl.GLSurfaceView
import android.opengl.Matrix
import android.util.Log
import com.antigravity.voidodyssey.game.Asteroid
import com.antigravity.voidodyssey.game.Laser
import com.antigravity.voidodyssey.game.PlayerShip
import javax.microedition.khronos.egl.EGLConfig
import javax.microedition.khronos.opengles.GL10

/**
 * Handles the actual OpenGL ES 3.0 graphics rendering logic.
 */
class GLES30Renderer(
    private val context: Context,
    private val qualityManager: QualityManager
) : GLSurfaceView.Renderer {

    lateinit var textureManager: TextureManager
        private set
    lateinit var pbrShader: Shader
        private set

    // Meshes
    lateinit var shipMesh: Mesh
        private set
    lateinit var asteroidMesh: Mesh
        private set
    lateinit var laserMesh: Mesh
        private set
    lateinit var backgroundQuad: Mesh
        private set

    // Texture IDs
    private var shipAlbedo = 0
    private var shipNormal = 0
    private var shipMR = 0

    private var asteroidAlbedo = 0
    private var asteroidNormal = 0
    private var asteroidMR = 0

    private var bgTexture = 0

    // Matrices
    private val viewMatrix = FloatArray(16)
    private val projectionMatrix = FloatArray(16)
    private val modelMatrix = FloatArray(16)
    private val normalMatrix = FloatArray(9)

    // Camera details
    var cameraX = 0f
    var cameraY = 0f
    var cameraZ = 20f

    // Light details
    private val lightDir = floatArrayOf(-0.5f, -1.0f, -0.5f)
    private val lightColor = floatArrayOf(1.5f, 1.4f, 1.2f) // Bright star light

    // Thread-safe copy of entities to render
    @Volatile private var renderPlayerShip: PlayerShip? = null
    @Volatile private var renderAsteroids = emptyList<Asteroid>()
    @Volatile private var renderLasers = emptyList<Laser>()

    fun updateRenderState(player: PlayerShip, asteroids: List<Asteroid>, lasers: List<Laser>) {
        renderPlayerShip = player
        renderAsteroids = asteroids
        renderLasers = lasers
    }

    override fun onSurfaceCreated(gl: GL10?, config: EGLConfig?) {
        // Enable depth testing
        GLES30.glEnable(GLES30.GL_DEPTH_TEST)
        GLES30.glDepthFunc(GLES30.GL_LEQUAL)

        // Enable backface culling to optimize draw calls
        GLES30.glEnable(GLES30.GL_CULL_FACE)
        GLES30.glCullFace(GLES30.GL_BACK)

        // Dark void background color
        GLES30.glClearColor(0.04f, 0.05f, 0.08f, 1.0f)

        textureManager = TextureManager(context)

        try {
            pbrShader = Shader(context, "shaders/pbr.vert", "shaders/pbr.frag")

            // Load default quad for background
            backgroundQuad = Mesh.createQuad()

            // To support running offline and prevent crashing on first compile if assets aren't in place yet,
            // we will try to load OBJ meshes, falling back to primitive generated shapes if missing!
            shipMesh = loadMeshOrFallback("models/ship.obj") { createSpaceshipVertices() }
            asteroidMesh = loadMeshOrFallback("models/asteroid.obj") { createAsteroidVertices() }
            laserMesh = loadMeshOrFallback("models/laser.obj") { createLaserVertices() }

            // Load textures (using fallbacks for missing assets)
            // Normal fallback: flat blue [128, 128, 255, 255]
            // MR fallback: metallic 0, roughness 128, ao 255 -> [0, 128, 255, 255]
            shipAlbedo = textureManager.loadTexture("textures/ship_albedo.png", intArrayOf(200, 200, 220, 255))
            shipNormal = textureManager.loadTexture("textures/ship_normal.png", intArrayOf(128, 128, 255, 255))
            shipMR = textureManager.loadTexture("textures/ship_mr.png", intArrayOf(150, 100, 255, 255)) // shiny metal

            asteroidAlbedo = textureManager.loadTexture("textures/asteroid_albedo.png", intArrayOf(100, 90, 80, 255))
            asteroidNormal = textureManager.loadTexture("textures/asteroid_normal.png", intArrayOf(128, 128, 255, 255))
            asteroidMR = textureManager.loadTexture("textures/asteroid_mr.png", intArrayOf(0, 220, 255, 255)) // rough rock

            bgTexture = textureManager.loadTexture("textures/space_background.png", intArrayOf(10, 12, 25, 255))

        } catch (e: Exception) {
            Log.e("GLES30Renderer", "Error initializing OpenGL resources: ${e.message}", e)
        }
    }

    override fun onSurfaceChanged(gl: GL10?, width: Int, height: Int) {
        val aspectScale = qualityManager.currentSettings.resolutionScale
        val renderWidth = (width * aspectScale).toInt()
        val renderHeight = (height * aspectScale).toInt()

        GLES30.glViewport(0, 0, renderWidth, renderHeight)

        val aspectRatio = renderWidth.toFloat() / renderHeight.toFloat()
        // Perspective projection matrix
        Matrix.perspectiveM(projectionMatrix, 0, 45f, aspectRatio, 0.1f, 100f)
    }

    override fun onDrawFrame(gl: GL10?) {
        // Clear color and depth buffers
        GLES30.glClear(GLES30.GL_COLOR_BUFFER_BIT or GLES30.GL_DEPTH_BUFFER_BIT)

        // Setup camera (view matrix)
        val player = renderPlayerShip
        if (player != null) {
            // Keep camera floating smoothly behind player ship
            cameraX = player.x * 0.8f
            cameraY = player.y * 0.8f + 3f
            cameraZ = player.z + 12f

            Matrix.setLookAtM(
                viewMatrix, 0,
                cameraX, cameraY, cameraZ,
                player.x, player.y, player.z,
                0f, 1f, 0f
            )
        } else {
            Matrix.setLookAtM(
                viewMatrix, 0,
                cameraX, cameraY, cameraZ,
                0f, 0f, 0f,
                0f, 1f, 0f
            )
        }

        // Draw space background first (without writing to depth buffer)
        GLES30.glDisable(GLES30.GL_DEPTH_TEST)
        drawBackground()
        GLES30.glEnable(GLES30.GL_DEPTH_TEST)

        // Use standard PBR shader program
        pbrShader.use()
        pbrShader.setMat4("uProjection", projectionMatrix)
        pbrShader.setMat4("uView", viewMatrix)
        pbrShader.setVec3("uLightDirection", lightDir)
        pbrShader.setVec3("uLightColor", lightColor)
        pbrShader.setVec3("uCameraPos", cameraX, cameraY, cameraZ)

        // Draw Player Ship
        player?.let { drawEntity(it.x, it.y, it.z, it.rotY, 1f, shipMesh, shipAlbedo, shipNormal, shipMR) }

        // Draw Asteroids
        val asteroids = renderAsteroids
        for (ast in asteroids) {
            if (ast.active) {
                drawEntity(ast.x, ast.y, ast.z, ast.rotY, ast.scale, asteroidMesh, asteroidAlbedo, asteroidNormal, asteroidMR)
            }
        }

        // Draw Lasers
        val lasers = renderLasers
        for (laser in lasers) {
            if (laser.active) {
                // Red glowing laser using fallback textures
                drawEntity(laser.x, laser.y, laser.z, 0f, 0.3f, laserMesh, 
                    textureManager.createSolidColorTexture(255, 50, 50, 255),
                    textureManager.createSolidColorTexture(128, 128, 255, 255),
                    textureManager.createSolidColorTexture(0, 255, 255, 255)
                )
            }
        }
    }

    private fun drawBackground() {
        // Draw quad in orthographic depth
        // Simple background representation
        pbrShader.use()
        // Override matrices for simple flat screen quad
        val identity = FloatArray(16).apply { Matrix.setIdentityM(this, 0) }
        pbrShader.setMat4("uProjection", identity)
        pbrShader.setMat4("uView", identity)
        pbrShader.setMat4("uModel", identity)
        
        val normalMat = FloatArray(9).apply {
            this[0]=1f; this[4]=1f; this[8]=1f
        }
        pbrShader.setMat3("uNormalMatrix", normalMat)
        pbrShader.setVec3("uCameraPos", 0f, 0f, 1f)
        pbrShader.setVec3("uLightDirection", 0f, 0f, -1f)

        // Bind background textures
        bindPbrTextures(bgTexture, 
            textureManager.createSolidColorTexture(128, 128, 255, 255),
            textureManager.createSolidColorTexture(0, 255, 255, 255)
        )
        backgroundQuad.draw()
    }

    private fun drawEntity(
        x: Float, y: Float, z: Float, rotY: Float, scale: Float,
        mesh: Mesh, albedo: Int, normal: Int, mr: Int
    ) {
        Matrix.setIdentityM(modelMatrix, 0)
        Matrix.translateM(modelMatrix, 0, x, y, z)
        Matrix.rotateM(modelMatrix, 0, rotY, 0f, 1f, 0f)
        Matrix.scaleM(modelMatrix, 0, scale, scale, scale)

        // Compute Normal matrix: inverse-transpose of 3x3 model matrix
        normalMatrix[0] = modelMatrix[0]; normalMatrix[1] = modelMatrix[1]; normalMatrix[2] = modelMatrix[2]
        normalMatrix[3] = modelMatrix[4]; normalMatrix[4] = modelMatrix[5]; normalMatrix[5] = modelMatrix[6]
        normalMatrix[6] = modelMatrix[8]; normalMatrix[7] = modelMatrix[9]; normalMatrix[8] = modelMatrix[10]
        
        pbrShader.setMat4("uModel", modelMatrix)
        pbrShader.setMat3("uNormalMatrix", normalMatrix)

        bindPbrTextures(albedo, normal, mr)
        mesh.draw()
    }

    private fun bindPbrTextures(albedo: Int, normal: Int, mr: Int) {
        GLES30.glActiveTexture(GLES30.GL_TEXTURE0)
        GLES30.glBindTexture(GLES30.GL_TEXTURE_2D, albedo)
        pbrShader.setInt("uAlbedoMap", 0)

        GLES30.glActiveTexture(GLES30.GL_TEXTURE1)
        GLES30.glBindTexture(GLES30.GL_TEXTURE_2D, normal)
        pbrShader.setInt("uNormalMap", 1)

        GLES30.glActiveTexture(GLES30.GL_TEXTURE2)
        GLES30.glBindTexture(GLES30.GL_TEXTURE_2D, mr)
        pbrShader.setInt("uMetallicRoughnessMap", 2)
    }

    private fun loadMeshOrFallback(path: String, fallbackGen: () -> Mesh): Mesh {
        return try {
            Mesh.loadFromObj(context, path)
        } catch (e: Exception) {
            Log.w("GLES30Renderer", "Could not load mesh from '$path', generating procedural fallback.")
            fallbackGen()
        }
    }

    // --- Procedural Fallbacks for Meshes (makes development testable immediately) ---
    private fun createSpaceshipVertices(): Mesh {
        // Rocket pyramid
        val vertices = floatArrayOf(
            // Nose cone
             0f,  0f, -2f,    0.5f, 0f,   0f, 0.5f, -0.8f,    1f, 0f, 0f,
            -1f, -0.5f, 1f,   0f, 1f,    -0.8f, -0.5f, 0.2f,  1f, 0f, 0f,
             1f, -0.5f, 1f,   1f, 1f,     0.8f, -0.5f, 0.2f,  1f, 0f, 0f,
             0f,  0.5f, 1f,   0.5f, 1f,   0f, 0.8f, 0.2f,     1f, 0f, 0f
        )
        val indices = shortArrayOf(
            0, 1, 2, // bottom side
            0, 2, 3, // right side
            0, 3, 1, // left side
            1, 3, 2  // back side
        )
        return Mesh(vertices, indices)
    }

    private fun createAsteroidVertices(): Mesh {
        // Double pyramid octahedron shape for rock asteroid
        val vertices = floatArrayOf(
             0f,  1f,  0f,    0.5f, 0f,   0f, 1f, 0f,    1f, 0f, 0f,
            -1f,  0f, -1f,    0f, 0.5f,  -1f, 0f, -1f,   1f, 0f, 0f,
             1f,  0f, -1f,    1f, 0.5f,   1f, 0f, -1f,   1f, 0f, 0f,
             1f,  0f,  1f,    1f, 1f,     1f, 0f, 1f,    1f, 0f, 0f,
            -1f,  0f,  1f,    0f, 1f,    -1f, 0f, 1f,    1f, 0f, 0f,
             0f, -1f,  0f,    0.5f, 1f,   0f, -1f, 0f,   1f, 0f, 0f
        )
        val indices = shortArrayOf(
            0, 1, 2,  0, 2, 3,  0, 3, 4,  0, 4, 1, // top pyramid
            5, 2, 1,  5, 3, 2,  5, 4, 3,  5, 1, 4  // bottom pyramid
        )
        return Mesh(vertices, indices)
    }

    private fun createLaserVertices(): Mesh {
        // Red narrow beam
        val vertices = floatArrayOf(
            -0.1f, -0.1f, -2f,   0f, 0f,   0f, 0f, -1f,   1f, 0f, 0f,
             0.1f, -0.1f, -2f,   1f, 0f,   0f, 0f, -1f,   1f, 0f, 0f,
             0f,    0.1f, -2f,   0.5f, 0f, 0f, 0f, -1f,   1f, 0f, 0f,
            -0.1f, -0.1f,  2f,   0f, 1f,   0f, 0f,  1f,   1f, 0f, 0f,
             0.1f, -0.1f,  2f,   1f, 1f,   0f, 0f,  1f,   1f, 0f, 0f,
             0f,    0.1f,  2f,   0.5f, 1f, 0f, 0f,  1f,   1f, 0f, 0f
        )
        val indices = shortArrayOf(
            0, 1, 2,  3, 5, 4,
            0, 2, 5,  0, 5, 3,
            1, 4, 5,  1, 5, 2,
            0, 3, 4,  0, 4, 1
        )
        return Mesh(vertices, indices)
    }
}
