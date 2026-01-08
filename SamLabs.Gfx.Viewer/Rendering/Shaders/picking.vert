#version 330 core
layout(location = 0) in vec3 aPos;

layout(std140) uniform ViewProjection
{
    mat4 view;
    mat4 projection;
};

uniform int uEntityId;
uniform mat4 uModel;
uniform int uVertexRenderSize;
flat out int vertexPickingId;

void main()
{
    gl_Position = projection *view * uModel * vec4(aPos, 1.0);
    
    gl_PointSize = (uVertexRenderSize > 0.0) ? uVertexRenderSize: 5.0;
    
    vertexPickingId = gl_VertexID;
    
}
