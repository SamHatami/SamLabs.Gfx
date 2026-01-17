#version 330 core

out vec4 FragColor;
uniform int uIsHovered;
uniform int uIsSelected;

void main()
{
    if(uIsHovered == 1 || uIsSelected == 1)
    {
        FragColor = vec4(0.95, 0.95, 0.5, 0.8);
    }
    else
    {
        
    }


}