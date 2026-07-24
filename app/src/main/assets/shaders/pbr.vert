#version 300 es
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoords;
layout(location = 2) in vec3 aNormal;
layout(location = 3) in vec3 aTangent;

out vec3 vWorldPos;
out vec2 vTexCoords;
out vec3 vNormal;
out mat3 vTBN;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;
uniform mat3 uNormalMatrix; // transpose(inverse(mat3(uModel)))

void main() {
    vWorldPos = vec3(uModel * vec4(aPosition, 1.0));
    vTexCoords = aTexCoords;
    
    // Normal mapping setup
    vNormal = normalize(uNormalMatrix * aNormal);
    vec3 T = normalize(uNormalMatrix * aTangent);
    vec3 N = vNormal;
    // Gram-Schmidt process to re-orthogonalize T and N
    T = normalize(T - dot(T, N) * N);
    vec3 B = cross(N, T);
    vTBN = mat3(T, B, N);

    gl_Position = uProjection * uView * vec4(vWorldPos, 1.0);
}
