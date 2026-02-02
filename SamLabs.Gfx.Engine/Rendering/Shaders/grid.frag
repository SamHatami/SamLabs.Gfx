#version 330 core
in vec3 fragWorldPosition;
layout(std140) uniform ViewProjection
{
    mat4 view;
    mat4 projection;
    vec3 cameraPosition;
};

uniform float uGridSize = 1.0;
uniform float uGridSpacing = 1.0;
uniform float uMajorLineFrequency = 10.0;

out vec4 FragColor;

float grid(vec2 pos, float spacing, float lineWidth) {
    vec2 coord = pos / spacing;
    vec2 derivative = fwidth(coord);
    vec2 grid = abs(fract(coord - 0.5) - 0.5) / derivative;
    float line = min(grid.x, grid.y);
    return 1.0 - min(line, lineWidth);
}

void main()
{
    float distToCamera = length(cameraPosition - fragWorldPosition);
    float fadeStart = 50.0;
    float fadeEnd = 100.0;
    float distanceFade = 1.0 - smoothstep(fadeStart, fadeEnd, distToCamera);

    // Minor grid lines
    float minorGrid = grid(fragWorldPosition.xz, uGridSpacing, 1.0);
    
    // Major grid lines
    float majorGrid = grid(fragWorldPosition.xz, uGridSpacing * uMajorLineFrequency, 1.0);

    // Combine grids (major lines are brighter)
    float g = max(minorGrid * 0.5, majorGrid);

    // World axes (X and Z)
    vec2 axisCoord = fragWorldPosition.xz / fwidth(fragWorldPosition.xz);
    float axisThickness = 3.0;

    float xAxis = 1.0 - min(abs(axisCoord.x) / axisThickness, 1.0);
    float zAxis = 1.0 - min(abs(axisCoord.y) / axisThickness, 1.0);

    vec3 gridColor = vec3(0.5, 0.5, 0.5);
    vec3 xAxisColor = vec3(1.0, 0.0, 0.0);
    vec3 zAxisColor = vec3(0.0, 0.0, 1.0);

    vec3 color = mix(gridColor, xAxisColor, xAxis * 0.85);
    color = mix(color, zAxisColor, zAxis);

    float alpha = max(g, max(xAxis * 0.85, zAxis)) * distanceFade;

    FragColor = vec4(color, alpha);
}