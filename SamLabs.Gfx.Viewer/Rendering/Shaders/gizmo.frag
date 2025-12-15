#version 330 core

out vec4 FragColor;
uniform bool uIsHovered;
void main()
{
    if(uIsHovered)
    { FragColor = vec4(0.5f, 0.9f, 0.5f, 1.0); }
    else
    { FragColor = vec4(0.9f, 0.9f, 0.9f, 1.0); }
}
