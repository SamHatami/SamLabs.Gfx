#version 330 core
layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aNormal;    
layout(location = 2) in vec2 aTexCoord;
uniform mat4 uModel;
layout(std140) uniform ViewProjection
{
    mat4 view;
    mat4 projection;
};

out vec3 vWorldPosition;
out vec3 vNormal;
void main()
{
    vec4 worldPosition = uModel * vec4(aPos, 1.0);
    vWorldPosition =worldPosition.xyz;
    vNormal = aNormal;
    gl_Position = projection *view * worldPosition;
}
