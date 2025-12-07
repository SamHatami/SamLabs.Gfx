#version 330 core
in vec3 vWorldPosition;
out vec4 FragColor;
void main()
{
    vec3 dx = dFdx(vWorldPosition);
    vec3 dy = dFdy(vWorldPosition);
    vec3 normal = normalize(cross(dx, dy));

    vec3 lightDir = normalize(vec3(0.5, 1.0, 0.5));
    float diff = max(dot(normal, lightDir), 0.0);

    FragColor = vec4(vec3(0.8) * diff, 1.0);
}
