#version 330 core
layout(location = 0) in vec3 aPos;
uniform mat4 uModel;
layout(std140) uniform ViewProjection
{
    mat4 view;
    mat4 projection;
};

void main()
{
    vec4 worldPosition = uModel * vec4(aPos, 1.0);
    gl_Position = projection *view * worldPosition;
}

