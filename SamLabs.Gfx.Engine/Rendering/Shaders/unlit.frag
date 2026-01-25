#version 330 core
in vec3 vWorldPosition;
out vec4 FragColor;
uniform int uIsHovered;
uniform int uIsSelected;

vec4 baseColor = vec4(0.5, 0.5, 0.5, 1.0);
void main()
{
    if(uIsSelected == 1)
        baseColor = vec4(0.4, 0.4, 0.9,1.0);
    if(uIsHovered == 1)
    {
        baseColor = vec4(0.25, 0.25, 1.0,1.0);
        vec4 litColor = baseColor * (0.8 + 0.4);
        FragColor = litColor;
    }
    else
    {
        FragColor = baseColor;
    }
}
