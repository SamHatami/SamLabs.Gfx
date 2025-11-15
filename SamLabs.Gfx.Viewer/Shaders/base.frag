#version 330 core
out uint FragColor;

uniform uint objectId;

void main()
{
    FragColor = objectId;
}
