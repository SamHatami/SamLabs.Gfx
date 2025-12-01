#version 330 core
layout(location = 0) in vec3 aPos;

layout(std140) uniform ViewProjection
{
    mat4 view;
    mat4 projection;
};

uniform uint objectId;

flat out uint vertexPickingId;

void main()
{
    gl_Position = projection *view * vec4(aPos, 1.0);
    
    uint vertexPickingId = (objectId << 20) | uint(gl_VertexID); 
}
