#version 330 core

// Quad vertex attributes
layout(location = 0) in vec3 aPos;

// Instance attributes (from LineInstance)
layout(location = 1) in vec3 aStart;
layout(location = 2) in vec3 aEnd;
layout(location = 3) in float aThickness;
layout(location = 4) in uint aStyleFlags;
layout(location = 5) in float aDashLength;
layout(location = 6) in float aGapLength;
layout(location = 7) in vec4 aColor;
layout(location = 8) in int aEntityId;

uniform mat4 uModel;
uniform int uIsPickingPass;
uniform vec2 uViewportSize;

layout(std140) uniform ViewProjection
{
    mat4 view;
    mat4 projection;
    vec3 cameraPosition;
};

out VS_OUT {
    vec4 color;
    vec2 uv;
    flat int entityId;
    flat uint styleFlags;
    float dashLength;
    float gapLength;
    float distanceAlongLine;
} vs_out;

void main()
{
    // Project start and end points to clip space
    vec4 clipStart = projection * view * uModel * vec4(aStart, 1.0);
    vec4 clipEnd = projection * view * uModel * vec4(aEnd, 1.0);
    
    // Convert to NDC
    vec3 ndcStart = clipStart.xyz / clipStart.w;
    vec3 ndcEnd = clipEnd.xyz / clipEnd.w;
    
    // Convert to screen space
    vec2 screenStart = (ndcStart.xy * 0.5 + 0.5) * uViewportSize;
    vec2 screenEnd = (ndcEnd.xy * 0.5 + 0.5) * uViewportSize;
    
    // Calculate line direction and perpendicular
    vec2 dir = normalize(screenEnd - screenStart);
    vec2 perp = vec2(-dir.y, dir.x);
    
    // Determine line width (fat picking for picking pass)
    float width = aThickness;
    if (uIsPickingPass == 1)
        width = max(width, 10.0);
    
    // Expand quad vertices along the perpendicular
    float t = aPos.x + 0.5;  // Interpolation factor along line (0 to 1)
    vec2 screenPos = mix(screenStart, screenEnd, t);
    screenPos += perp * width * aPos.y;  // aPos.y is -0.5 or 0.5
    
    // Convert back to NDC
    vec2 ndcPos = (screenPos / uViewportSize) * 2.0 - 1.0;
    float depth = mix(ndcStart.z, ndcEnd.z, t);
    
    gl_Position = vec4(ndcPos, depth, 1.0);
    
    // Pass data to fragment shader
    vs_out.color = aColor;
    vs_out.uv = aPos.xy;
    vs_out.entityId = aEntityId;
    vs_out.styleFlags = aStyleFlags;
    vs_out.dashLength = aDashLength;
    vs_out.gapLength = aGapLength;
    vs_out.distanceAlongLine = t * distance(screenStart, screenEnd);
}

