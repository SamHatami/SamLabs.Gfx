#version 330 core
in vec3 fragWorldPosition;
in vec2 TexCoord;

layout(std140) uniform ViewProjection
{
    mat4 view;
    mat4 projection;
    vec3 cameraPosition;
};

uniform float uGridSize = 1.0;
uniform float uGridSpacing = 1.0;
uniform float uMajorLineFrequency =20.0;

out vec4 FragColor;

void main()
{
    vec2 majorGridCellCoords = mod(TexCoord + 0.5*uMajorLineFrequency, uMajorLineFrequency);
    vec2 minorGridCellCoords = floor(fragWorldPosition.xz / (uGridSize * uGridSpacing));

    vec2 majorGridLineDist = abs(fract(fragWorldPosition.xz / (uGridSize * uGridSpacing * uMajorLineFrequency)) - 0.5);
    vec2 minorGridLineDist = abs(fract(fragWorldPosition.xz / (uGridSize * uGridSpacing)) - 0.5);
    FragColor = vec4(majorGridCellCoords,0,1);
}