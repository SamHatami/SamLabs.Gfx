#version 330 core
layout(location = 0) in vec3 aPos;
layout(std140) uniform ViewProjection
{
    mat4 view;
    mat4 projection;
};

out vec3 fragWorldPosition;

void main()
{
    vec4 worldPos = vec4(aPos, 1.0);

    fragWorldPosition = worldPos.xyz;

    gl_Position = projection * view * worldPos;
}
