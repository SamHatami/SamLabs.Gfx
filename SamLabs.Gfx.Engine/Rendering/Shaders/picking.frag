#version 330 core
layout(location = 0) out ivec2 FragOutput;

uniform int uPickingType;
uniform int uEntityId;
flat in int vertexPickingId;


void main()
{
    int elementID = 0;

    if (uPickingType == 3)
    {
        elementID = vertexPickingId;
    }
    else
    {
        elementID = gl_PrimitiveID;
    }

    int packedID = (elementID & 0x0FFFFFFF) | (uPickingType << 28);

    FragOutput = ivec2(uEntityId, packedID);
}
