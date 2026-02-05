#version 330 core
layout(location = 0) in vec3 aPos;
layout(location = 2) in vec2 aTexCoord;

uniform mat4 uModel;

layout(std140) uniform ViewProjection
{
    mat4 view;
    mat4 projection;
    vec3 cameraPosition;
};

out vec3 fragWorldPosition;
out vec2 TexCoord;
uniform float uGridSize = 100.0;

void main()
{
    TexCoord = aTexCoord * (uGridSize*0.5);
    vec4 worldPos = uModel * vec4(aPos*uGridSize*0.5, 1);
    fragWorldPosition = worldPos.xyz;

    gl_Position = projection * view * worldPos;
}
