#version 300 es
precision highp float;

out vec4 FragColor;

in vec3 vWorldPos;
in vec2 vTexCoords;
in vec3 vNormal;
in mat3 vTBN;

// PBR Material Maps
uniform sampler2D uAlbedoMap;
uniform sampler2D uNormalMap;
uniform sampler2D uMetallicRoughnessMap; // Red: Metallic, Green: Roughness, Blue: AO

// Lighting
uniform vec3 uLightDirection;
uniform vec3 uLightColor;
uniform vec3 uCameraPos;

const float PI = 3.14159265359;

// Trowbridge-Reitz GGX Normal Distribution Function
float DistributionGGX(vec3 N, vec3 H, float roughness) {
    float a = roughness * roughness;
    float a2 = a * a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH * NdotH;

    float num = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return num / max(denom, 0.0000001);
}

// Schlick-GGX Geometry Function for single vector
float GeometrySchlickGGX(float NdotV, float roughness) {
    float r = (roughness + 1.0);
    float k = (r * r) / 8.0;

    float num = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return num / max(denom, 0.0000001);
}

// Smith's method for Geometry shadowing/masking
float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness) {
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2 = GeometrySchlickGGX(NdotV, roughness);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness);

    return ggx1 * ggx2;
}

// Fresnel Schlick approximation
vec3 FresnelSchlick(float cosTheta, vec3 F0) {
    return F0 + (1.0 - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}

void main() {
    // Sample texture inputs
    vec3 albedo = texture(uAlbedoMap, vTexCoords).rgb;
    
    // Sample normal map and convert from tangent space [0,1] to [-1,1]
    vec3 localNormal = texture(uNormalMap, vTexCoords).rgb * 2.0 - 1.0;
    vec3 N = normalize(vTBN * localNormal);

    // Sample metallic, roughness, AO
    vec3 mrSample = texture(uMetallicRoughnessMap, vTexCoords).rgb;
    float metallic = mrSample.r;
    float roughness = mrSample.g;
    float ao = mrSample.b;

    vec3 V = normalize(uCameraPos - vWorldPos);
    vec3 L = normalize(-uLightDirection);
    vec3 H = normalize(V + L);

    // Cook-Torrance BRDF calculation
    // F0 represents base reflectance (0.04 for dielectrics, albedo for metals)
    vec3 F0 = vec3(0.04); 
    F0 = mix(F0, albedo, metallic);

    // Lighting factors
    float NDF = DistributionGGX(N, H, roughness);       
    float G   = GeometrySmith(N, V, L, roughness);      
    vec3 F    = FresnelSchlick(max(dot(H, V), 0.0), F0);       
    
    // Specular lobe numerator and denominator
    vec3 numerator    = NDF * G * F;
    float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0);
    vec3 specular     = numerator / max(denominator, 0.0001);  
    
    // kS is specular contribution, kD is diffuse contribution
    vec3 kS = F;
    vec3 kD = vec3(1.0) - kS;
    kD *= 1.0 - metallic;     
            
    // Cook-Torrance reflection equation
    float NdotL = max(dot(N, L), 0.0);        
    vec3 Lo = (kD * albedo / PI + specular) * uLightColor * NdotL;

    // Ambient lighting (ambient occlusion modulated)
    vec3 ambient = vec3(0.03) * albedo * ao;
    vec3 color = ambient + Lo;

    // Reinhard tone mapping to map high dynamic range to LDR
    color = color / (color + vec3(1.0));
    // Gamma correction
    color = pow(color, vec3(1.0 / 2.2));

    FragColor = vec4(color, 1.0);
}
