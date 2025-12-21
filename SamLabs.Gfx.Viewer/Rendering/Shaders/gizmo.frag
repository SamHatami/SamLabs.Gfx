#version 330 core

out vec4 FragColor;
uniform int uIsHovered;
uniform int uIsSelected;
void main()
{
    if(uIsHovered == 1)
    { FragColor = vec4(0.5, 0.9, 0.5, 1.0); }
    else
    { 
        FragColor = vec4(0.9, 0.9, 0.9, 1.0); 
    }
    

}
