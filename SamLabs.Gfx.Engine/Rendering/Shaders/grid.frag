#version 330 core
in vec3 fragWorldPosition;
in vec2 TexCoord;

layout(std140) uniform ViewProjection
{
    mat4 view;
    mat4 projection;
    vec3 cameraPosition;
};

uniform float uGridSize = 100.0; 
uniform float uGridSpacing = 0.1;
uniform float uMajorLineFrequency =2.0;

vec3 subLineColor = vec3(0.1, 0.1, 0.1);
vec3 mainLineColor = vec3(0.5, 0.5, 0.5);
 
out vec4 FragColor;
float majorCellHalfSize = uMajorLineFrequency * 0.5;
float minorCellHalfSize = uGridSpacing;
void main() 
{ //Tes
    vec2 majorGridCellCoords = mod(TexCoord + majorCellHalfSize, uMajorLineFrequency);
    vec2 minorGridCellCoords = mod(TexCoord + uGridSpacing, uGridSpacing);

    vec2 majorGridLineDist = min(majorGridCellCoords, uMajorLineFrequency - majorGridCellCoords);
    vec2 minorGridLineDist = min(minorGridCellCoords, uGridSpacing - minorGridCellCoords);

    float majorLineWidth = 0.01; 
    float minorLineWidth = 0.001;

    float cellLineT    = 0.5 * (majorLineWidth + fwidth(min(majorGridLineDist.x, majorGridLineDist.y)));
    float subCellLineT = 0.5 * (minorLineWidth + fwidth(min(minorGridLineDist.x, minorGridLineDist.y)));


    vec3 gridColor = vec3(0.0);
    if (any(lessThan(majorGridLineDist, vec2(cellLineT)))) { gridColor = mainLineColor; }
    if (any(lessThan(minorGridLineDist, vec2(subCellLineT)))) { gridColor = subLineColor; }



    FragColor = vec4(gridColor, 1.0);
}