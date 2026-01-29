#version 330 core
layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aNormal;    
layout(location = 2) in vec2 aTexCoord;
layout(std140) uniform ViewProjection
{
    mat4 view;
    mat4 projection;
};

out vec3 vWorldPosition;
uniform mat4 uModel;
uniform float uNodeSize;
void main()
{
    vec4 worldPosition = uModel * vec4(aPos, 1.0);
    vWorldPosition =worldPosition.xyz;
    gl_Position = projection *view * worldPosition;
    gl_PointSize = uNodeSize;
}
