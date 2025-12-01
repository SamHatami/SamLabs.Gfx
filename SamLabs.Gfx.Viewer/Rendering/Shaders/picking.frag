#version 330 core
out uint FragColor;

flat in uint vertexPickingId;

void main()
{
    FragColor = vertexPickingId;
}
