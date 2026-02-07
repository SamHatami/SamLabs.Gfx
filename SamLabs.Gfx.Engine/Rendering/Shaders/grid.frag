#version 330 core
in vec3 fragWorldPosition;

layout(std140) uniform ViewProjection
{
    mat4 view;
    mat4 projection;
    vec3 cameraPosition;
};

uniform float uGridSize = 100.0;
uniform float uGridSpacing = 1.0;
uniform float uMajorLineFrequency = 5.0;

out vec4 FragColor;

float gridLine(vec2 coord, float spacing, float lineWidth)
{
    vec2 grid = abs(fract(coord / spacing - 0.5) - 0.5) / fwidth(coord / spacing);
    float line = min(grid.x, grid.y);
    return 1.0 - min(line / lineWidth, 1.0);
}

void main()
{
    vec2 coord = fragWorldPosition.xz;

    vec3 viewDir = normalize(cameraPosition - fragWorldPosition);
    float viewAngle = abs(viewDir.y); // 0 = edge-on, 1 = top-down

    // Adjust line width based on view angle - thicker when looking straight down
    float angleAdjustment = mix(1.0, 3.0, viewAngle);
    // Minor grid lines
    float minorLineWidth = 1.0 * angleAdjustment;
    float minorGrid = gridLine(coord, uGridSpacing, minorLineWidth);

    // Major grid lines (every N minor lines)
    float majorLineWidth = 1.5 * angleAdjustment;
    float majorGrid = gridLine(coord, uGridSpacing * uMajorLineFrequency, majorLineWidth);

    // Axis lines (X and Z)
    vec2 axisCoord = coord / fwidth(coord);
    float axisWidth = 2.0;
    float xAxis = 1.0 - min(abs(axisCoord.y) / axisWidth, 1.0);
    float zAxis = 1.0 - min(abs(axisCoord.x) / axisWidth, 1.0);

    // Distance fade
    float distToCamera = length(cameraPosition - fragWorldPosition);
    float fadeStart = 90.0;
    float fadeEnd = 150.0;
    float distanceFade = 1.0 - smoothstep(fadeStart, fadeEnd, distToCamera);

    // Colors
    vec3 minorColor = vec3(0.2, 0.2, 0.2);
    vec3 majorColor = vec3(0.4, 0.4, 0.4);
    vec3 xAxisColor = vec3(1.0, 0.0, 0.0);
    vec3 zAxisColor = vec3(0.0, 0.0, 1.0);

    // Blend: axes > major > minor
    vec3 color = minorColor * minorGrid;
    color = mix(color, majorColor, majorGrid);
    color = mix(color, xAxisColor, xAxis);
    color = mix(color, zAxisColor, zAxis);

    // Alpha: show any line that's visible
    float alpha = max(max(minorGrid, majorGrid), max(xAxis, zAxis)) * distanceFade;

    FragColor = vec4(color, alpha);
}