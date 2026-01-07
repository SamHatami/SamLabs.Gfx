#version 330 core
layout(location = 0) in vec3 aPos;

layout(std140) uniform ViewProjection
{
    mat4 view;
    mat4 projection;
};

uniform uint uEntityId;
uniform uint uPickingType;
uniform mat4 uModel;
flat out uint vertexPickingId;

void main()
{
    gl_Position = projection *view * uModel * vec4(aPos, 1.0);
    
    //vertexPickingId = (objectId << 20) | uint(gl_VertexID);
    vertexPickingId = uPickingId;
}
