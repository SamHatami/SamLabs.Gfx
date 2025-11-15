#version 330 core
out int FragColor;

uniform int objectId;

void main()
{
    FragColor = objectId;
}
