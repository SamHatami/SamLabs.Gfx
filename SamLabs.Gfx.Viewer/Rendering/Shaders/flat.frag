#version 330 core
in vec3 vWorldPosition;
out vec4 FragColor;
uniform int uIsHovered;
uniform int uIsSelected;
void main()
{
    vec3 dx = dFdx(vWorldPosition);
    vec3 dy = dFdy(vWorldPosition);
    vec3 normal = normalize(cross(dx, dy));

    vec3 lightDir = normalize(vec3(0.5, 1.0, 0.5));
    float diff = max(dot(normal, lightDir), 0.0);

    vec3 baseColor = vec3(0.5);
    
    if(uIsSelected == 1)
        baseColor = vec3(0.4, 0.4, 0.9);

    if(uIsHovered == 1)
    {  baseColor = vec3(0.5, 0.9, 0.5); }
    
    FragColor = vec4(baseColor * diff, 1.0);
}
