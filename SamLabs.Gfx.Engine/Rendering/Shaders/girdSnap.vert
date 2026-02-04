#version 330 core

layout(location = 0) in vec3 aPos;
layout(location = 2) in vec2 aTexCoord;
layout(std140) uniform ViewProjection
{
    mat4 view;
    mat4 projection;
};
uniform mat4 uModel;

out vec2 TexCoord;
void main()
{
    vec4 worldPosition = uModel * vec4(aPos, 1.0);
    gl_Position = projection *view * worldPosition;
    TexCoord = aTexCoord;
}
